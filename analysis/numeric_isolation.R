library(tidyverse)

args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."

measurements_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                               "JsonBench.Benchmarks.Isolation.NumericIsolationStringBench-measurements.csv")
plot_dir <- file.path(script_dir, "plots", "numeric")
dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)

raw <- read_csv(measurements_path, show_col_types = FALSE)

# Numeric is special: IDs are I100, I70, I50, I30, F100
# Extract the full suffix after Ser_/Deser_ as the level ID
df <- raw %>%
  filter(Measurement_IterationMode == "Workload",
         Measurement_IterationStage == "Result") %>%
  mutate(
    Library   = str_extract(Target_Method, "^[^_]+"),
    Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
    LevelID   = str_extract(Target_Method, "[IF]\\d+$"),
    EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation,
    DramPerOp   = Measurement_DramEnergyPerOperation,
    PkgPerOp    = Measurement_PackageEnergyPerOperation,
    Temperature = Measurement_Temperature
  ) %>%
  mutate(
    # Map to integer % for numeric x-axis: I100=100, I70=70, I50=50, I30=30, F100=0
    IntPct = case_when(
      LevelID == "I100" ~ 100L,
      LevelID == "I70"  ~ 70L,
      LevelID == "I50"  ~ 50L,
      LevelID == "I30"  ~ 30L,
      LevelID == "F100" ~ 0L
    ),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"), labels = c("Deserialize", "Serialize"))
  )

means <- df %>%
  group_by(Library, Operation, IntPct, LevelID) %>%
  summarise(MeanEnergy = mean(EnergyPerOp), MeanPkg = mean(PkgPerOp),
            MeanDram = mean(DramPerOp), SDEnergy = sd(EnergyPerOp),
            N = n(), .groups = "drop") %>%
  mutate(CV = SDEnergy / MeanEnergy * 100)

lib_colors <- c("SpanJson" = "#2196F3", "Utf8Json" = "#4CAF50", "Jil" = "#FF9800",
                "STJ" = "#9C27B0", "Newtonsoft" = "#F44336")

# Ordered levels for factor labels
level_order <- c("F100", "I30", "I50", "I70", "I100")
int_pct_breaks <- c(0, 30, 50, 70, 100)

plot_scaling <- function(op_label) {
  pd <- means %>% filter(Operation == op_label)
  p <- ggplot(pd, aes(x = IntPct, y = MeanEnergy, color = Library, group = Library)) +
    geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
    scale_color_manual(values = lib_colors) +
    scale_x_continuous(breaks = int_pct_breaks, labels = c("0\n(F100)", "30\n(I30)", "50\n(I50)", "70\n(I70)", "100\n(I100)")) +
    labs(title = paste0(op_label, " – Energy Scaling with Integer/Float Ratio"),
         subtitle = "Isolation benchmark: only integer % varies, content is 100% numeric",
         x = "Integer Ratio (%)", y = "Total Energy (uJ/op)  [Package + DRAM]", color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid.minor = element_blank())
  ggsave(file.path(plot_dir, paste0("scaling_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 10, height = 6, dpi = 150)
}

plot_relative_heatmap <- function(op_label) {
  hm <- means %>% filter(Operation == op_label) %>%
    group_by(IntPct) %>% mutate(NormEnergy = MeanEnergy / min(MeanEnergy)) %>% ungroup() %>%
    mutate(DimLabel = factor(LevelID, levels = level_order))
  p <- ggplot(hm, aes(x = DimLabel, y = Library, fill = NormEnergy)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.2fx", NormEnergy)), size = 3.5) +
    scale_fill_gradient2(low = "#4CAF50", mid = "#FFC107", high = "#F44336",
                         midpoint = 2, limits = c(1, NA), name = "Ratio to best") +
    labs(title = paste0(op_label, " – Relative Energy Cost per Numeric Mix"),
         subtitle = "1.00x = most efficient | higher = worse", x = "Numeric Mix", y = NULL) +
    theme_minimal(base_size = 11) +
    theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
          plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid = element_blank())
  ggsave(file.path(plot_dir, paste0("heatmap_relative_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 10, height = 4, dpi = 150)
}

plot_rank_bump <- function(op_label) {
  rd <- means %>% filter(Operation == op_label) %>%
    group_by(IntPct) %>% mutate(Rank = rank(MeanEnergy, ties.method = "min")) %>% ungroup()
  p <- ggplot(rd, aes(x = IntPct, y = Rank, color = Library, group = Library)) +
    geom_line(linewidth = 1.2) + geom_point(size = 3) +
    scale_y_reverse(breaks = 1:5, labels = paste0("#", 1:5)) +
    scale_x_continuous(breaks = int_pct_breaks, labels = c("0\n(F100)", "30", "50", "70", "100\n(I100)")) +
    scale_color_manual(values = lib_colors) +
    labs(title = paste0(op_label, " – Library Ranking Stability Across Numeric Mixes"),
         subtitle = "#1 = most energy-efficient | crossovers indicate ranking instability",
         x = "Integer Ratio (%)", y = "Rank", color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid.minor = element_blank())
  ggsave(file.path(plot_dir, paste0("rank_bump_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 10, height = 5, dpi = 150)
}

plot_energy_breakdown <- function(op_label) {
  bd <- means %>% filter(Operation == op_label) %>%
    select(Library, LevelID, MeanPkg, MeanDram) %>%
    pivot_longer(cols = c(MeanPkg, MeanDram), names_to = "Component", values_to = "Energy") %>%
    mutate(Component = factor(Component, levels = c("MeanDram","MeanPkg"), labels = c("DRAM","Package")),
           DimLabel = factor(LevelID, levels = level_order))
  p <- ggplot(bd, aes(x = DimLabel, y = Energy, fill = Component)) +
    geom_col(position = "stack", width = 0.7) + facet_wrap(~ Library, nrow = 1) +
    scale_fill_manual(values = c("Package" = "#1976D2", "DRAM" = "#FF8F00")) +
    labs(title = paste0(op_label, " – Energy Breakdown: Package vs DRAM per Numeric Mix"),
         subtitle = "Total Energy = Package + DRAM | stacked bars show both RAPL domains",
         x = "Numeric Mix", y = "Energy (uJ/op)", fill = "Component") +
    theme_minimal(base_size = 11) +
    theme(strip.text = element_text(face = "bold", size = 10),
          axis.text.x = element_text(angle = 45, hjust = 1, size = 8),
          legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"))
  ggsave(file.path(plot_dir, paste0("breakdown_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 14, height = 5, dpi = 150)
}

plot_dram_fraction_heatmap <- function(op_label) {
  fd <- means %>% filter(Operation == op_label) %>%
    mutate(DramFrac = MeanDram / (MeanPkg + MeanDram) * 100,
           DimLabel = factor(LevelID, levels = level_order))
  p <- ggplot(fd, aes(x = DimLabel, y = Library, fill = DramFrac)) +
    geom_tile(color = "white", linewidth = 0.5) +
    geom_text(aes(label = sprintf("%.1f%%", DramFrac)), size = 3.5) +
    scale_fill_gradient2(low = "#1976D2", mid = "#FFC107", high = "#FF8F00", midpoint = 5, name = "DRAM %") +
    labs(title = paste0(op_label, " – DRAM Energy Fraction by Numeric Mix"),
         subtitle = "Higher % → more memory-bound", x = "Numeric Mix", y = NULL) +
    theme_minimal(base_size = 11) +
    theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
          plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid = element_blank())
  ggsave(file.path(plot_dir, paste0("heatmap_dram_frac_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 10, height = 4, dpi = 150)
}

plot_boxplot <- function(op_label) {
  pd <- df %>% filter(Operation == op_label) %>%
    mutate(DimLabel = factor(LevelID, levels = level_order))
  p <- ggplot(pd, aes(x = Library, y = EnergyPerOp, fill = Library)) +
    geom_boxplot(outlier.size = 0.8, outlier.alpha = 0.5) +
    facet_wrap(~ DimLabel, scales = "free_y", nrow = 1) + scale_fill_manual(values = lib_colors) +
    labs(title = paste0(op_label, " – Per-Iteration Energy Distribution by Numeric Mix"),
         subtitle = "Wider boxes = higher variability | outliers may indicate GC events",
         x = NULL, y = "Total Energy (uJ/op)", fill = "Library") +
    theme_minimal(base_size = 11) +
    theme(strip.text = element_text(face = "bold", size = 10), axis.text.x = element_blank(),
          legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"))
  ggsave(file.path(plot_dir, paste0("boxplot_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 16, height = 5, dpi = 150)
}

plot_time_vs_energy <- function(op_label) {
  pd <- df %>% filter(Operation == op_label) %>%
    mutate(TimeUs = Measurement_Nanoseconds / Measurement_Operations / 1000)
  fit <- lm(EnergyPerOp ~ TimeUs, data = pd)
  p <- ggplot(pd, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
    geom_point(alpha = 0.5, size = 1.5) +
    geom_smooth(method = "lm", se = FALSE, linewidth = 0.7, linetype = "dashed") +
    scale_color_manual(values = lib_colors) +
    labs(title = paste0(op_label, " – Time vs Energy per Operation"),
         subtitle = paste0("Overall R² = ", round(summary(fit)$r.squared, 4)),
         x = "Time (μs/op)", y = "Total Energy (uJ/op)", color = "Library") +
    theme_minimal(base_size = 11) +
    theme(legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
          plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid.minor = element_blank())
  ggsave(file.path(plot_dir, paste0("time_vs_energy_", tolower(gsub("ialize","",op_label)), ".png")),
         p, width = 10, height = 6, dpi = 150)
}

save_scaling_table <- function() {
  td <- means %>% select(Library, Operation, LevelID, MeanEnergy) %>%
    pivot_wider(names_from = LevelID, values_from = MeanEnergy) %>%
    mutate(Ratio_F100_to_I100 = round(F100 / I100, 2),
           across(all_of(level_order), ~ round(.x, 2))) %>%
    arrange(Operation, Library)
  write_csv(td, file.path(plot_dir, "scaling_table.csv"))
}

for (op in c("Deserialize", "Serialize")) {
  plot_scaling(op); plot_relative_heatmap(op); plot_rank_bump(op)
  plot_energy_breakdown(op); plot_dram_fraction_heatmap(op)
  plot_boxplot(op); plot_time_vs_energy(op)
}
save_scaling_table()
cat("\nDone! All plots saved to:", plot_dir, "\n")
