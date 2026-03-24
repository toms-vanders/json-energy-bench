library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialStringBench-measurements.csv")
plot_dir <- file.path(script_dir, "plots")
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

# Content labels
df <- df %>%
  mutate(
    Content = factor(Content, levels = c("T", "N", "B"),
                     labels = c("Textual", "Numeric", "Boolean")),
    Depth   = factor(Depth, levels = c(2, 5, 10, 20),
                     labels = paste0("Depth ", c(2, 5, 10, 20))),
    Width   = factor(Width, levels = c(5, 20, 50, 100)),
    Library = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize"))
  )

# Compute mean energy per combination
means <- df %>%
  group_by(Library, Operation, Depth, Width, Content) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

# Library colors
lib_colors <- c(
  "SpanJson"  = "#2196F3",
  "Utf8Json"  = "#4CAF50",
  "Jil"       = "#FF9800",
  "STJ"       = "#9C27B0",
  "Newtonsoft" = "#F44336"
)

# Content fill colors
content_colors <- c(
  "Textual" = "#E57373",
  "Numeric" = "#64B5F6",
  "Boolean" = "#81C784"
)

# --- Plot: Facet grid Library (cols) x Depth (rows) ---
# X-axis: Width, Fill: Content, Y: Energy
plot_facet <- function(op_label) {
  plot_data <- means %>% filter(Operation == op_label)

  p <- ggplot(plot_data, aes(x = Width, y = MeanEnergy, fill = Content)) +
    geom_col(position = position_dodge(width = 0.8), width = 0.7) +
    facet_grid(Depth ~ Library, scales = "free_y") +
    scale_fill_manual(values = content_colors) +
    labs(
      title = paste0(op_label, " - Energy per Operation (uJ/op)"),
      subtitle = "Rows: Nesting Depth | Columns: Library | X: Width | Color: Content Type",
      x = "Width (fields per object)",
      y = "Energy (uJ/op)",
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

  fname <- file.path(plot_dir, paste0("factorial_",
                   tolower(gsub("ialize", "", op_label)), ".png"))
  dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)
  ggsave(fname, p, width = 16, height = 10, dpi = 150)
  cat("Saved:", fname, "\n")
}

plot_facet("Deserialize")
plot_facet("Serialize")

cat("\nDone!\n")
