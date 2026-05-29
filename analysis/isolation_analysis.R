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

# ---------------------------------------------------------------------------
# Endpoint energy/time table generator
# ---------------------------------------------------------------------------
# Writes a ready-to-\input LaTeX table per isolation dimension: mean execution
# time (us/op) and energy (uJ/op, Package+DRAM) at the two ends of the sweep,
# for both operations. Energy cells are Wistia-shaded within each (operation,
# level) column; time is plain. Output: <plot_dir>/tables/endpoint_energy_time.tex
write_endpoint_table <- function(means, bench, plot_dir) {
  wistia <- c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00")
  ramp   <- scales::colour_ramp(wistia)

  lv <- levels(means$DimLabel)
  if (length(lv) < 2) return(invisible(NULL))
  lo <- lv[1]; hi <- lv[length(lv)]

  libs     <- levels(means$Library)
  op_short <- c(Deserialize = "Deser", Serialize = "Ser")
  blocks   <- data.frame(
    Operation = c("Deserialize", "Deserialize", "Serialize", "Serialize"),
    Level     = c(lo, hi, lo, hi),
    stringsAsFactors = FALSE
  )

  fmt_E <- function(x) ifelse(x < 100,
    formatC(x, format = "f", digits = 1, big.mark = ","),
    formatC(round(x), format = "d", big.mark = ","))
  fmt_t <- function(x) formatC(x, format = "f", digits = 2, big.mark = ",")
  cell_hex <- function(e) {
    rng <- range(e)
    n   <- if (diff(rng) == 0) rep(0.5, length(e)) else (e - rng[1]) / diff(rng)
    toupper(sub("#", "", ramp(n)))
  }

  get_block <- function(opn, lvl) {
    d <- means %>% filter(Operation == opn, DimLabel == lvl)
    d <- d[match(libs, as.character(d$Library)), ]
    list(t = fmt_t(d$MeanTime), E = fmt_E(d$MeanEnergy), hex = cell_hex(d$MeanEnergy))
  }
  B <- Map(get_block, blocks$Operation, blocks$Level)

  nb      <- nrow(blocks)
  colspec <- paste0("l", strrep("r", nb * 2))
  hdr1 <- paste0(" & ", paste(sprintf("\\multicolumn{2}{c}{%s %s}",
                  op_short[blocks$Operation], blocks$Level), collapse = " & "), " \\\\")
  cmids <- paste(sprintf("\\cmidrule(lr){%d-%d}",
                  seq(2, by = 2, length.out = nb), seq(3, by = 2, length.out = nb)),
                  collapse = "")
  hdr2 <- paste0("Library & ", paste(rep("$t$ & $E$", nb), collapse = " & "), " \\\\")
  body <- vapply(seq_along(libs), function(i) {
    cells <- unlist(lapply(B, function(b)
      c(b$t[i], sprintf("\\cellcolor[HTML]{%s}%s", b$hex[i], b$E[i]))))
    paste0(libs[i], " & ", paste(cells, collapse = " & "), " \\\\")
  }, character(1))

  dl <- gsub("%", "\\\\%", bench$dim_label)  # escape % for LaTeX
  caption <- sprintf(paste0("Execution time ($t$, $\\mu$s/op) and energy ",
    "($E$, $\\mu$J/op, Package${+}$DRAM) across %s sweep (%s, %s). Energy cell ",
    "colours are normalised within each column, lower (cool) to higher (hot)."),
    dl, lo, hi)

  lines <- c(
    "% Auto-generated by isolation_analysis.R -- do not edit by hand.",
    "% Energy = Package + DRAM (uJ/op); time = us/op. Wistia shading normalised within each (operation, level) energy column.",
    "\\begin{table}[H]",
    "  \\centering",
    "  \\small",
    sprintf("  \\caption{%s}", caption),
    sprintf("  \\label{tab:iso-%s-energy-time}", bench$name),
    sprintf("  \\begin{tabular}{%s}", colspec),
    "    \\toprule",
    paste0("    ", hdr1),
    paste0("    ", cmids),
    paste0("    ", hdr2),
    "    \\midrule",
    paste0("    ", body),
    "    \\bottomrule",
    "  \\end{tabular}",
    "\\end{table}"
  )

  tdir <- file.path(plot_dir, "tables")
  dir.create(tdir, showWarnings = FALSE, recursive = TRUE)
  fname <- sprintf("%s_endpoint_energy_time.tex", bench$name)
  writeLines(lines, file.path(tdir, fname))
  cat(sprintf("  Saved: tables/%s\n", fname))
}

# ---------------------------------------------------------------------------
# Faithful report_friendly longtable generator (appendix)
# ---------------------------------------------------------------------------
# Dumps the per-dimension report_friendly table (all columns) as a landscape
# longtable for the appendix. Output: <plot_dir>/tables/<name>_report_friendly.tex
# Requires LaTeX packages: longtable, booktabs, pdflscape.
write_report_longtable <- function(rf, bench, plot_dir) {
  esc <- function(x) { x[is.na(x)] <- ""; gsub("([&%#_])", "\\\\\\1", x) }
  esc_hdr <- function(h) {
    h <- gsub("[µμ]", "$\\\\mu$", h)   # micro / Greek mu
    h <- gsub("°", "$^\\\\circ$", h)          # degree sign
    gsub("([&%#_])", "\\\\\\1", h)
  }

  cols    <- colnames(rf)
  ncol    <- length(cols)
  colspec <- paste0("l", strrep("r", ncol - 1))
  header  <- paste0(paste(esc_hdr(cols), collapse = " & "), " \\\\")

  cell_df <- as.data.frame(lapply(rf, function(col) esc(as.character(col))),
                           stringsAsFactors = FALSE)
  rows    <- do.call(paste, c(cell_df, sep = " & "))
  body    <- paste0("  ", rows, " \\\\")

  dl  <- gsub("%", "\\\\%", bench$dim_label)
  cap <- sprintf(paste0("Full per-configuration BenchmarkDotNet report for the %s ",
    "isolation sweep: timing, Package and DRAM energy per operation, GC counts, and ",
    "allocations, one row per library $\\times$ operation $\\times$ level."), dl)

  lines <- c(
    "% Auto-generated by isolation_analysis.R -- faithful dump of report_friendly.csv.",
    "% Requires LaTeX packages: longtable, booktabs, pdflscape.",
    "\\begin{landscape}",
    "\\footnotesize",
    sprintf("\\begin{longtable}{%s}", colspec),
    sprintf("\\caption{%s}\\label{tab:report-%s}\\\\", cap, bench$name),
    "\\toprule",
    header,
    "\\midrule \\endfirsthead",
    "\\toprule",
    header,
    "\\midrule \\endhead",
    sprintf("\\midrule \\multicolumn{%d}{r}{\\textit{continued on next page}}\\\\ \\endfoot", ncol),
    "\\bottomrule \\endlastfoot",
    body,
    "\\end{longtable}",
    "\\end{landscape}"
  )

  tdir  <- file.path(plot_dir, "tables")
  dir.create(tdir, showWarnings = FALSE, recursive = TRUE)
  fname <- sprintf("%s_report_friendly.tex", bench$name)
  writeLines(lines, file.path(tdir, fname))
  cat(sprintf("  Saved: tables/%s\n", fname))
}

# ---------------------------------------------------------------------------
# Scaling statistics table generator (RQ2)
# ---------------------------------------------------------------------------
# Writes a ready-to-\input LaTeX table per ordinal isolation dimension: the
# log-log slope, its fit R^2, and the endpoint energy ratio (extreme/baseline)
# per library, with Deserialise/Serialise as column groups. Slope cells are
# diverging-shaded, centred at 1.0 (linear); cool below, warm above. Skipped
# for categorical/percentage sweeps where the slope is undefined.
# Output: <plot_dir>/tables/<name>_scaling_stats.tex
write_scaling_table <- function(effect_summary, bench, plot_dir) {
  es <- effect_summary %>% filter(!is.na(slope))
  if (nrow(es) == 0) return(invisible(NULL))

  libs <- levels(effect_summary$Library)
  if (is.null(libs)) libs <- unique(as.character(effect_summary$Library))

  # Diverging fill centred at 1.0, symmetric extent across all slopes in the table.
  diverging <- scales::colour_ramp(c("#56B4E9", "#F5F5F5", "#D55E00"))
  max_dev   <- max(abs(es$slope - 1.0), na.rm = TRUE)
  slope_hex <- function(s) {
    n <- pmin(pmax(0.5 + (s - 1.0) / (2 * max_dev), 0), 1)
    toupper(sub("#", "", diverging(n)))
  }

  fmt_slope <- function(x) formatC(x, format = "f", digits = 2)
  fmt_r2    <- function(x) formatC(x, format = "f", digits = 3)
  fmt_ratio <- function(x) ifelse(x < 10,
    formatC(x, format = "f", digits = 1),
    formatC(round(x), format = "d", big.mark = ","))

  get_cells <- function(opn, i) {
    r <- es %>% filter(Operation == opn, Library == libs[i])
    if (nrow(r) == 0) return(c("--", "--", "--"))
    c(sprintf("\\cellcolor[HTML]{%s}%s", slope_hex(r$slope), fmt_slope(r$slope)),
      fmt_r2(r$r2), fmt_ratio(r$ratio_extreme))
  }

  body <- vapply(seq_along(libs), function(i) {
    cells <- c(get_cells("Deserialize", i), get_cells("Serialize", i))
    paste0(libs[i], " & ", paste(cells, collapse = " & "), " \\\\")
  }, character(1))

  dl <- gsub("%", "\\\\%", bench$dim_label)
  caption <- sprintf(paste0("Per-library %s scaling: log-log slope (fit $R^2$) ",
    "and the endpoint energy ratio $E_{\\mathrm{extreme}}/E_{\\mathrm{base}}$, per ",
    "operation. Slope cells are shaded on a diverging scale centred at $1.0$ ",
    "(linear scaling), cool below and warm above."), dl)

  lines <- c(
    "% Auto-generated by isolation_analysis.R -- do not edit by hand.",
    "% slope = log-log exponent; R2 = fit; ratio = E(extreme)/E(base). Slope cells diverging-shaded, centred at 1.0.",
    "\\begin{table}[H]",
    "  \\centering",
    "  \\small",
    sprintf("  \\caption{%s}", caption),
    sprintf("  \\label{tab:rq2-%s-stats}", bench$name),
    "  \\begin{tabular}{lrrrrrr}",
    "    \\toprule",
    "     & \\multicolumn{3}{c}{Deserialise} & \\multicolumn{3}{c}{Serialise} \\\\",
    "    \\cmidrule(lr){2-4}\\cmidrule(lr){5-7}",
    "    Library & slope & $R^2$ & ratio & slope & $R^2$ & ratio \\\\",
    "    \\midrule",
    paste0("    ", body),
    "    \\bottomrule",
    "  \\end{tabular}",
    "\\end{table}"
  )

  tdir  <- file.path(plot_dir, "tables")
  dir.create(tdir, showWarnings = FALSE, recursive = TRUE)
  fname <- sprintf("%s_scaling_stats.tex", bench$name)
  writeLines(lines, file.path(tdir, fname))
  cat(sprintf("  Saved: tables/%s\n", fname))
}

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
      MeanTime   = mean(TimeUs),
      SDEnergy   = sd(EnergyPerOp),
      MeanPkg    = mean(PkgPerOp),
      MeanDram   = mean(DramPerOp),
      .groups = "drop"
    )

  # --- Endpoint energy/time LaTeX table (sweep ends) for \input in the report ---
  write_endpoint_table(means, bench, plot_dir)

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
      scale_y_discrete(limits = rev) +
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
           p, width = 10, height = 4, dpi = 300)
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

  # Endpoint magnitude ratio: mean energy at the extreme sweep level divided
  # by mean energy at the baseline level. Complement to the log-log slope —
  # doesn't assume a power-law shape, is well-defined for categorical and
  # percentage sweeps, and reads as "going from baseline to extreme makes
  # this library Xx more (or less) expensive". Feeds the cross-dim ratio
  # heatmap in 01_isolation/.
  endpoint_ratio_per_group <- means %>%
    group_by(Library, Operation) %>%
    summarise(
      e_baseline    = first(MeanEnergy[DimValue == base_value]),
      e_extreme     = first(MeanEnergy[DimValue == extreme_value]),
      ratio_extreme = e_extreme / e_baseline,
      .groups = "drop"
    ) %>%
    select(Library, Operation, ratio_extreme)

  # Per-element endpoint ratio: E_ratio divided by dim_ratio, i.e.\ the
  # growth factor of energy-per-element from baseline to extreme. Defined
  # only when the dimension counts elements (per_element_unit set) and the
  # baseline level is positive (otherwise dim_ratio is undefined). Equals 1
  # at perfectly linear scaling regardless of how wide the sweep range is;
  # >1 indicates super-linear, <1 sub-linear. NA-filled for percentage and
  # categorical sweeps where per-element has no clean semantic.
  if (!is.null(bench$per_element_unit) && is.numeric(df$DimValue) && base_value > 0) {
    dim_ratio <- extreme_value / base_value
    per_element_endpoint <- endpoint_ratio_per_group %>%
      mutate(per_element_ratio_extreme = ratio_extreme / dim_ratio) %>%
      select(Library, Operation, per_element_ratio_extreme)
  } else {
    per_element_endpoint <- endpoint_ratio_per_group %>%
      mutate(per_element_ratio_extreme = NA_real_) %>%
      select(Library, Operation, per_element_ratio_extreme)
  }

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
    # equally legible at a glance. Each cell stacks two numbers: local slope
    # (bold, top) and the per-step energy ratio (parenthesised, below). The
    # column header carries the corresponding W-ratio so the reader can recover
    # the relationship local_slope = log10(E_ratio) / log10(W_ratio).
    ls_plot <- local_slopes %>%
      mutate(
        TransitionLabel = sprintf("%s → %s\n(×%.3g)",
                                  from_label, to_label, W_ratio),
        slope_label     = sprintf("%.2f", local_slope),
        ratio_label     = sprintf("(×%.2f)", E_ratio)
      ) %>%
      arrange(from_level, to_level) %>%
      mutate(TransitionLabel = factor(TransitionLabel,
                                      levels = unique(TransitionLabel)))

    # Centre the diverging scale on 1.0, extend symmetrically to cover the data.
    max_dev      <- max(abs(ls_plot$local_slope - 1.0), na.rm = TRUE)
    slope_limits <- c(1 - max_dev, 1 + max_dev)

    p <- ggplot(ls_plot, aes(x = TransitionLabel, y = Library, fill = local_slope)) +
      geom_tile(color = "white", linewidth = 0.5) +
      scale_y_discrete(limits = rev) +
      geom_text(aes(label = slope_label), nudge_y = 0.18,
                size = 4.5, fontface = "bold") +
      geom_text(aes(label = ratio_label), nudge_y = -0.20,
                size = 3.0, fontface = "italic") +
      scale_fill_gradient2(
        low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
        midpoint = 1.0, limits = slope_limits, name = "Local slope"
      ) +
      guides(fill = guide_colourbar(title.position = "top", title.hjust = 0.5,
                                    barwidth = grid::unit(8, "cm"),
                                    barheight = grid::unit(0.5, "cm"))) +
      facet_wrap(~ Operation, ncol = 1) +
      labs(x = NULL, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y    = element_text(face = "bold"),
            axis.text.x    = element_text(angle = 0, hjust = 0.5,
                                          lineheight = 0.95),
            strip.text     = element_text(face = "bold"),
            legend.position = "bottom",
            panel.grid     = element_blank())

    ggsave(file.path(plot_dir, "local_slopes_heatmap.png"),
           p, width = 11, height = 9, dpi = 300)
    cat("  Saved: local_slopes_heatmap\n")

    # Full per-dim table: wide pivot with one row per (Library, Operation) and
    # one column per adjacent transition. Each cell carries "slope (×E_ratio)"
    # and the column header carries the W-ratio. Mirrors the annotated heatmap
    # numerically — useful as a reference table for the chapter appendix.
    full_table <- local_slopes %>%
      mutate(
        cell_str  = sprintf("%.2f (×%.2f)", local_slope, E_ratio),
        col_label = sprintf("%s→%s (W×%.3g)",
                            from_label, to_label, W_ratio)
      ) %>%
      arrange(from_level, to_level) %>%
      mutate(col_label = factor(col_label, levels = unique(col_label))) %>%
      select(Library, Operation, col_label, cell_str) %>%
      pivot_wider(names_from = col_label, values_from = cell_str) %>%
      arrange(Operation, Library)

    write_csv(full_table, file.path(plot_dir, "local_slopes_full_table.csv"))
    cat("  Saved: local_slopes_full_table.csv\n")

    # Local-ratio heatmap: per-step energy multiplication factor (E_ratio at
    # each adjacent transition). Complement to the local-slope view — the
    # slope normalises for the per-step dim-ratio in log space, so it reads
    # as "scaling regime"; the ratio is the plain per-step energy jump and
    # reads as "energy was Xx after this step". Caveat: adjacent W-ratios
    # are not constant across the sweep (e.g.\ W=2→5 is 2.5x while W=50→100
    # is 2x), so cells in different columns are not directly comparable as
    # "scaling strength" — for that comparison, use local_slopes_heatmap.
    lr_plot <- local_slopes %>%
      mutate(
        TransitionLabel = paste0(from_label, " → ", to_label),
        ratio_label     = ifelse(E_ratio >= 10,
                                 sprintf("%.1fx", E_ratio),
                                 sprintf("%.2fx", E_ratio))
      ) %>%
      arrange(from_level, to_level) %>%
      mutate(TransitionLabel = factor(TransitionLabel,
                                      levels = unique(TransitionLabel)))

    lr_log         <- log10(lr_plot$E_ratio)
    max_dev_r      <- max(abs(lr_log), na.rm = TRUE)
    ratio_limits_r <- c(-max_dev_r, max_dev_r)

    p <- ggplot(lr_plot, aes(x = TransitionLabel, y = Library,
                              fill = log10(E_ratio))) +
      geom_tile(color = "white", linewidth = 0.5) +
      scale_y_discrete(limits = rev) +
      geom_text(aes(label = ratio_label), size = 3.5) +
      scale_fill_gradient2(
        low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
        midpoint = 0, limits = ratio_limits_r,
        name   = "Step ratio (log)",
        labels = function(x) sprintf("%.1fx", 10^x)
      ) +
      facet_wrap(~ Operation, ncol = 1) +
      labs(x = NULL, y = NULL) +
      theme_minimal(base_size = 14) +
      theme(axis.text.y     = element_text(face = "bold"),
            axis.text.x     = element_text(angle = 30, hjust = 1),
            strip.text      = element_text(face = "bold"),
            legend.position = "right",
            panel.grid      = element_blank())

    ggsave(file.path(plot_dir, "local_ratios_heatmap.png"),
           p, width = 10, height = 5, dpi = 300)
    cat("  Saved: local_ratios_heatmap\n")

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

      # Local per-element ratio heatmap: each adjacent transition's per-element
      # energy growth factor (= to_per_element / from_per_element, equivalently
      # E_ratio / W_ratio). At linear scaling all cells read 1.0x regardless of
      # the underlying W-step, so unlike local_ratios this view is roughly
      # column-comparable for data sitting near slope = 1. Only emitted for
      # count-based dims (Size, Depth, Width, ValueLength) where the
      # per-element semantic is meaningful.
      pe_plot <- local_slopes %>%
        mutate(
          TransitionLabel   = paste0(from_label, " → ", to_label),
          per_element_ratio = to_per_element / from_per_element,
          pe_label = ifelse(per_element_ratio >= 10,
                            sprintf("%.1fx", per_element_ratio),
                            sprintf("%.2fx", per_element_ratio))
        ) %>%
        arrange(from_level, to_level) %>%
        mutate(TransitionLabel = factor(TransitionLabel,
                                        levels = unique(TransitionLabel)))

      pe_log         <- log10(pe_plot$per_element_ratio)
      max_dev_pe     <- max(abs(pe_log), na.rm = TRUE)
      pe_limits      <- c(-max_dev_pe, max_dev_pe)

      p <- ggplot(pe_plot, aes(x = TransitionLabel, y = Library,
                                fill = log10(per_element_ratio))) +
        geom_tile(color = "white", linewidth = 0.5) +
      scale_y_discrete(limits = rev) +
        geom_text(aes(label = pe_label), size = 3.5) +
        scale_fill_gradient2(
          low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
          midpoint = 0, limits = pe_limits,
          name   = "Per-element ratio (log)",
          labels = function(x) sprintf("%.2fx", 10^x)
        ) +
        facet_wrap(~ Operation, ncol = 1) +
        labs(x = NULL, y = NULL) +
        theme_minimal(base_size = 14) +
        theme(axis.text.y     = element_text(face = "bold"),
              axis.text.x     = element_text(angle = 30, hjust = 1),
              strip.text      = element_text(face = "bold"),
              legend.position = "right",
              panel.grid      = element_blank())

      ggsave(file.path(plot_dir, "local_per_element_ratio_heatmap.png"),
             p, width = 10, height = 5, dpi = 300)
      cat("  Saved: local_per_element_ratio_heatmap\n")
    }
  }

  effect_summary <- kw_per_group %>%
    left_join(delta_per_group, by = c("Library", "Operation")) %>%
    left_join(slope_per_group, by = c("Library", "Operation")) %>%
    left_join(endpoint_ratio_per_group, by = c("Library", "Operation")) %>%
    left_join(per_element_endpoint, by = c("Library", "Operation")) %>%
    mutate(
      Dimension    = bench$name,
      BaseLevel    = base_label,
      ExtremeLevel = extreme_label
    )

  write_csv(effect_summary, file.path(plot_dir, "effect_summary.csv"))
  write_scaling_table(effect_summary, bench, plot_dir)

  # Accumulate for the aggregate (Holm-Bonferroni + cross-dim heatmap + LaTeX tables).
  if (!exists("all_effects")) all_effects <- list()
  all_effects[[bench$name]] <- effect_summary

  cat("  Saved: effect_summary\n")

  # ================================================================
  # 1. ENERGY SCALING LINE PLOT (Deser + Ser stacked)
  # ================================================================
  if (is.numeric(means$DimValue)) {
    p <- ggplot(means, aes(x = DimValue, y = MeanEnergy,
                           color = Library, group = Library)) +
      geom_line(linewidth = 0.9) + geom_point(size = 2.5) +
      scale_color_manual(values = lib_colors)
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
         color = "Library") +
    theme_minimal(base_size = 14) +
    theme(legend.position = "bottom",
          panel.grid.minor = element_blank(),
          strip.text = element_text(face = "bold"))

  ggsave(file.path(plot_dir, "scaling.png"),
         p, width = 10, height = 9, dpi = 300)
  cat("  Saved: scaling\n")

  # ================================================================
  # 2. RANK-AND-RATIO HEATMAP (RQ2: rank + magnitude in one figure)
  # ================================================================
  # At each level, libraries are ranked by mean energy (1 = cheapest) and the
  # ratio to the cheapest is computed. Cell fill = rank (Wistia palette,
  # matches the factorial chapter's rank heatmap). Cell text stacks the two:
  # rank renders bold on the upper half, ratio renders plain italic on the
  # lower half. Both operations are facet-stacked in one PNG.
  wistia_palette <- c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00")

  rank_ratio_data <- means %>%
    group_by(Operation, DimLabel) %>%
    mutate(
      Rank       = rank(MeanEnergy, ties.method = "min"),
      NormRatio  = MeanEnergy / min(MeanEnergy),
      RatioLabel = sprintf(ifelse(NormRatio >= 10, "(%.1fx)", "(%.2fx)"),
                           NormRatio)
    ) %>%
    ungroup()

  # Accumulate per-dim rank data for the cross-dim summary at the bottom.
  # Only the columns the aggregate actually uses are kept; carrying DimValue
  # forward would break the bind_rows because Numeric isolation stores it
  # as character (F100, I30, …) while other dims store it as integer.
  if (!exists("all_rank_data")) all_rank_data <- list()
  all_rank_data[[bench$name]] <- rank_ratio_data %>%
    select(Library, Operation, Rank, NormRatio) %>%
    mutate(Dimension = bench$name)

  p <- ggplot(rank_ratio_data,
              aes(x = DimLabel, y = Library, fill = factor(Rank))) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = Rank), nudge_y = 0.18,
              size = 4.5, fontface = "bold") +
    geom_text(aes(label = RatioLabel), nudge_y = -0.20,
              size = 3.0, fontface = "italic") +
    scale_fill_manual(
      values = setNames(wistia_palette, as.character(1:5)),
      name   = "Rank", drop = FALSE
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = bench$dim_label, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y    = element_text(face = "bold"),
          strip.text     = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid     = element_blank())
  ggsave(file.path(plot_dir, "heatmap_rank_ratio.png"),
         p, width = 10, height = 9, dpi = 300)
  cat("  Saved: heatmap_rank_ratio\n")

  # ================================================================
  # 3. ALLOCATION BAR CHART (from report CSV)
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

    # ================================================================
    # REPORT-FRIENDLY CSV (appendix-ready summary table)
    # ================================================================
    # Trimmed view of the BDN report.csv keeping only the columns useful
    # for the thesis appendix. The original full report.csv stays in
    # BenchmarkArtifacts/results/ for full traceability. The socket-index
    # "0" suffix is dropped from energy/temperature columns since all
    # measurements come from a single socket. Rows are sorted
    # Library × Operation × DimLabel for grouped readability.
    report_friendly <- report %>%
      arrange(Library, Operation, DimLabel) %>%
      select(
        Method,
        Iters              = Iterations,
        Ops                = Operations,
        Mean,
        Error,
        StdDev,
        `Pkg (μJ/op)`    = `PkgE0 (uJ/op)`,
        `StdDev (μJ/op)` = `StdDev0 (uJ/op)`,
        `DRAM (μJ/op)`   = `DRAM E0 (uJ/op)`,
        `Temp (°C)`      = `Avg Temp0 (degC)`,
        Gen0,
        Gen1,
        Allocated
      )
    write_csv(report_friendly, file.path(plot_dir, "report_friendly.csv"))
    cat("  Saved: report_friendly\n")
    write_report_longtable(report_friendly, bench, plot_dir)

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
             p, width = 12, height = 6, dpi = 300)
    }
    cat("  Saved: alloc\n")

    # ================================================================
    # 4. GC COLLECTIONS BAR CHARTS
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
             p, width = 14, height = 5, dpi = 300)
    }
    cat("  Saved: gc\n")
  }

  # ================================================================
  # 5. DRAM BREAKDOWN (Size isolation only — large files make DRAM relevant)
  # ================================================================
  if (bench$name == "size") {
    for (op in levels(df$Operation)) {
      # DRAM fraction heatmap
      fd <- means %>% filter(Operation == op) %>%
        mutate(DramFrac = MeanDram / (MeanPkg + MeanDram) * 100)
      p <- ggplot(fd, aes(x = DimLabel, y = Library, fill = DramFrac)) +
        geom_tile(color = "white", linewidth = 0.5) +
      scale_y_discrete(limits = rev) +
        geom_text(aes(label = sprintf("%.1f%%", DramFrac)), size = 3.5) +
        scale_fill_gradient2(low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
                             midpoint = 5, name = "DRAM %") +
        labs(x = bench$dim_label, y = NULL) +
        theme_minimal(base_size = 14) +
        theme(axis.text.y = element_text(face = "bold"), legend.position = "right",
              panel.grid = element_blank())
      op_short <- ifelse(op == "Deserialize", "deser", "ser")
      ggsave(file.path(plot_dir, sprintf("heatmap_dram_frac_%s.png", op_short)),
             p, width = 10, height = 4, dpi = 300)

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
             p, width = 14, height = 5, dpi = 300)
    }
    cat("  Saved: dram_fraction + breakdown (size isolation)\n")
  }

  # ================================================================
  # 6. SCALING TABLE
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
  agg_dir <- file.path(base_plot_dir, "01_isolation")
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
  write_csv(by_lib_op, file.path(agg_dir, "normality_shapiro_summary.csv"))
  write_csv(all_sw,    file.path(agg_dir, "normality_shapiro_per_group.csv"))

  # Histogram of SW p-values across all isolation groups.
  p_hist <- ggplot(all_sw, aes(x = sw_p_value)) +
    geom_histogram(bins = 30, fill = "#56B4E9", color = "white", alpha = 0.9) +
    geom_vline(xintercept = 0.05, linetype = "dashed", color = "#D55E00", linewidth = 0.6) +
    annotate("text", x = 0.07, y = Inf, label = "alpha = 0.05",
             vjust = 2, hjust = 0, color = "#D55E00", size = 4) +
    labs(x = "Shapiro-Wilk p-value", y = "Group count") +
    theme_minimal(base_size = 14)
  ggsave(file.path(agg_dir, "normality_shapiro_pvalue_distribution.png"),
         p_hist, width = 8, height = 5, dpi = 300)

  cat(sprintf("Saved aggregate to: %s\n", agg_dir))
}

# ===========================================================================
# AGGREGATE EFFECT SUMMARY + CROSS-DIM HEATMAP + LATEX TABLES (RQ1 §5.3.3)
# ===========================================================================
if (exists("all_effects") && length(all_effects) > 0) {
  stat_dir <- file.path(base_plot_dir, "01_isolation")
  dir.create(stat_dir, showWarnings = FALSE, recursive = TRUE)

  all_eff <- bind_rows(all_effects)

  # Holm-Bonferroni across the entire RQ1 family
  # (n dims × 5 libraries × 2 operations).
  all_eff <- all_eff %>%
    mutate(kw_p_adj = p.adjust(kw_p, method = "holm"))

  write_csv(all_eff, file.path(stat_dir, "cross_dim_effect_summary.csv"))

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
      scale_y_discrete(limits = rev) +
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
    ggsave(file.path(stat_dir, sprintf("cross_dim_cliffs_delta_%s.png", op_short)),
           p, width = 11, height = 5, dpi = 300)
  }
  cat(sprintf("Saved cross-dim heatmaps to: %s\n", stat_dir))

  # --- Per-dimension CSV tables for §5.3.2 ---
  # One file per dimension, combining Deser + Ser side by side. Kruskal-Wallis
  # p-values and Cliff's δ saturate for Tier-1 ordinal sweeps (every library hits
  # p≈0, δ=1, magnitude=L) and so are dropped from the table — they live in the
  # cross-dim Cliff's δ heatmap instead. The columns that actually discriminate
  # libraries are slope (relative steepness), R² (fit quality), shape (verbal
  # shorthand), and ratio (absolute magnitude response across the sweep). Values
  # are rounded for thesis-table readability; full precision stays in
  # cross_dim_effect_summary.csv.
  format_ratio_x <- function(r) {
    case_when(
      is.na(r) ~ "—",
      r >= 100 ~ sprintf("%.0fx", r),
      r >= 10  ~ sprintf("%.1fx", r),
      TRUE     ~ sprintf("%.2fx", r)
    )
  }

  for (dim_name in unique(all_eff$Dimension)) {
    deser_rows <- all_eff %>%
      filter(Dimension == dim_name, Operation == "Deserialize") %>%
      arrange(Library) %>%
      transmute(Library,
                slope_deser = sprintf("%.2f", slope),
                r2_deser    = sprintf("%.3f", r2),
                shape_deser = shape,
                ratio_deser = format_ratio_x(ratio_extreme))

    ser_rows <- all_eff %>%
      filter(Dimension == dim_name, Operation == "Serialize") %>%
      arrange(Library) %>%
      transmute(Library,
                slope_ser = sprintf("%.2f", slope),
                r2_ser    = sprintf("%.3f", r2),
                shape_ser = shape,
                ratio_ser = format_ratio_x(ratio_extreme))

    combined <- full_join(deser_rows, ser_rows, by = "Library")
    if (nrow(combined) == 0) next

    bench_plot_dir <- file.path(base_plot_dir, dim_name)
    dir.create(bench_plot_dir, showWarnings = FALSE, recursive = TRUE)
    write_csv(combined, file.path(bench_plot_dir, "table.csv"))
  }
  cat("Saved per-dim CSV tables\n")
}

# ===========================================================================
# AGGREGATE CROSS-DIM SLOPE HEATMAP (RQ1 §5.3.5)
# ===========================================================================
# Distinct from the δ heatmap (cross_dim_cliffs_delta_*): that one answers
# "does the dimension matter at all" and saturates at ±1.0 for wide-range
# sweeps; this one answers "how steeply does each library scale on each
# dimension". Restricted to ordinal sweeps where a log-log slope is
# meaningful — the categorical Numeric sweep and the substitution-percentage
# sweeps (Unicode, Escape, UnicodeEscape) are dropped here and reported
# separately later.
if (exists("all_effects") && length(all_effects) > 0) {
  slope_dir <- file.path(base_plot_dir, "01_isolation")
  dir.create(slope_dir, showWarnings = FALSE, recursive = TRUE)

  slope_dim_order  <- c("size", "depth", "width", "value_length", "redundancy")
  slope_dim_labels <- c("Size", "Depth", "Width", "ValueLength", "Redundancy")

  slope_data <- bind_rows(all_effects) %>%
    filter(Dimension %in% slope_dim_order, !is.na(slope)) %>%
    mutate(
      Dimension   = factor(Dimension, levels = slope_dim_order,
                           labels = slope_dim_labels),
      slope_label = sprintf("%.2f", slope),
      r2_label    = sprintf("R²=%.2f", r2)
    )

  write_csv(slope_data, file.path(slope_dir, "cross_dim_slopes.csv"))

  # Diverging palette centred at 1.0 (linear scaling), extent set symmetrically
  # around 1.0 to keep the colour reading honest.
  max_dev      <- max(abs(slope_data$slope - 1.0), na.rm = TRUE)
  slope_limits <- c(1 - max_dev, 1 + max_dev)

  p <- ggplot(slope_data, aes(x = Dimension, y = Library, fill = slope)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = slope_label), nudge_y = 0.15,
              size = 4, fontface = "bold") +
    geom_text(aes(label = r2_label), nudge_y = -0.20,
              size = 3, fontface = "italic") +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 1.0, limits = slope_limits, name = "Log-log slope"
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          strip.text      = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid      = element_blank())

  ggsave(file.path(slope_dir, "cross_dim_slopes.png"),
         p, width = 10, height = 7, dpi = 300)
  cat(sprintf("Saved cross-dim slope heatmap to: %s\n", slope_dir))
}

# ===========================================================================
# AGGREGATE CROSS-DIM RATIO HEATMAP (RQ1 §5.3.5 — endpoint magnitude view)
# ===========================================================================
# Complement to the slope heatmap. The slope panel answers "how steeply does
# each library scale on this dimension"; this panel answers "going from the
# baseline to the extreme sweep level, how many times more (or less) energy
# does each library spend". Works for all nine isolation sweeps including the
# categorical Numeric sweep and the percentage-substitution sweeps that the
# slope panel excludes, because it does not assume the curve is a power law.
if (exists("all_effects") && length(all_effects) > 0) {
  ratio_dir <- file.path(base_plot_dir, "01_isolation")
  dir.create(ratio_dir, showWarnings = FALSE, recursive = TRUE)

  ratio_dim_order  <- c("size", "depth", "width", "value_length",
                        "numeric", "unicode", "escape", "unicode_escape",
                        "redundancy")
  ratio_dim_labels <- c("Size", "Depth", "Width", "ValueLength",
                        "Numeric", "Unicode", "Escape", "Unicode-Escape",
                        "Redundancy")

  # Cell-text formatter: drop unhelpful precision at large ratios and keep two
  # decimals when the ratio is below 1 (energy decreased) so the sign is
  # visible at a glance.
  format_ratio <- function(r) {
    case_when(
      is.na(r) ~ "—",
      r >= 100 ~ sprintf("%.0fx", r),
      r >= 10  ~ sprintf("%.1fx", r),
      r >= 1   ~ sprintf("%.2fx", r),
      TRUE     ~ sprintf("%.2fx", r)
    )
  }

  ratio_data <- bind_rows(all_effects) %>%
    filter(Dimension %in% ratio_dim_order, !is.na(ratio_extreme)) %>%
    mutate(
      Dimension   = factor(Dimension, levels = ratio_dim_order,
                           labels = ratio_dim_labels),
      log_ratio   = log10(ratio_extreme),
      ratio_label = format_ratio(ratio_extreme)
    )

  write_csv(ratio_data, file.path(ratio_dir, "cross_dim_ratios.csv"))

  # Diverging palette centred at log10(1) = 0 (no change). Limits set
  # symmetrically around 0 so equal-strength increases and decreases read
  # with equal colour saturation.
  max_dev      <- max(abs(ratio_data$log_ratio), na.rm = TRUE)
  ratio_limits <- c(-max_dev, max_dev)

  p <- ggplot(ratio_data, aes(x = Dimension, y = Library, fill = log_ratio)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = ratio_label), size = 3.5, fontface = "bold") +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 0, limits = ratio_limits,
      name   = "Ratio (log)",
      labels = function(x) sprintf("%.1fx", 10^x)
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          axis.text.x     = element_text(angle = 30, hjust = 1),
          strip.text      = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid      = element_blank())

  ggsave(file.path(ratio_dir, "cross_dim_ratios.png"),
         p, width = 12, height = 7, dpi = 300)
  cat(sprintf("Saved cross-dim ratio heatmap to: %s\n", ratio_dir))
}

# ===========================================================================
# AGGREGATE CROSS-DIM PER-ELEMENT RATIO HEATMAP (RQ1 §5.3.5 — normalised view)
# ===========================================================================
# E_ratio / dim_ratio per (library, operation, dimension). Removes the sweep
# range from the magnitude reading: at perfectly linear scaling all cells
# read 1.0x regardless of whether the sweep covers 100x (Width) or 10000x
# (Size). Residual W-ratio dependence remains for non-linear scaling
# (= W_ratio^(slope-1)) but is small for slopes near 1.0. Restricted to
# count-based dims (Size, Depth, Width, ValueLength); percentage and
# categorical sweeps have no clean per-element semantic.
if (exists("all_effects") && length(all_effects) > 0) {
  pe_dir <- file.path(base_plot_dir, "01_isolation")
  dir.create(pe_dir, showWarnings = FALSE, recursive = TRUE)

  pe_dim_order  <- c("size", "depth", "width", "value_length")
  pe_dim_labels <- c("Size", "Depth", "Width", "ValueLength")

  format_pe_ratio <- function(r) {
    case_when(
      is.na(r) ~ "—",
      r >= 10  ~ sprintf("%.1fx", r),
      r >= 1   ~ sprintf("%.2fx", r),
      TRUE     ~ sprintf("%.2fx", r)
    )
  }

  pe_data <- bind_rows(all_effects) %>%
    filter(Dimension %in% pe_dim_order,
           !is.na(per_element_ratio_extreme)) %>%
    mutate(
      Dimension   = factor(Dimension, levels = pe_dim_order,
                           labels = pe_dim_labels),
      log_pe      = log10(per_element_ratio_extreme),
      pe_label    = format_pe_ratio(per_element_ratio_extreme)
    )

  write_csv(pe_data, file.path(pe_dir, "cross_dim_per_element_ratios.csv"))

  max_dev_pe   <- max(abs(pe_data$log_pe), na.rm = TRUE)
  pe_limits    <- c(-max_dev_pe, max_dev_pe)

  p <- ggplot(pe_data, aes(x = Dimension, y = Library, fill = log_pe)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = pe_label), size = 3.8, fontface = "bold") +
    scale_fill_gradient2(
      low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
      midpoint = 0, limits = pe_limits,
      name   = "Per-element ratio (log)",
      labels = function(x) sprintf("%.2fx", 10^x)
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          strip.text      = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid      = element_blank())

  ggsave(file.path(pe_dir, "cross_dim_per_element_ratios.png"),
         p, width = 10, height = 7, dpi = 300)
  cat(sprintf("Saved cross-dim per-element ratio heatmap to: %s\n", pe_dir))
}

# ===========================================================================
# AGGREGATE CROSS-DIM RANK SUMMARY (RQ2: who dominates each dimension)
# ===========================================================================
# Mean rank per (Library × Operation × Dimension) computed from the per-cell
# ranks built inside each per-bench iteration. Captures the cross-dim story:
# does the same library dominate across all dimensions, or do different
# libraries excel on different dimensions?
if (exists("all_rank_data") && length(all_rank_data) > 0) {
  rank_dir <- file.path(base_plot_dir, "01_isolation")
  dir.create(rank_dir, showWarnings = FALSE, recursive = TRUE)

  # Dimension ordering matches the §5.3.2 / §5.4.3 subsections.
  dim_order_chap  <- c("size", "depth", "width", "value_length",
                       "numeric", "unicode", "escape", "unicode_escape",
                       "redundancy")
  dim_labels_chap <- c("Size", "Depth", "Width", "ValueLength",
                       "Numeric", "Unicode", "Escape", "Unicode-Escape",
                       "Redundancy")

  all_ranks <- bind_rows(all_rank_data)

  cross_dim_summary <- all_ranks %>%
    group_by(Library, Operation, Dimension) %>%
    summarise(
      n_cells     = n(),
      mean_rank   = mean(Rank),
      median_rank = median(Rank),
      pct_rank_1  = 100 * mean(Rank == 1),
      pct_rank_5  = 100 * mean(Rank == 5),
      mean_ratio  = mean(NormRatio),
      .groups = "drop"
    ) %>%
    filter(Dimension %in% dim_order_chap) %>%
    mutate(Dimension = factor(Dimension, levels = dim_order_chap,
                               labels = dim_labels_chap))

  write_csv(cross_dim_summary,
            file.path(rank_dir, "cross_dim_rank_summary.csv"))

  # Mean rank heatmap: rows = libraries, cols = dims, facet by operation.
  # Wistia palette continuous over [1, 5] matches the per-dim rank heatmap.
  p <- ggplot(cross_dim_summary,
              aes(x = Dimension, y = Library, fill = mean_rank)) +
    geom_tile(color = "white", linewidth = 0.5) +
    scale_y_discrete(limits = rev) +
    geom_text(aes(label = sprintf("%.2f", mean_rank)),
              size = 3.5, fontface = "bold") +
    scale_fill_gradientn(
      colours = c("#e4ff7a", "#ffe81a", "#ffbd00", "#ffa000", "#fc7f00"),
      limits  = c(1, 5),
      name    = "Mean rank"
    ) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y    = element_text(face = "bold"),
          axis.text.x    = element_text(angle = 30, hjust = 1),
          strip.text     = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid     = element_blank())
  ggsave(file.path(rank_dir, "cross_dim_mean_rank.png"),
         p, width = 11, height = 7, dpi = 300)
  cat(sprintf("Saved cross-dim rank summary to: %s\n", rank_dir))
}

cat("\nAll done!\n")
