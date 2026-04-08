library(tidyverse)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialNormalizedStringBench-measurements.csv")
out_dir <- file.path(script_dir, "plots", "05_time_energy")
dir.create(out_dir, showWarnings = FALSE, recursive = TRUE)
raw <- read_csv(csv_path, show_col_types = FALSE)

# Filter to workload actual iterations
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Actual")

# Parse method name
df <- df %>%
  mutate(
    Library   = str_extract(Target_Method, "^[^_]+"),
    Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
    Depth     = as.integer(str_extract(Target_Method, "(?<=_D)\\d+")),
    Width     = as.integer(str_extract(Target_Method, "(?<=_W)\\d+")),
    Content   = str_extract(Target_Method, "[TNB]$"),
    EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation,
    TimeUs      = Measurement_Nanoseconds / Measurement_Operations / 1000
  ) %>%
  mutate(
    Content   = factor(Content, levels = c("T", "N", "B"),
                       labels = c("Textual", "Numeric", "Boolean")),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize")),
    DepthLabel = factor(paste0("Depth ", Depth),
                        levels = paste0("Depth ", c(2, 5, 10, 20))),
    WidthLabel = factor(paste0("Width ", Width),
                        levels = paste0("Width ", c(5, 20, 50, 100)))
  )

# --- Colors ---
lib_colors <- c(
  "SpanJson" = "#2196F3", "Utf8Json" = "#4CAF50", "Jil" = "#FF9800",
  "STJ" = "#9C27B0", "Newtonsoft" = "#F44336"
)

# ===========================================================================
# PART 1: Overall scatter — one per operation (overview)
# ===========================================================================
cat("\n=== Time vs Energy Correlation ===\n")

lib_op_stats <- df %>%
  group_by(Library, Operation) %>%
  summarise(
    n         = n(),
    R2        = summary(lm(EnergyPerOp ~ TimeUs))$r.squared,
    AvgPowerW = mean(EnergyPerOp / TimeUs),
    .groups = "drop"
  )

print(lib_op_stats, n = Inf)
write_csv(lib_op_stats, file.path(out_dir, "r2_library_operation.csv"))

for (op in levels(df$Operation)) {
  pd <- df %>% filter(Operation == op)
  stats <- lib_op_stats %>% filter(Operation == op)
  label_str <- paste(
    paste0(stats$Library, ": R²=", sprintf("%.4f", stats$R2),
           ", ", sprintf("%.1fW", stats$AvgPowerW)),
    collapse = "  |  "
  )

  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.4, size = 1.5) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.7, linetype = "dashed") +
    scale_color_manual(values = lib_colors) +
    labs(title = sprintf("Time vs Energy — %s", op),
         subtitle = label_str,
         x = "Time (μs/op)", y = "Total Energy (μJ/op) [Package + DRAM]",
         color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom",
          plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 8, color = "grey40"),
          panel.grid.minor = element_blank())
  ggsave(file.path(out_dir, sprintf("scatter_%s.png", tolower(op))),
         p, width = 10, height = 6, dpi = 150)
}
cat("Overall scatter plots saved.\n")

# ===========================================================================
# PART 2: Faceted scatter plots — by Content, Depth, Width
# ===========================================================================

# --- By Content Type: one plot per operation, faceted by Content ---
for (op in levels(df$Operation)) {
  pd <- df %>% filter(Operation == op)

  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.4, size = 1.2) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.6, linetype = "dashed") +
    facet_wrap(~ Content, scales = "free") +
    scale_color_manual(values = lib_colors) +
    labs(title = sprintf("Time vs Energy — %s — by Content Type", op),
         x = "Time (μs/op)", y = "Energy (μJ/op)",
         color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom",
          plot.title = element_text(size = 14, face = "bold"),
          strip.text = element_text(face = "bold"),
          panel.grid.minor = element_blank())
  ggsave(file.path(out_dir, sprintf("scatter_content_%s.png", tolower(op))),
         p, width = 14, height = 5, dpi = 150)
}

# --- By Depth: one plot per operation, faceted by Depth ---
for (op in levels(df$Operation)) {
  pd <- df %>% filter(Operation == op)

  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.4, size = 1.2) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.6, linetype = "dashed") +
    facet_wrap(~ DepthLabel, scales = "free") +
    scale_color_manual(values = lib_colors) +
    labs(title = sprintf("Time vs Energy — %s — by Depth", op),
         x = "Time (μs/op)", y = "Energy (μJ/op)",
         color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom",
          plot.title = element_text(size = 14, face = "bold"),
          strip.text = element_text(face = "bold"),
          panel.grid.minor = element_blank())
  ggsave(file.path(out_dir, sprintf("scatter_depth_%s.png", tolower(op))),
         p, width = 14, height = 5, dpi = 150)
}

# --- By Width: one plot per operation, faceted by Width ---
for (op in levels(df$Operation)) {
  pd <- df %>% filter(Operation == op)

  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.4, size = 1.2) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.6, linetype = "dashed") +
    facet_wrap(~ WidthLabel, scales = "free") +
    scale_color_manual(values = lib_colors) +
    labs(title = sprintf("Time vs Energy — %s — by Width", op),
         x = "Time (μs/op)", y = "Energy (μJ/op)",
         color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom",
          plot.title = element_text(size = 14, face = "bold"),
          strip.text = element_text(face = "bold"),
          panel.grid.minor = element_blank())
  ggsave(file.path(out_dir, sprintf("scatter_width_%s.png", tolower(op))),
         p, width = 14, height = 5, dpi = 150)
}

# --- Full grid: Depth (rows) x Content (cols), per operation ---
for (op in levels(df$Operation)) {
  pd <- df %>% filter(Operation == op)

  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.4, size = 1) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.5, linetype = "dashed") +
    facet_grid(DepthLabel ~ Content, scales = "free") +
    scale_color_manual(values = lib_colors) +
    labs(title = sprintf("Time vs Energy — %s — Depth x Content", op),
         x = "Time (μs/op)", y = "Energy (μJ/op)",
         color = "Library") +
    theme_minimal(base_size = 10) +
    theme(legend.position = "bottom",
          plot.title = element_text(size = 14, face = "bold"),
          strip.text = element_text(face = "bold", size = 9),
          panel.grid.minor = element_blank())
  ggsave(file.path(out_dir, sprintf("scatter_grid_depth_content_%s.png", tolower(op))),
         p, width = 12, height = 10, dpi = 150)
}

cat("Faceted scatter plots saved.\n")

# ===========================================================================
# PART 3: Granular R² and Power tables
# ===========================================================================

# --- R² by Library x Operation x Content ---
r2_content <- df %>%
  group_by(Library, Operation, Content) %>%
  summarise(
    n         = n(),
    R2        = summary(lm(EnergyPerOp ~ TimeUs))$r.squared,
    AvgPowerW = mean(EnergyPerOp / TimeUs),
    .groups = "drop"
  )
write_csv(r2_content, file.path(out_dir, "r2_by_content.csv"))

# --- R² by Library x Operation x Depth ---
r2_depth <- df %>%
  group_by(Library, Operation, Depth) %>%
  summarise(
    n         = n(),
    R2        = summary(lm(EnergyPerOp ~ TimeUs))$r.squared,
    AvgPowerW = mean(EnergyPerOp / TimeUs),
    .groups = "drop"
  )
write_csv(r2_depth, file.path(out_dir, "r2_by_depth.csv"))

# --- R² by Library x Operation x Width ---
r2_width <- df %>%
  group_by(Library, Operation, Width) %>%
  summarise(
    n         = n(),
    R2        = summary(lm(EnergyPerOp ~ TimeUs))$r.squared,
    AvgPowerW = mean(EnergyPerOp / TimeUs),
    .groups = "drop"
  )
write_csv(r2_width, file.path(out_dir, "r2_by_width.csv"))

# ===========================================================================
# PART 4: R² heatmaps
# ===========================================================================

# --- R² heatmap: Library x Operation (overall) ---
p_r2 <- ggplot(lib_op_stats, aes(x = Operation, y = Library, fill = R2)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.4f", R2)), size = 4, fontface = "bold") +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "R²", limits = c(0.99, 1)) +
  labs(title = "Time-Energy R² per Library and Operation",
       subtitle = "Higher R² = time predicts energy almost perfectly",
       x = "Operation", y = "Library") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"), panel.grid = element_blank())
ggsave(file.path(out_dir, "r2_heatmap.png"), p_r2,
       width = 7, height = 5, dpi = 150)

# --- R² heatmap by Content: Library (y) x Content (x), faceted by Operation ---
p_r2_content <- ggplot(r2_content, aes(x = Content, y = Library, fill = R2)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.4f", R2)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "R²", limits = c(0.99, 1)) +
  labs(title = "Time-Energy R² by Content Type",
       x = "Content Type", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "r2_heatmap_content.png"), p_r2_content,
       width = 10, height = 5, dpi = 150)

# --- R² heatmap by Depth: Library (y) x Depth (x), faceted by Operation ---
p_r2_depth <- ggplot(r2_depth, aes(x = factor(Depth), y = Library, fill = R2)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.4f", R2)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "R²", limits = c(0.99, 1)) +
  labs(title = "Time-Energy R² by Depth",
       x = "Depth", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "r2_heatmap_depth.png"), p_r2_depth,
       width = 10, height = 5, dpi = 150)

# --- R² heatmap by Width: Library (y) x Width (x), faceted by Operation ---
p_r2_width <- ggplot(r2_width, aes(x = factor(Width), y = Library, fill = R2)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.4f", R2)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                      name = "R²", limits = c(0.99, 1)) +
  labs(title = "Time-Energy R² by Width",
       x = "Width", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "r2_heatmap_width.png"), p_r2_width,
       width = 10, height = 5, dpi = 150)

cat("R² heatmaps saved.\n")

# ===========================================================================
# PART 5: Power heatmaps
# ===========================================================================

# --- Overall power heatmap ---
p_power <- ggplot(lib_op_stats, aes(x = Operation, y = Library, fill = AvgPowerW)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.1fW", AvgPowerW)), size = 4, fontface = "bold") +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Avg Power (W)") +
  labs(title = "Average Power Draw per Library and Operation",
       subtitle = "Energy / Time = effective power (W)",
       x = "Operation", y = "Library") +
  theme_minimal(base_size = 12) +
  theme(plot.title = element_text(face = "bold"), panel.grid = element_blank())
ggsave(file.path(out_dir, "power_heatmap.png"), p_power,
       width = 7, height = 5, dpi = 150)

# --- Power by Content ---
p_power_content <- ggplot(r2_content, aes(x = Content, y = Library, fill = AvgPowerW)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.1fW", AvgPowerW)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Avg Power (W)") +
  labs(title = "Average Power Draw by Content Type",
       x = "Content Type", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "power_heatmap_content.png"), p_power_content,
       width = 10, height = 5, dpi = 150)

# --- Power by Depth ---
p_power_depth <- ggplot(r2_depth, aes(x = factor(Depth), y = Library, fill = AvgPowerW)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.1fW", AvgPowerW)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Avg Power (W)") +
  labs(title = "Average Power Draw by Depth",
       x = "Depth", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "power_heatmap_depth.png"), p_power_depth,
       width = 10, height = 5, dpi = 150)

# --- Power by Width ---
p_power_width <- ggplot(r2_width, aes(x = factor(Width), y = Library, fill = AvgPowerW)) +
  geom_tile(color = "white", linewidth = 0.5) +
  geom_text(aes(label = sprintf("%.1fW", AvgPowerW)), size = 3.5, fontface = "bold") +
  facet_wrap(~ Operation) +
  scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Avg Power (W)") +
  labs(title = "Average Power Draw by Width",
       x = "Width", y = "Library") +
  theme_minimal(base_size = 11) +
  theme(plot.title = element_text(face = "bold"),
        strip.text = element_text(face = "bold"),
        panel.grid = element_blank())
ggsave(file.path(out_dir, "power_heatmap_width.png"), p_power_width,
       width = 10, height = 5, dpi = 150)

cat("Power heatmaps saved.\n")

# ===========================================================================
# PART 6: Per-workload R² and Power (across libraries within each config)
# ===========================================================================
cat("\n=== Per-Workload R² and Power ===\n")

# Workload ordering: sorted by Depth then Width
workload_order <- df %>%
  distinct(Depth, Width) %>%
  arrange(Depth, Width) %>%
  mutate(Workload = sprintf("D%d_W%d", Depth, Width)) %>%
  pull(Workload)

r2_workload <- df %>%
  group_by(Operation, Depth, Width, Content) %>%
  summarise(
    n         = n(),
    R2        = summary(lm(EnergyPerOp ~ TimeUs))$r.squared,
    AvgPowerW = mean(EnergyPerOp / TimeUs),
    .groups = "drop"
  ) %>%
  mutate(Workload = factor(sprintf("D%d_W%d", Depth, Width), levels = workload_order))

write_csv(r2_workload, file.path(out_dir, "r2_per_workload.csv"))

cat(sprintf("Workload configs: %d\n", nrow(r2_workload)))
cat(sprintf("R² range: %.4f – %.4f\n", min(r2_workload$R2), max(r2_workload$R2)))
cat(sprintf("Power range: %.1fW – %.1fW\n", min(r2_workload$AvgPowerW), max(r2_workload$AvgPowerW)))

# --- R² heatmap per workload: Workload (x) x Content (y), faceted by Operation ---
for (op in levels(df$Operation)) {
  sub <- r2_workload %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = Content, fill = R2)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.4f", R2)), size = 2.8, fontface = "bold") +
    scale_fill_gradient(low = "#FFF9C4", high = "#2E7D32",
                        name = "R²", limits = c(min(r2_workload$R2), 1)) +
    labs(title = sprintf("Time-Energy R² per Workload — %s", op),
         subtitle = "R² computed across all 5 libraries within each workload config",
         x = "Workload (Depth x Width)", y = "Content Type") +
    theme_minimal(base_size = 11) +
    theme(plot.title = element_text(face = "bold"),
          axis.text.x = element_text(angle = 45, hjust = 1),
          panel.grid = element_blank())
  ggsave(file.path(out_dir, sprintf("r2_per_workload_%s.png", tolower(op))),
         p, width = 14, height = 4, dpi = 150)
}

# --- Power heatmap per workload: Workload (x) x Content (y), faceted by Operation ---
for (op in levels(df$Operation)) {
  sub <- r2_workload %>% filter(Operation == op)

  p <- ggplot(sub, aes(x = Workload, y = Content, fill = AvgPowerW)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.1fW", AvgPowerW)), size = 2.8, fontface = "bold") +
    scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Avg Power (W)") +
    labs(title = sprintf("Average Power per Workload — %s", op),
         subtitle = "Energy / Time across all libraries within each workload config",
         x = "Workload (Depth x Width)", y = "Content Type") +
    theme_minimal(base_size = 11) +
    theme(plot.title = element_text(face = "bold"),
          axis.text.x = element_text(angle = 45, hjust = 1),
          panel.grid = element_blank())
  ggsave(file.path(out_dir, sprintf("power_per_workload_%s.png", tolower(op))),
         p, width = 14, height = 4, dpi = 150)
}

# --- Scatter plots per workload: Depth (rows) x Width (cols), faceted, per operation x content ---
for (op in levels(df$Operation)) {
  for (ct in levels(df$Content)) {
    pd <- df %>% filter(Operation == op, Content == ct)

    p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
      geom_point(alpha = 0.5, size = 1.5) +
      geom_smooth(method = "lm", se = FALSE, linewidth = 0.5, linetype = "dashed") +
      facet_grid(DepthLabel ~ WidthLabel, scales = "free") +
      scale_color_manual(values = lib_colors) +
      labs(title = sprintf("Time vs Energy — %s — %s", op, ct),
           subtitle = "Each panel = one Depth x Width config, points = libraries x iterations",
           x = "Time (μs/op)", y = "Energy (μJ/op)",
           color = "Library") +
      theme_minimal(base_size = 10) +
      theme(legend.position = "bottom",
            plot.title = element_text(size = 14, face = "bold"),
            strip.text = element_text(face = "bold", size = 9),
            panel.grid.minor = element_blank())
    ggsave(file.path(out_dir, sprintf("scatter_workload_%s_%s.png",
                                       tolower(op), tolower(ct))),
           p, width = 14, height = 10, dpi = 150)
  }
}

cat("Per-workload plots saved.\n")

cat("\nDone.\n")
