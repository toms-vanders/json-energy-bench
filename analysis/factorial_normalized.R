library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
variant <- Sys.getenv("BENCH_VARIANT", "Byte")
stopifnot(variant %in% c("Byte", "String"))
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      sprintf("JsonBench.Benchmarks.Factorial.FactorialNormalized%sBench-measurements.csv", variant))
plot_dir <- file.path(script_dir, "plots", tolower(variant), "factorial_normalized")
dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)
raw <- read_csv(csv_path, show_col_types = FALSE)

# Filter to workload result iterations only
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Result")

# Parse method name: e.g. "SpanJson_Deser_D10_W100_B"
df <- df %>%
  mutate(
    Library   = str_extract(Target_Method, "^[^_]+"),
    Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
    Depth     = as.integer(str_extract(Target_Method, "(?<=_D)\\d+")),
    Width     = as.integer(str_extract(Target_Method, "(?<=_W)\\d+")),
    Content   = str_extract(Target_Method, "[TNB]$"),
    EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation
  )

# Labels (keep numeric Depth/Width for scaling plots, add factor versions for bar/facet plots)
df <- df %>%
  mutate(
    Content   = factor(Content, levels = c("T", "N", "B"),
                       labels = c("Textual", "Numeric", "Boolean")),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize")),
    DepthLabel = factor(paste0("Depth ", Depth),
                        levels = paste0("Depth ", c(2, 5, 10, 20))),
    WidthFactor = factor(Width, levels = c(5, 20, 50, 100))
  )

# Compute mean energy per combination
means <- df %>%
  group_by(Library, Operation, Depth, DepthLabel, Width, WidthFactor, Content) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

# --- Colors ---
lib_colors <- c(
  "SpanJson"   = "#0072B2",
  "Utf8Json"   = "#009E73",
  "STJRefGen"  = "#F0E442",
  "STJSrcGen"  = "#CC79A7",
  "Newtonsoft" = "#D55E00"
)

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
# BAR PLOTS (from factorial_bars.R)
# ============================================================

plot_bars <- function(op_label) {
  plot_data <- means %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = WidthFactor, y = MeanEnergy, fill = Content)) +
    geom_col(position = position_dodge(width = 0.8), width = 0.7) +
    facet_grid(DepthLabel ~ Library, scales = "free_y") +
    scale_fill_manual(values = content_colors) +
    labs(
      x = "Width (fields per object)",
      y = "Total Energy (uJ/op)",
      fill = "Content"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 12),
      axis.text.x = element_text(angle = 45, hjust = 1),
      legend.position = "bottom"
    )

  fname <- file.path(plot_dir, paste0("bars_",
                   tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 16, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

# ============================================================
# SCALING LINE PLOTS (from factorial_scaling.R)
# ============================================================

plot_width_scaling <- function(op_label) {
  plot_data <- means %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Width, y = MeanEnergy,
                              color = Library, group = Library)) +
    geom_line(linewidth = 0.9) +
    geom_point(size = 2) +
    facet_grid(DepthLabel ~ Content, scales = "free_y") +
    scale_color_manual(values = lib_colors) +
    scale_x_continuous(breaks = c(5, 20, 50, 100)) +
    labs(
      x = "Width (fields per object)",
      y = "Total Energy (uJ/op)",
      color = "Library"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 12),
      legend.position = "bottom",
      panel.grid.minor = element_blank()
    )

  fname <- file.path(plot_dir, paste0("scaling_width_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 12, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

plot_depth_scaling <- function(op_label) {
  plot_data <- means %>%
    filter(Operation == op_label) %>%
    mutate(WidthLabel = factor(paste0("Width ", Width),
                               levels = paste0("Width ", c(5, 20, 50, 100))))

  p <- ggplot(plot_data, aes(x = Depth, y = MeanEnergy,
                              color = Library, group = Library)) +
    geom_line(linewidth = 0.9) +
    geom_point(size = 2) +
    facet_grid(WidthLabel ~ Content, scales = "free_y") +
    scale_color_manual(values = lib_colors) +
    scale_x_continuous(breaks = c(2, 5, 10, 20)) +
    labs(
      x = "Nesting Depth",
      y = "Total Energy (uJ/op)",
      color = "Library"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 12),
      legend.position = "bottom",
      panel.grid.minor = element_blank()
    )

  fname <- file.path(plot_dir, paste0("scaling_depth_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 12, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

# ============================================================
# SCALING TABLES
# ============================================================

save_scaling_tables <- function() {
  width_table <- means %>%
    select(-DepthLabel, -WidthFactor) %>%
    pivot_wider(names_from = Width, values_from = MeanEnergy, names_prefix = "W") %>%
    mutate(
      Ratio_W100_W5 = round(W100 / W5, 1),
      across(starts_with("W"), ~ round(.x, 1))
    ) %>%
    arrange(Operation, Library, Content, Depth)

  write_csv(width_table, file.path(plot_dir, "scaling_width_table.csv"))
  cat("Saved:", file.path(plot_dir, "scaling_width_table.csv"), "\n")

  depth_table <- means %>%
    select(-DepthLabel, -WidthFactor) %>%
    pivot_wider(names_from = Depth, values_from = MeanEnergy, names_prefix = "D") %>%
    mutate(
      Ratio_D20_D2 = round(D20 / D2, 1),
      across(starts_with("D"), ~ round(.x, 1))
    ) %>%
    arrange(Operation, Library, Content, Width)

  write_csv(depth_table, file.path(plot_dir, "scaling_depth_table.csv"))
  cat("Saved:", file.path(plot_dir, "scaling_depth_table.csv"), "\n")
}

# ============================================================
# SCALING RATIO HEATMAPS
# ============================================================

plot_width_ratio_heatmap <- function(op_label) {
  hm_data <- means %>%
    filter(Width %in% c(5, 100), Operation == op_label) %>%
    select(-DepthLabel, -WidthFactor) %>%
    pivot_wider(names_from = Width, values_from = MeanEnergy, names_prefix = "W") %>%
    mutate(
      Ratio = round(W100 / W5, 1),
      DepthLabel = factor(paste0("D", Depth), levels = paste0("D", c(2, 5, 10, 20)))
    )

  p <- ggplot(hm_data, aes(x = DepthLabel, y = Library, fill = Ratio)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = Ratio), size = 3.5) +
    facet_wrap(~ Content, nrow = 1) +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 20, limits = c(10, 75),
      name = "Ratio\n(linear = 20)"
    ) +
    labs(
      x = "Depth", y = NULL
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 12),
      legend.position = "right",
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_width_ratio_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 14, height = 5, dpi = 150)
  cat("Saved:", fname, "\n")
}

plot_depth_ratio_heatmap <- function(op_label) {
  hm_data <- means %>%
    filter(Depth %in% c(2, 20), Operation == op_label) %>%
    select(-DepthLabel, -WidthFactor) %>%
    pivot_wider(names_from = Depth, values_from = MeanEnergy, names_prefix = "D") %>%
    mutate(
      Ratio = round(D20 / D2, 1),
      WidthLabel = factor(paste0("W", Width), levels = paste0("W", c(5, 20, 50, 100)))
    )

  p <- ggplot(hm_data, aes(x = WidthLabel, y = Library, fill = Ratio)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = Ratio), size = 3.5) +
    facet_wrap(~ Content, nrow = 1) +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 10, limits = c(5, 25),
      name = "Ratio\n(linear = 10)"
    ) +
    labs(
      x = "Width", y = NULL
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 12),
      legend.position = "right",
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_depth_ratio_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 14, height = 5, dpi = 150)
  cat("Saved:", fname, "\n")
}

# ============================================================
# RANKING HEATMAPS
# ============================================================

ranked <- means %>%
  group_by(Operation, Depth, Width, Content) %>%
  mutate(
    Rank = rank(MeanEnergy, ties.method = "min"),
    NormEnergy = MeanEnergy / min(MeanEnergy)
  ) %>%
  ungroup() %>%
  mutate(Config = factor(
    paste0("D", Depth, "\nW", Width),
    levels = {
      d <- expand.grid(Depth = c(2, 5, 10, 20), Width = c(5, 20, 50, 100))
      d <- d[order(d$Depth, d$Width), ]
      paste0("D", d$Depth, "\nW", d$Width)
    }
  ))

plot_rank_heatmap <- function(op_label) {
  plot_data <- ranked %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Config, y = Library, fill = factor(Rank))) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = Rank), size = 3.5, fontface = "bold") +
    facet_wrap(~ Content, ncol = 1) +
    scale_fill_manual(
      values = setNames(wistia_palette, as.character(1:5)),
      name = "Rank"
    ) +
    labs(
      x = "Workload Configuration", y = NULL
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 13),
      axis.text.x = element_text(size = 9, lineheight = 0.9),
      axis.text.y = element_text(face = "bold"),
      legend.position = "bottom",
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_rank_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 8, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

plot_norm_heatmap <- function(op_label) {
  plot_data <- ranked %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Config, y = Library, fill = NormEnergy)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.1fx", NormEnergy)), size = 3.2) +
    facet_wrap(~ Content, ncol = 1) +
    scale_fill_gradientn(
      colors = wistia_palette,
      limits = c(1, NA),
      name = "Ratio to best"
    ) +
    labs(
      x = "Workload Configuration", y = NULL
    ) +
    theme_minimal(base_size = 14) +
    theme(
      strip.text = element_text(face = "bold", size = 13),
      axis.text.x = element_text(size = 9, lineheight = 0.9),
      axis.text.y = element_text(face = "bold"),
      legend.position = "bottom",
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_norm_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 8, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

# ============================================================
# OVERVIEW DISTRIBUTIONS AND SUMMARIES
# ============================================================
# Section 5.2 (Workload Landscape / Factorial Overview) artifacts:
#   - overview_distribution_ratio_{op}.png : per-library ratio-to-best
#       across the 48-cell grid (boxplot + 48 jittered cells coloured by
#       content type). y = 1.0x = cell winner. Grid-scaling effect cancels
#       out since the ratio is computed within each cell, so spread
#       reflects how the gap to the cell-best varies across workloads.
#   - overview_rank_composition_{op}.png : per-library stacked bar
#       showing % of the 48 cells the library placed Rank 1 through 5.
#       Wistia palette (Rank 1 = cool, Rank 5 = hot) matches the per-cell
#       rank heatmap in the appendix so the reader's encoding carries
#       through. Complements the ratio strip-box: rank tells positional
#       standing, ratio tells gap magnitude.
#   - overview_library_summary.csv : per-(library, operation) summary
#       statistics; source for numerical claims in the chapter prose.
#   - overview_cell_spread.csv : per-cell min/max/spread + winning library;
#       lets the prose cite specific flat-vs-spread configurations.

plot_overview_distribution_ratio <- function(op_label) {
  plot_data <- ranked %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Library, y = NormEnergy)) +
    geom_hline(yintercept = 1, linetype = "dashed",
               color = "grey60", linewidth = 0.4) +
    geom_boxplot(width = 0.45, fill = "white", color = "grey25",
                 alpha = 0.9, outlier.shape = NA) +
    geom_jitter(aes(color = Content),
                position = position_jitter(width = 0.18, seed = 42),
                size = 2.1, alpha = 0.8) +
    scale_color_manual(values = content_colors, name = "Content") +
    scale_y_continuous(labels = function(x) paste0(x, "x")) +
    labs(
      x = NULL,
      y = "Ratio to best in workload"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      axis.text.x = element_text(face = "bold"),
      panel.grid.major.x = element_blank(),
      panel.grid.minor = element_blank(),
      legend.position = "bottom"
    )

  fname <- file.path(plot_dir, paste0("overview_distribution_ratio_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 10, height = 6, dpi = 150)
  cat("Saved:", fname, "\n")
}

# Rank composition: stacked bar of how often each library placed
# 1st through 5th across the 48 cells. Same Wistia palette as the
# per-cell rank heatmap so the encoding carries through.
plot_overview_rank_composition <- function(op_label) {
  plot_data <- ranked %>%
    filter(Operation == op_label) %>%
    count(Library, Rank) %>%
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
    labs(
      x = NULL,
      y = "Workloads"
    ) +
    theme_minimal(base_size = 14) +
    theme(
      axis.text.x = element_text(face = "bold"),
      panel.grid.major.x = element_blank(),
      panel.grid.minor = element_blank(),
      legend.position = "bottom"
    )

  fname <- file.path(plot_dir, paste0("overview_rank_composition_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 10, height = 6, dpi = 150)
  cat("Saved:", fname, "\n")
}

save_library_summary <- function() {
  summary <- ranked %>%
    group_by(Library, Operation) %>%
    summarise(
      MedianEnergy = median(MeanEnergy),
      MinEnergy    = min(MeanEnergy),
      MaxEnergy    = max(MeanEnergy),
      .groups = "drop"
    ) %>%
    mutate(across(c(MedianEnergy, MinEnergy, MaxEnergy), ~ round(.x, 1))) %>%
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
# Run all
# ============================================================

plot_bars("Deserialize")
plot_bars("Serialize")
plot_width_scaling("Deserialize")
plot_width_scaling("Serialize")
plot_depth_scaling("Deserialize")
plot_depth_scaling("Serialize")
save_scaling_tables()
plot_width_ratio_heatmap("Deserialize")
plot_width_ratio_heatmap("Serialize")
plot_depth_ratio_heatmap("Deserialize")
plot_depth_ratio_heatmap("Serialize")
plot_rank_heatmap("Deserialize")
plot_rank_heatmap("Serialize")
plot_norm_heatmap("Deserialize")
plot_norm_heatmap("Serialize")
plot_overview_distribution_ratio("Deserialize")
plot_overview_distribution_ratio("Serialize")
plot_overview_rank_composition("Deserialize")
plot_overview_rank_composition("Serialize")
save_library_summary()
save_cell_spread()

cat("\nDone!\n")
