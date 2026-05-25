library(tidyverse)

# --- Paths ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
variant <- Sys.getenv("BENCH_VARIANT", "Byte")
stopifnot(variant %in% c("Byte", "String"))
results_dir <- file.path(script_dir, "..", "BenchmarkArtifacts", "results")
base_plot_dir <- file.path(script_dir, "plots", tolower(variant))

# --- Colors ---
lib_colors <- c(
  "SpanJson" = "#0072B2", "Utf8Json" = "#009E73",
  "STJRefGen" = "#F0E442", "STJSrcGen" = "#CC79A7", "Newtonsoft" = "#D55E00"
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
    parse_dim = function(x) as.integer(x),
    per_element_unit = "field"
  ),
  list(
    name = "depth",
    file_stem = "DepthIsolation",
    dim_regex = "(?<=_D)\\d+",
    dim_prefix = "D",
    dim_label = "Depth",
    x_label = "Nesting Depth",
    parse_dim = function(x) as.integer(x),
    per_element_unit = "level"
  ),
  list(
    name = "size",
    file_stem = "SizeIsolation",
    dim_regex = "(?<=_C)[0-9]+K?",
    dim_prefix = "C",
    dim_label = "Size",
    x_label = "Object Count",
    parse_dim = function(x) {
      # Strip the optional K suffix unconditionally before as.integer, so the
      # coercion never sees "1K"/"10K"/"100K" (which would emit the NA-by-
      # coercion warning even though ifelse would discard them).
      num <- as.integer(sub("K$", "", x))
      ifelse(grepl("K$", x), num * 1000L, num)
    },
    per_element_unit = "object"
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
    parse_dim = function(x) x,  # Keep as string (F100, I30, I50, etc.)
    dim_levels = c("F100", "I30", "I50", "I70", "I100")  # 0% -> 100% integer density
  ),
  list(
    name = "redundancy",
    file_stem = "RedundancyIsolation",
    dim_regex = "(?<=_R)\\d+",
    dim_prefix = "R",
    dim_label = "Redundancy %",
    x_label = "Key Redundancy Percentage",
    parse_dim = function(x) as.integer(x)
  ),
  list(
    name = "value_length",
    file_stem = "ValueLengthIsolation",
    dim_regex = "(?<=_L)\\d+",
    dim_prefix = "L",
    dim_label = "Value Length",
    x_label = "Per-value string length (characters)",
    parse_dim = function(x) as.integer(x),
    per_element_unit = "char"
  )
)

# ===========================================================================
# MAIN PROCESSING LOOP
# ===========================================================================

for (bench in benchmarks) {
  measurements_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%s%sBench-measurements.csv", bench$file_stem, variant))
  report_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%s%sBench-report.csv", bench$file_stem, variant))

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
    dim_order <- if (!is.null(bench$dim_levels)) bench$dim_levels else sort(unique(df$DimRaw))
    df <- df %>% mutate(DimLabel = factor(DimRaw, levels = dim_order))
    dim_labels <- dim_order
  }

  # --- Means + SD ---
  means <- df %>%
    group_by(Library, Operation, DimValue, DimLabel) %>%
    summarise(
      MeanEnergy = mean(EnergyPerOp),
      SDEnergy   = sd(EnergyPerOp),
      MeanPkg    = mean(PkgPerOp),
      MeanDram   = mean(DramPerOp),
      .groups = "drop"
    )

  # ================================================================
  # 0. SHAPIRO-WILK NORMALITY TEST (per Library × Operation × DimLabel)
  # ================================================================
  # Short-circuit `if` is used (not ifelse) so shapiro.test() is not evaluated
  # when n() < 3, which would error out ("sample size must be between 3 and 5000").
  shapiro_results <- df %>%
    group_by(Library, Operation, DimLabel) %>%
    summarise(
      n          = n(),
      sw_stat    = if (n() >= 3) shapiro.test(EnergyPerOp)$statistic else NA_real_,
      sw_p_value = if (n() >= 3) shapiro.test(EnergyPerOp)$p.value   else NA_real_,
      .groups = "drop"
    ) %>%
    mutate(
      normal    = sw_p_value >= 0.05,
      Dimension = bench$name
    )

  write_csv(shapiro_results, file.path(plot_dir, "shapiro_wilk_results.csv"))

  for (op in levels(df$Operation)) {
    sub <- shapiro_results %>% filter(Operation == op)
    p <- ggplot(sub, aes(x = DimLabel, y = Library, fill = sw_p_value)) +
      geom_tile(color = "white", linewidth = 0.3) +
      geom_text(aes(label = ifelse(normal, "", "*")),
                color = "#D55E00", size = 4, fontface = "bold") +
      scale_fill_gradient2(low = "#D55E00", mid = "#F5F5F5", high = "#56B4E9",
                           midpoint = 0.05, name = "p-value", limits = c(0, 1)) +
      labs(x = bench$dim_label, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
            panel.grid = element_blank())
    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(plot_dir, sprintf("normality_heatmap_%s.png", op_short)),
           p, width = 10, height = 4, dpi = 150)
  }
  cat("  Saved: shapiro_wilk\n")

  # Accumulate per-bench SW results for the aggregate summary at the end.
  if (!exists("all_shapiro")) all_shapiro <- list()
  all_shapiro[[bench$name]] <- shapiro_results

  # ================================================================
  # 0b. KW + CLIFF'S δ + LOG-LOG SLOPE (per Library × Operation)
  # ================================================================
  # KW asks whether at least one DimLabel group differs from the others.
  # Wrapped in a helper so we can pull three scalars from one kruskal.test call.
  run_kw <- function(d) {
    if (n_distinct(d$DimLabel) <= 1) {
      return(tibble(kw_chi = NA_real_, kw_p = NA_real_, kw_df = NA_real_))
    }
    k <- kruskal.test(d$EnergyPerOp ~ d$DimLabel)
    tibble(kw_chi = unname(k$statistic),
           kw_p   = k$p.value,
           kw_df  = unname(k$parameter))
  }
  kw_per_group <- df %>%
    group_by(Library, Operation) %>%
    group_modify(~ run_kw(.x)) %>%
    ungroup()

  # Cliff's δ between baseline (smallest level) and extreme (largest level).
  cliffs_delta <- function(x, y) {
    if (length(x) == 0 || length(y) == 0) return(NA_real_)
    diffs <- outer(y, x, FUN = "-")
    (sum(diffs > 0) - sum(diffs < 0)) / length(diffs)
  }
  # Romano thresholds: negligible / small / medium / large.
  magnitude_tier <- function(abs_d) {
    case_when(
      is.na(abs_d)   ~ NA_character_,
      abs_d < 0.147  ~ "N",
      abs_d < 0.33   ~ "S",
      abs_d < 0.474  ~ "M",
      TRUE           ~ "L"
    )
  }
  base_value    <- dim_order[1]
  extreme_value <- dim_order[length(dim_order)]
  base_label    <- as.character(levels(df$DimLabel)[1])
  extreme_label <- as.character(levels(df$DimLabel)[nlevels(df$DimLabel)])

  delta_per_group <- df %>%
    group_by(Library, Operation) %>%
    summarise(
      delta = cliffs_delta(
        EnergyPerOp[DimValue == base_value],
        EnergyPerOp[DimValue == extreme_value]
      ),
      .groups = "drop"
    ) %>%
    mutate(
      abs_delta = abs(delta),
      magnitude = magnitude_tier(abs_delta)
    )

  # Log-log slope: only for numeric, positive DimValue. Drops 0% levels because
  # log10(0) = -Inf would break the regression.
  if (is.numeric(df$DimValue)) {
    slope_per_group <- means %>%
      filter(DimValue > 0) %>%
      group_by(Library, Operation) %>%
      summarise(
        slope = if (n() >= 2) coef(lm(log10(MeanEnergy) ~ log10(DimValue)))[[2]] else NA_real_,
        r2    = if (n() >= 2) summary(lm(log10(MeanEnergy) ~ log10(DimValue)))$r.squared else NA_real_,
        .groups = "drop"
      ) %>%
      mutate(
        shape = case_when(
          is.na(slope)      ~ NA_character_,
          r2 < 0.85         ~ "non-monotonic/plateau",
          abs(slope) < 0.2  ~ "flat",
          slope < -0.2      ~ "decreasing",
          slope <  0.7      ~ "sub-linear",
          slope <= 1.3      ~ "linear",
          TRUE              ~ "super-linear"
        )
      )
  } else {
    # Categorical x: slope/shape not defined; only δ + KW apply.
    slope_per_group <- delta_per_group %>%
      select(Library, Operation) %>%
      mutate(slope = NA_real_, r2 = NA_real_, shape = NA_character_)
  }

  # Adjacent-pair (local) log-log slopes: n-1 values per (Library, Operation).
  # Cliff's δ saturates at ±1 between well-spaced levels on clean data, so a
  # local slope is the right tool for seeing how the per-step rate of change
  # varies along the sweep. Supplementary to the global slope; written out as
  # its own CSV.
  if (is.numeric(df$DimValue)) {
    local_slopes <- means %>%
      filter(DimValue > 0) %>%
      group_by(Library, Operation) %>%
      arrange(DimValue, .by_group = TRUE) %>%
      mutate(
        to_DimValue   = lead(DimValue),
        to_DimLabel   = lead(DimLabel),
        to_MeanEnergy = lead(MeanEnergy)
      ) %>%
      ungroup() %>%
      filter(!is.na(to_DimValue)) %>%
      mutate(
        W_ratio          = to_DimValue / DimValue,
        E_ratio          = to_MeanEnergy / MeanEnergy,
        local_slope      = log10(E_ratio) / log10(W_ratio),
        # Per-element cost at each end of the step: energy divided by the
        # number of units the dimension counts (fields for width, objects for
        # size, characters for value_length, etc.). Useful as a concrete-units
        # complement to the abstract scaling exponent.
        from_per_element = MeanEnergy   / DimValue,
        to_per_element   = to_MeanEnergy / to_DimValue
      ) %>%
      select(Library, Operation,
             from_level = DimValue, to_level   = to_DimValue,
             from_label = DimLabel, to_label   = to_DimLabel,
             from_E     = MeanEnergy, to_E     = to_MeanEnergy,
             from_per_element, to_per_element,
             W_ratio, E_ratio, local_slope)
    write_csv(local_slopes, file.path(plot_dir, "local_slopes.csv"))
    cat("  Saved: local_slopes\n")

    # Local-slope heatmap: rows = libraries, columns = adjacent transitions,
    # fill = local exponent, midpoint = 1.0 (linear scaling). Diverging palette
    # so deviations below linear (cool/blue) and above linear (warm/red) are
    # equally legible at a glance.
    ls_plot <- local_slopes %>%
      mutate(
        TransitionLabel = paste0(from_label, " → ", to_label),
        slope_label     = sprintf("%.2f", local_slope)
      ) %>%
      arrange(from_level, to_level) %>%
      mutate(TransitionLabel = factor(TransitionLabel,
                                      levels = unique(TransitionLabel)))

    # Centre the diverging scale on 1.0, extend symmetrically to cover the data.
    max_dev      <- max(abs(ls_plot$local_slope - 1.0), na.rm = TRUE)
    slope_limits <- c(1 - max_dev, 1 + max_dev)

    p <- ggplot(ls_plot, aes(x = TransitionLabel, y = Library, fill = local_slope)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = slope_label), size = 3.5) +
      scale_fill_gradient2(
        low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
        midpoint = 1.0, limits = slope_limits, name = "Local slope"
      ) +
      facet_wrap(~ Operation, ncol = 1) +
      labs(x = NULL, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y    = element_text(face = "bold"),
            axis.text.x    = element_text(angle = 30, hjust = 1),
            strip.text     = element_text(face = "bold"),
            legend.position = "right",
            panel.grid     = element_blank())

    ggsave(file.path(plot_dir, "local_slopes_heatmap.png"),
           p, width = 10, height = 5, dpi = 150)
    cat("  Saved: local_slopes_heatmap\n")

    # Per-element cost (μJ/field, μJ/object, μJ/char, etc.). Emitted only for
    # "count-of-things" dimensions that carry a per_element_unit; skipped for
    # percentage-of-substitution sweeps (Unicode, Escape, Numeric, Redundancy)
    # where dividing by a percentage yields no useful unit. Wide pivot: rows
    # are (Operation, Library); columns are sweep levels in ascending order.
    if (!is.null(bench$per_element_unit)) {
      per_element_wide <- means %>%
        filter(DimValue > 0) %>%
        mutate(per_element = MeanEnergy / DimValue) %>%
        select(Library, Operation, DimLabel, per_element) %>%
        pivot_wider(names_from = DimLabel, values_from = per_element) %>%
        arrange(Operation, Library)

      write_csv(per_element_wide, file.path(plot_dir, "per_element_cost.csv"))
      cat(sprintf("  Saved: per_element_cost (μJ/%s)\n", bench$per_element_unit))
    }
  }

  effect_summary <- kw_per_group %>%
    left_join(delta_per_group, by = c("Library", "Operation")) %>%
    left_join(slope_per_group, by = c("Library", "Operation")) %>%
    mutate(
      Dimension    = bench$name,
      BaseLevel    = base_label,
      ExtremeLevel = extreme_label
    )

  write_csv(effect_summary, file.path(plot_dir, "effect_summary.csv"))

  # Accumulate for the aggregate (Holm-Bonferroni + cross-dim heatmap + LaTeX tables).
  if (!exists("all_effects")) all_effects <- list()
  all_effects[[bench$name]] <- effect_summary

  cat("  Saved: effect_summary\n")

  # ================================================================
  # 1. ENERGY SCALING LINE PLOT (Deser + Ser stacked, SD variability)
  # ================================================================
  if (is.numeric(means$DimValue)) {
    p <- ggplot(means, aes(x = DimValue, y = MeanEnergy,
                           ymin = pmax(MeanEnergy - SDEnergy, 0),
                           ymax = MeanEnergy + SDEnergy,
                           color = Library, fill = Library, group = Library)) +
      geom_ribbon(alpha = 0.15, color = NA) +
      geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
      scale_color_manual(values = lib_colors) +
      scale_fill_manual(values = lib_colors)
    # Log-x only when the level range really spans >2 orders of magnitude AND
    # all values are positive (otherwise log10(0) drops the 0% level silently).
    if (min(dim_order) > 0 && max(dim_order) / min(dim_order) > 100) {
      p <- p + scale_x_log10(breaks = dim_order, labels = dim_labels)
    } else {
      p <- p + scale_x_continuous(breaks = dim_order)
    }
  } else {
    # Categorical x: error bars instead of ribbon (ribbon over a factor is meaningless).
    p <- ggplot(means, aes(x = DimLabel, y = MeanEnergy,
                           color = Library, group = Library)) +
      geom_errorbar(aes(ymin = pmax(MeanEnergy - SDEnergy, 0),
                        ymax = MeanEnergy + SDEnergy),
                    width = 0.15, linewidth = 0.5, alpha = 0.6) +
      geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
      scale_color_manual(values = lib_colors)
  }

  # Log-y is reserved for the Size sweep, where the energy range exceeds 10000x
  # and a linear y-axis is unreadable. All other dimensions use linear-y; this
  # exception is documented in the methodology chapter.
  if (bench$name == "size") {
    p <- p + scale_y_log10(labels = scales::label_number(big.mark = ","))
  }

  p <- p +
    facet_wrap(~ Operation, ncol = 1, scales = "free_y") +
    labs(x = bench$x_label, y = "Total Energy (μJ/op) [Package + DRAM]",
         color = "Library", fill = "Library") +
    theme_minimal(base_size = 14) +
    theme(legend.position = "bottom",
          panel.grid.minor = element_blank(),
          strip.text = element_text(face = "bold"))

  ggsave(file.path(plot_dir, "scaling.png"),
         p, width = 10, height = 9, dpi = 150)
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
      scale_fill_gradientn(
        colours = c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00"),
        limits = c(1, NA), name = "Ratio to best"
      ) +
      labs(x = bench$dim_label, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
            panel.grid = element_blank())
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
      if (min(dim_order) > 0 && max(dim_order) / min(dim_order) > 100) {
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
      labs(x = bench$x_label, y = "Rank", color = "Library") +
      theme_minimal(base_size = 14) +
      theme(legend.position = "bottom", panel.grid.minor = element_blank())
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
      scale_fill_gradientn(
        colours = c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00"),
        name = "Power (W)"
      ) +
      labs(x = bench$dim_label, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
            panel.grid = element_blank())
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
        # BDN formats Allocated with mixed units ("9016 B", "6.8 KB", etc.).
        # parse_number() drops the unit, so we have to multiply explicitly.
        AllocBytes = case_when(
          str_detect(Allocated, "GB") ~ parse_number(Allocated) * 1024^3,
          str_detect(Allocated, "MB") ~ parse_number(Allocated) * 1024^2,
          str_detect(Allocated, "KB") ~ parse_number(Allocated) * 1024,
          TRUE                         ~ parse_number(Allocated)
        )
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
        labs(x = bench$dim_label, y = "Allocated (KB/op)", fill = "Library") +
        theme_minimal(base_size = 14) +
        theme(legend.position = "bottom",
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

      gen_colors <- c("Gen0" = "#56B4E9", "Gen1" = "#E69F00")

      p <- ggplot(pd_long, aes(x = DimLabel, y = Collections, fill = Generation)) +
        geom_col(position = "stack", width = 0.7) +
        facet_wrap(~ Library, nrow = 1) +
        scale_fill_manual(values = gen_colors) +
        labs(x = bench$dim_label, y = "GC Collections / 1000 ops", fill = "Generation") +
        theme_minimal(base_size = 14) +
        theme(strip.text = element_text(face = "bold"),
              legend.position = "bottom",
              axis.text.x = element_text(angle = 45, hjust = 1, size = 9))
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
        scale_fill_gradient2(low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
                             midpoint = 5, name = "DRAM %") +
        labs(x = bench$dim_label, y = NULL) +
        theme_minimal(base_size = 14) +
        theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
              panel.grid = element_blank())
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
        scale_fill_manual(values = c("Package" = "#56B4E9", "DRAM" = "#E69F00")) +
        labs(x = bench$dim_label, y = "Energy (μJ/op)", fill = "Component") +
        theme_minimal(base_size = 14) +
        theme(strip.text = element_text(face = "bold"),
              axis.text.x = element_text(angle = 45, hjust = 1, size = 9),
              legend.position = "bottom")
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

# ===========================================================================
# AGGREGATE NORMALITY SUMMARY (across all isolation dimensions)
# ===========================================================================
if (exists("all_shapiro") && length(all_shapiro) > 0) {
  agg_dir <- file.path(base_plot_dir, "01_isolation_normality")
  dir.create(agg_dir, showWarnings = FALSE, recursive = TRUE)

  all_sw <- bind_rows(all_shapiro)

  cat("\n=== Isolation Shapiro-Wilk Aggregate ===\n")
  cat(sprintf("Total groups tested: %d\n", nrow(all_sw)))
  cat(sprintf("Normal     (p >= 0.05): %d (%.1f%%)\n",
              sum(all_sw$normal, na.rm = TRUE),
              100 * mean(all_sw$normal, na.rm = TRUE)))
  cat(sprintf("Non-normal (p <  0.05): %d (%.1f%%)\n",
              sum(!all_sw$normal, na.rm = TRUE),
              100 * mean(!all_sw$normal, na.rm = TRUE)))

  # Per (Library, Operation) breakdown — used by the methodology chapter.
  by_lib_op <- all_sw %>%
    group_by(Library, Operation) %>%
    summarise(
      n_groups   = n(),
      pct_normal = 100 * mean(normal, na.rm = TRUE),
      .groups = "drop"
    )
  write_csv(by_lib_op, file.path(agg_dir, "isolation_shapiro_summary.csv"))
  write_csv(all_sw,    file.path(agg_dir, "isolation_shapiro_all.csv"))

  # Histogram of SW p-values across all isolation groups.
  p_hist <- ggplot(all_sw, aes(x = sw_p_value)) +
    geom_histogram(bins = 30, fill = "#56B4E9", color = "white", alpha = 0.9) +
    geom_vline(xintercept = 0.05, linetype = "dashed", color = "#D55E00", linewidth = 0.6) +
    annotate("text", x = 0.07, y = Inf, label = "alpha = 0.05",
             vjust = 2, hjust = 0, color = "#D55E00", size = 4) +
    labs(x = "Shapiro-Wilk p-value", y = "Group count") +
    theme_minimal(base_size = 14)
  ggsave(file.path(agg_dir, "shapiro_pvalue_distribution.png"),
         p_hist, width = 8, height = 5, dpi = 150)

  cat(sprintf("Saved aggregate to: %s\n", agg_dir))
}

# ===========================================================================
# AGGREGATE EFFECT SUMMARY + CROSS-DIM HEATMAP + LATEX TABLES (RQ1 §5.3.3)
# ===========================================================================
if (exists("all_effects") && length(all_effects) > 0) {
  stat_dir <- file.path(base_plot_dir, "01_isolation_stats")
  dir.create(stat_dir, showWarnings = FALSE, recursive = TRUE)

  all_eff <- bind_rows(all_effects)

  # Holm-Bonferroni across the entire RQ1 family
  # (n dims × 5 libraries × 2 operations).
  all_eff <- all_eff %>%
    mutate(kw_p_adj = p.adjust(kw_p, method = "holm"))

  write_csv(all_eff, file.path(stat_dir, "isolation_effect_summary.csv"))

  # --- Cross-dimension summary heatmap (rows = libs, cols = dims) ---
  # Dimension ordering matches the §5.3.2 subsections.
  dim_order_chap  <- c("size", "depth", "width", "value_length",
                       "numeric", "unicode", "escape", "unicode_escape", "redundancy")
  dim_labels_chap <- c("Size", "Depth", "Width", "ValueLength",
                       "Numeric", "Unicode", "Escape", "Unicode-Escape", "Redundancy")

  for (op in c("Deserialize", "Serialize")) {
    sub <- all_eff %>%
      filter(Operation == op, Dimension %in% dim_order_chap) %>%
      mutate(
        Dimension = factor(Dimension, levels = dim_order_chap,
                           labels = dim_labels_chap),
        sig_mark  = ifelse(!is.na(kw_p_adj) & kw_p_adj < 0.05, "*", ""),
        cell_txt  = ifelse(
          is.na(delta), "—",
          sprintf("%+.2f %s%s", delta, magnitude, sig_mark)
        )
      )

    p <- ggplot(sub, aes(x = Dimension, y = Library, fill = abs_delta)) +
      geom_tile(color = "white", linewidth = 0.5) +
      geom_text(aes(label = cell_txt), size = 3.2) +
      scale_fill_gradientn(
        colours = c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00"),
        limits = c(0, 1), name = "|δ|"
      ) +
      labs(x = NULL, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y = element_text(face = "bold"),
            axis.text.x = element_text(angle = 30, hjust = 1),
            legend.position = "right",
            panel.grid = element_blank())

    op_short <- ifelse(op == "Deserialize", "deser", "ser")
    ggsave(file.path(stat_dir, sprintf("heatmap_dim_summary_%s.png", op_short)),
           p, width = 11, height = 5, dpi = 150)
  }
  cat(sprintf("Saved cross-dim heatmaps to: %s\n", stat_dir))

  # --- Per-(dimension, operation) CSV tables for §5.3.2 ---
  for (dim_name in unique(all_eff$Dimension)) {
    for (op in c("Deserialize", "Serialize")) {
      rows <- all_eff %>%
        filter(Dimension == dim_name, Operation == op) %>%
        arrange(Library) %>%
        select(Library, BaseLevel, ExtremeLevel,
               kw_p, kw_p_adj, delta, magnitude, slope, r2, shape)
      if (nrow(rows) == 0) next

      op_short       <- ifelse(op == "Deserialize", "deser", "ser")
      bench_plot_dir <- file.path(base_plot_dir, dim_name)
      dir.create(bench_plot_dir, showWarnings = FALSE, recursive = TRUE)
      write_csv(rows,
                file.path(bench_plot_dir, sprintf("table_%s.csv", op_short)))
    }
  }
  cat("Saved per-dim CSV tables\n")
}

cat("\nAll done!\n")
