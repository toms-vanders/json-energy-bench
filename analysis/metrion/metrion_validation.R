library(dplyr)
library(tidyr)
library(ggplot2)
library(readr)
library(xtable)

# -- Constants -----------------------------------------------------------------

RAPL_UNCLEAN0_GLOB    <- "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv0_EnergyDiagnoser_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"
METRION_UNCLEAN0_GLOB <- "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv0_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"
OUT_DIR               <- "figures/metrion/validation"

preferred  <- c("SpanJson", "Utf8Json", "STJSrcGen", "STJRefGen", "Newtonsoft")
lib_colors <- c(
  "SpanJson"   = "#0072B2", "Utf8Json"   = "#009E73",
  "STJRefGen"  = "#F0E442", "STJSrcGen"  = "#CC79A7", "Newtonsoft" = "#D55E00"
)

dir.create(OUT_DIR, recursive = TRUE, showWarnings = FALSE)

# -- Shared helpers ------------------------------------------------------------

read_rapl_runs <- function(glob, n_runs = 5) {
  files <- head(sort(Sys.glob(glob)), n_runs)
  if (length(files) == 0) stop(sprintf("No RAPL files found for glob: %s", glob))
  lapply(seq_along(files), function(i) {
    read.csv(files[[i]], check.names = FALSE) |>
      transmute(Method, Run = i, PkgE0 = parse_number(`PkgE0 (uJ/op)`))
  }) |> bind_rows()
}

read_metrion_runs <- function(glob, n_runs = 5) {
  files <- head(sort(Sys.glob(glob)), n_runs)
  if (length(files) == 0) stop(sprintf("No Metrion files found for glob: %s", glob))
  lapply(seq_along(files), function(i) {
    read.csv(files[[i]], check.names = FALSE) |>
      transmute(Method, Run = i, Metrion = as.numeric(`Metrion CPU Energy (uJ/op)`))
  }) |> bind_rows()
}

parse_methods <- function(df) {
  df |>
    separate(Method, into = c("Library", "Operation"), sep = "_") |>
    mutate(
      Operation = factor(Operation, levels = c("Deser", "Ser"),
                         labels = c("Deserialization", "Serialization")),
      Library   = factor(Library, levels = c(
        intersect(preferred, unique(Library)),
        setdiff(unique(Library), preferred)
      ))
    )
}

# Read raw runs once; each section aggregates as needed.
rapl_raw    <- read_rapl_runs(RAPL_UNCLEAN0_GLOB)
metrion_raw <- read_metrion_runs(METRION_UNCLEAN0_GLOB)

# == 1. Ratio bar chart ========================================================
# metrion_energydiagnoser_ratio_barchart.R
# Bar chart of EnergyDiagnoser/Metrion mean uJ/op ratio per library + operation.

rapl_mean    <- rapl_raw    |> group_by(Method) |> summarise(MeanRAPL    = mean(PkgE0,   na.rm = TRUE), .groups = "drop")
metrion_mean <- metrion_raw |> group_by(Method) |> summarise(MeanMetrion = mean(Metrion, na.rm = TRUE), .groups = "drop")

ratio_df <- inner_join(rapl_mean, metrion_mean, by = "Method") |>
  mutate(Ratio = MeanRAPL / MeanMetrion) |>
  parse_methods()

p_ratio <- ggplot(ratio_df, aes(x = Library, y = Ratio, fill = Library)) +
  geom_col(width = 0.6) +
  geom_hline(yintercept = 1, linetype = "dashed", colour = "grey40", linewidth = 0.7) +
  geom_text(aes(label = sprintf("%.2f", Ratio)),
            vjust = -0.4, size = 3.5, colour = "black") +
  facet_wrap(~ Operation, nrow = 2) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.12))) +
  scale_fill_manual(values = lib_colors, guide = "none") +
  labs(
    x = NULL, y = "EnergyDiagnoser / MetrionProfiler (ratio)"
  ) +
  theme_minimal(base_size = 13) +
  theme(
    panel.grid.major.x = element_blank(),
    strip.text         = element_text(face = "bold"),
    axis.text.x        = element_text(angle = 20, hjust = 1),
    axis.title.y       = element_text(margin = margin(r = 12)),
    panel.spacing      = unit(2, "lines")
  )

path_ratio <- file.path(OUT_DIR, "ratio_barchart.pdf")
ggsave(path_ratio, p_ratio, width = 12, height = 7.5, device = "pdf")
cat(sprintf("Saved %s\n", path_ratio))

# == 2. Normalized ratio plots =================================================
# metrion_energydiagnoser_normalized_ratio.R
# Three plots: grouped bar (normalized energy per tool), scatter, and delta bar + CSV.

rapl_norm    <- rapl_raw    |> group_by(Method) |> summarise(Mean = mean(PkgE0,   na.rm = TRUE), .groups = "drop") |> mutate(Tool = "EnergyDiagnoser")
metrion_norm <- metrion_raw |> group_by(Method) |> summarise(Mean = mean(Metrion, na.rm = TRUE), .groups = "drop") |> mutate(Tool = "MetrionProfiler")

norm_df <- bind_rows(rapl_norm, metrion_norm) |>
  parse_methods() |>
  group_by(Tool, Operation) |>
  mutate(NormRatio = Mean / min(Mean, na.rm = TRUE)) |>
  ungroup() |>
  mutate(Tool = factor(Tool, levels = c("EnergyDiagnoser", "MetrionProfiler")))

tool_colors <- c("EnergyDiagnoser" = "#2166AC", "MetrionProfiler" = "#D6604D")

# 2a: Grouped bar chart -- side-by-side bars per library, height = normalized energy.
p_norm_grouped <- ggplot(norm_df, aes(x = Library, y = NormRatio, fill = Tool)) +
  geom_col(position = position_dodge(width = 0.72), width = 0.68) +
  geom_hline(yintercept = 1, linetype = "dashed", colour = "grey40", linewidth = 0.7) +
  geom_text(aes(label = sprintf("%.2f", NormRatio)),
            position = position_dodge(width = 0.72),
            vjust = -0.4, size = 3, colour = "black") +
  facet_wrap(~ Operation, nrow = 2) +
  scale_y_continuous(expand = expansion(mult = c(0, 0.15))) +
  scale_fill_manual(values = tool_colors, name = "Tool") +
  labs(
    x = NULL, y = "Normalized energy (1 = lowest-energy library)"
  ) +
  theme_minimal(base_size = 13) +
  theme(
    panel.grid.major.x = element_blank(),
    strip.text         = element_text(face = "bold"),
    axis.text.x        = element_text(angle = 20, hjust = 1),
    axis.title.y       = element_text(margin = margin(r = 12)),
    panel.spacing      = unit(1, "lines"),
    legend.position    = "bottom"
  )

path_norm_grouped <- file.path(OUT_DIR, "normalized_ratios_grouped.pdf")
ggsave(path_norm_grouped, p_norm_grouped, width = 10, height = 5, device = "pdf")
cat(sprintf("Saved %s\n", path_norm_grouped))

# 2b: Scatter -- RAPL ratio vs. Metrion ratio; points near diagonal = agreement.
scatter_df <- norm_df |>
  select(Library, Operation, Tool, NormRatio) |>
  pivot_wider(names_from = Tool, values_from = NormRatio)

axis_max <- max(c(scatter_df$EnergyDiagnoser, scatter_df$MetrionProfiler), na.rm = TRUE) * 1.08

p_norm_scatter <- ggplot(scatter_df, aes(x = EnergyDiagnoser, y = MetrionProfiler,
                                          colour = Library, shape = Operation)) +
  geom_abline(slope = 1, intercept = 0,
              linetype = "dashed", colour = "grey40", linewidth = 0.7) +
  geom_point(size = 4, stroke = 0.9) +
  geom_text(aes(label = Library),
            nudge_y = axis_max * 0.025, size = 3,
            show.legend = FALSE) +
  scale_colour_manual(values = lib_colors, name = "Library") +
  scale_shape_manual(values = c("Deserialization" = 16, "Serialization" = 17),
                     name = "Operation") +
  coord_equal(xlim = c(1, axis_max), ylim = c(1, axis_max)) +
  labs(
    x = "EnergyDiagnoser normalized energy",
    y = "MetrionProfiler normalized energy"
  ) +
  theme_minimal(base_size = 13) +
  theme(
    legend.position = "right",
    axis.title      = element_text(margin = margin(r = 8))
  )

path_norm_scatter <- file.path(OUT_DIR, "normalized_ratios_scatter.pdf")
ggsave(path_norm_scatter, p_norm_scatter, width = 9, height = 8, device = "pdf")
cat(sprintf("Saved %s\n", path_norm_scatter))

# 2c: Delta bar chart (EnergyDiagnoser ratio - Metrion ratio) + CSV export.
delta_df <- scatter_df |>
  mutate(Delta = EnergyDiagnoser - MetrionProfiler)

ratios_csv <- delta_df |>
  rename(
    EnergyDiagnoser_Ratio = EnergyDiagnoser,
    MetrionProfiler_Ratio = MetrionProfiler,
    Ratio_Delta           = Delta
  ) |>
  mutate(Abs_Ratio_Delta = abs(Ratio_Delta)) |>
  arrange(Operation, match(as.character(Library), preferred))

write.csv(ratios_csv,
          file.path(OUT_DIR, "normalized_ratios.csv"),
          row.names = FALSE)
cat(sprintf("Saved %s\n", file.path(OUT_DIR, "normalized_ratios.csv")))

p_norm_delta <- ggplot(delta_df, aes(x = Library, y = Delta, fill = Library)) +
  geom_col(width = 0.6) +
  geom_hline(yintercept = 0, linetype = "dashed", colour = "grey40", linewidth = 0.7) +
  geom_text(aes(label = sprintf("%+.2f", Delta)),
            vjust = ifelse(delta_df$Delta >= 0, -0.4, 1.2),
            size = 3.5, colour = "black") +
  facet_wrap(~ Operation, nrow = 2) +
  scale_y_continuous(expand = expansion(mult = c(0.12, 0.12))) +
  scale_fill_manual(values = lib_colors, guide = "none") +
  labs(
    x = NULL, y = "Delta normalized energy (EnergyDiagnoser - Metrion)"
  ) +
  theme_minimal(base_size = 13) +
  theme(
    panel.grid.major.x = element_blank(),
    strip.text         = element_text(face = "bold"),
    axis.text.x        = element_text(angle = 20, hjust = 1),
    axis.title.y       = element_text(margin = margin(r = 12)),
    panel.spacing      = unit(2, "lines")
  )

path_norm_delta <- file.path(OUT_DIR, "normalized_ratios_delta.pdf")
ggsave(path_norm_delta, p_norm_delta, width = 8, height = 5, device = "pdf")
cat(sprintf("Saved %s\n", path_norm_delta))

# == 3. Ratio heatmap (all environments) ======================================
# metrion_energydiagnoser_ratio_heatmap.R
# Heatmap of EnergyDiagnoser/Metrion ratio across all unclean-env load levels.

aggregate_rapl_median <- function(glob, n_runs = 5) {
  read_rapl_runs(glob, n_runs) |>
    group_by(Method) |>
    summarise(PkgE0 = median(PkgE0, na.rm = TRUE), .groups = "drop")
}

build_ratio_data <- function(metrion_runs, rapl_df) {
  metrion_runs |>
    group_by(Method) |>
    summarise(MeanMetrion = mean(Metrion, na.rm = TRUE), .groups = "drop") |>
    inner_join(rapl_df, by = "Method") |>
    mutate(Ratio = PkgE0 / MeanMetrion) |>
    parse_methods()
}

overview_configs <- list(
  list(label = "Clean",        n_runs = 5,
       glob = "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_CleanEnv_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"),
  list(label = "Unclean 0%",   n_runs = 5, glob = METRION_UNCLEAN0_GLOB),
  list(label = "Unclean 25%",  n_runs = 5,
       glob = "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv25_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"),
  list(label = "Unclean 50%",  n_runs = 5,
       glob = "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv50_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"),
  list(label = "Unclean 75%",  n_runs = 5,
       glob = "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv75_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv"),
  list(label = "Unclean 100%", n_runs = 5,
       glob = "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv100_Metrion1000msRaw_ActualStage_Run*/results/JsonBench.Benchmarks.SmokeBenchByte-report.csv")
)

env_levels <- sapply(overview_configs, `[[`, "label")

build_overview <- function(rapl_df) {
  lapply(overview_configs, function(cfg) {
    build_ratio_data(read_metrion_runs(cfg$glob, n_runs = cfg$n_runs), rapl_df) |>
      mutate(EnvLevel = factor(cfg$label, levels = env_levels))
  }) |>
    bind_rows() |>
    mutate(Library = factor(Library, levels = c(
      intersect(preferred, unique(as.character(Library))),
      setdiff(unique(as.character(Library)), preferred)
    )))
}

make_overview_plot <- function(data, title) {
  ggplot(data, aes(x = EnvLevel, y = Library, fill = Ratio)) +
    geom_tile(color = "grey90", linewidth = 0.6) +
    geom_text(aes(label = sprintf("%.2f", Ratio)), size = 3.5, colour = "black") +
    scale_fill_gradientn(
      colours = c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00"),
      name    = "EnergyDiagnoser\n/ MetrionProfiler"
    ) +
    scale_y_discrete(limits = rev(levels(data$Library))) +
    facet_wrap(~ Operation, nrow = 2) +
    labs(
      x = NULL, y = NULL
    ) +
    theme_minimal(base_size = 13) +
    theme(
      panel.grid  = element_blank(),
      strip.text  = element_text(face = "bold"),
      axis.text.x = element_text(angle = 20, hjust = 1)
    )
}

rapl_unclean_median <- aggregate_rapl_median(RAPL_UNCLEAN0_GLOB)
overview_data       <- build_overview(rapl_unclean_median)
p_heatmap           <- make_overview_plot(
  overview_data,
  "EnergyDiagnoser (Unclean 0%, median of 5) vs. Metrion ratio -- all environments"
)
path_heatmap <- file.path(OUT_DIR, "ratio_heatmap_overview.png")
ggsave(path_heatmap, p_heatmap, width = 11, height = 7, dpi = 150)
cat(sprintf("Saved %s\n", path_heatmap))

# == 4. Spearman rank correlation ==============================================
# metrion_energydiagnoser_spearman.R
# Rank scatter plot + Spearman rho table (CSV + LaTeX).

mean_ranks <- bind_rows(
  rapl_raw    |> rename(Energy = PkgE0)   |> mutate(Tool = "EnergyDiagnoser"),
  metrion_raw |> rename(Energy = Metrion) |> mutate(Tool = "MetrionProfiler")
) |>
  parse_methods() |>
  group_by(Tool, Operation, Library) |>
  summarise(MeanEnergy = mean(Energy, na.rm = TRUE), .groups = "drop") |>
  group_by(Tool, Operation) |>
  mutate(Rank = rank(MeanEnergy, ties.method = "first")) |>
  ungroup()

wide <- mean_ranks |>
  select(Library, Operation, Tool, Rank) |>
  pivot_wider(names_from = Tool, values_from = Rank)

spearman_tbl <- wide |>
  group_by(Operation) |>
  summarise(
    rho  = cor.test(EnergyDiagnoser, MetrionProfiler, method = "spearman", exact = TRUE)$estimate,
    pval = cor.test(EnergyDiagnoser, MetrionProfiler, method = "spearman", exact = TRUE)$p.value,
    n    = n(),
    .groups = "drop"
  ) |>
  mutate(label = sprintf("rho = %.3f,  p = %.4f", rho, pval))

cat("\n-- Spearman rho: EnergyDiagnoser vs. Metrion (UncleanEnv 0%) --\n")
print(as.data.frame(spearman_tbl[, c("Operation", "rho", "pval", "n")]),
      row.names = FALSE, digits = 4)
cat("\n")

write.csv(spearman_tbl[, c("Operation", "rho", "pval", "n")],
          file.path(OUT_DIR, "spearman_rho_summary.csv"),
          row.names = FALSE)

annotation_df <- spearman_tbl |>
  mutate(x = 1.1, y = 4.85)

p_spearman <- ggplot(wide, aes(x = EnergyDiagnoser, y = MetrionProfiler, colour = Library)) +
  geom_abline(slope = 1, intercept = 0,
              linetype = "dashed", colour = "grey50", linewidth = 0.7) +
  geom_point(size = 5, stroke = 0.8) +
  geom_text(aes(label = Library),
            nudge_y = 0.22, size = 3.3, fontface = "bold",
            show.legend = FALSE) +
  geom_text(data = annotation_df,
            aes(x = x, y = y, label = label),
            inherit.aes = FALSE,
            hjust = 0, size = 3.8, fontface = "italic", colour = "grey20") +
  facet_wrap(~ Operation) +
  scale_x_continuous(name = "EnergyDiagnoser rank",
                     breaks = 1:5, limits = c(0.6, 5.4)) +
  scale_y_continuous(name = "MetrionProfiler rank",
                     breaks = 1:5, limits = c(0.6, 5.4)) +
  scale_colour_manual(values = lib_colors, guide = "none") +
  coord_equal() +
  labs() +
  theme_minimal(base_size = 13) +
  theme(
    panel.grid.minor = element_blank(),
    strip.text       = element_text(face = "bold"),
    panel.spacing    = unit(2, "lines")
  )

path_spearman <- file.path(OUT_DIR, "spearman_rank_scatter.pdf")
ggsave(path_spearman, p_spearman, width = 10, height = 5.5, device = "pdf")
cat(sprintf("Saved %s\n", path_spearman))

latex_df <- spearman_tbl |>
  mutate(
    rho_fmt  = sprintf("%.3f", rho),
    pval_fmt = ifelse(pval < 0.001, "$<$0.001", sprintf("%.4f", pval))
  ) |>
  select(Operation, `$\\rho$` = rho_fmt, `$p$-value` = pval_fmt, `$n$` = n)

xt <- xtable(
  latex_df,
  caption = paste0(
    "Spearman rank correlation (\\( \\rho \\)) between EnergyDiagnoser and Metrion ",
    "library orderings at UncleanEnv 0\\,\\% background load. ",
    "Ranks are assigned per tool and operation by mean energy across runs ",
    "(rank~1 = lowest energy). Exact permutation p-values; \\( n = 5 \\) libraries."
  ),
  label = "tab:spearman_rho"
)

tex_path <- file.path(OUT_DIR, "spearman_rho_summary.tex")
print(xt,
      file                   = tex_path,
      include.rownames       = FALSE,
      booktabs               = TRUE,
      sanitize.text.function = identity,
      caption.placement      = "top",
      comment                = FALSE)
cat(sprintf("Saved %s\n", tex_path))

# == 5. Run stability (Metrion, all unclean-env levels) ========================
# metrion_stability_over_runs_unclean_env.R
# Per-run line plots and mean-vs-load plots for Metrion across UncleanEnv 0-100%.

STABILITY_OUT_DIR <- file.path(OUT_DIR, "stability")
dir.create(STABILITY_OUT_DIR, recursive = TRUE, showWarnings = FALSE)

PERCENTS    <- c(0, 25, 50, 75, 100)
REPORT_FILE <- "JsonBench.Benchmarks.SmokeBenchByte-report.csv"
ENERGY_COL  <- "Metrion CPU Energy (uJ/op)"

read_pct_runs <- function(pct) {
  glob  <- sprintf(
    "../BenchmarkArtifacts/BenchmarkArtifactsSmoke_All/BenchmarkArtifacts_UncleanEnv%d_Metrion1000msRaw_ActualStage*/results/%s",
    pct, REPORT_FILE
  )
  files <- head(sort(Sys.glob(glob)), 5)
  if (length(files) == 0) stop(sprintf("No files found for UncleanEnv%d", pct))
  run_labels <- sub(".*_Run(\\d+)/results/.*", "Run\\1", files)
  Map(\(f, lbl) {
    read.csv(f, check.names = FALSE)[, c("Method", ENERGY_COL)] |>
      rename(Energy = all_of(ENERGY_COL)) |>
      mutate(Run = lbl)
  }, files, run_labels) |>
    bind_rows() |>
    mutate(Pct = pct)
}

stability_raw <- lapply(PERCENTS, read_pct_runs) |>
  bind_rows() |>
  mutate(
    Pct       = factor(Pct, levels = PERCENTS),
    PctLabel  = factor(paste0("Unclean ", Pct, "%"), levels = paste0("Unclean ", PERCENTS, "%")),
    Operation = factor(
      if_else(grepl("_Ser$", Method), "Serialization", "Deserialization"),
      levels = c("Deserialization", "Serialization")
    ),
    Library   = factor(sub("_.*", "", Method), levels = preferred)
  )

stab_run_levels <- sort(unique(stability_raw$Run))
stability_raw   <- mutate(stability_raw, Run = factor(Run, levels = stab_run_levels))

make_runs_plot <- function(data) {
  first_run <- stab_run_levels[1]
  ggplot(data, aes(x = Run, y = Energy, group = Method, color = Library)) +
    geom_line(linewidth = 0.9) +
    geom_point(size = 2.5) +
    geom_text(
      data = filter(data, Run == first_run),
      aes(label = Library), hjust = 1.1, size = 2.6, show.legend = FALSE
    ) +
    geom_text(
      aes(label = round(Energy, 1)),
      vjust = -0.8, size = 2.4, show.legend = FALSE
    ) +
    scale_x_discrete(expand = expansion(add = c(2.8, 0.5))) +
    scale_colour_manual(values = lib_colors, name = "Library") +
    facet_grid(Operation ~ PctLabel) +
    labs(
      x = NULL,
      y = "Energy (uJ/op)"
    ) +
    theme_minimal(base_size = 12) +
    theme(
      legend.position = "right",
      strip.text      = element_text(face = "bold")
    )
}

make_means_plot <- function(data) {
  means <- data |>
    group_by(Library, Operation, Pct, PctLabel) |>
    summarise(MeanEnergy = mean(Energy, na.rm = TRUE), .groups = "drop")

  ggplot(means, aes(x = Pct, y = MeanEnergy, group = Library, color = Library)) +
    geom_line(linewidth = 1) +
    geom_point(size = 3) +
    geom_text(
      data = filter(means, Pct == levels(means$Pct)[1]),
      aes(label = Library), hjust = 1.1, size = 3, show.legend = FALSE
    ) +
    geom_text(
      aes(label = round(MeanEnergy, 1)),
      vjust = -0.8, size = 2.8, show.legend = FALSE
    ) +
    scale_x_discrete(
      labels = paste0(PERCENTS, "%"),
      expand = expansion(add = c(2.5, 0.5))
    ) +
    scale_colour_manual(values = lib_colors, name = "Library") +
    facet_wrap(~ Operation, nrow = 1) +
    labs(
      x = "Unclean environment (%)",
      y = "Mean energy (uJ/op)"
    ) +
    theme_minimal(base_size = 13) +
    theme(
      legend.position = "right",
      strip.text      = element_text(face = "bold")
    )
}

p_runs <- make_runs_plot(stability_raw)
out_runs <- file.path(STABILITY_OUT_DIR, "run_stability.png")
ggsave(out_runs, p_runs, width = 22, height = 10, dpi = 150)
cat(sprintf("Saved %s\n", out_runs))

p_means <- make_means_plot(stability_raw)
out_means <- file.path(STABILITY_OUT_DIR, "mean_by_env_load.png")
ggsave(out_means, p_means, width = 16, height = 7, dpi = 150)
cat(sprintf("Saved %s\n", out_means))