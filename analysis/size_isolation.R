library(tidyverse)

# --- Config: which bench to run ---
# Pass "byte" or "string" as first CLI arg (default: both)
cli_args <- commandArgs(trailingOnly = TRUE)
benches <- if (length(cli_args) > 0) cli_args else c("byte", "string")

args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."

# --- Helper: parse size suffix like C10, C100, C1K, C10K, C100K ---
parse_size <- function(s) {
  num <- as.numeric(str_extract(s, "\\d+"))
  has_k <- str_detect(s, "K$")
  ifelse(has_k, num * 1000, num)
}

# --- Colors ---
lib_colors <- c(
  "SpanJson"   = "#2196F3",
  "Utf8Json"   = "#4CAF50",
  "Jil"        = "#FF9800",
  "STJ"        = "#9C27B0",
  "Newtonsoft"  = "#F44336"
)

# --- Main analysis function ---
run_analysis <- function(bench_type) {
  bench_label <- ifelse(bench_type == "byte", "Byte", "String")
  bench_file  <- ifelse(bench_type == "byte",
                        "JsonBench.Benchmarks.Isolation.SizeIsolationByteBench",
                        "JsonBench.Benchmarks.Isolation.SizeIsolationStringBench")

  measurements_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                                 paste0(bench_file, "-measurements.csv"))
  plot_dir <- file.path(script_dir, "plots", paste0("size_", bench_type))
  dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)

  cat("\n=== Processing:", bench_label, "bench ===\n")

  # --- Load measurements ---
  raw <- read_csv(measurements_path, show_col_types = FALSE)

  df <- raw %>%
    filter(Measurement_IterationMode == "Workload",
           Measurement_IterationStage == "Result") %>%
    mutate(
      Library   = str_extract(Target_Method, "^[^_]+"),
      Operation = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
      SizeRaw   = str_extract(Target_Method, "(?<=_C)[\\d]+K?"),
      Size      = parse_size(SizeRaw),
      EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation,
      CorePerOp   = Measurement_CoreEnergyPerOperation,
      DramPerOp   = Measurement_DramEnergyPerOperation,
      PkgPerOp    = Measurement_PackageEnergyPerOperation,
      Temperature = Measurement_Temperature
    ) %>%
    mutate(
      Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
      Operation = factor(Operation, levels = c("Deser", "Ser"),
                         labels = c("Deserialize", "Serialize"))
    )

  size_levels <- sort(unique(df$Size))
  size_labels <- ifelse(size_levels >= 1000,
                        paste0(size_levels / 1000, "K"),
                        as.character(size_levels))

  # --- Means ---
  means <- df %>%
    group_by(Library, Operation, Size) %>%
    summarise(
      MeanEnergy = mean(EnergyPerOp),
      MeanPkg    = mean(PkgPerOp),
      MeanDram   = mean(DramPerOp),
      MeanCore   = mean(CorePerOp),
      MeanTemp   = mean(Temperature),
      SDEnergy   = sd(EnergyPerOp),
      N          = n(),
      .groups    = "drop"
    ) %>%
    mutate(
      CV = SDEnergy / MeanEnergy * 100,
      SizeLabel = factor(ifelse(Size >= 1000, paste0(Size / 1000, "K"), as.character(Size)),
                         levels = size_labels)
    )

  # ============================================================
  # 1. ENERGY SCALING LINE PLOT
  # ============================================================
  plot_scaling <- function(op_label) {
    plot_data <- means %>% filter(Operation == op_label)

    p <- ggplot(plot_data, aes(x = Size, y = MeanEnergy,
                                color = Library, group = Library)) +
      geom_line(linewidth = 0.9) +
      geom_point(size = 2.5) +
      scale_color_manual(values = lib_colors) +
      scale_x_log10(breaks = size_levels, labels = size_labels) +
      labs(
        title = paste0(op_label, " – Energy Scaling with Size (", bench_label, ")"),
        subtitle = "Isolation benchmark: only size varies, all other dimensions held constant",
        x = "Size (number of elements)",
        y = "Total Energy (uJ/op)  [Package + DRAM]",
        color = "Library"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        legend.position = "bottom",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        panel.grid.minor = element_blank()
      )

    fname <- file.path(plot_dir, paste0("scaling_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 10, height = 6, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 2. RELATIVE EFFICIENCY HEATMAP
  # ============================================================
  plot_relative_heatmap <- function(op_label) {
    hm_data <- means %>%
      filter(Operation == op_label) %>%
      group_by(Size) %>%
      mutate(NormEnergy = MeanEnergy / min(MeanEnergy)) %>%
      ungroup()

    p <- ggplot(hm_data, aes(x = SizeLabel, y = Library, fill = NormEnergy)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sprintf("%.2fx", NormEnergy)), size = 3.5) +
      scale_fill_gradient2(
        low = "#4CAF50", mid = "#FFC107", high = "#F44336",
        midpoint = 2, limits = c(1, NA),
        name = "Ratio to best"
      ) +
      labs(
        title = paste0(op_label, " – Relative Energy Cost per Size (", bench_label, ")"),
        subtitle = "1.00x = most efficient library at that size | higher = worse",
        x = "Size (elements)", y = NULL
      ) +
      theme_minimal(base_size = 11) +
      theme(
        axis.text.y  = element_text(face = "bold"),
        legend.position = "right",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        panel.grid    = element_blank()
      )

    fname <- file.path(plot_dir, paste0("heatmap_relative_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 10, height = 4, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 3. RANK STABILITY
  # ============================================================
  plot_rank_bump <- function(op_label) {
    rank_data <- means %>%
      filter(Operation == op_label) %>%
      group_by(Size) %>%
      mutate(Rank = rank(MeanEnergy, ties.method = "min")) %>%
      ungroup()

    p <- ggplot(rank_data, aes(x = Size, y = Rank, color = Library, group = Library)) +
      geom_line(linewidth = 1.2) +
      geom_point(size = 3) +
      scale_y_reverse(breaks = 1:5, labels = paste0("#", 1:5)) +
      scale_x_log10(breaks = size_levels, labels = size_labels) +
      scale_color_manual(values = lib_colors) +
      labs(
        title = paste0(op_label, " – Library Ranking Stability Across Sizes (", bench_label, ")"),
        subtitle = "#1 = most energy-efficient | crossovers indicate ranking instability",
        x = "Size (elements)",
        y = "Rank",
        color = "Library"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        legend.position = "bottom",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        panel.grid.minor = element_blank()
      )

    fname <- file.path(plot_dir, paste0("rank_bump_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 10, height = 5, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 4. ENERGY BREAKDOWN (Package vs DRAM)
  # ============================================================
  plot_energy_breakdown <- function(op_label) {
    bd_data <- means %>%
      filter(Operation == op_label) %>%
      select(Library, Size, SizeLabel, MeanPkg, MeanDram) %>%
      pivot_longer(cols = c(MeanPkg, MeanDram),
                   names_to = "Component", values_to = "Energy") %>%
      mutate(
        Component = factor(Component,
                           levels = c("MeanDram", "MeanPkg"),
                           labels = c("DRAM", "Package"))
      )

    p <- ggplot(bd_data, aes(x = SizeLabel, y = Energy, fill = Component)) +
      geom_col(position = "stack", width = 0.7) +
      facet_wrap(~ Library, nrow = 1) +
      scale_fill_manual(values = c("Package" = "#1976D2", "DRAM" = "#FF8F00")) +
      labs(
        title = paste0(op_label, " – Energy Breakdown: Package vs DRAM per Size (", bench_label, ")"),
        subtitle = "Total Energy = Package + DRAM | stacked bars show both RAPL domains",
        x = "Size (elements)",
        y = "Energy (uJ/op)",
        fill = "Component"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        strip.text    = element_text(face = "bold", size = 10),
        axis.text.x   = element_text(angle = 45, hjust = 1, size = 8),
        legend.position = "bottom",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40")
      )

    fname <- file.path(plot_dir, paste0("breakdown_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 14, height = 5, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # DRAM fraction heatmap
  plot_dram_fraction_heatmap <- function(op_label) {
    frac_data <- means %>%
      filter(Operation == op_label) %>%
      mutate(DramFrac = MeanDram / (MeanPkg + MeanDram) * 100)

    p <- ggplot(frac_data, aes(x = SizeLabel, y = Library, fill = DramFrac)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sprintf("%.1f%%", DramFrac)), size = 3.5) +
      scale_fill_gradient2(
        low = "#1976D2", mid = "#FFC107", high = "#FF8F00",
        midpoint = 5, name = "DRAM %"
      ) +
      labs(
        title = paste0(op_label, " – DRAM Energy Fraction by Size (", bench_label, ")"),
        subtitle = "Higher % → more memory-bound (investigate allocations, GC pressure)",
        x = "Size (elements)", y = NULL
      ) +
      theme_minimal(base_size = 11) +
      theme(
        axis.text.y  = element_text(face = "bold"),
        legend.position = "right",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        panel.grid    = element_blank()
      )

    fname <- file.path(plot_dir, paste0("heatmap_dram_frac_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 10, height = 4, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 5. BOXPLOT (per-iteration variability)
  # ============================================================
  plot_boxplot <- function(op_label) {
    plot_data <- df %>%
      filter(Operation == op_label) %>%
      mutate(SizeLabel = factor(ifelse(Size >= 1000, paste0(Size / 1000, "K"), as.character(Size)),
                                levels = size_labels))

    p <- ggplot(plot_data, aes(x = Library, y = EnergyPerOp, fill = Library)) +
      geom_boxplot(outlier.size = 0.8, outlier.alpha = 0.5) +
      facet_wrap(~ SizeLabel, scales = "free_y", nrow = 1) +
      scale_fill_manual(values = lib_colors) +
      labs(
        title = paste0(op_label, " – Per-Iteration Energy Distribution by Size (", bench_label, ")"),
        subtitle = "Wider boxes = higher variability | outliers may indicate GC events",
        x = NULL,
        y = "Total Energy (uJ/op)",
        fill = "Library"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        strip.text    = element_text(face = "bold", size = 10),
        axis.text.x   = element_blank(),
        legend.position = "bottom",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40")
      )

    fname <- file.path(plot_dir, paste0("boxplot_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 16, height = 5, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 6. TIME VS ENERGY SCATTER
  # ============================================================
  plot_time_vs_energy <- function(op_label) {
    plot_data <- df %>%
      filter(Operation == op_label) %>%
      mutate(TimeUs = Measurement_Nanoseconds / Measurement_Operations / 1000)

    fit <- lm(EnergyPerOp ~ TimeUs, data = plot_data)

    p <- ggplot(plot_data, aes(x = TimeUs, y = EnergyPerOp, color = Library)) +
      geom_point(alpha = 0.5, size = 1.5) +
      geom_smooth(method = "lm", se = FALSE, linewidth = 0.7, linetype = "dashed") +
      scale_color_manual(values = lib_colors) +
      labs(
        title = paste0(op_label, " – Time vs Energy per Operation (", bench_label, ")"),
        subtitle = paste0("Overall R² = ", round(summary(fit)$r.squared, 4),
                          " | Deviations from line = energy efficiency differences"),
        x = "Time (μs/op)",
        y = "Total Energy (uJ/op)",
        color = "Library"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        legend.position = "bottom",
        plot.title    = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        panel.grid.minor = element_blank()
      )

    fname <- file.path(plot_dir, paste0("time_vs_energy_",
                       tolower(gsub("ialize", "", op_label)), ".png"))
    ggsave(fname, p, width = 10, height = 6, dpi = 150)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # 7. SCALING RATIO TABLE
  # ============================================================
  save_scaling_table <- function() {
    s_min <- min(size_levels)
    s_max <- max(size_levels)
    min_label <- ifelse(s_min >= 1000, paste0("C", s_min / 1000, "K"), paste0("C", s_min))
    max_label <- ifelse(s_max >= 1000, paste0("C", s_max / 1000, "K"), paste0("C", s_max))

    table_data <- means %>%
      select(Library, Operation, Size, MeanEnergy) %>%
      mutate(SizeCol = ifelse(Size >= 1000, paste0("C", Size / 1000, "K"), paste0("C", Size))) %>%
      select(-Size) %>%
      pivot_wider(names_from = SizeCol, values_from = MeanEnergy) %>%
      mutate(
        Ratio = round(.data[[max_label]] / .data[[min_label]], 2),
        across(starts_with("C"), ~ round(.x, 2))
      ) %>%
      arrange(Operation, Library)

    fname <- file.path(plot_dir, "scaling_table.csv")
    write_csv(table_data, fname)
    cat("Saved:", fname, "\n")
  }

  # ============================================================
  # Run all
  # ============================================================
  for (op in c("Deserialize", "Serialize")) {
    plot_scaling(op)
    plot_relative_heatmap(op)
    plot_rank_bump(op)
    plot_energy_breakdown(op)
    plot_dram_fraction_heatmap(op)
    plot_boxplot(op)
    plot_time_vs_energy(op)
  }
  save_scaling_table()

  cat("\nDone! Plots saved to:", plot_dir, "\n")
}

# --- Run for requested bench types ---
for (bt in benches) {
  run_analysis(bt)
}
