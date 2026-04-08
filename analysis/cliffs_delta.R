library(tidyverse)
library(effsize)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedStringBench-measurements.csv")
base_dir <- file.path(script_dir, "plots", "04_cliffs_delta")
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

# Cliff's Delta magnitude labels (standard thresholds)
delta_magnitude <- function(d) {
  ad <- abs(d)
  case_when(
    ad < 0.147 ~ "negligible",
    ad < 0.33  ~ "small",
    ad < 0.474 ~ "medium",
    TRUE       ~ "large"
  )
}

# ===========================================================================
# PART 1: Cliff's Delta — pairwise library comparisons per workload config
# ===========================================================================
cat("\n=== Cliff's Delta: Pairwise Library Comparisons ===\n")

lib_levels <- levels(df$Library)
lib_pairs  <- combn(lib_levels, 2, simplify = FALSE)

all_cd_lib <- tibble()

for (op in levels(df$Operation)) {
  for (d in sort(unique(df$Depth))) {
    for (w in sort(unique(df$Width))) {
      for (ct in levels(df$Content)) {
        sub <- df %>% filter(Operation == op, Depth == d, Width == w, Content == ct)
        if (nrow(sub) == 0) next

        for (pair in lib_pairs) {
          a <- sub %>% filter(Library == pair[1]) %>% pull(EnergyPerOp)
          b <- sub %>% filter(Library == pair[2]) %>% pull(EnergyPerOp)
          if (length(a) < 2 || length(b) < 2) next

          cd <- cliff.delta(a, b)

          all_cd_lib <- bind_rows(all_cd_lib, tibble(
            LibA       = pair[1],
            LibB       = pair[2],
            comparison = sprintf("%s vs %s", pair[1], pair[2]),
            Operation  = op,
            Depth      = d,
            Width      = w,
            Content    = ct,
            delta      = cd$estimate,
            delta_abs  = abs(cd$estimate),
            magnitude  = as.character(cd$magnitude),
            ci_lower   = cd$conf.int[1],
            ci_upper   = cd$conf.int[2]
          ))
        }
      }
    }
  }
}

# Enforce pair ordering: fast-to-slow ladder
pair_order <- sapply(lib_pairs, function(p) sprintf("%s vs %s", p[1], p[2]))
all_cd_lib <- all_cd_lib %>%
  mutate(comparison = factor(comparison, levels = pair_order))

cat(sprintf("Total pairwise comparisons: %d\n", nrow(all_cd_lib)))

cat("\n--- Effect size distribution (library pairs) ---\n")
all_cd_lib %>%
  count(magnitude) %>%
  mutate(pct = sprintf("%.1f%%", 100 * n / sum(n))) %>%
  print()

cat("\n--- Magnitude breakdown per library pair ---\n")
all_cd_lib %>%
  group_by(comparison) %>%
  summarise(
    n          = n(),
    mean_delta = sprintf("%.3f", mean(delta)),
    negligible = sum(magnitude == "negligible"),
    small      = sum(magnitude == "small"),
    medium     = sum(magnitude == "medium"),
    large      = sum(magnitude == "large"),
    .groups = "drop"
  ) %>%
  print(n = Inf)

write_csv(all_cd_lib, file.path(lib_dir, "cd_library_pairwise_results.csv"))

# --- Library pairwise heatmaps: delta per workload config ---
# Order workloads by Depth then Width
workload_order <- df %>%
  distinct(Depth, Width) %>%
  arrange(Depth, Width) %>%
  mutate(Workload = sprintf("D%d_W%d", Depth, Width)) %>%
  pull(Workload)

heatmap_data <- all_cd_lib %>%
  mutate(Workload = factor(sprintf("D%d_W%d", Depth, Width), levels = workload_order))

for (op in levels(df$Operation)) {
  sub <- heatmap_data %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = comparison, fill = delta)) +
    geom_tile(color = "white", linewidth = 0.3) +
    geom_text(aes(label = sprintf("%.2f", delta)), size = 2.2) +
    facet_wrap(~ Content, ncol = 1) +
    scale_y_discrete(limits = rev(pair_order)) +
    scale_fill_gradient2(low = "#1565C0", mid = "#FFF9C4", high = "#D32F2F",
                         midpoint = 0, name = "Cliff's δ",
                         limits = c(-1, 1)) +
    labs(title = sprintf("Cliff's Delta: Library Pairs — %s", op),
         subtitle = "δ < 0: first library uses LESS energy. |δ| < 0.147 negligible, < 0.33 small, < 0.474 medium, ≥ 0.474 large",
         x = "Workload (Depth x Width)", y = "Library Pair") +
    theme_minimal(base_size = 10) +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(face = "bold"),
          strip.text = element_text(face = "bold"),
          plot.subtitle = element_text(size = 8))
  ggsave(file.path(lib_dir, sprintf("cd_library_%s.png", tolower(op))),
         p, width = 14, height = 10, dpi = 150)
}

# --- Summary heatmap: mean |delta| per library pair x operation ---
lib_pair_summary <- all_cd_lib %>%
  group_by(comparison, Operation) %>%
  summarise(
    mean_delta     = mean(delta),
    mean_abs_delta = mean(delta_abs),
    median_delta   = median(delta),
    pct_large      = 100 * mean(magnitude == "large"),
    .groups = "drop"
  )

write_csv(lib_pair_summary, file.path(lib_dir, "cd_library_pair_summary.csv"))

p_lib_summary <- ggplot(lib_pair_summary,
                        aes(x = Operation, y = comparison, fill = mean_abs_delta)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.2f", mean_abs_delta)), size = 4, fontface = "bold") +
  scale_y_discrete(limits = rev(pair_order)) +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F",
                      name = "Mean |δ|", limits = c(0, 1)) +
  labs(title = "Cliff's Delta: Mean effect size per library pair",
       subtitle = "|δ| < 0.147 negligible, < 0.33 small, < 0.474 medium, ≥ 0.474 large",
       x = "Operation", y = "Library Pair") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"))
ggsave(file.path(lib_dir, "cd_library_pair_overview.png"), p_lib_summary,
       width = 8, height = 6, dpi = 150)

cat("Library pairwise plots saved.\n")

# ===========================================================================
# PART 2: Per-library Cliff's Delta — pairwise within Depth, Width, Content
# ===========================================================================
cat("\n=== Per-Library Cliff's Delta ===\n")

run_cd <- function(data, group_var, fixed_vars) {
  levels_vec <- sort(unique(data[[group_var]]))
  if (is.factor(levels_vec)) levels_vec <- levels(levels_vec)
  pairs <- combn(levels_vec, 2, simplify = FALSE)

  results <- tibble()
  combos <- data %>% distinct(across(all_of(fixed_vars)))

  for (i in seq_len(nrow(combos))) {
    sub <- data
    for (v in fixed_vars) {
      sub <- sub %>% filter(.data[[v]] == combos[[v]][i])
    }

    for (pair in pairs) {
      a <- sub %>% filter(.data[[group_var]] == pair[1]) %>% pull(EnergyPerOp)
      b <- sub %>% filter(.data[[group_var]] == pair[2]) %>% pull(EnergyPerOp)
      if (length(a) < 2 || length(b) < 2) next

      cd <- cliff.delta(a, b)

      row <- tibble(
        comparison    = sprintf("%s vs %s", pair[1], pair[2]),
        level_a       = as.character(pair[1]),
        level_b       = as.character(pair[2]),
        delta         = cd$estimate,
        delta_abs     = abs(cd$estimate),
        magnitude     = as.character(cd$magnitude),
        ci_lower      = cd$conf.int[1],
        ci_upper      = cd$conf.int[2],
        factor_tested = group_var
      )
      for (v in fixed_vars) {
        row[[v]] <- combos[[v]][i]
      }
      results <- bind_rows(results, row)
    }
  }
  results
}

all_per_lib <- tibble()

for (lib in levels(df$Library)) {
  lib_data <- df %>% filter(Library == lib)
  lib_out  <- file.path(pl_dir, tolower(lib))
  dir.create(lib_out, showWarnings = FALSE, recursive = TRUE)

  cat(sprintf("\n--- %s ---\n", lib))

  cd_content <- run_cd(lib_data, "Content", c("Operation", "Depth", "Width"))

  lib_results <- cd_content %>%
    mutate(Library = lib)

  all_per_lib <- bind_rows(all_per_lib, lib_results)

  lib_results %>%
    group_by(factor_tested) %>%
    summarise(
      n_pairs    = n(),
      mean_delta = sprintf("%.3f", mean(delta_abs)),
      negligible = sum(magnitude == "negligible"),
      small      = sum(magnitude == "small"),
      medium     = sum(magnitude == "medium"),
      large      = sum(magnitude == "large"),
      .groups = "drop"
    ) %>%
    print()

  write_csv(lib_results, file.path(lib_out, "cd_results.csv"))

  # Per-library heatmaps per factor x operation
  for (op in levels(df$Operation)) {
    op_short <- ifelse(op == "Deserialize", "deser", "ser")

    # Content pairs: comparison (y) x Depth (x), faceted by Width
    sub <- cd_content %>% filter(Operation == op)
    if (nrow(sub) > 0) {
      p <- ggplot(sub, aes(x = factor(Depth), y = comparison, fill = delta)) +
        geom_tile(color = "white", linewidth = 0.3) +
        geom_text(aes(label = sprintf("%.2f", delta)), size = 3) +
        facet_wrap(~ factor(Width), ncol = 1,
                   labeller = labeller(`factor(Width)` = function(x) paste0("Width ", x))) +
        scale_fill_gradient2(low = "#1565C0", mid = "#FFF9C4", high = "#D32F2F",
                             midpoint = 0, name = "δ", limits = c(-1, 1)) +
        labs(title = sprintf("%s %s — Cliff's Delta: Content Type Pairs", lib, op),
             subtitle = "δ > 0: first type uses MORE energy",
             x = "Depth", y = "Content Pair") +
        theme_minimal(base_size = 10) +
        theme(plot.title = element_text(face = "bold"),
              strip.text = element_text(face = "bold"))
      ggsave(file.path(lib_out, sprintf("cd_content_%s.png", op_short)),
             p, width = 8, height = 6, dpi = 150)
    }
  }

  cat(sprintf("  Plots saved for %s\n", lib))
}

write_csv(all_per_lib, file.path(pl_dir, "cd_per_library_all_results.csv"))

# ===========================================================================
# PART 3: Summary
# ===========================================================================
cat("\n=== Effect Size Summary ===\n")

# Per-library factor summary
factor_summary <- all_per_lib %>%
  group_by(Library, factor_tested, Operation) %>%
  summarise(
    n_pairs        = n(),
    mean_abs_delta = mean(delta_abs),
    pct_large      = 100 * mean(magnitude == "large"),
    .groups = "drop"
  )

cat("\n--- Mean |δ| per library x factor x operation ---\n")
print(factor_summary, n = Inf)
write_csv(factor_summary, file.path(pl_dir, "cd_factor_summary.csv"))

# Summary heatmap: mean |delta| per library x factor, faceted by operation
p_factor <- ggplot(factor_summary,
                   aes(x = factor_tested, y = Library, fill = mean_abs_delta)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.2f", mean_abs_delta)), size = 4, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F",
                      name = "Mean |δ|", limits = c(0, 1)) +
  labs(title = "Cliff's Delta: Mean effect size per library and factor",
       subtitle = "|δ| < 0.147 negligible, < 0.33 small, < 0.474 medium, ≥ 0.474 large",
       x = "Grouping Factor", y = "Library") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"))
ggsave(file.path(pl_dir, "cd_factor_overview.png"), p_factor,
       width = 10, height = 5, dpi = 150)

cat("\nDone.\n")
