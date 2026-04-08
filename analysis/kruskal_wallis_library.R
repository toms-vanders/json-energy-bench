library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedStringBench-measurements.csv")
base_dir <- file.path(script_dir, "plots", "02_kruskal_wallis")
lib_dir  <- file.path(base_dir, "library")
pl_dir   <- file.path(base_dir, "per_library")
dir.create(lib_dir, showWarnings = FALSE, recursive = TRUE)
dir.create(pl_dir, showWarnings = FALSE, recursive = TRUE)
raw <- read_csv(csv_path, show_col_types = FALSE)

# Filter to workload actual iterations (including outliers)
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
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize"))
  )

# Helper for significance labels
sig_label <- function(p) {
  case_when(
    p < 0.001 ~ "***",
    p < 0.01  ~ "**",
    p < 0.05  ~ "*",
    TRUE      ~ "ns"
  )
}

# ===========================================================================
# PART 1: Kruskal-Wallis — Energy ~ Library (overall library effect)
# ===========================================================================
cat("\n=== Kruskal-Wallis Test: Energy ~ Library ===\n")

kw_lib <- df %>%
  group_by(Operation, Depth, Width, Content) %>%
  summarise(
    n_obs      = n(),
    n_groups   = n_distinct(Library),
    kw_chi2    = kruskal.test(EnergyPerOp ~ Library)$statistic,
    kw_df      = kruskal.test(EnergyPerOp ~ Library)$parameter,
    kw_p_value = kruskal.test(EnergyPerOp ~ Library)$p.value,
    .groups = "drop"
  ) %>%
  mutate(
    significant = kw_p_value < 0.05,
    sig_label   = sig_label(kw_p_value)
  )

cat(sprintf("Total comparisons: %d\n", nrow(kw_lib)))
cat(sprintf("Significant (p < 0.05): %d (%.1f%%)\n",
            sum(kw_lib$significant), 100 * mean(kw_lib$significant)))

write_csv(kw_lib, file.path(lib_dir, "kw_library_results.csv"))

# --- Library p-value heatmap ---
heatmap_data <- kw_lib %>% mutate(Workload = sprintf("D%d_W%d", Depth, Width))

for (op in levels(df$Operation)) {
  sub <- heatmap_data %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = Content, fill = -log10(kw_p_value))) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sig_label), size = 3.5) +
    scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
    labs(title = sprintf("Kruskal-Wallis: Energy ~ Library — %s", op),
         subtitle = "*** p<0.001, ** p<0.01, * p<0.05, ns = not significant",
         x = "Workload (Depth x Width)", y = "Content Type") +
    theme_minimal(base_size = 11) +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(face = "bold"))
  ggsave(file.path(lib_dir, sprintf("kw_pvalue_%s.png", tolower(op))),
         p, width = 12, height = 5, dpi = 150)
}
cat("Library plots saved.\n")

# ===========================================================================
# PART 2: Per-library Kruskal-Wallis — Energy ~ Depth, Width, Content
# ===========================================================================
cat("\n=== Per-Library Kruskal-Wallis Tests ===\n")

run_kw <- function(data, group_var, fixed_vars) {
  data %>%
    group_by(across(all_of(fixed_vars))) %>%
    summarise(
      n_obs      = n(),
      n_groups   = n_distinct(.data[[group_var]]),
      kw_chi2    = kruskal.test(EnergyPerOp ~ .data[[group_var]])$statistic,
      kw_df      = kruskal.test(EnergyPerOp ~ .data[[group_var]])$parameter,
      kw_p_value = kruskal.test(EnergyPerOp ~ .data[[group_var]])$p.value,
      .groups = "drop"
    ) %>%
    mutate(
      factor_tested = group_var,
      significant   = kw_p_value < 0.05,
      sig_label     = sig_label(kw_p_value)
    )
}

all_per_lib <- tibble()

for (lib in levels(df$Library)) {
  lib_data <- df %>% filter(Library == lib)
  lib_out  <- file.path(pl_dir, tolower(lib))
  dir.create(lib_out, showWarnings = FALSE, recursive = TRUE)

  cat(sprintf("\n--- %s ---\n", lib))

  kw_depth   <- run_kw(lib_data, "Depth",   c("Operation", "Width", "Content"))
  kw_width   <- run_kw(lib_data, "Width",   c("Operation", "Depth", "Content"))
  kw_content <- run_kw(lib_data, "Content", c("Operation", "Depth", "Width"))

  lib_results <- bind_rows(kw_depth, kw_width, kw_content) %>%
    mutate(Library = lib)

  all_per_lib <- bind_rows(all_per_lib, lib_results)

  lib_results %>%
    group_by(factor_tested) %>%
    summarise(
      n_tests         = n(),
      pct_significant = sprintf("%.1f%%", 100 * mean(significant)),
      .groups = "drop"
    ) %>%
    print()

  write_csv(lib_results, file.path(lib_out, "kw_results.csv"))

  # Per-library p-value heatmaps per factor × operation
  for (op in levels(df$Operation)) {
    op_short <- ifelse(op == "Deserialize", "deser", "ser")

    # Depth effect: Width (x) × Content (y)
    sub <- kw_depth %>% filter(Operation == op)
    p <- ggplot(sub, aes(x = factor(Width), y = Content, fill = -log10(kw_p_value))) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sig_label), size = 4) +
      scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
      labs(title = sprintf("%s %s — Energy ~ Depth", lib, op),
           subtitle = "*** p<0.001, ** p<0.01, * p<0.05, ns = not significant",
           x = "Width", y = "Content Type") +
      theme_minimal(base_size = 12) +
      theme(plot.title = element_text(face = "bold"))
    ggsave(file.path(lib_out, sprintf("kw_depth_%s.png", op_short)),
           p, width = 8, height = 4, dpi = 150)

    # Width effect: Depth (x) × Content (y)
    sub <- kw_width %>% filter(Operation == op)
    p <- ggplot(sub, aes(x = factor(Depth), y = Content, fill = -log10(kw_p_value))) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sig_label), size = 4) +
      scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
      labs(title = sprintf("%s %s — Energy ~ Width", lib, op),
           subtitle = "*** p<0.001, ** p<0.01, * p<0.05, ns = not significant",
           x = "Depth", y = "Content Type") +
      theme_minimal(base_size = 12) +
      theme(plot.title = element_text(face = "bold"))
    ggsave(file.path(lib_out, sprintf("kw_width_%s.png", op_short)),
           p, width = 8, height = 4, dpi = 150)

    # Content effect: Depth (x) × Width (y)
    sub <- kw_content %>% filter(Operation == op)
    p <- ggplot(sub, aes(x = factor(Depth), y = factor(Width), fill = -log10(kw_p_value))) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sig_label), size = 4) +
      scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
      labs(title = sprintf("%s %s — Energy ~ Content Type", lib, op),
           subtitle = "*** p<0.001, ** p<0.01, * p<0.05, ns = not significant",
           x = "Depth", y = "Width") +
      theme_minimal(base_size = 12) +
      theme(plot.title = element_text(face = "bold"))
    ggsave(file.path(lib_out, sprintf("kw_content_%s.png", op_short)),
           p, width = 8, height = 5, dpi = 150)
  }

  cat(sprintf("  Plots saved for %s\n", lib))
}

write_csv(all_per_lib, file.path(pl_dir, "kw_per_library_all_results.csv"))

# ===========================================================================
# PART 3: Summary — significance overview across all libraries and factors
# ===========================================================================
cat("\n=== Significance Summary ===\n")

summary_table <- all_per_lib %>%
  group_by(Library, factor_tested) %>%
  summarise(
    n_tests = n(),
    pct_sig = 100 * mean(significant),
    .groups = "drop"
  )

print(summary_table, n = Inf)
write_csv(summary_table, file.path(pl_dir, "significance_summary.csv"))

# Summary heatmap: Library (y) × Factor (x), showing % significant, faceted by operation
summary_by_op <- all_per_lib %>%
  group_by(Library, factor_tested, Operation) %>%
  summarise(pct_sig = 100 * mean(significant), .groups = "drop")

p_summary <- ggplot(summary_by_op,
                    aes(x = factor_tested, y = Library, fill = pct_sig)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.0f%%", pct_sig)), size = 4, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "% Significant", limits = c(0, 100)) +
  labs(title = "Kruskal-Wallis significance across libraries and factors",
       subtitle = "Percentage of tests where p < 0.05",
       x = "Grouping Factor", y = "Library") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"))
ggsave(file.path(pl_dir, "significance_overview.png"), p_summary,
       width = 10, height = 5, dpi = 150)

cat("\nDone.\n")
