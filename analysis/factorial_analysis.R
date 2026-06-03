library(tidyverse)
library(effsize)

# ============================================================
# Factorial design (4 Depth x 4 Width x 3 Content = 48 cells).
#
# Combines:
#   - Descriptive plots (bars / scaling / ratio / rank / overview)
#     for the workload-landscape figures in Ch5 sec 5.1.
#   - Statistical pipeline (Shapiro-Wilk -> Kruskal-Wallis -> Cliff's delta)
#     kept as repo-only supporting material; factorial was demoted from
#     formal analytical dimension to descriptive in Ch5.
# ============================================================

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedByteBench-measurements.csv")

# Output directories: descriptive at top level, statistical pipeline under
# stats/, LaTeX tables under tables/ (matches isolation_analysis.R layout).
plot_dir      <- file.path(script_dir, "figures", "factorial")
tables_dir    <- file.path(plot_dir, "tables")
stats_dir     <- file.path(plot_dir, "stats")
norm_dir      <- file.path(stats_dir, "normality")
kw_lib_dir    <- file.path(stats_dir, "kruskal_wallis")
delta_lib_dir <- file.path(stats_dir, "cliffs_delta")

for (d in c(plot_dir, tables_dir, norm_dir, kw_lib_dir, delta_lib_dir)) {
  dir.create(d, showWarnings = FALSE, recursive = TRUE)
}

raw <- read_csv(csv_path, show_col_types = FALSE)

# Parse method names; shared between descriptive (Result) and statistical
# (Actual) pipelines. e.g. "SpanJson_Deser_D10_W100_B"
parse_methods <- function(d) {
  d %>%
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
}

# Descriptive: BDN per-group summary (one row per (lib, op, D, W, content)).
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Result") %>%
  parse_methods() %>%
  mutate(
    DepthLabel  = factor(paste0("Depth ", Depth),
                         levels = paste0("Depth ", c(2, 5, 10, 20))),
    WidthFactor = factor(Width, levels = c(5, 20, 50, 100))
  )

# Statistical: per-sample iterations (Actual), needed by SW / KW / Cliff's delta.
df_actual <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Actual") %>%
  parse_methods()

# Mean energy per combination (used by descriptive plots)
means <- df %>%
  group_by(Library, Operation, Depth, DepthLabel, Width, WidthFactor, Content) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

# BDN's report.csv carries one summary row per benchmark method with
# Allocated bytes and Gen0/Gen1 collection counts. Loaded once here so the
# alloc/gc plots and the LaTeX summary table can share parsing.
report_csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                             "JsonBench.Benchmarks.Factorial.FactorialNormalizedByteBench-report.csv")

report <- if (file.exists(report_csv_path)) {
  read_csv(report_csv_path, show_col_types = FALSE) %>%
    mutate(
      Library    = str_extract(Method, "^[^_]+"),
      Operation  = str_extract(Method, "(?<=_)(Deser|Ser)"),
      Depth      = as.integer(str_extract(Method, "(?<=_D)\\d+")),
      Width      = as.integer(str_extract(Method, "(?<=_W)\\d+")),
      Content    = str_extract(Method, "[TNB]$"),
      # BDN formats Allocated with mixed units ("9016 B", "6.8 KB"); parse_number
      # drops the unit so the multiplier has to be applied explicitly.
      AllocBytes = case_when(
        str_detect(Allocated, "GB") ~ parse_number(Allocated) * 1024^3,
        str_detect(Allocated, "MB") ~ parse_number(Allocated) * 1024^2,
        str_detect(Allocated, "KB") ~ parse_number(Allocated) * 1024,
        TRUE                         ~ parse_number(Allocated)
      )
    ) %>%
    mutate(
      Content   = factor(Content, levels = c("T", "N", "B"),
                         labels = c("Textual", "Numeric", "Boolean")),
      Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
      Operation = factor(Operation, levels = c("Deser", "Ser"),
                         labels = c("Deserialize", "Serialize"))
    )
} else {
  NULL
}

# --- Colors ---
lib_colors <- c(
  "SpanJson"   = "#0072B2",
  "Utf8Json"   = "#009E73",
  "STJRefGen"  = "#F0E442",
  "STJSrcGen"  = "#CC79A7",
  "Newtonsoft" = "#D55E00"
)

# Summarise data per group and append a grand-total row at the bottom.
# group_cols are coerced to character so they can carry the total_label.
# Each named aggregation in `...` is evaluated twice: once per group, once on
# the full data for the total row — n() on the un-grouped pass yields the
# whole-data count, matching the manual nrow() the old code used.
summarise_with_total <- function(data, group_cols, ..., total_label = "All") {
  args <- enquos(...)
  per_group <- data %>%
    group_by(across(all_of(group_cols))) %>%
    summarise(!!!args, .groups = "drop") %>%
    mutate(across(all_of(group_cols), as.character))
  total_header <- as_tibble(setNames(
    as.list(rep(total_label, length(group_cols))), group_cols))
  total <- bind_cols(total_header, data %>% summarise(!!!args))
  bind_rows(per_group, total)
}

# Non-library categories use the unused Okabe-Ito hues
# to avoid clashing with library identity.
content_colors <- c(
  "Textual" = "#E69F00",
  "Numeric" = "#56B4E9",
  "Boolean" = "#999999"
)

# Wistia 5-stop heatmap palette (https://github.com/wistia/heatmap-palette).
# Sequential, deuteranopia-safe by varying luminance: light yellow-green to
# dark orange. Used for rank and ratio-to-best heatmaps.
wistia_palette <- c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00")

# ============================================================
# ============================================================
# PART A: DESCRIPTIVE
# Bar plots, scaling plots, ratio heatmaps, rank heatmaps,
# and 48-cell overview distributions.
# ============================================================
# ============================================================

# ============================================================
# RANKING DATA
# ============================================================
# Used by both the combined rank+ratio heatmap below and the overview
# composition/distribution plots further down.

ranked <- means %>%
  group_by(Operation, Depth, Width, Content) %>%
  mutate(
    Rank = rank(MeanEnergy, ties.method = "min"),
    NormEnergy = MeanEnergy / min(MeanEnergy)
  ) %>%
  ungroup() %>%
  mutate(Config = factor(
    sprintf("D%d_W%d", Depth, Width),
    levels = {
      d <- expand.grid(Depth = c(2, 5, 10, 20), Width = c(5, 20, 50, 100))
      d <- d[order(d$Depth, d$Width), ]
      sprintf("D%d_W%d", d$Depth, d$Width)
    }
  ))

# ============================================================
# COMBINED RANK + RATIO HEATMAP (isolation-style)
# ============================================================
# Per operation, faceted by Content. Each cell shows the library's rank
# (1 = cheapest) bold on top and the ratio-to-best beneath in italic.
# Replaces the older separate heatmap_rank_<op> and heatmap_norm_<op>.

plot_heatmap_rank_ratio <- function(op_label) {
  plot_data <- ranked %>%
    filter(Operation == op_label) %>%
    mutate(RatioLabel = sprintf(ifelse(NormEnergy >= 10, "(%.1fx)", "(%.2fx)"),
                                 NormEnergy))

  p <- ggplot(plot_data, aes(x = Config, y = Library, fill = factor(Rank))) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = Rank), nudge_y = 0.18,
              size = 4.5, fontface = "bold") +
    geom_text(aes(label = RatioLabel), nudge_y = -0.20,
              size = 3.0, fontface = "italic") +
    scale_fill_manual(
      values = setNames(wistia_palette, as.character(1:5)),
      name = "Rank", drop = FALSE
    ) +
    facet_wrap(~ Content, ncol = 1) +
    labs(x = "Workload (Depth x Width)", y = NULL) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text      = element_text(face = "bold", size = 13),
      axis.text.x     = element_text(angle = 30, hjust = 1, size = 9),
      axis.text.y     = element_text(face = "bold"),
      legend.position = "bottom",
      panel.grid      = element_blank()
    )

  fname <- file.path(plot_dir, sprintf("heatmap_rank_ratio_%s.png",
                                       tolower(gsub("ialize", "", op_label))))
  ggsave(fname, p, width = 12, height = 12, dpi = 300)
  cat("Saved:", fname, "\n")
}

# ============================================================
# ALLOCATION + GC CHARTS (isolation-style, from report.csv)
# ============================================================

plot_alloc <- function() {
  if (is.null(report)) {
    cat("Skipping alloc.png: report CSV not found\n")
    return(invisible(NULL))
  }
  pd <- report %>% mutate(AllocKB = AllocBytes / 1024)

  p <- ggplot(pd, aes(x = factor(sprintf("D%d_W%d", Depth, Width),
                                  levels = levels(ranked$Config)),
                       y = AllocKB, fill = Library)) +
    geom_col(position = position_dodge(width = 0.8), width = 0.7) +
    scale_fill_manual(values = lib_colors) +
    facet_grid(Content ~ Operation, scales = "free_y") +
    labs(x = "Workload (Depth x Width)", y = "Allocated (KB/op)", fill = "Library") +
    theme_minimal(base_size = 14) +
    theme(
      strip.text      = element_text(face = "bold"),
      axis.text.x     = element_text(angle = 45, hjust = 1, size = 9),
      legend.position = "bottom"
    )
  ggsave(file.path(plot_dir, "alloc.png"), p, width = 16, height = 10, dpi = 300)
  cat("Saved:", file.path(plot_dir, "alloc.png"), "\n")
}

plot_gc <- function() {
  if (is.null(report)) {
    cat("Skipping gc.png: report CSV not found\n")
    return(invisible(NULL))
  }
  # Mean across the 3 content variants per (Library, Operation, D, W) cell;
  # factorial has Content as a third axis that isolation lacks. Aggregating
  # keeps the layout close to the isolation Operation x Library grid.
  gc_long <- report %>%
    select(Library, Operation, Depth, Width, Gen0, Gen1) %>%
    mutate(across(c(Gen0, Gen1), ~ tidyr::replace_na(.x, 0))) %>%
    group_by(Library, Operation, Depth, Width) %>%
    summarise(Gen0 = mean(Gen0, na.rm = TRUE),
              Gen1 = mean(Gen1, na.rm = TRUE),
              .groups = "drop") %>%
    mutate(Config = factor(sprintf("D%d_W%d", Depth, Width),
                           levels = levels(ranked$Config))) %>%
    pivot_longer(cols = c(Gen0, Gen1),
                 names_to = "Generation", values_to = "Collections") %>%
    mutate(Generation = factor(Generation, levels = c("Gen1", "Gen0")))

  gen_colors <- c("Gen0" = "#56B4E9", "Gen1" = "#E69F00")

  p <- ggplot(gc_long, aes(x = Config, y = Collections, fill = Generation)) +
    geom_col(position = "stack", width = 0.7) +
    scale_fill_manual(values = gen_colors) +
    facet_grid(Library ~ Operation, scales = "free_y") +
    labs(x = "Workload (Depth x Width)",
         y = "GC Collections / 1000 ops (mean across content)",
         fill = "Generation") +
    theme_minimal(base_size = 14) +
    theme(
      strip.text      = element_text(face = "bold"),
      axis.text.x     = element_text(angle = 45, hjust = 1, size = 9),
      legend.position = "bottom"
    )
  ggsave(file.path(plot_dir, "gc.png"), p, width = 14, height = 10, dpi = 300)
  cat("Saved:", file.path(plot_dir, "gc.png"), "\n")
}

# ============================================================
# LOCAL PER-ELEMENT RATIO (isolation-style)
# ============================================================
# Per-step growth ratio between adjacent sweep levels along Depth and Width:
# (E_next / level_next) / (E_now / level_now). Linear scaling reads as 1.0x.
#
# build_local_steps()   -> per-cell raw step data, written to local_steps.csv.
# plot_local_per_element() -> single PNG, facet_grid(Operation ~ Dimension),
#   each cell = median across the orthogonal-dim x Content combos. Content is
#   categorical (no ordinal step pairs) so it's collapsed, not a step axis.

build_local_steps <- function() {
  depth_steps <- means %>%
    group_by(Library, Operation, Content, Width) %>%
    arrange(Depth, .by_group = TRUE) %>%
    mutate(next_Depth  = lead(Depth),
           next_Energy = lead(MeanEnergy)) %>%
    filter(!is.na(next_Depth)) %>%
    mutate(
      Dimension         = "Depth",
      from_level        = Depth,
      to_level          = next_Depth,
      W_ratio           = next_Depth / Depth,
      E_ratio           = next_Energy / MeanEnergy,
      from_per_element  = MeanEnergy   / Depth,
      to_per_element    = next_Energy  / next_Depth,
      per_element_ratio = to_per_element / from_per_element,
      Transition        = sprintf("D%d -> D%d", Depth, next_Depth),
      FixedOther        = sprintf("W=%d", Width)
    ) %>%
    ungroup() %>%
    select(Library, Operation, Dimension, Content, FixedOther,
           from_level, to_level,
           from_E = MeanEnergy, to_E = next_Energy,
           from_per_element, to_per_element,
           W_ratio, E_ratio, per_element_ratio, Transition)

  width_steps <- means %>%
    group_by(Library, Operation, Content, Depth) %>%
    arrange(Width, .by_group = TRUE) %>%
    mutate(next_Width  = lead(Width),
           next_Energy = lead(MeanEnergy)) %>%
    filter(!is.na(next_Width)) %>%
    mutate(
      Dimension         = "Width",
      from_level        = Width,
      to_level          = next_Width,
      W_ratio           = next_Width / Width,
      E_ratio           = next_Energy / MeanEnergy,
      from_per_element  = MeanEnergy   / Width,
      to_per_element    = next_Energy  / next_Width,
      per_element_ratio = to_per_element / from_per_element,
      Transition        = sprintf("W%d -> W%d", Width, next_Width),
      FixedOther        = sprintf("D=%d", Depth)
    ) %>%
    ungroup() %>%
    select(Library, Operation, Dimension, Content, FixedOther,
           from_level, to_level,
           from_E = MeanEnergy, to_E = next_Energy,
           from_per_element, to_per_element,
           W_ratio, E_ratio, per_element_ratio, Transition)

  bind_rows(depth_steps, width_steps)
}

save_local_steps <- function(local_steps) {
  fname <- file.path(plot_dir, "local_steps.csv")
  write_csv(local_steps, fname)
  cat("Saved:", fname, "\n")
}

plot_local_per_element <- function(local_steps) {
  agg <- local_steps %>%
    group_by(Library, Operation, Dimension, Transition) %>%
    summarise(per_element_ratio = median(per_element_ratio, na.rm = TRUE),
              .groups = "drop") %>%
    mutate(
      Dimension  = factor(Dimension, levels = c("Depth", "Width")),
      Transition = factor(Transition, levels = c(
        "D2 -> D5", "D5 -> D10", "D10 -> D20",
        "W5 -> W20", "W20 -> W50", "W50 -> W100"
      )),
      pe_label = ifelse(per_element_ratio >= 10,
                         sprintf("%.1fx", per_element_ratio),
                         sprintf("%.2fx", per_element_ratio))
    )

  pe_log     <- log10(agg$per_element_ratio)
  max_dev_pe <- max(abs(pe_log), na.rm = TRUE)
  pe_limits  <- c(-max_dev_pe, max_dev_pe)

  p <- ggplot(agg, aes(x = Transition, y = Library,
                        fill = log10(per_element_ratio))) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = pe_label), size = 3.5) +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 0, limits = pe_limits,
      name   = "Per-element ratio",
      labels = function(x) sprintf("%.2fx", 10^x)
    ) +
    guides(fill = guide_colourbar(title.position = "top", title.hjust = 0.5,
                                  barwidth  = grid::unit(8, "cm"),
                                  barheight = grid::unit(0.4, "cm"))) +
    facet_grid(Operation ~ Dimension, scales = "free_x") +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(
      axis.text.y     = element_text(face = "bold"),
      axis.text.x     = element_text(angle = 30, hjust = 1),
      strip.text      = element_text(face = "bold"),
      legend.position = "bottom",
      panel.grid      = element_blank()
    )

  fname <- file.path(plot_dir, "local_per_element_ratio.png")
  ggsave(fname, p, width = 12, height = 8, dpi = 300)
  cat("Saved:", fname, "\n")
}

# ============================================================
# OVERVIEW DISTRIBUTIONS AND SUMMARIES
# ============================================================
# Section 5.1 (Factorial Benchmark Overview) artifacts:
#   - overview_distribution_ratio.png : per-library ratio-to-best across
#       the 48-cell grid, faceted by operation (Deserialize / Serialize)
#       into one figure (boxplot + 48 jittered cells coloured by content
#       type). y = 1.0x = cell winner. Grid-scaling effect cancels out
#       since the ratio is computed within each cell, so spread reflects
#       how the gap to the cell-best varies across workloads.
#   - overview_rank_composition.png : per-library stacked bar, faceted by
#       operation (Deserialize / Serialize) into one figure, showing how
#       many of the 48 cells the library placed Rank 1 through 5. Wistia
#       palette (Rank 1 = cool, Rank 5 = hot) matches the per-cell rank
#       heatmap in the appendix so the reader's encoding carries through.
#       Complements the ratio strip-box: rank tells positional standing,
#       ratio tells gap magnitude.
#   - overview_library_summary.csv : per-(library, operation) summary
#       statistics -- absolute energy (median/min/max) and ratio-to-best
#       (median/Q1/Q3/max); source for numerical claims in the chapter prose.
#   - overview_cell_spread.csv : per-cell min/max/spread + winning library;
#       lets the prose cite specific flat-vs-spread configurations.

plot_overview_distribution_ratio <- function() {
  p <- ggplot(ranked, aes(x = Library, y = NormEnergy)) +
    geom_hline(yintercept = 1, linetype = "dashed",
               color = "grey60", linewidth = 0.4) +
    geom_boxplot(width = 0.45, fill = "white", color = "grey25",
                 alpha = 0.9, outlier.shape = NA) +
    geom_jitter(aes(color = Content),
                position = position_jitter(width = 0.18, seed = 42),
                size = 2.1, alpha = 0.8) +
    scale_color_manual(values = content_colors, name = "Content") +
    scale_y_continuous(labels = function(x) paste0(x, "x")) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(
      x = NULL,
      y = "Ratio to best in workload"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text         = element_text(face = "bold", size = 13),
      axis.text.x        = element_text(face = "bold"),
      panel.grid.major.x = element_blank(),
      panel.grid.minor   = element_blank(),
      legend.position    = "bottom"
    )

  fname <- file.path(plot_dir, "overview_distribution_ratio.png")
  ggsave(fname, p, width = 10, height = 9, dpi = 300)
  cat("Saved:", fname, "\n")
}

# Rank composition: stacked bar of how often each library placed
# 1st through 5th across the 48 cells, faceted by operation so both
# Deserialize and Serialize share one figure (and one Rank legend).
# Same Wistia palette as the per-cell rank heatmap so the encoding
# carries through.
plot_overview_rank_composition <- function() {
  plot_data <- ranked %>%
    count(Library, Operation, Rank) %>%
    mutate(RankLabel = factor(Rank, levels = 1:5))

  p <- ggplot(plot_data, aes(x = Library, y = n, fill = RankLabel)) +
    geom_col(width = 0.7, color = "white", linewidth = 0.5) +
    geom_text(
      aes(label = ifelse(n >= 3, as.character(n), "")),
      position = position_stack(vjust = 0.5),
      size = 4.6,
      fontface = "bold"
    ) +
    scale_fill_manual(
      values = setNames(wistia_palette, as.character(1:5)),
      name = "Rank",
      drop = FALSE
    ) +
    scale_y_continuous(
      expand = c(0, 0),
      breaks = seq(0, 48, by = 12)
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(
      x = NULL,
      y = "Workloads"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text         = element_text(face = "bold", size = 13),
      axis.text.x        = element_text(face = "bold"),
      panel.grid.major.x = element_blank(),
      panel.grid.minor   = element_blank(),
      legend.position    = "bottom"
    )

  fname <- file.path(plot_dir, "overview_rank_composition.png")
  ggsave(fname, p, width = 10, height = 9, dpi = 300)
  cat("Saved:", fname, "\n")
}

save_library_summary <- function() {
  summary <- ranked %>%
    group_by(Library, Operation) %>%
    summarise(
      MedianEnergy = median(MeanEnergy),
      MinEnergy    = min(MeanEnergy),
      MaxEnergy    = max(MeanEnergy),
      MedianRatio  = median(NormEnergy),
      Q1Ratio      = quantile(NormEnergy, 0.25, names = FALSE),
      Q3Ratio      = quantile(NormEnergy, 0.75, names = FALSE),
      MaxRatio     = max(NormEnergy),
      .groups = "drop"
    ) %>%
    mutate(
      across(c(MedianEnergy, MinEnergy, MaxEnergy), ~ round(.x, 1)),
      across(c(MedianRatio, Q1Ratio, Q3Ratio, MaxRatio), ~ round(.x, 2))
    ) %>%
    arrange(Operation, Library)

  fname <- file.path(plot_dir, "overview_library_summary.csv")
  write_csv(summary, fname)
  cat("Saved:", fname, "\n")
}

save_cell_spread <- function() {
  spread <- means %>%
    group_by(Operation, Depth, Width, Content) %>%
    summarise(
      MinEnergy     = min(MeanEnergy),
      MaxEnergy     = max(MeanEnergy),
      SpreadRatio   = MaxEnergy / MinEnergy,
      WinnerLibrary = as.character(Library[which.min(MeanEnergy)]),
      .groups = "drop"
    ) %>%
    mutate(
      across(c(MinEnergy, MaxEnergy), ~ round(.x, 1)),
      SpreadRatio = round(SpreadRatio, 2)
    ) %>%
    arrange(Operation, desc(SpreadRatio))

  fname <- file.path(plot_dir, "overview_cell_spread.csv")
  write_csv(spread, fname)
  cat("Saved:", fname, "\n")
}

# ============================================================
# REPORT-FRIENDLY CSV + LATEX LONGTABLE (isolation-style)
# ============================================================
# Trimmed view of BDN's report.csv, matching isolation's column schema:
# Method / Iters / Ops / Mean / Error / StdDev / Pkg / DRAM / Temp / GC /
# Allocated. The longtable .tex is a faithful dump of the CSV, suitable for
# the appendix. Skipped silently if the report CSV is missing.

save_report_friendly <- function() {
  if (is.null(report)) {
    cat("Skipping report_friendly: report CSV not found\n")
    return(invisible(NULL))
  }

  # Re-read raw report.csv to grab the columns we didn't keep on the parsed
  # `report` object (Iterations, Operations, Mean, Error, StdDev, energy
  # columns, temperature). The Method column carries the workload coords so
  # the trimmed view stays self-contained.
  raw_report <- read_csv(report_csv_path, show_col_types = FALSE)

  rf <- raw_report %>%
    select(
      Method,
      Iters              = Iterations,
      Ops                = Operations,
      Mean,
      Error,
      StdDev,
      `Pkg (μJ/op)`    = `PkgE0 (uJ/op)`,
      `StdDev (μJ/op)` = `StdDev0 (uJ/op)`,
      `DRAM (μJ/op)`   = `DRAM E0 (uJ/op)`,
      `Temp (°C)`      = `Avg Temp0 (degC)`,
      Gen0,
      Gen1,
      Allocated
    ) %>%
    arrange(Method)

  fname <- file.path(plot_dir, "report_friendly.csv")
  write_csv(rf, fname)
  cat("Saved:", fname, "\n")

  write_report_longtable(rf)
}

write_report_longtable <- function(rf) {
  esc <- function(x) { x[is.na(x)] <- ""; gsub("([&%#_])", "\\\\\\1", x) }
  esc_hdr <- function(h) {
    h <- gsub("[µμ]", "$\\\\mu$", h)
    h <- gsub("°", "$^\\\\circ$", h)
    gsub("([&%#_])", "\\\\\\1", h)
  }

  cols    <- colnames(rf)
  ncol    <- length(cols)
  colspec <- paste0("l", strrep("r", ncol - 1))
  header  <- paste0(paste(esc_hdr(cols), collapse = " & "), " \\\\")

  cell_df <- as.data.frame(lapply(rf, function(col) esc(as.character(col))),
                           stringsAsFactors = FALSE)
  rows    <- do.call(paste, c(cell_df, sep = " & "))
  body    <- paste0("  ", rows, " \\\\")

  cap <- paste0("Full per-configuration BenchmarkDotNet report for the ",
                "48-cell factorial: timing, Package and DRAM energy per ",
                "operation, GC counts, and allocations, one row per library ",
                "$\\times$ operation $\\times$ workload cell.")

  lines <- c(
    "% Auto-generated by analysis/factorial_analysis.R -- faithful dump of report_friendly.csv.",
    "% Requires LaTeX packages: longtable, booktabs, pdflscape.",
    "\\begin{landscape}",
    "\\footnotesize",
    sprintf("\\begin{longtable}{%s}", colspec),
    sprintf("\\caption{%s}\\label{tab:factorial-report}\\\\", cap),
    "\\toprule",
    header,
    "\\midrule \\endfirsthead",
    "\\toprule",
    header,
    "\\midrule \\endhead",
    sprintf("\\midrule \\multicolumn{%d}{r}{\\textit{continued on next page}}\\\\ \\endfoot", ncol),
    "\\bottomrule \\endlastfoot",
    body,
    "\\end{longtable}",
    "\\end{landscape}"
  )

  fname <- file.path(tables_dir, "factorial_report_friendly.tex")
  writeLines(lines, fname)
  cat("Saved:", fname, "\n")
}

# ============================================================
# LATEX SUMMARY TABLE (factorial_energy_summary.tex)
# ============================================================
# Per library: median / min / max energy across the 48-cell grid, split by
# Operation. Referenced from chapter5_outline.tex via
# \input{Figures/tables/factorial_energy_summary}.

save_energy_summary_tex <- function() {
  d <- ranked %>%
    group_by(Library, Operation) %>%
    summarise(
      Median = median(MeanEnergy),
      Min    = min(MeanEnergy),
      Max    = max(MeanEnergy),
      .groups = "drop"
    ) %>%
    mutate(across(c(Median, Min, Max),
                  ~ formatC(.x, format = "f", digits = 1, big.mark = ",")))

  wide <- d %>%
    pivot_wider(
      names_from  = Operation,
      values_from = c(Median, Min, Max),
      names_glue  = "{Operation}_{.value}"
    ) %>%
    arrange(Library)

  body_lines <- apply(wide, 1, function(r) {
    sprintf("    %s & %s & %s & %s & %s & %s & %s \\\\",
            r["Library"],
            r["Deserialize_Median"], r["Deserialize_Min"], r["Deserialize_Max"],
            r["Serialize_Median"],   r["Serialize_Min"],   r["Serialize_Max"])
  })

  tex <- c(
    "% Generated by analysis/factorial_analysis.R",
    "\\begin{table}[ht]",
    "  \\centering",
    "  \\caption{Per-library energy across the 48-cell factorial: median, minimum, and maximum across the grid, per operation. Values in $\\mu$J/op.}",
    "  \\label{tab:factorial-energy-summary}",
    "  \\begin{tabular}{lrrrrrr}",
    "    \\toprule",
    "    & \\multicolumn{3}{c}{Deserialize} & \\multicolumn{3}{c}{Serialize} \\\\",
    "    \\cmidrule(lr){2-4} \\cmidrule(lr){5-7}",
    "    Library & Median & Min & Max & Median & Min & Max \\\\",
    "    \\midrule",
    body_lines,
    "    \\bottomrule",
    "  \\end{tabular}",
    "\\end{table}"
  )

  fname <- file.path(tables_dir, "factorial_energy_summary.tex")
  writeLines(tex, fname)
  cat("Saved:", fname, "\n")
}

# ============================================================
# RUN DESCRIPTIVE
# ============================================================

plot_heatmap_rank_ratio("Deserialize")
plot_heatmap_rank_ratio("Serialize")
plot_alloc()
plot_gc()
local_steps <- build_local_steps()
save_local_steps(local_steps)
plot_local_per_element(local_steps)
plot_overview_distribution_ratio()
plot_overview_rank_composition()
save_library_summary()
save_cell_spread()
save_energy_summary_tex()
save_report_friendly()

# ============================================================
# ============================================================
# PART B: STATISTICAL PIPELINE
# Shapiro-Wilk normality -> Kruskal-Wallis significance ->
# Cliff's delta effect size. All on per-sample Actual iterations.
# Outputs under factorial_normalized/stats/.
# ============================================================
# ============================================================

# Helper for significance labels (used by KW)
sig_label <- function(p) {
  case_when(
    p < 0.001 ~ "***",
    p < 0.01  ~ "**",
    p < 0.05  ~ "*",
    TRUE      ~ "ns"
  )
}

# Cliff's Delta magnitude labels (Romano 2006 thresholds)
delta_magnitude <- function(d) {
  ad <- abs(d)
  case_when(
    ad < 0.147 ~ "negligible",
    ad < 0.33  ~ "small",
    ad < 0.474 ~ "medium",
    TRUE       ~ "large"
  )
}

# ============================================================
# B1: SHAPIRO-WILK NORMALITY
# ============================================================

cat("\n=== Shapiro-Wilk Normality Test Summary ===\n")

shapiro_results <- df_actual %>%
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

cat(sprintf("Total groups tested: %d\n", nrow(shapiro_results)))
cat(sprintf("Normal (p >= 0.05):  %d (%.1f%%)\n",
            sum(shapiro_results$normal, na.rm = TRUE),
            100 * mean(shapiro_results$normal, na.rm = TRUE)))
cat(sprintf("Non-normal (p < 0.05): %d (%.1f%%)\n",
            sum(!shapiro_results$normal, na.rm = TRUE),
            100 * mean(!shapiro_results$normal, na.rm = TRUE)))

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

# Per (Library, Operation) breakdown — counts + percentages, plus a grand-total
# row so the chapter-wide headline is in the same file. Matches the
# shapiro_summary.csv format produced by isolation_analysis.R.
sw_by_lib_op <- summarise_with_total(
  shapiro_results, c("Library", "Operation"),
  n_groups       = n(),
  n_normal       = sum(normal, na.rm = TRUE),
  n_non_normal   = sum(!normal, na.rm = TRUE),
  pct_normal     = 100 * mean(normal, na.rm = TRUE),
  pct_non_normal = 100 * mean(!normal, na.rm = TRUE)
)
write_csv(sw_by_lib_op,    file.path(norm_dir, "shapiro_summary.csv"))
write_csv(shapiro_results, file.path(norm_dir, "shapiro_results.csv"))
cat(sprintf("\nSaved: %s\n", file.path(norm_dir, "shapiro_summary.csv")))
cat(sprintf("Saved: %s\n",   file.path(norm_dir, "shapiro_results.csv")))

# --- QQ Plots ---
cat("\nGenerating QQ plots...\n")

for (lib in levels(df_actual$Library)) {
  lib_qq_dir <- file.path(norm_dir, "qq", tolower(lib))
  dir.create(lib_qq_dir, showWarnings = FALSE, recursive = TRUE)

  for (op in levels(df_actual$Operation)) {
    for (ct in levels(df_actual$Content)) {
      for (w in c(5, 20, 50, 100)) {
        sub <- df_actual %>% filter(Library == lib, Operation == op,
                                     Content == ct, Width == w)
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
          theme(strip.text = element_text(face = "bold"))

        op_short <- ifelse(op == "Deserialize", "deser", "ser")
        fname <- sprintf("qq_%s_%s_w%d.png", op_short, tolower(ct), w)
        ggsave(file.path(lib_qq_dir, fname), p, width = 12, height = 4, dpi = 300)
      }
    }
  }
}
cat("  QQ plots saved.\n")

# --- Summary histogram of Shapiro-Wilk p-values ---
p_hist <- ggplot(shapiro_results, aes(x = sw_p_value)) +
  geom_histogram(bins = 30, fill = "#56B4E9", color = "white", alpha = 0.9) +
  geom_vline(xintercept = 0.05, linetype = "dashed",
             color = "#D55E00", linewidth = 0.6) +
  annotate("text", x = 0.07, y = Inf, label = "alpha = 0.05", vjust = 2,
           hjust = 0, color = "#D55E00", size = 4) +
  labs(
    x = "Shapiro-Wilk p-value",
    y = "Count"
  ) +
  theme_minimal(base_size = 14)

ggsave(file.path(norm_dir, "shapiro_pvalue_distribution.png"),
       p_hist, width = 8, height = 5, dpi = 300)
cat("  Saved: shapiro_pvalue_distribution.png\n")

# ============================================================
# B2: KRUSKAL-WALLIS
# Across-libraries library effect, per workload cell (Depth x Width x Content).
# ============================================================

cat("\n=== Kruskal-Wallis Test: Energy ~ Library ===\n")

kw_lib <- df_actual %>%
  group_by(Operation, Depth, Width, Content) %>%
  group_modify(~ {
    k <- kruskal.test(.x$EnergyPerOp ~ .x$Library)
    tibble(n_obs      = nrow(.x),
           n_groups   = n_distinct(.x$Library),
           kw_chi2    = unname(k$statistic),
           kw_df      = unname(k$parameter),
           kw_p_value = k$p.value)
  }) %>%
  ungroup() %>%
  mutate(
    # Holm-Bonferroni across the within-cell KW family (96 tests, one per
    # cell × operation). Mirrors the isolation pipeline so the two designs
    # apply the same multiple-comparisons correction.
    kw_p_adj    = p.adjust(kw_p_value, method = "holm"),
    significant = kw_p_adj < 0.05,
    sig_label   = sig_label(kw_p_adj)
  )

cat(sprintf("Total comparisons: %d\n", nrow(kw_lib)))
cat(sprintf("Significant (p_adj < 0.05, Holm): %d (%.1f%%)\n",
            sum(kw_lib$significant), 100 * mean(kw_lib$significant)))

write_csv(kw_lib, file.path(kw_lib_dir, "kw_library_results.csv"))

# Per-Operation breakdown + grand-total row — quotable as "library effect
# significant in N/M cells (P%)". Matches the shapiro_summary.csv
# format.
kw_lib_by_op <- summarise_with_total(
  kw_lib, c("Operation"),
  n_cells             = n(),
  n_significant       = sum(significant, na.rm = TRUE),
  n_not_significant   = sum(!significant, na.rm = TRUE),
  pct_significant     = 100 * mean(significant, na.rm = TRUE),
  pct_not_significant = 100 * mean(!significant, na.rm = TRUE)
)
write_csv(kw_lib_by_op, file.path(kw_lib_dir, "kw_library_summary.csv"))
cat(sprintf("Saved: %s\n", file.path(kw_lib_dir, "kw_library_summary.csv")))

# ============================================================
# B3: CLIFF'S DELTA
# Between-library pairwise effect size, per workload cell.
# ============================================================

cat("\n=== Cliff's Delta: Pairwise Library Comparisons ===\n")

lib_levels <- levels(df_actual$Library)
lib_pairs  <- combn(lib_levels, 2, simplify = FALSE)

all_cd_lib <- tibble()

for (op in levels(df_actual$Operation)) {
  for (d in sort(unique(df_actual$Depth))) {
    for (w in sort(unique(df_actual$Width))) {
      for (ct in levels(df_actual$Content)) {
        sub <- df_actual %>%
          filter(Operation == op, Depth == d, Width == w, Content == ct)
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

write_csv(all_cd_lib, file.path(delta_lib_dir, "cd_library_pairwise_results.csv"))

# --- Library pairwise heatmaps: delta per workload config ---
delta_workload_order <- df_actual %>%
  distinct(Depth, Width) %>%
  arrange(Depth, Width) %>%
  mutate(Workload = sprintf("D%d_W%d", Depth, Width)) %>%
  pull(Workload)

delta_heatmap_data <- all_cd_lib %>%
  mutate(Workload = factor(sprintf("D%d_W%d", Depth, Width),
                           levels = delta_workload_order))

for (op in levels(df_actual$Operation)) {
  sub <- delta_heatmap_data %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = comparison, fill = delta)) +
    geom_tile(color = "white", linewidth = 0.3) +
    geom_text(aes(label = sprintf("%.2f", delta)), size = 2.2) +
    facet_wrap(~ Content, ncol = 1) +
    scale_y_discrete(limits = rev(pair_order)) +
    scale_fill_gradient2(low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
                         midpoint = 0, name = "Cliff's delta",
                         limits = c(-1, 1)) +
    labs(title = sprintf("Cliff's Delta: Library Pairs — %s", op),
         subtitle = "delta < 0: first library uses LESS energy. |d| < 0.147 negligible, < 0.33 small, < 0.474 medium, >= 0.474 large",
         x = "Workload (Depth x Width)", y = "Library Pair") +
    theme_minimal(base_size = 10) +
    theme(axis.text.x = element_text(angle = 45, hjust = 1),
          plot.title = element_text(face = "bold"),
          strip.text = element_text(face = "bold"),
          plot.subtitle = element_text(size = 8))
  ggsave(file.path(delta_lib_dir, sprintf("cd_library_%s.png", tolower(op))),
         p, width = 14, height = 10, dpi = 300)
}

# --- Magnitude overview: per library-pair tier counts + %, with an "All" total
# row. Same shape as isolation's _aggregate cd_magnitude_summary.csv; the
# non-large rows are the statistically-tied between-library comparisons. ---
cd_magnitude_summary <- summarise_with_total(
  all_cd_lib, "comparison",
  n_cells       = n(),
  n_L           = sum(magnitude == "large",      na.rm = TRUE),
  n_M           = sum(magnitude == "medium",     na.rm = TRUE),
  n_S           = sum(magnitude == "small",      na.rm = TRUE),
  n_N           = sum(magnitude == "negligible", na.rm = TRUE),
  n_not_large   = sum(magnitude != "large",      na.rm = TRUE),
  pct_large     = 100 * mean(magnitude == "large", na.rm = TRUE),
  pct_not_large = 100 * mean(magnitude != "large", na.rm = TRUE)
)

write_csv(cd_magnitude_summary, file.path(delta_lib_dir, "cd_magnitude_summary.csv"))
cat("Library pairwise CSVs and per-cell heatmaps saved.\n")

cat("\nDone!\n")
