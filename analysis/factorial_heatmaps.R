library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialStringBench-measurements.csv")
plot_dir <- file.path(script_dir, "plots")
dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)
raw <- read_csv(csv_path, show_col_types = FALSE)

# Filter to workload result iterations only
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Result")

# Parse method name
df <- df %>%
  mutate(
    Library   = str_extract(Target_Method, "^[^_]+"),
    Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
    Depth     = as.integer(str_extract(Target_Method, "(?<=_D)\\d+")),
    Width     = as.integer(str_extract(Target_Method, "(?<=_W)\\d+")),
    Content   = str_extract(Target_Method, "[TNB]$"),
    EnergyPerOp = Measurement_PackageEnergyPerOperation
  )

# Labels
df <- df %>%
  mutate(
    Content   = factor(Content, levels = c("T", "N", "B"),
                       labels = c("Textual", "Numeric", "Boolean")),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize"))
  )

# Compute mean energy per combination
means <- df %>%
  group_by(Library, Operation, Depth, Width, Content) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

# Add rank per config (1 = most efficient)
means <- means %>%
  group_by(Operation, Depth, Width, Content) %>%
  mutate(
    Rank = rank(MeanEnergy, ties.method = "min"),
    MinEnergy = min(MeanEnergy),
    NormEnergy = MeanEnergy / MinEnergy
  ) %>%
  ungroup()

# Config label for x-axis
means <- means %>%
  mutate(Config = paste0("D", Depth, "\nW", Width))

# Order configs logically
config_order <- means %>%
  distinct(Depth, Width, Config) %>%
  arrange(Depth, Width) %>%
  pull(Config)
means <- means %>%
  mutate(Config = factor(Config, levels = config_order))

# --- Rank heatmap ---
plot_rank_heatmap <- function(op_label) {
  plot_data <- means %>% filter(Operation == op_label)

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
      title = paste0(op_label, " - Library Energy Ranking by Workload"),
      subtitle = "1 = most efficient (green) | 5 = least efficient (red)",
      x = "Workload Configuration",
      y = NULL
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

# --- Normalized energy heatmap ---
plot_norm_heatmap <- function(op_label) {
  plot_data <- means %>% filter(Operation == op_label)

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
      title = paste0(op_label, " - Relative Energy Cost by Workload"),
      subtitle = "1.0x = most efficient library for that config | Higher = worse",
      x = "Workload Configuration",
      y = NULL
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

plot_rank_heatmap("Deserialize")
plot_rank_heatmap("Serialize")
plot_norm_heatmap("Deserialize")
plot_norm_heatmap("Serialize")

cat("\nDone!\n")
