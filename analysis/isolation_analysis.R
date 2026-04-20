library(tidyverse)

# --- Paths ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
results_dir <- file.path(script_dir, "..", "BenchmarkArtifacts", "results")
base_plot_dir <- file.path(script_dir, "plots")

# --- Colors ---
lib_colors <- c(
  "SpanJson" = "#2196F3", "Utf8Json" = "#4CAF50",
  "STJRefGen" = "#9C27B0", "STJSrcGen" = "#7B1FA2", "Newtonsoft" = "#F44336"
)

# --- Isolation benchmark definitions ---
benchmarks <- list(
  list(
    name = "width",
    file_stem = "WidthIsolation",
    dim_regex = "(?<=_W)\\d+",
    dim_prefix = "W",
    dim_label = "Width",
    x_label = "Width (fields per object)",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "depth",
    file_stem = "DepthIsolation",
    dim_regex = "(?<=_D)\\d+",
    dim_prefix = "D",
    dim_label = "Depth",
    x_label = "Nesting Depth",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "size",
    file_stem = "SizeIsolation",
    dim_regex = "(?<=_C)[0-9]+K?",
    dim_prefix = "C",
    dim_label = "Size",
    x_label = "Object Count",
    parse_dim = function(x) {
      ifelse(grepl("K$", x), as.integer(sub("K", "", x)) * 1000, as.integer(x))
    }
  ),
  list(
    name = "escape",
    file_stem = "EscapeIsolation",
    dim_regex = "(?<=_E)\\d+",
    dim_prefix = "E",
    dim_label = "Escape %",
    x_label = "Escape Character Percentage",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "unicode",
    file_stem = "UnicodeIsolation",
    dim_regex = "(?<=_U)\\d+",
    dim_prefix = "U",
    dim_label = "Unicode %",
    x_label = "Unicode Character Percentage",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "unicode_escape",
    file_stem = "UnicodeEscapeIsolation",
    dim_regex = "(?<=_UE)\\d+",
    dim_prefix = "UE",
    dim_label = "Unicode Escape %",
    x_label = "Unicode Escape Percentage",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "numeric",
    file_stem = "NumericIsolation",
    dim_regex = "(?<=_)[FI]\\d+",
    dim_prefix = "",
    dim_label = "Numeric Type",
    x_label = "Numeric Composition",
    parse_dim = function(x) x  # Keep as string (F100, I30, I50, etc.)
  ),
  list(
    name = "redundancy",
    file_stem = "RedundancyIsolation",
    dim_regex = "(?<=_R)\\d+",
    dim_prefix = "R",
    dim_label = "Redundancy %",
    x_label = "Key Redundancy Percentage",
    parse_dim = function(x) as.integer(x)
  )
)

# ===========================================================================
# MAIN PROCESSING LOOP
# ===========================================================================

for (bench in benchmarks) {
  measurements_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%sStringBench-measurements.csv", bench$file_stem))
  report_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%sStringBench-report.csv", bench$file_stem))

  if (!file.exists(measurements_file)) {
    cat(sprintf("Skipping %s — measurements file not found\n", bench$name))
    next
  }

  cat(sprintf("\n========== %s ==========\n", toupper(bench$name)))

  plot_dir <- file.path(base_plot_dir, bench$name)
  dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)

  # --- Load measurements ---
  raw <- read_csv(measurements_file, show_col_types = FALSE)

  df <- raw %>%
    filter(Measurement_IterationMode == "Workload",
           Measurement_IterationStage == "Result") %>%
    mutate(
      Library     = str_extract(Target_Method, "^[^_]+"),
      Operation   = str_extract(Target_Method, "(?<=_)(Deser|Ser)"),
      DimRaw      = str_extract(Target_Method, bench$dim_regex),
      DimValue    = bench$parse_dim(DimRaw),
      EnergyPerOp = Measurement_PackageEnergyPerOperation + Measurement_DramEnergyPerOperation,
      DramPerOp   = Measurement_DramEnergyPerOperation,
      PkgPerOp    = Measurement_PackageEnergyPerOperation,
      TimeUs      = Measurement_Nanoseconds / Measurement_Operations / 1000
    ) %>%
    mutate(
      Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
      Operation = factor(Operation, levels = c("Deser", "Ser"),
                         labels = c("Deserialize", "Serialize"))
    )

  # Create ordered dimension labels (sort DimRaw by DimValue)
  if (is.numeric(df$DimValue)) {
    raw_sorted <- df %>% distinct(DimRaw, DimValue) %>% arrange(DimValue) %>% pull(DimRaw)
    dim_order <- sort(unique(df$DimValue))
    dim_labels <- paste0(bench$dim_prefix, raw_sorted)
    df <- df %>% mutate(
      DimLabel = factor(paste0(bench$dim_prefix, DimRaw), levels = dim_labels)
    )
  } else {
    dim_order <- sort(unique(df$DimRaw))
    df <- df %>% mutate(DimLabel = factor(DimRaw, levels = dim_order))
    dim_labels <- dim_order
  }

  # --- Means ---
  means <- df %>%
    group_by(Library, Operation, DimValue, DimLabel) %>%
    summarise(
      MeanEnergy = mean(EnergyPerOp),
      MeanPkg    = mean(PkgPerOp),
      MeanDram   = mean(DramPerOp),
      .groups = "drop"
    )

  # ================================================================
  # 1. ENERGY SCALING LINE PLOT
  # ================================================================
  for (op in levels(df$Operation)) {
    pd <- means %>% filter(Operation == op)

    if (is.numeric(pd$DimValue)) {
      p <- ggplot(pd, aes(x = DimValue, y = MeanEnergy, color = Library, group = Library)) +
        geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
        scale_color_manual(values = lib_colors)
      # Use log scale if values span more than 2 orders of magnitude
      if (max(dim_order) / min(dim_order) > 100) {
        p <- p + scale_x_log10(breaks = dim_order, labels = dim_labels)
      } else {
        p <- p + scale_x_continuous(breaks = dim_order)
      }
    } else {
      p <- ggplot(pd, aes(x = DimLabel, y = MeanEnergy, color = Library, group = Library)) +
        geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
        scale_color_manual(values = lib_colors)
    }
    p <- p + labs(
      title = sprintf("%s – Energy Scaling with %s", op, bench$dim_label),
      subtitle = sprintf("Isolation benchmark: only %s varies", tolower(bench$dim_label)),
      x = bench$x_label, y = "Total Energy (μJ/op) [Package + DRAM]", color = "Library"
    ) +
      theme_minimal(base_size = 11) +
      theme(legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
            plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid.minor = element_blank())
    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(plot_dir, sprintf("scaling_%s.png", op_short)),
           p, width = 10, height = 6, dpi = 150)
  }
  cat("  Saved: scaling\n")

  # ================================================================
  # 2. RELATIVE EFFICIENCY HEATMAP
  # ================================================================
  for (op in levels(df$Operation)) {
    hm <- means %>% filter(Operation == op) %>%
      group_by(DimLabel) %>% mutate(NormEnergy = MeanEnergy / min(MeanEnergy)) %>% ungroup()
    p <- ggplot(hm, aes(x = DimLabel, y = Library, fill = NormEnergy)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sprintf("%.2fx", NormEnergy)), size = 3.5) +
      scale_fill_gradient2(low = "#4CAF50", mid = "#FFC107", high = "#F44336",
                           midpoint = 2, limits = c(1, NA), name = "Ratio to best") +
      labs(title = sprintf("%s – Relative Energy Cost per %s", op, bench$dim_label),
           subtitle = "1.00x = most efficient | higher = worse",
           x = bench$dim_label, y = NULL) +
      theme_minimal(base_size = 11) +
      theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
            plot.title = element_text(size = 14, face = "bold"),
            plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid = element_blank())
    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(plot_dir, sprintf("heatmap_relative_%s.png", op_short)),
           p, width = 10, height = 4, dpi = 150)
  }
  cat("  Saved: heatmap_relative\n")

  # ================================================================
  # 3. RANK BUMP CHART
  # ================================================================
  for (op in levels(df$Operation)) {
    rd <- means %>% filter(Operation == op) %>%
      group_by(DimLabel) %>% mutate(Rank = rank(MeanEnergy, ties.method = "min")) %>% ungroup()

    if (is.numeric(rd$DimValue)) {
      p <- ggplot(rd, aes(x = DimValue, y = Rank, color = Library, group = Library))
      if (max(dim_order) / min(dim_order) > 100) {
        p <- p + scale_x_log10(breaks = dim_order, labels = dim_labels)
      } else {
        p <- p + scale_x_continuous(breaks = dim_order)
      }
    } else {
      p <- ggplot(rd, aes(x = DimLabel, y = Rank, color = Library, group = Library))
    }
    p <- p +
      geom_line(linewidth = 1.2) + geom_point(size = 3) +
      scale_y_reverse(breaks = 1:5, labels = paste0("#", 1:5)) +
      scale_color_manual(values = lib_colors) +
      labs(title = sprintf("%s – Library Ranking Stability Across %s", op, bench$dim_label),
           subtitle = "#1 = most energy-efficient | crossovers indicate ranking instability",
           x = bench$x_label, y = "Rank", color = "Library") +
      theme_minimal(base_size = 11) +
      theme(legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
            plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid.minor = element_blank())
    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(plot_dir, sprintf("rank_bump_%s.png", op_short)),
           p, width = 10, height = 5, dpi = 150)
  }
  cat("  Saved: rank_bump\n")

  # ================================================================
  # 4. POWER HEATMAP
  # ================================================================
  for (op in levels(df$Operation)) {
    pd <- df %>% filter(Operation == op) %>%
      group_by(Library, DimLabel) %>%
      summarise(AvgPowerW = mean(EnergyPerOp / TimeUs), .groups = "drop") %>%
      group_by(DimLabel) %>%
      mutate(PowerRatio = AvgPowerW / min(AvgPowerW)) %>%
      ungroup()

    p <- ggplot(pd, aes(x = DimLabel, y = Library, fill = AvgPowerW)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = sprintf("%.1fW (%.2fx)", AvgPowerW, PowerRatio)),
                size = 3.2, fontface = "bold") +
      scale_fill_gradient(low = "#FFF9C4", high = "#D32F2F", name = "Power (W)") +
      labs(title = sprintf("%s – Average Power Draw per %s", op, bench$dim_label),
           subtitle = "Power = Energy / Time (W). Ratio relative to most efficient per level",
           x = bench$dim_label, y = NULL) +
      theme_minimal(base_size = 11) +
      theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
            plot.title = element_text(size = 14, face = "bold"),
            plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid = element_blank())
    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(plot_dir, sprintf("power_heatmap_%s.png", op_short)),
           p, width = 12, height = 4, dpi = 150)
  }
  cat("  Saved: power_heatmap\n")

  # ================================================================
  # 5. ALLOCATION BAR CHART (from report CSV)
  # ================================================================
  if (file.exists(report_file)) {
    report <- read_csv(report_file, show_col_types = FALSE) %>%
      mutate(
        Library    = str_extract(Method, "^[^_]+"),
        Operation  = str_extract(Method, "(?<=_)(Deser|Ser)"),
        DimRaw     = str_extract(Method, bench$dim_regex),
        DimValue   = bench$parse_dim(DimRaw),
        AllocBytes = parse_number(Allocated)
      ) %>%
      mutate(
        Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "STJRefGen", "STJSrcGen", "Newtonsoft")),
        Operation = factor(Operation, levels = c("Deser", "Ser"),
                           labels = c("Deserialize", "Serialize"))
      )

    if (is.numeric(report$DimValue)) {
      raw_sorted_r <- report %>% distinct(DimRaw, DimValue) %>% arrange(DimValue) %>% pull(DimRaw)
      report <- report %>% mutate(
        DimLabel = factor(paste0(bench$dim_prefix, DimRaw),
                          levels = paste0(bench$dim_prefix, raw_sorted_r))
      )
    } else {
      report <- report %>% mutate(DimLabel = factor(DimRaw, levels = sort(unique(DimRaw))))
    }

    for (op in levels(df$Operation)) {
      pd <- report %>% filter(Operation == op) %>%
        mutate(AllocKB = AllocBytes / 1024)

      p <- ggplot(pd, aes(x = DimLabel, y = AllocKB, fill = Library)) +
        geom_col(position = position_dodge(width = 0.8), width = 0.7) +
        scale_fill_manual(values = lib_colors) +
        labs(title = sprintf("%s – Allocated Bytes per Operation by %s", op, bench$dim_label),
             subtitle = "Lower = fewer allocations per operation",
             x = bench$dim_label, y = "Allocated (KB/op)", fill = "Library") +
        theme_minimal(base_size = 11) +
        theme(legend.position = "bottom",
              plot.title = element_text(size = 14, face = "bold"),
              plot.subtitle = element_text(size = 10, color = "grey40"),
              axis.text.x = element_text(angle = 45, hjust = 1))
      op_short <- ifelse(op == "Deserialize", "deser", "ser")
      ggsave(file.path(plot_dir, sprintf("alloc_%s.png", op_short)),
             p, width = 12, height = 6, dpi = 150)
    }
    cat("  Saved: alloc\n")

    # ================================================================
    # 6. GC COLLECTIONS BAR CHARTS
    # ================================================================
    for (op in levels(df$Operation)) {
      pd <- report %>% filter(Operation == op) %>%
        select(Library, DimLabel, Gen0, Gen1)

      has_gen1 <- max(pd$Gen1, na.rm = TRUE) > 0
      gen_cols <- "Gen0"
      if (has_gen1) gen_cols <- c(gen_cols, "Gen1")

      pd_long <- pd %>%
        pivot_longer(cols = all_of(gen_cols), names_to = "Generation", values_to = "Collections") %>%
        mutate(Generation = factor(Generation, levels = c("Gen1", "Gen0"))) %>%
        filter(!is.na(Collections))

      gen_colors <- c("Gen0" = "#4CAF50", "Gen1" = "#FF9800")

      p <- ggplot(pd_long, aes(x = DimLabel, y = Collections, fill = Generation)) +
        geom_col(position = "stack", width = 0.7) +
        facet_wrap(~ Library, nrow = 1) +
        scale_fill_manual(values = gen_colors) +
        labs(title = sprintf("%s – GC Collections per Operation by %s", op, bench$dim_label),
             subtitle = "Stacked: Gen0 (green) + Gen1 (orange). Higher = more GC pressure per op",
             x = bench$dim_label, y = "GC Collections / op", fill = "Generation") +
        theme_minimal(base_size = 11) +
        theme(strip.text = element_text(face = "bold", size = 10),
              legend.position = "bottom",
              plot.title = element_text(size = 14, face = "bold"),
              plot.subtitle = element_text(size = 10, color = "grey40"),
              axis.text.x = element_text(angle = 45, hjust = 1, size = 8))
      op_short <- ifelse(op == "Deserialize", "deser", "ser")
      ggsave(file.path(plot_dir, sprintf("gc_%s.png", op_short)),
             p, width = 14, height = 5, dpi = 150)
    }
    cat("  Saved: gc\n")
  }

  # ================================================================
  # 7. DRAM BREAKDOWN (Size isolation only — large files make DRAM relevant)
  # ================================================================
  if (bench$name == "size") {
    for (op in levels(df$Operation)) {
      # DRAM fraction heatmap
      fd <- means %>% filter(Operation == op) %>%
        mutate(DramFrac = MeanDram / (MeanPkg + MeanDram) * 100)
      p <- ggplot(fd, aes(x = DimLabel, y = Library, fill = DramFrac)) +
        geom_tile(color = "white", linewidth = 0.5) +
        geom_text(aes(label = sprintf("%.1f%%", DramFrac)), size = 3.5) +
        scale_fill_gradient2(low = "#1976D2", mid = "#FFC107", high = "#FF8F00",
                             midpoint = 5, name = "DRAM %") +
        labs(title = sprintf("%s – DRAM Energy Fraction by %s", op, bench$dim_label),
             subtitle = "Higher % = more memory-bound workload",
             x = bench$dim_label, y = NULL) +
        theme_minimal(base_size = 11) +
        theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
              plot.title = element_text(size = 14, face = "bold"),
              plot.subtitle = element_text(size = 10, color = "grey40"), panel.grid = element_blank())
      op_short <- ifelse(op == "Deserialize", "deser", "ser")
      ggsave(file.path(plot_dir, sprintf("heatmap_dram_frac_%s.png", op_short)),
             p, width = 10, height = 4, dpi = 150)

      # Energy breakdown stacked bars
      bd <- means %>% filter(Operation == op) %>%
        select(Library, DimLabel, MeanPkg, MeanDram) %>%
        pivot_longer(cols = c(MeanPkg, MeanDram), names_to = "Component", values_to = "Energy") %>%
        mutate(Component = factor(Component, levels = c("MeanDram", "MeanPkg"),
                                  labels = c("DRAM", "Package")))
      p <- ggplot(bd, aes(x = DimLabel, y = Energy, fill = Component)) +
        geom_col(position = "stack", width = 0.7) + facet_wrap(~ Library, nrow = 1) +
        scale_fill_manual(values = c("Package" = "#1976D2", "DRAM" = "#FF8F00")) +
        labs(title = sprintf("%s – Energy Breakdown: Package vs DRAM per %s", op, bench$dim_label),
             subtitle = "Stacked bars show both RAPL domains",
             x = bench$dim_label, y = "Energy (μJ/op)", fill = "Component") +
        theme_minimal(base_size = 11) +
        theme(strip.text = element_text(face = "bold", size = 10),
              axis.text.x = element_text(angle = 45, hjust = 1, size = 8),
              legend.position = "bottom", plot.title = element_text(size = 14, face = "bold"),
              plot.subtitle = element_text(size = 10, color = "grey40"))
      op_short <- ifelse(op == "Deserialize", "deser", "ser")
      ggsave(file.path(plot_dir, sprintf("breakdown_%s.png", op_short)),
             p, width = 14, height = 5, dpi = 150)
    }
    cat("  Saved: dram_fraction + breakdown (size isolation)\n")
  }

  # ================================================================
  # 8. SCALING TABLE
  # ================================================================
  if (is.numeric(df$DimValue)) {
    d_min <- min(dim_order); d_max <- max(dim_order)
    td <- means %>%
      select(Library, Operation, DimValue, MeanEnergy) %>%
      pivot_wider(names_from = DimValue, values_from = MeanEnergy,
                  names_prefix = paste0(bench$dim_prefix, "")) %>%
      arrange(Operation, Library)
    write_csv(td, file.path(plot_dir, "scaling_table.csv"))
    cat("  Saved: scaling_table.csv\n")
  }
}

cat("\nAll done!\n")
