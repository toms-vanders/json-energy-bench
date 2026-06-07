# ===========================================================================
# Metrion isolation energy rank heatmaps, per dimension and background load.
# ===========================================================================
# For each isolation dimension (Width, Depth, Size) and each unclean-environment
# stress level (the number after "UncleanEnv" in the run-directory name:
# 25/50/75/100 %), build rank heatmaps of Metrion per-operation energy across the
# dimension sweep. Cell fill = rank (1 = cheapest) on the yellowish Wistia
# palette. Two figures per dimension:
#   * {dim}_rank_ratio_stress{LVL}.pdf  one per stress level; cell text stacks
#       rank (bold) over ratio-to-cheapest (italic). Operations facet-stacked.
#   * {dim}_rank_all_stress.pdf  combined; primary group = sweep level (facet
#       column), secondary group = load condition on the x-axis with a "Clean"
#       RAPL reference column first. Rank text only.
#
# Data: BenchmarkArtifactsIsolation_UncleanEnv_Metrion/
#         BenchmarkArtifactsIsolation_UncleanEnv{LVL}_..._Run{N}/results/
#           JsonBench.Benchmarks.Isolation.{Stem}ByteBench-report.csv
#       BenchmarkArtifacts_CleanEnv_All/results/  (clean reference, RAPL package)
# Output: analysis/figures/metrion/isolation/
library(tidyverse)

# --- Paths (resolved relative to this script, mirroring isolation_analysis.R) ---
args        <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir  <- if (length(script_path) > 0) dirname(script_path) else "."
repo_root   <- normalizePath(file.path(script_dir, "..", ".."))
metrion_dir <- file.path(repo_root, "BenchmarkArtifacts",
                         "BenchmarkArtifactsIsolation_UncleanEnv_Metrion")
clean_dir   <- file.path(repo_root, "BenchmarkArtifacts", "results")
plot_dir    <- file.path(script_dir, "..", "figures", "metrion", "environmental-effects")
dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)

energy_col   <- "Metrion CPU Energy (uJ/op)"   # Metrion (unclean env) energy
lib_levels   <- c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")
# Yellowish Wistia palette: rank 1 (cool/cheap) -> rank 5 (hot/expensive).
wistia_palette <- c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00")
lib_colors <- c(
  "SpanJson"   = "#0072B2", "Utf8Json"   = "#009E73",
  "STJRefGen"  = "#F0E442", "STJSrcGen"  = "#CC79A7", "Newtonsoft" = "#D55E00"
)

# --- Isolation dimensions to plot ---
# dim_regex captures the level token after the prefix in the method name;
# parse_dim turns that token into the numeric value used to order the sweep.
dimensions <- list(
  list(name = "width", stem = "WidthIsolation",
       dim_regex = "(?<=_W)\\d+", prefix = "W",
       parse_dim = function(x) as.integer(x),
       sweep_label = "Width", x_label = "Width (fields per object)"),
  list(name = "depth", stem = "DepthIsolation",
       dim_regex = "(?<=_D)\\d+", prefix = "D",
       parse_dim = function(x) as.integer(x),
       sweep_label = "Depth", x_label = "Nesting depth"),
  list(name = "size", stem = "SizeIsolation",
       dim_regex = "(?<=_C)[0-9]+K?", prefix = "C",
       # Object-count tokens carry an optional "K" suffix (C1K = 1000); strip it
       # before coercion and scale so the sweep orders numerically.
       parse_dim = function(x) {
         num <- as.integer(sub("K$", "", x))
         ifelse(grepl("K$", x), num * 1000L, num)
       },
       sweep_label = "Size", x_label = "Object count")
)

# --- Discover per-stress-level run directories (shared across dimensions) ---
run_dirs <- list.dirs(metrion_dir, recursive = FALSE, full.names = TRUE)
run_dirs <- run_dirs[grepl("UncleanEnv\\d+_.*_Run\\d+$", basename(run_dirs))]
if (length(run_dirs) == 0) stop("No UncleanEnv run directories found under ", metrion_dir)

run_base_all <- tibble(
  dir    = run_dirs,
  stress = as.integer(str_extract(basename(run_dirs), "(?<=UncleanEnv)\\d+")),
  run_n  = as.integer(str_extract(basename(run_dirs), "(?<=_Run)\\d+"))
)
# Stressed runs only (used for all plots except the CV baseline column).
run_base <- run_base_all %>%
  filter(stress != 0)   # drop the 0% (no-noise) baseline from every figure
if (nrow(run_base) == 0) stop("No non-zero stress-level runs found.")

# Warn if any stress level does not have exactly 5 runs.
runs_per_stress <- run_base %>% count(stress, name = "n_runs")
off <- runs_per_stress %>% filter(n_runs != 5L)
if (nrow(off) > 0)
  warning(sprintf("Expected 5 runs per stress level; got: %s",
                  paste(sprintf("%d%%=%d", off$stress, off$n_runs), collapse = ", ")))

# ===========================================================================
# Per-dimension processing
# ===========================================================================
process_dimension <- function(dim) {
  report_name  <- sprintf("JsonBench.Benchmarks.Isolation.%sByteBench-report.csv", dim$stem)
  dim_plot_dir <- file.path(plot_dir, dim$name)
  dir.create(dim_plot_dir, showWarnings = FALSE, recursive = TRUE)

  run_meta <- run_base_all %>%
    mutate(file = file.path(dir, "results", report_name)) %>%
    filter(file.exists(file))
  if (nrow(run_meta) == 0) {
    cat(sprintf("Skipping %s - no report CSVs found.\n", dim$name))
    return(invisible(NULL))
  }
  cat(sprintf("\n========== %s ==========\n", toupper(dim$name)))

  # Validate per-dimension run counts (a file could be missing for one dimension
  # even if it exists for others).
  dim_runs_per_stress <- run_meta %>% count(stress, name = "n_runs")
  dim_off <- dim_runs_per_stress %>% filter(n_runs != 5L)
  if (nrow(dim_off) > 0)
    warning(sprintf("[%s] Expected 5 runs per stress level; got: %s", dim$name,
                    paste(sprintf("%d%%=%d", dim_off$stress, dim_off$n_runs), collapse = ", ")))
  cat(sprintf("  Runs per stress level: %s\n",
              paste(sprintf("%d%%->%d", dim_runs_per_stress$stress, dim_runs_per_stress$n_runs),
                    collapse = ", ")))

  # --- Load + parse every run's report for this dimension ---
  parse_run <- function(file, stress, run_n) {
    read_csv(file, show_col_types = FALSE) %>%
      transmute(
        Stress      = stress,
        Run         = run_n,
        Library     = str_extract(Method, "^[^_]+"),
        Operation   = str_extract(Method, "(?<=_)(Deser|Ser)"),
        DimRaw      = str_extract(Method, dim$dim_regex),
        DimValue    = dim$parse_dim(DimRaw),
        EnergyPerOp = .data[[energy_col]]
      ) %>%
      filter(!is.na(DimValue), !is.na(EnergyPerOp))
  }
  raw <- pmap_dfr(list(run_meta$file, run_meta$stress, run_meta$run_n),
                  ~ parse_run(..1, ..2, ..3))

  # Ordered sweep labels (e.g. C10 < C100 < C1K < C10K < C100K by DimValue).
  lvl_order <- raw %>% distinct(DimRaw, DimValue) %>% arrange(DimValue) %>%
    mutate(lab = paste0(dim$prefix, DimRaw)) %>% pull(lab)

  # --- Mean energy: first collapse within each run, then average across the 5 runs ---
  # Stage 1: per-run mean (guards against BDN emitting multiple rows per method).
  per_run <- raw %>%
    group_by(Stress, Run, Library, Operation, DimValue, DimRaw) %>%
    summarise(RunEnergy = mean(EnergyPerOp), .groups = "drop")

  # Stage 2: mean of the 5 per-run means for each (Stress, Library, Operation, level).
  means <- per_run %>%
    group_by(Stress, Library, Operation, DimValue, DimRaw) %>%
    summarise(MeanEnergy = mean(RunEnergy), .groups = "drop") %>%
    mutate(
      Library   = factor(Library, levels = lib_levels),
      Operation = factor(Operation, levels = c("Deser", "Ser"),
                         labels = c("Deserialize", "Serialize")),
      DimLabel  = factor(paste0(dim$prefix, DimRaw), levels = lvl_order)
    )

  # --- Rank + ratio-to-cheapest within each (Stress, Operation, level) cell ---
  rank_ratio <- means %>%
    group_by(Stress, Operation, DimLabel) %>%
    mutate(
      Rank       = rank(MeanEnergy, ties.method = "min"),
      NormRatio  = MeanEnergy / min(MeanEnergy),
      RatioLabel = sprintf(ifelse(NormRatio >= 10, "(%.1fx)", "(%.2fx)"), NormRatio)
    ) %>%
    ungroup()

  # --- Clean-environment reference ranks (RAPL/EnergyDiagnoser, Package only) ---
  # A different tool and environment, kept purely as a ranking reference column.
  # Ranks are comparable to Metrion's because they are computed within each
  # (Operation, level) cell; the absolute energy units are not comparable.
  clean_file      <- file.path(clean_dir, report_name)
  clean_ranks     <- NULL
  clean_means_ref <- NULL
  if (file.exists(clean_file)) {
    clean_base <- read_csv(clean_file, show_col_types = FALSE) %>%
      transmute(
        Library   = str_extract(Method, "^[^_]+"),
        Operation = str_extract(Method, "(?<=_)(Deser|Ser)"),
        DimRaw    = str_extract(Method, dim$dim_regex),
        DimValue  = dim$parse_dim(DimRaw),
        # Package energy only (DRAM excluded). The " uj" unit suffix is stripped
        # by parse_number. Metrion measures package-domain energy, so ranking
        # against the clean RAPL package energy is the like-for-like comparison.
        EnergyPerOp = parse_number(.data[["PkgE0 (uJ/op)"]])
      ) %>%
      filter(!is.na(DimValue), !is.na(EnergyPerOp)) %>%
      group_by(Library, Operation, DimValue, DimRaw) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop") %>%
      mutate(
        Library   = factor(Library, levels = lib_levels),
        Operation = factor(Operation, levels = c("Deser", "Ser"),
                           labels = c("Deserialize", "Serialize")),
        DimLabel  = factor(paste0(dim$prefix, DimRaw), levels = lvl_order)
      ) %>%
      group_by(Operation, DimLabel) %>%
      mutate(
        Rank       = rank(MeanEnergy, ties.method = "min"),
        NormRatio  = MeanEnergy / min(MeanEnergy),
        RatioLabel = sprintf(ifelse(NormRatio >= 10, "(%.1fx)", "(%.2fx)"), NormRatio)
      ) %>%
      ungroup()
    clean_means_ref <- clean_base %>%
      select(Library, Operation, DimLabel, CleanEnergy = MeanEnergy)
    clean_ranks <- clean_base %>%
      mutate(Group = "Clean") %>%
      select(Library, Operation, DimLabel, Rank, RatioLabel, MeanEnergy, Group)
  } else {
    warning(sprintf("Clean-env %s report not found; combined heatmap omits the Clean column.",
                    dim$name))
  }

  # --- Combined heatmap: Clean reference + all stress levels (rank text only) ---
  # Primary grouping is the sweep level (one facet column per level); the load
  # condition is the secondary group along the x-axis: a "Clean" RAPL reference
  # column first, then the Metrion background-load levels. Operation is the facet
  # row. The ratio-to-best label is dropped here to keep the dense grid readable.
  stressed_levels <- sort(unique(rank_ratio$Stress[rank_ratio$Stress != 0]))
  group_levels <- c(if (!is.null(clean_ranks)) "Clean",
                    "0%",
                    paste0(stressed_levels, "%"))
  rank_all <- bind_rows(
    clean_ranks,
    rank_ratio %>% mutate(Group = paste0(Stress, "%")) %>%
      select(Library, Operation, DimLabel, Rank, RatioLabel, MeanEnergy, Group)
  ) %>%
    mutate(
      Group       = factor(Group, levels = group_levels),
      EnergyLabel = ifelse(MeanEnergy >= 1000,
                           sprintf("%.1fk", MeanEnergy / 1000),
                           sprintf("%.0f", MeanEnergy))
    )

  p_all <- ggplot(rank_all, aes(x = Group, y = Library, fill = factor(Rank))) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = Rank), nudge_y = 0.18, size = 4, fontface = "bold") +
    geom_text(aes(label = RatioLabel), nudge_y = -0.20, size = 2.8, fontface = "italic") +
    scale_fill_manual(values = setNames(wistia_palette, as.character(1:5)),
                      name = "Rank", drop = FALSE) +
    facet_grid(Operation ~ DimLabel) +
    labs(x = "Load condition (Clean = Energy Diagnoser reference)", y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y = element_text(face = "bold"),
          axis.text.x = element_text(angle = 45, hjust = 1),
          strip.text  = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid  = element_blank())

  # Scale combined width with the number of sweep levels (facet columns).
  cmb_w <- 3 + 2.3 * nlevels(rank_all$DimLabel)
  out_all <- file.path(dim_plot_dir, sprintf("%s_rank_ratio_all_stress.pdf", dim$name))
  ggsave(out_all, p_all, width = cmb_w, height = 8)
  cat(sprintf("  Saved: %s\n", basename(out_all)))

  # --- Rank-only combined heatmap (no ratio labels) ---
  p_all_rank <- ggplot(rank_all, aes(x = Group, y = Library, fill = factor(Rank))) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = Rank), size = 4, fontface = "bold") +
    scale_fill_manual(values = setNames(wistia_palette, as.character(1:5)),
                      name = "Rank", drop = FALSE) +
    facet_grid(Operation ~ DimLabel) +
    labs(x = "Load condition (Clean = Energy Diagnoser reference)", y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y = element_text(face = "bold"),
          axis.text.x = element_text(angle = 45, hjust = 1),
          strip.text  = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid  = element_blank())
  out_all_rank <- file.path(dim_plot_dir, sprintf("%s_rank_all_stress.pdf", dim$name))
  ggsave(out_all_rank, p_all_rank, width = cmb_w, height = 8)
  cat(sprintf("  Saved: %s\n", basename(out_all_rank)))

  # --- Bump chart: rank trajectory across load conditions ---
  # x = load condition (Clean -> stress levels), y = rank (1 = best, at top).
  # One colored line+point per library; faceted by Operation x DimLabel.
  # Crossing lines indicate rank changes driven by background load.
  p_bump <- ggplot(rank_all,
                   aes(x = Group, y = Rank, color = Library, group = Library)) +
    geom_line(linewidth = 1.1) +
    geom_point(size = 3.5) +
    geom_text(aes(label = EnergyLabel), nudge_y = -0.28, size = 2.6, color = "black") +
    scale_y_reverse(breaks = 1:5, minor_breaks = NULL,
                    expand = expansion(add = 0.35)) +
    scale_color_manual(values = lib_colors, drop = FALSE) +
    facet_grid(Operation ~ DimLabel) +
    labs(x = "Load condition (Clean = Energy Diagnoser reference)",
         y = "Rank (1 = cheapest)") +
    theme_minimal(base_size = 14) +
    theme(axis.text.x      = element_text(angle = 45, hjust = 1),
          strip.text        = element_text(face = "bold"),
          legend.position   = "bottom",
          panel.spacing.y   = unit(2, "lines"),
          panel.grid.minor  = element_blank())
  out_bump <- file.path(dim_plot_dir, sprintf("%s_rank_bump.pdf", dim$name))
  ggsave(out_bump, p_bump, width = cmb_w, height = 8)
  cat(sprintf("  Saved: %s\n", basename(out_bump)))

  # --- Rank frequency stacked bar chart ---
  # For each library: how many times it landed on each rank (1-5) across all
  # (DimLabel x Group) cells shown in the combined heatmap. Faceted by operation.
  rank_counts <- rank_all %>%
    count(Library, Operation, Rank) %>%
    mutate(Rank = factor(Rank, levels = 1:5))

  p_bar <- ggplot(rank_counts, aes(x = Library, y = n, fill = Rank)) +
    geom_col(width = 0.7) +
    scale_fill_manual(values = setNames(wistia_palette, as.character(1:5)),
                      name = "Rank", drop = FALSE) +
    scale_y_continuous(breaks = scales::breaks_pretty()) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = "Count") +
    theme_minimal(base_size = 14) +
    theme(axis.text.x  = element_text(angle = 45, hjust = 1, face = "bold"),
          strip.text    = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid.minor = element_blank())
  out_bar <- file.path(dim_plot_dir, sprintf("%s_rank_frequency.pdf", dim$name))
  ggsave(out_bar, p_bar, width = 7, height = 8)
  cat(sprintf("  Saved: %s\n", basename(out_bar)))

  # --- Mean rank heatmap (aggregated across dimension levels) ---
  # Each cell: mean rank across all DimLabel values for that (Library, Operation,
  # load condition). Summarises rank behaviour over the full dimension range into
  # a single value per stress level, making library differences easier to read.
  mean_rank <- rank_all %>%
    group_by(Library, Operation, Group) %>%
    summarise(MeanRank = mean(Rank), .groups = "drop") %>%
    mutate(MeanRankLabel = sprintf("%.2f", MeanRank))

  out_mean_rank_csv <- file.path(dim_plot_dir, sprintf("%s_mean_rank.csv", dim$name))
  write_csv(mean_rank %>% select(Library, Operation, Group, MeanRank), out_mean_rank_csv)
  cat(sprintf("  Saved: %s\n", basename(out_mean_rank_csv)))

  p_mean_rank <- ggplot(mean_rank, aes(x = Group, y = Library, fill = MeanRank)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = MeanRankLabel), size = 4, fontface = "bold", color = "black") +
    scale_fill_gradientn(
      colors = wistia_palette, limits = c(1, 5),
      name = "Mean rank"
    ) +
    facet_wrap(~ Operation, ncol = 2) +
    labs(x = "Load condition (Clean = Energy Diagnoser reference)", y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          axis.text.x     = element_text(angle = 45, hjust = 1),
          strip.text      = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid      = element_blank())

  out_mean_rank <- file.path(dim_plot_dir, sprintf("%s_mean_rank.pdf", dim$name))
  ggsave(out_mean_rank, p_mean_rank, width = 11, height = 5)
  cat(sprintf("  Saved: %s\n", basename(out_mean_rank)))

  # --- Stress-vs-clean ratio heatmap ---
  # Each cell: Metrion mean energy at that stress level divided by the clean-env
  # RAPL mean energy for the same (Library, Operation, DimLabel). A ratio of 1
  # means identical magnitude; >1 means more energy under background load.
  # Cross-tool comparison (Metrion vs RAPL) -- treat as a directional indicator.
  if (!is.null(clean_means_ref)) {
    stress_vs_clean <- means %>%
      select(Stress, Library, Operation, DimLabel, MeanEnergy) %>%
      inner_join(clean_means_ref, by = c("Library", "Operation", "DimLabel")) %>%
      mutate(
        Ratio       = MeanEnergy / CleanEnergy,
        RatioLabel  = sprintf("%.2fx", Ratio),
        StressGroup = factor(paste0(Stress, "%"),
                             levels = paste0(sort(unique(Stress)), "%"))
      )

    p_stress_ratio <- ggplot(stress_vs_clean,
                             aes(x = StressGroup, y = Library, fill = Ratio)) +
      geom_tile(color = "white", linewidth = 0.5) +
      scale_y_discrete(limits = rev) +
      geom_text(aes(label = RatioLabel), size = 3.5, fontface = "bold") +
      scale_fill_gradient2(
        low      = "#4393c3",
        mid      = "#f7f7f7",
        high     = "#d6604d",
        midpoint = median(stress_vs_clean$Ratio, na.rm = TRUE),
        name     = "Metrion Profiler / Energy Diagnoser"
      ) +
      facet_grid(Operation ~ DimLabel) +
      labs(x = "Background load level", y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y  = element_text(face = "bold"),
            axis.text.x  = element_text(angle = 45, hjust = 1),
            strip.text   = element_text(face = "bold"),
            legend.position = "right",
            panel.grid   = element_blank())

    cmb_w_ratio <- 2.5 + 2.8 * nlevels(stress_vs_clean$DimLabel)
    out_stress_ratio <- file.path(dim_plot_dir,
                                  sprintf("%s_stress_vs_clean_ratio.pdf", dim$name))
    ggsave(out_stress_ratio, p_stress_ratio, width = cmb_w_ratio, height = 8)
    cat(sprintf("  Saved: %s\n", basename(out_stress_ratio)))
  }

  # --- CV heatmap: run-to-run variability under background load ---
  # CV = sd / mean across the 5 runs per (Stress, Library, Operation, DimLabel).
  # The 0% stress level is included as a "tool noise baseline": any CV there
  # reflects Metrion's own measurement variability, not the environment. CV
  # increases at higher stress that exceed the 0% baseline are attributable to
  # genuine environmental destabilisation.
  stress_levels_cv <- c("0% (tool baseline)",
                        paste0(stressed_levels, "%"))

  cv_data <- per_run %>%
    group_by(Stress, Library, Operation, DimValue, DimRaw) %>%
    summarise(
      CV = sd(RunEnergy) / mean(RunEnergy),
      .groups = "drop"
    ) %>%
    mutate(
      Library     = factor(Library, levels = lib_levels),
      Operation   = factor(Operation, levels = c("Deser", "Ser"),
                           labels = c("Deserialize", "Serialize")),
      DimLabel    = factor(paste0(dim$prefix, DimRaw), levels = lvl_order),
      StressGroup = factor(ifelse(Stress == 0, "0% (tool baseline)", paste0(Stress, "%")),
                           levels = stress_levels_cv),
      CVLabel     = sprintf("%.1f%%", CV * 100)
    )

  p_cv <- ggplot(cv_data, aes(x = StressGroup, y = Library, fill = CV)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = CVLabel), size = 3.5, fontface = "bold", color = "black") +
    scale_fill_gradient(
      low  = "#fff7ec", high = "#d73027",
      name = "CV (sd/mean)", labels = scales::percent_format(accuracy = 1)
    ) +
    facet_grid(Operation ~ DimLabel) +
    labs(x = "Background load level", y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          axis.text.x     = element_text(angle = 45, hjust = 1),
          strip.text      = element_text(face = "bold"),
          legend.position = "right",
          panel.grid      = element_blank())

  cv_w <- 2 + 2.8 * nlevels(cv_data$DimLabel)
  out_cv <- file.path(dim_plot_dir, sprintf("%s_cv_heatmap.pdf", dim$name))
  ggsave(out_cv, p_cv, width = cv_w, height = 8)
  cat(sprintf("  Saved: %s\n", basename(out_cv)))

  # --- Stress sensitivity curve ---
  # y = MeanEnergy_stress / MeanEnergy_0% (both Metrion). Aggregated across
  # sweep levels (mean ratio across DimLabel values per library). Flat lines
  # mean noise adds a uniform overhead; diverging lines reveal differential
  # sensitivity between libraries. Dashed reference at y = 1 (no change).
  clean_metrion <- means %>%
    filter(Stress == 0) %>%
    select(Library, Operation, DimLabel, CleanMetrion = MeanEnergy)

  if (nrow(clean_metrion) == 0) {
    warning(sprintf("[%s] No 0%% stress Metrion runs found; skipping sensitivity curve.", dim$name))
  } else {
    sensitivity <- means %>%
      filter(Stress != 0) %>%
      inner_join(clean_metrion, by = c("Library", "Operation", "DimLabel")) %>%
      mutate(Ratio = MeanEnergy / CleanMetrion) %>%
      group_by(Stress, Library, Operation) %>%
      summarise(MeanRatio = mean(Ratio), .groups = "drop") %>%
      mutate(StressGroup = factor(paste0(Stress, "%"),
                                  levels = paste0(sort(unique(Stress)), "%")))

    p_sensitivity <- ggplot(sensitivity,
                            aes(x = StressGroup, y = MeanRatio,
                                color = Library, group = Library)) +
      geom_hline(yintercept = 1, linetype = "dashed", color = "grey50", linewidth = 0.8) +
      geom_line(linewidth = 1.1) +
      geom_point(size = 3.5) +
      scale_color_manual(values = lib_colors, drop = FALSE) +
      facet_wrap(~ Operation, ncol = 2) +
      labs(x = "Background load level",
           y = "Energy relative to\nno-load baseline (1.0 = no change)") +
      theme_minimal(base_size = 14) +
      theme(strip.text       = element_text(face = "bold"),
            legend.position  = "bottom",
            panel.grid.minor = element_blank())

    out_sensitivity <- file.path(dim_plot_dir,
                                 sprintf("%s_stress_sensitivity.pdf", dim$name))
    ggsave(out_sensitivity, p_sensitivity, width = 12, height = 5)
    cat(sprintf("  Saved: %s\n", basename(out_sensitivity)))
  }
}

walk(dimensions, process_dimension)
