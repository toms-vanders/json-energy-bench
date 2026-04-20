library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedStringBench-measurements.csv")
plot_dir <- file.path(script_dir, "plots", "factorial_normalized")
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
  "SpanJson"   = "#2196F3",
  "Utf8Json"   = "#4CAF50",
  "STJRefGen"  = "#9C27B0",
  "STJSrcGen"  = "#7B1FA2",
  "Newtonsoft" = "#F44336"
)

content_colors <- c(
  "Textual" = "#E57373",
  "Numeric" = "#64B5F6",
  "Boolean" = "#81C784"
)

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
      title = paste0("Normalized ", op_label, " - Total Energy per Operation (Package + DRAM, uJ/op)"),
      subtitle = "Rows: Nesting Depth | Columns: Library | X: Width | Color: Content Type (~5 bytes/value)",
      x = "Width (fields per object)",
      y = "Total Energy (uJ/op)",
      fill = "Content"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      axis.text.x = element_text(angle = 45, hjust = 1),
      legend.position = "bottom",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40")
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
      title = paste0("Normalized ", op_label, " - Energy Scaling with Width (Package + DRAM)"),
      subtitle = "Rows: Nesting Depth | Columns: Content Type | Lines: Library",
      x = "Width (fields per object)",
      y = "Total Energy (uJ/op)",
      color = "Library"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      legend.position = "bottom",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
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
      title = paste0("Normalized ", op_label, " - Energy Scaling with Depth (Package + DRAM)"),
      subtitle = "Rows: Structural Width | Columns: Content Type | Lines: Library",
      x = "Nesting Depth",
      y = "Total Energy (uJ/op)",
      color = "Library"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      legend.position = "bottom",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
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
      low = "#4CAF50", mid = "#FFEB3B", high = "#F44336",
      midpoint = 20, limits = c(10, 75),
      name = "Ratio\n(linear = 20)"
    ) +
    labs(
      title = paste0("Normalized ", op_label, " - Width Scaling Ratio (W100 / W5)"),
      subtitle = "Green = ~linear (20x) | Yellow = moderate | Red = superlinear",
      x = "Depth", y = NULL
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      legend.position = "right",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
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
      low = "#4CAF50", mid = "#FFEB3B", high = "#F44336",
      midpoint = 10, limits = c(5, 25),
      name = "Ratio\n(linear = 10)"
    ) +
    labs(
      title = paste0("Normalized ", op_label, " - Depth Scaling Ratio (D20 / D2)"),
      subtitle = "Green = ~linear (10x) | Yellow = moderate | Red = superlinear",
      x = "Width", y = NULL
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      legend.position = "right",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
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
    geom_text(aes(label = Rank), size = 3, fontface = "bold") +
    facet_wrap(~ Content, nrow = 1) +
    scale_fill_manual(
      values = c("1" = "#4CAF50", "2" = "#8BC34A", "3" = "#FFC107",
                 "4" = "#FF9800", "5" = "#F44336"),
      name = "Rank"
    ) +
    labs(
      title = paste0("Normalized ", op_label, " - Library Energy Ranking by Workload"),
      subtitle = "1 = most efficient (green) | 5 = least efficient (red)",
      x = "Workload Configuration", y = NULL
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 11),
      axis.text.x = element_text(size = 7, lineheight = 0.9),
      axis.text.y = element_text(face = "bold"),
      legend.position = "bottom",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_rank_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 16, height = 5, dpi = 150)
  cat("Saved:", fname, "\n")
}

plot_norm_heatmap <- function(op_label) {
  plot_data <- ranked %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Config, y = Library, fill = NormEnergy)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.1fx", NormEnergy)), size = 2.8) +
    facet_wrap(~ Content, nrow = 1) +
    scale_fill_gradient2(
      low = "#4CAF50", mid = "#FFC107", high = "#F44336",
      midpoint = 2, limits = c(1, NA),
      name = "Ratio to best"
    ) +
    labs(
      title = paste0("Normalized ", op_label, " - Relative Energy Cost by Workload"),
      subtitle = "1.0x = most efficient library for that config | Higher = worse",
      x = "Workload Configuration", y = NULL
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 11),
      axis.text.x = element_text(size = 7, lineheight = 0.9),
      axis.text.y = element_text(face = "bold"),
      legend.position = "bottom",
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      panel.grid = element_blank()
    )

  fname <- file.path(plot_dir, paste0("heatmap_norm_",
                     tolower(gsub("ialize", "", op_label)), ".png"))
  ggsave(fname, p, width = 16, height = 5, dpi = 150)
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

cat("\nDone!\n")
