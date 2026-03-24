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

# Parse method name: e.g. "SpanJson_Deser_D10_W100_B"
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
                       labels = c("Deserialize", "Serialize")),
    DepthLabel = factor(paste0("Depth ", Depth),
                        levels = paste0("Depth ", c(2, 5, 10, 20)))
  )

# Compute mean energy per combination
means <- df %>%
  group_by(Library, Operation, Depth, DepthLabel, Width, Content) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

# Library colors
lib_colors <- c(
  "SpanJson"   = "#2196F3",
  "Utf8Json"   = "#4CAF50",
  "Jil"        = "#FF9800",
  "STJ"        = "#9C27B0",
  "Newtonsoft"  = "#F44336"
)

# --- Width scaling: X = Width, lines = Library ---
# Facet grid: Depth (rows) x Content (cols)
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
      title = paste0(op_label, " - Energy Scaling with Width"),
      subtitle = "Rows: Nesting Depth | Columns: Content Type | Lines: Library",
      x = "Width (fields per object)",
      y = "Energy (uJ/op)",
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

# --- Depth scaling: X = Depth, lines = Library ---
# Facet grid: Width (rows) x Content (cols)
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
      title = paste0(op_label, " - Energy Scaling with Depth"),
      subtitle = "Rows: Structural Width | Columns: Content Type | Lines: Library",
      x = "Nesting Depth",
      y = "Energy (uJ/op)",
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

plot_width_scaling("Deserialize")
plot_width_scaling("Serialize")
plot_depth_scaling("Deserialize")
plot_depth_scaling("Serialize")

cat("\nDone!\n")
