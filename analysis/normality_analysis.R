library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
variant <- Sys.getenv("BENCH_VARIANT", "Byte")
stopifnot(variant %in% c("Byte", "String"))
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      sprintf("JsonBench.Benchmarks.Factorial.FactorialNormalized%sBench-measurements.csv", variant))
out_dir <- file.path(script_dir, "plots", tolower(variant), "01_normality")
dir.create(out_dir, showWarnings = FALSE, recursive = TRUE)
raw <- read_csv(csv_path, show_col_types = FALSE)

# Filter to workload result iterations only
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Actual")

# Parse method name: e.g. "SpanJson_Deser_D10_W100_B"
df <- df %>%
  mutate(
    Library   = str_extract(Target_Method, "^[^_]+"),
    Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
    Depth     = as.integer(str_extract(Target_Method, "(?<=_D)\\d+")),
    Width     = as.integer(str_extract(Target_Method, "(?<=_W)\\d+")),
    Content   = str_extract(Target_Method, "[TNB]$"),
    EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation
  ) %>%
  mutate(
    Content   = factor(Content, levels = c("T", "N", "B"),
                       labels = c("Textual", "Numeric", "Boolean")),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize"))
  )

# ---------------------------------------------------------------------------
# Shapiro-Wilk test per group (each unique benchmark method)
# ---------------------------------------------------------------------------
shapiro_results <- df %>%
  group_by(Library, Operation, Depth, Width, Content) %>%
  summarise(
    n          = n(),
    # if (...) is short-circuit; ifelse() would evaluate shapiro.test() even when
    # n() < 3, which throws "sample size must be between 3 and 5000".
    sw_stat    = if (n() >= 3) shapiro.test(EnergyPerOp)$statistic else NA_real_,
    sw_p_value = if (n() >= 3) shapiro.test(EnergyPerOp)$p.value   else NA_real_,
    .groups = "drop"
  ) %>%
  mutate(
    normal = sw_p_value >= 0.05
  )

cat("\n=== Shapiro-Wilk Normality Test Summary ===\n")
cat(sprintf("Total groups tested: %d\n", nrow(shapiro_results)))
cat(sprintf("Normal (p >= 0.05):  %d (%.1f%%)\n",
            sum(shapiro_results$normal, na.rm = TRUE),
            100 * mean(shapiro_results$normal, na.rm = TRUE)))
cat(sprintf("Non-normal (p < 0.05): %d (%.1f%%)\n",
            sum(!shapiro_results$normal, na.rm = TRUE),
            100 * mean(!shapiro_results$normal, na.rm = TRUE)))

# Breakdown by factor
cat("\n--- Normality by Library ---\n")
shapiro_results %>%
  group_by(Library) %>%
  summarise(pct_normal = 100 * mean(normal, na.rm = TRUE), .groups = "drop") %>%
  print(n = Inf)

cat("\n--- Normality by Operation ---\n")
shapiro_results %>%
  group_by(Operation) %>%
  summarise(pct_normal = 100 * mean(normal, na.rm = TRUE), .groups = "drop") %>%
  print(n = Inf)

cat("\n--- Normality by Content ---\n")
shapiro_results %>%
  group_by(Content) %>%
  summarise(pct_normal = 100 * mean(normal, na.rm = TRUE), .groups = "drop") %>%
  print(n = Inf)

# Save full results table
write_csv(shapiro_results, file.path(out_dir, "shapiro_wilk_results.csv"))
cat(sprintf("\nFull results saved to: %s\n", file.path(out_dir, "shapiro_wilk_results.csv")))

# ---------------------------------------------------------------------------
# QQ Plots — one per library×operation×content, faceted by depth(row)×width(col)
# ---------------------------------------------------------------------------
cat("\nGenerating QQ plots...\n")

content_labels <- c("Textual" = "Textual", "Numeric" = "Numeric", "Boolean" = "Boolean")

for (lib in levels(df$Library)) {
  lib_dir <- file.path(out_dir, "qq", tolower(lib))
  dir.create(lib_dir, showWarnings = FALSE, recursive = TRUE)

  for (op in levels(df$Operation)) {
    for (ct in levels(df$Content)) {
      for (w in c(5, 20, 50, 100)) {
        sub <- df %>% filter(Library == lib, Operation == op, Content == ct, Width == w)
        if (nrow(sub) == 0) next

        p <- ggplot(sub, aes(sample = EnergyPerOp)) +
          stat_qq(size = 2, alpha = 0.7, color = "#56B4E9") +
          stat_qq_line(linewidth = 0.6, color = "#D55E00") +
          facet_wrap(~ Depth, nrow = 1, scales = "free_y",
                     labeller = labeller(Depth = function(x) paste0("Depth ", x))) +
          labs(
            x = "Theoretical Quantiles",
            y = "Sample Quantiles (Energy/Op)"
          ) +
          theme_minimal(base_size = 14) +
          theme(
            strip.text = element_text(face = "bold")
          )

        op_short <- ifelse(op == "Deserialize", "deser", "ser")
        fname <- sprintf("qq_%s_%s_w%d.png", op_short, tolower(ct), w)
        ggsave(file.path(lib_dir, fname), p, width = 12, height = 4, dpi = 300)
        cat(sprintf("  Saved: %s/%s\n", tolower(lib), fname))
      }
    }
  }
}

# ---------------------------------------------------------------------------
# Summary histogram of Shapiro-Wilk p-values
# ---------------------------------------------------------------------------
p_hist <- ggplot(shapiro_results, aes(x = sw_p_value)) +
  geom_histogram(bins = 30, fill = "#56B4E9", color = "white", alpha = 0.9) +
  geom_vline(xintercept = 0.05, linetype = "dashed", color = "#D55E00", linewidth = 0.6) +
  annotate("text", x = 0.07, y = Inf, label = "alpha = 0.05", vjust = 2,
           hjust = 0, color = "#D55E00", size = 4) +
  labs(
    x = "Shapiro-Wilk p-value",
    y = "Count"
  ) +
  theme_minimal(base_size = 14)

ggsave(file.path(out_dir, "shapiro_pvalue_distribution.png"), p_hist,
       width = 8, height = 5, dpi = 300)
cat("  Saved: shapiro_pvalue_distribution.png\n")

# ---------------------------------------------------------------------------
# Normality heatmap — library × (depth_width_content) per operation
# ---------------------------------------------------------------------------
# Order workloads numerically by Depth then Width (sprintf alone gives a
# lex-sorted axis where D10_* precedes D2_*).
workload_order <- shapiro_results %>%
  distinct(Depth, Width, Content) %>%
  arrange(Depth, Width, Content) %>%
  mutate(Workload = sprintf("D%d_W%d_%s", Depth, Width, Content)) %>%
  pull(Workload)

heatmap_data <- shapiro_results %>%
  mutate(Workload = factor(sprintf("D%d_W%d_%s", Depth, Width, Content),
                           levels = workload_order))

for (op in levels(df$Operation)) {
  sub <- heatmap_data %>% filter(Operation == op)

  p_heat <- ggplot(sub, aes(x = Workload, y = Library, fill = sw_p_value)) +
    geom_tile(color = "white", linewidth = 0.3) +
    geom_text(aes(label = ifelse(normal, "", "*")),
              color = "#D55E00", size = 3, fontface = "bold") +
    scale_fill_gradient2(low = "#D55E00", mid = "#F5F5F5", high = "#56B4E9",
                         midpoint = 0.05, name = "p-value",
                         limits = c(0, 1)) +
    labs(
      x = "Workload Configuration",
      y = "Library"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      axis.text.x = element_text(angle = 45, hjust = 1, size = 7),
      legend.position = "right"
    )

  fname <- sprintf("normality_heatmap_%s.png", tolower(op))
  ggsave(file.path(out_dir, fname), p_heat, width = 16, height = 5, dpi = 300)
  cat(sprintf("  Saved: %s\n", fname))
}

cat("\nDone.\n")
