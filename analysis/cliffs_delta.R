library(tidyverse)
library(effsize)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
variant <- Sys.getenv("BENCH_VARIANT", "Byte")
stopifnot(variant %in% c("Byte", "String"))
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      sprintf("JsonBench.Benchmarks.Factorial.FactorialNormalized%sBench-measurements.csv", variant))
base_dir <- file.path(script_dir, "plots", tolower(variant), "04_cliffs_delta")
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
                        aes(x = Operation, y = comparison, fill = mean_delta)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("δ=%.2f\n%.0f%% large", mean_delta, pct_large)),
            size = 3.5, fontface = "bold") +
  scale_y_discrete(limits = rev(pair_order)) +
  scale_fill_gradient2(low = "#1565C0", mid = "#FFF9C4", high = "#D32F2F",
                       midpoint = 0, name = "Mean δ", limits = c(-1, 1)) +
  labs(title = "Cliff's Delta: Mean signed effect per library pair",
       subtitle = "Fill = mean δ across 48 cells; δ < 0 means first library uses LESS energy. Text: mean δ / % of cells with |δ| ≥ 0.474.",
       x = "Operation", y = "Library Pair") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"),
        plot.subtitle = element_text(size = 9))
ggsave(file.path(lib_dir, "cd_library_pair_overview.png"), p_lib_summary,
       width = 9, height = 6, dpi = 150)

# --- Write README describing the library-level CSVs ---
readme_lib <- r"[Cliff's Delta — library-pair outputs

This directory holds effect-size results from the factorial (48-cell) design:
  Depth x Width x Content = 4 x 4 x 3 = 48 workload cells, per operation.

Files
-----
- cd_library_pairwise_results.csv : per-cell delta. One row per (library pair, operation, workload cell). 10 pairs x 2 ops x 48 cells = 960 rows.
- cd_library_pair_summary.csv     : aggregated across the 48 cells. One row per (library pair, operation). 10 pairs x 2 ops = 20 rows.
- cd_library_<op>.png             : per-cell heatmap of delta for each pair. Supporting material only; per-cell delta is unstable for close library pairs.
- cd_library_pair_overview.png    : headline summary — mean signed delta (fill) and percent large cells (text) per pair x operation.

Column definitions — per-cell (cd_library_pairwise_results.csv)
---------------------------------------------------------------
LibA, LibB       Two libraries being compared.
comparison       Formatted pair string, e.g. SpanJson vs Utf8Json.
Operation        Serialize or Deserialize.
Depth, Width     Workload cell coordinates (values used in the factorial matrix).
Content          T / N / B -> Textual / Numeric / Boolean.
delta            Cliff's delta in [-1, 1]. Sign convention: delta < 0 means LibA uses LESS energy than LibB in this cell. delta = +/- 1 means complete rank separation (every sample of one group is above the other).
delta_abs        |delta|, magnitude only, direction blind.
magnitude        Romano (2006) category:
                   negligible : |delta| < 0.147
                   small      : |delta| < 0.33
                   medium     : |delta| < 0.474
                   large      : |delta| >= 0.474
ci_lower/upper   Bootstrap 95 percent CI bounds for delta.

Column definitions — aggregate (cd_library_pair_summary.csv)
------------------------------------------------------------
comparison, Operation   As above.
mean_delta              Mean of delta across the 48 cells. Direction-sensitive. Answers: on average, which library wins? Near 0 means direction cancels out across cells.
mean_abs_delta          Mean of |delta|. Direction-blind. Answers: on average, how big is the effect?
median_delta            Median of delta across cells. Robust to a few outlier cells.
pct_large               Percentage of the 48 cells where |delta| >= 0.474 (Romano's large threshold). Answers: how often is there any meaningful effect at all? 100 means every workload cell showed a large effect, regardless of direction.

Reading mean_delta and pct_large together
-----------------------------------------
  pct_large high, |mean_delta| high : consistent winner across the workload grid.
  pct_large high, |mean_delta| ~ 0  : big effect in every cell, but direction flips cell-to-cell -> effectively tied on average.
  pct_large low                     : effect is rarely meaningful; pair is close on most workloads.

Why we report aggregates, not per-cell
--------------------------------------
Cliff's delta is rank-based and ignores effect magnitude. When two libraries are practically tied (tiny mean difference, tight within-group variance), a small noise-driven shift between runs can flip every pairwise comparison, so delta swings from +1 to -1 with confident CIs in both directions. The aggregate metrics (mean_delta, pct_large) are much more stable between runs than individual cell deltas.
]"
writeLines(readme_lib, file.path(lib_dir, "README.md"))

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
