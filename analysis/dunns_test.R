library(tidyverse)
library(dunn.test)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedStringBench-measurements.csv")
base_dir <- file.path(script_dir, "plots", "03_dunns_test")
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
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
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
# PART 1: Dunn's test — pairwise library comparisons per workload config
# ===========================================================================
cat("\n=== Dunn's Test: Pairwise Library Comparisons ===\n")

all_dunn_lib <- tibble()

for (op in levels(df$Operation)) {
  for (d in sort(unique(df$Depth))) {
    for (w in sort(unique(df$Width))) {
      for (ct in levels(df$Content)) {
        sub <- df %>% filter(Operation == op, Depth == d, Width == w, Content == ct)
        if (nrow(sub) < 6 || n_distinct(sub$Library) < 2) next

        # dunn.test returns results silently
        dt <- dunn.test(sub$EnergyPerOp, sub$Library, method = "holm",
                        kw = FALSE, table = FALSE, list = FALSE)

        # Parse the comparison names (format: "GroupA - GroupB")
        pairs <- data.frame(
          comparison = dt$comparisons,
          z_stat     = dt$Z,
          p_raw      = dt$P,
          p_adjusted = dt$P.adjusted,
          stringsAsFactors = FALSE
        ) %>%
          mutate(
            Operation  = op,
            Depth      = d,
            Width      = w,
            Content    = ct,
            significant = p_adjusted < 0.05,
            sig_label   = sig_label(p_adjusted)
          )

        all_dunn_lib <- bind_rows(all_dunn_lib, pairs)
      }
    }
  }
}

cat(sprintf("Total pairwise comparisons: %d\n", nrow(all_dunn_lib)))
cat(sprintf("Significant (adjusted p < 0.05): %d (%.1f%%)\n",
            sum(all_dunn_lib$significant),
            100 * mean(all_dunn_lib$significant)))

# Breakdown by pair
cat("\n--- Significance rate per library pair ---\n")
all_dunn_lib %>%
  group_by(comparison) %>%
  summarise(
    n_tests = n(),
    n_sig   = sum(significant),
    pct_sig = sprintf("%.1f%%", 100 * mean(significant)),
    .groups = "drop"
  ) %>%
  print(n = Inf)

write_csv(all_dunn_lib, file.path(lib_dir, "dunn_library_pairwise_results.csv"))

# --- Library pairwise p-value heatmaps ---
heatmap_data <- all_dunn_lib %>%
  mutate(Workload = sprintf("D%d_W%d", Depth, Width))

for (op in levels(df$Operation)) {
  sub <- heatmap_data %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = comparison, fill = -log10(p_adjusted))) +
    geom_tile(color = "white", linewidth = 0.3) +
    geom_text(aes(label = sig_label), size = 2.5) +
    facet_wrap(~ Content, ncol = 1) +
    scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
    labs(title = sprintf("Dunn's Test: Pairwise Library Comparisons — %s", op),
         subtitle = "Holm-adjusted p-values. *** p<0.001, ** p<0.01, * p<0.05, ns = not significant",
         x = "Workload (Depth x Width)", y = "Library Pair") +
    theme_minimal(base_size = 10) +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(face = "bold"),
          strip.text = element_text(face = "bold"))
  ggsave(file.path(lib_dir, sprintf("dunn_pairwise_%s.png", tolower(op))),
         p, width = 14, height = 10, dpi = 150)
}
cat("Library pairwise plots saved.\n")

# ===========================================================================
# PART 2: Per-library Dunn's test — pairwise comparisons within dimensions
# ===========================================================================
cat("\n=== Per-Library Dunn's Tests ===\n")

run_dunn <- function(data, group_var, fixed_vars) {
  results <- tibble()
  combos <- data %>% distinct(across(all_of(fixed_vars)))

  for (i in seq_len(nrow(combos))) {
    sub <- data
    for (v in fixed_vars) {
      sub <- sub %>% filter(.data[[v]] == combos[[v]][i])
    }
    if (nrow(sub) < 6 || n_distinct(sub[[group_var]]) < 2) next

    dt <- dunn.test(sub$EnergyPerOp, sub[[group_var]], method = "holm",
                    kw = FALSE, table = FALSE, list = FALSE)

    pairs <- data.frame(
      comparison = dt$comparisons,
      z_stat     = dt$Z,
      p_raw      = dt$P,
      p_adjusted = dt$P.adjusted,
      stringsAsFactors = FALSE
    )
    for (v in fixed_vars) {
      pairs[[v]] <- combos[[v]][i]
    }
    pairs$factor_tested <- group_var
    pairs$significant   <- pairs$p_adjusted < 0.05
    pairs$sig_label     <- sig_label(pairs$p_adjusted)

    results <- bind_rows(results, pairs)
  }
  results
}

all_per_lib <- tibble()

for (lib in levels(df$Library)) {
  lib_data <- df %>% filter(Library == lib)
  lib_out  <- file.path(pl_dir, tolower(lib))
  dir.create(lib_out, showWarnings = FALSE, recursive = TRUE)

  cat(sprintf("\n--- %s ---\n", lib))

  dunn_depth   <- run_dunn(lib_data, "Depth",   c("Operation", "Width", "Content"))
  dunn_width   <- run_dunn(lib_data, "Width",   c("Operation", "Depth", "Content"))
  dunn_content <- run_dunn(lib_data, "Content", c("Operation", "Depth", "Width"))

  lib_results <- bind_rows(dunn_depth, dunn_width, dunn_content) %>%
    mutate(Library = lib)

  all_per_lib <- bind_rows(all_per_lib, lib_results)

  lib_results %>%
    group_by(factor_tested) %>%
    summarise(
      n_tests         = n(),
      n_sig           = sum(significant),
      pct_significant = sprintf("%.1f%%", 100 * mean(significant)),
      .groups = "drop"
    ) %>%
    print()

  write_csv(lib_results, file.path(lib_out, "dunn_results.csv"))

  # Per-library p-value heatmaps per factor x operation
  for (op in levels(df$Operation)) {
    op_short <- ifelse(op == "Deserialize", "deser", "ser")

    # Depth pairwise: comparison (y) x Width (x), faceted by Content
    sub <- dunn_depth %>% filter(Operation == op)
    if (nrow(sub) > 0) {
      p <- ggplot(sub, aes(x = factor(Width), y = comparison, fill = -log10(p_adjusted))) +
        geom_tile(color = "white", linewidth = 0.3) +
        geom_text(aes(label = sig_label), size = 3) +
        facet_wrap(~ Content, ncol = 1) +
        scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
        labs(title = sprintf("%s %s — Dunn's: Depth Pairs", lib, op),
             subtitle = "Holm-adjusted. *** p<0.001, ** p<0.01, * p<0.05, ns",
             x = "Width", y = "Depth Pair") +
        theme_minimal(base_size = 10) +
        theme(plot.title = element_text(face = "bold"),
              strip.text = element_text(face = "bold"))
      ggsave(file.path(lib_out, sprintf("dunn_depth_%s.png", op_short)),
             p, width = 8, height = 8, dpi = 150)
    }

    # Width pairwise: comparison (y) x Depth (x), faceted by Content
    sub <- dunn_width %>% filter(Operation == op)
    if (nrow(sub) > 0) {
      p <- ggplot(sub, aes(x = factor(Depth), y = comparison, fill = -log10(p_adjusted))) +
        geom_tile(color = "white", linewidth = 0.3) +
        geom_text(aes(label = sig_label), size = 3) +
        facet_wrap(~ Content, ncol = 1) +
        scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
        labs(title = sprintf("%s %s — Dunn's: Width Pairs", lib, op),
             subtitle = "Holm-adjusted. *** p<0.001, ** p<0.01, * p<0.05, ns",
             x = "Depth", y = "Width Pair") +
        theme_minimal(base_size = 10) +
        theme(plot.title = element_text(face = "bold"),
              strip.text = element_text(face = "bold"))
      ggsave(file.path(lib_out, sprintf("dunn_width_%s.png", op_short)),
             p, width = 8, height = 8, dpi = 150)
    }

    # Content pairwise: comparison (y) x Depth (x), faceted by Width
    sub <- dunn_content %>% filter(Operation == op)
    if (nrow(sub) > 0) {
      p <- ggplot(sub, aes(x = factor(Depth), y = comparison, fill = -log10(p_adjusted))) +
        geom_tile(color = "white", linewidth = 0.3) +
        geom_text(aes(label = sig_label), size = 3) +
        facet_wrap(~ factor(Width), ncol = 1,
                   labeller = labeller(`factor(Width)` = function(x) paste0("Width ", x))) +
        scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "-log10(p)") +
        labs(title = sprintf("%s %s — Dunn's: Content Type Pairs", lib, op),
             subtitle = "Holm-adjusted. *** p<0.001, ** p<0.01, * p<0.05, ns",
             x = "Depth", y = "Content Pair") +
        theme_minimal(base_size = 10) +
        theme(plot.title = element_text(face = "bold"),
              strip.text = element_text(face = "bold"))
      ggsave(file.path(lib_out, sprintf("dunn_content_%s.png", op_short)),
             p, width = 8, height = 6, dpi = 150)
    }
  }

  cat(sprintf("  Plots saved for %s\n", lib))
}

write_csv(all_per_lib, file.path(pl_dir, "dunn_per_library_all_results.csv"))

# ===========================================================================
# PART 3: Summary — significance overview across all libraries and factors
# ===========================================================================
cat("\n=== Significance Summary ===\n")

# Library pairwise summary
cat("\n--- Library pair significance by operation ---\n")
lib_summary <- all_dunn_lib %>%
  group_by(comparison, Operation) %>%
  summarise(
    n_tests = n(),
    pct_sig = sprintf("%.1f%%", 100 * mean(significant)),
    .groups = "drop"
  )
print(lib_summary, n = Inf)
write_csv(lib_summary, file.path(lib_dir, "dunn_library_pair_summary.csv"))

# Per-library factor summary
cat("\n--- Per-library factor significance by operation ---\n")
factor_summary <- all_per_lib %>%
  group_by(Library, factor_tested, Operation) %>%
  summarise(
    n_tests = n(),
    pct_sig = 100 * mean(significant),
    .groups = "drop"
  )
print(factor_summary, n = Inf)
write_csv(factor_summary, file.path(pl_dir, "dunn_factor_significance_summary.csv"))

# Summary heatmap: Library pair (y) x Operation, showing % significant
p_lib_summary <- ggplot(lib_summary %>% mutate(pct_sig_num = parse_number(pct_sig)),
                        aes(x = Operation, y = comparison, fill = pct_sig_num)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = pct_sig), size = 4, fontface = "bold") +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "% Significant", limits = c(0, 100)) +
  labs(title = "Dunn's Test: % significant pairwise comparisons per library pair",
       subtitle = "Holm-adjusted p < 0.05",
       x = "Operation", y = "Library Pair") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"))
ggsave(file.path(lib_dir, "dunn_library_pair_overview.png"), p_lib_summary,
       width = 8, height = 6, dpi = 150)

# Per-library factor summary heatmap
p_factor_summary <- ggplot(factor_summary,
                           aes(x = factor_tested, y = Library, fill = pct_sig)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.0f%%", pct_sig)), size = 4, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "% Significant", limits = c(0, 100)) +
  labs(title = "Dunn's Test: % significant pairwise comparisons per library and factor",
       subtitle = "Holm-adjusted p < 0.05",
       x = "Grouping Factor", y = "Library") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"))
ggsave(file.path(pl_dir, "dunn_factor_overview.png"), p_factor_summary,
       width = 10, height = 5, dpi = 150)

cat("\nDone.\n")
