library(tidyverse)

# --- Paths ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
results_dir <- file.path(script_dir, "..", "BenchmarkArtifacts", "results")
base_plot_dir <- file.path(script_dir, "figures", "isolation")

# --- Colors ---
lib_colors <- c(
  "SpanJson" = "#0072B2", "Utf8Json" = "#009E73",
  "STJRefGen" = "#F0E442", "STJSrcGen" = "#CC79A7", "Newtonsoft" = "#D55E00"
)

# Summarise data per group and append a grand-total row at the bottom.
# group_cols are coerced to character so they can carry the total_label.
# Each named aggregation in `...` is evaluated twice: once per group, once on
# the full data for the total row — n() on the un-grouped pass yields the
# whole-data count, matching the manual nrow() the old code used.
summarise_with_total <- function(data, group_cols, ..., total_label = "All") {
  args <- enquos(...)
  per_group <- data %>%
    group_by(across(all_of(group_cols))) %>%
    summarise(!!!args, .groups = "drop") %>%
    mutate(across(all_of(group_cols), as.character))
  total_header <- as_tibble(setNames(
    as.list(rep(total_label, length(group_cols))), group_cols))
  total <- bind_cols(total_header, data %>% summarise(!!!args))
  bind_rows(per_group, total)
}

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
    name = "string_composition",
    file_stem = "StringCompositionIsolation",
    # Captures the full label: A0 (shared ASCII baseline), U/E/UE + density.
    # The regex's alternation lists `UE` before `U` so it doesn't get partially
    # consumed; same for parse_dim's substitution.
    dim_regex      = "(?<=_)(A0|UE\\d+|U\\d+|E\\d+)",
    variant_regex  = "(?<=_)(UE|U|E)(?=\\d)",
    variant_map    = c("U" = "Unicode", "E" = "Escape", "UE" = "UnicodeEscape"),
    # The A0 label has no variant prefix that matches `variant_regex`, so
    # VariantCode comes back NA. Setting `baseline_label` triggers the
    # fan-out step in the per-bench loop: each A0 row is duplicated once
    # per real variant at DimValue = 0, so every variant facet has a clean
    # 0% baseline. Same physical measurement, three logical placements.
    baseline_label = "A0",
    dim_prefix     = "",
    dim_label      = "Special-character density (%)",
    x_label        = "Special character density (%)",
    parse_dim      = function(x) as.integer(sub("^(A|UE|U|E)", "", x)),
    # Per-element unit is the special-character count in the 20-char baseline
    # string: U5 = 1 special char, U10 = 2, U25 = 5, U50 = 10, U100 = 20. The
    # per-element block in run_per_bench skips the 0% (A0) baseline because
    # log10(0) is undefined; the cross-dim per-element heatmap then uses the
    # lowest positive level (5%) as the reference, giving a 20x sweep range
    # directly comparable to count-based sweeps.
    per_element_unit = "special_char"
  ),
  list(
    name = "numeric",
    file_stem = "NumericIsolation",
    # Captures the full label (F10, I5, …); parse_dim strips the prefix so
    # DimValue becomes the integer significant-digit count and the dim flows
    # through the same magnitude path as Size/Depth/Width/ValueLength.
    dim_regex = "(?<=_)[FI]\\d+",
    # variant_regex picks the F/I letter only; variant_map turns it into the
    # human-readable Integer/Float label used in faceting and table row-blocks.
    variant_regex = "(?<=_)[FI](?=\\d)",
    variant_map = c("I" = "Integer", "F" = "Float"),
    dim_prefix = "",
    dim_label = "Numeric Length",
    x_label = "Numeric value length (significant digits)",
    parse_dim = function(x) as.integer(sub("^[FI]", "", x)),
    per_element_unit = "digit"
  ),
  # DEPRECATED: the redundancy sweep measures a weak proxy (see Ch6 Future Work)
  # and is EXCLUDED from every cross-dim statistic. Its output folder is named
  # "redundancy_deprecated"; the aggregate blocks filter this name out. The input
  # file_stem is unchanged, so the raw RedundancyIsolation* files still load.
  list(
    name = "redundancy_deprecated",
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

  libs     <- levels(means$Library)
  op_short <- c(Deserialize = "Deser", Serialize = "Ser")

  has_var  <- "Variant" %in% colnames(means) && n_distinct(means$Variant) > 1
  variants <- if (has_var) levels(droplevels(means$Variant)) else NA_character_

  fmt_E <- function(x) ifelse(x < 100,
    formatC(x, format = "f", digits = 1, big.mark = ","),
    formatC(round(x), format = "d", big.mark = ","))
  fmt_t <- function(x) formatC(x, format = "f", digits = 2, big.mark = ",")
  cell_hex <- function(e) {
    rng <- range(e)
    n   <- if (diff(rng) == 0) rep(0.5, length(e)) else (e - rng[1]) / diff(rng)
    toupper(sub("#", "", ramp(n)))
  }

  # Find baseline / extreme labels: per-variant when variants are present so
  # I2/I10 and F2/F10 don't get cross-picked, otherwise the global first/last.
  endpoints_for <- function(v = NA) {
    sub <- if (is.na(v)) means else means %>% filter(Variant == v)
    lv  <- as.character(unique(sub$DimLabel))
    lv  <- lv[order(match(lv, levels(sub$DimLabel)))]
    c(lo = lv[1], hi = lv[length(lv)])
  }

  get_block <- function(opn, lvl, v = NA) {
    d <- means %>% filter(Operation == opn, DimLabel == lvl)
    if (!is.na(v)) d <- d %>% filter(Variant == v)
    d <- d[match(libs, as.character(d$Library)), ]
    list(t = fmt_t(d$MeanTime), E = fmt_E(d$MeanEnergy), hex = cell_hex(d$MeanEnergy))
  }

  build_lib_row <- function(i, B) {
    cells <- unlist(lapply(B, function(b)
      c(b$t[i], sprintf("\\cellcolor[HTML]{%s}%s", b$hex[i], b$E[i]))))
    paste0(libs[i], " & ", paste(cells, collapse = " & "), " \\\\")
  }

  if (has_var) {
    # 4 column groups: Deser base, Deser extreme, Ser base, Ser extreme.
    # Column headers stay generic; the per-variant row-block header carries
    # the actual baseline/extreme labels so I2/I10 and F2/F10 are clear.
    nb      <- 4
    colspec <- paste0("l", strrep("r", nb * 2))
    hdr_top <- paste0(" & \\multicolumn{4}{c}{Deserialise} ",
                       "& \\multicolumn{4}{c}{Serialise} \\\\")
    cmids   <- "\\cmidrule(lr){2-5}\\cmidrule(lr){6-9}"
    hdr_mid <- paste0(" & \\multicolumn{2}{c}{base} & \\multicolumn{2}{c}{extreme}",
                       " & \\multicolumn{2}{c}{base} & \\multicolumn{2}{c}{extreme} \\\\")
    cmids2  <- paste("\\cmidrule(lr){2-3}\\cmidrule(lr){4-5}",
                     "\\cmidrule(lr){6-7}\\cmidrule(lr){8-9}", sep = "")
    hdr_unit <- paste0("Library & ",
                       paste(rep("$t$ & $E$", nb), collapse = " & "), " \\\\")

    body <- character(0)
    for (vi in seq_along(variants)) {
      v  <- variants[vi]
      ep <- endpoints_for(v)
      B  <- list(
        get_block("Deserialize", ep[["lo"]], v),
        get_block("Deserialize", ep[["hi"]], v),
        get_block("Serialize",   ep[["lo"]], v),
        get_block("Serialize",   ep[["hi"]], v)
      )
      block_header <- sprintf(
        "    \\multicolumn{%d}{l}{\\textit{%s} \\;(base = %s, extreme = %s)} \\\\",
        nb * 2 + 1, v, ep[["lo"]], ep[["hi"]])
      body <- c(body, block_header,
                vapply(seq_along(libs),
                       function(i) paste0("    ", build_lib_row(i, B)),
                       character(1)))
      if (vi < length(variants)) body <- c(body, "    \\midrule")
    }

    dl <- gsub("%", "\\\\%", bench$dim_label)
    caption <- sprintf(paste0("Execution time ($t$, $\\mu$s/op) and energy ",
      "($E$, $\\mu$J/op, Package${+}$DRAM) across the %s sweep, with %s variants ",
      "as row-blocks. Energy cell colours are normalised within each column, ",
      "lower (cool) to higher (hot)."), dl,
      paste(variants, collapse = " / "))

    lines <- c(
      "% Auto-generated by isolation_analysis.R -- do not edit by hand.",
      "% Energy = Package + DRAM (uJ/op); time = us/op. Wistia shading normalised within each (operation, base/extreme) energy column.",
      "\\begin{table}[H]",
      "  \\centering",
      "  \\small",
      sprintf("  \\caption{%s}", caption),
      sprintf("  \\label{tab:iso-%s-energy-time}", bench$name),
      sprintf("  \\begin{tabular}{%s}", colspec),
      "    \\toprule",
      paste0("    ", hdr_top),
      paste0("    ", cmids),
      paste0("    ", hdr_mid),
      paste0("    ", cmids2),
      paste0("    ", hdr_unit),
      "    \\midrule",
      body,
      "    \\bottomrule",
      "  \\end{tabular}",
      "\\end{table}"
    )
  } else {
    lv <- levels(means$DimLabel)
    if (length(lv) < 2) return(invisible(NULL))
    lo <- lv[1]; hi <- lv[length(lv)]

    blocks <- data.frame(
      Operation = c("Deserialize", "Deserialize", "Serialize", "Serialize"),
      Level     = c(lo, hi, lo, hi),
      stringsAsFactors = FALSE
    )
    B <- Map(get_block, blocks$Operation, blocks$Level)

    nb      <- nrow(blocks)
    colspec <- paste0("l", strrep("r", nb * 2))
    hdr1 <- paste0(" & ", paste(sprintf("\\multicolumn{2}{c}{%s %s}",
                    op_short[blocks$Operation], blocks$Level), collapse = " & "), " \\\\")
    cmids <- paste(sprintf("\\cmidrule(lr){%d-%d}",
                    seq(2, by = 2, length.out = nb), seq(3, by = 2, length.out = nb)),
                    collapse = "")
    hdr2 <- paste0("Library & ", paste(rep("$t$ & $E$", nb), collapse = " & "), " \\\\")
    body <- vapply(seq_along(libs),
                   function(i) paste0("    ", build_lib_row(i, B)),
                   character(1))

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
      body,
      "    \\bottomrule",
      "  \\end{tabular}",
      "\\end{table}"
    )
  }

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

# ===========================================================================
# MAIN PROCESSING LOOP
# ===========================================================================

for (bench in benchmarks) {
  measurements_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%sByteBench-measurements.csv", bench$file_stem))
  report_file <- file.path(results_dir,
    sprintf("JsonBench.Benchmarks.Isolation.%sByteBench-report.csv", bench$file_stem))

  if (!file.exists(measurements_file)) {
    cat(sprintf("Skipping %s — measurements file not found\n", bench$name))
    next
  }

  cat(sprintf("\n========== %s ==========\n", toupper(bench$name)))

  plot_dir       <- file.path(base_plot_dir, bench$name)
  plot_stats_dir <- file.path(plot_dir, "stats")
  dir.create(plot_dir,       showWarnings = FALSE, recursive = TRUE)
  dir.create(plot_stats_dir, showWarnings = FALSE, recursive = TRUE)

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

  # Variant axis: when bench$variant_regex is set, split the sweep into named
  # variants (e.g., Numeric → Integer / Float at matched digit counts). All
  # per-(Library, Operation) stats are then computed per Variant so int and
  # float never get pooled. For benches without a variant axis, Variant is a
  # single-level factor "All" and acts as a no-op in every group_by below.
  has_variant <- !is.null(bench$variant_regex)
  if (has_variant) {
    df <- df %>%
      mutate(
        VariantCode = str_extract(Target_Method, bench$variant_regex),
        Variant     = factor(unname(bench$variant_map[VariantCode]),
                             levels = unname(bench$variant_map))
      )
  } else {
    df <- df %>% mutate(Variant = factor("All", levels = "All"))
  }

  # Shared-baseline fan-out: some variant benches (e.g. String Composition)
  # share one ASCII baseline label across every variant — the regex misses
  # it (no variant prefix), so Variant is NA on those rows. Duplicate each
  # baseline row once per real variant at DimValue = 0 so every variant facet
  # gets a clean 0% point keyed off the same physical measurement. Numeric
  # has no shared baseline (each variant has its own lowest endpoint — I2
  # for integers, F2 for floats), so this step is a no-op for it.
  if (has_variant && !is.null(bench$baseline_label)) {
    baseline_rows <- df %>% filter(DimRaw == bench$baseline_label)
    if (nrow(baseline_rows) > 0) {
      variant_levels <- levels(df$Variant)
      fanned <- bind_rows(lapply(variant_levels, function(v) {
        baseline_rows %>% mutate(Variant = factor(v, levels = variant_levels))
      }))
      df <- df %>% filter(DimRaw != bench$baseline_label) %>% bind_rows(fanned)
    }
  }

  # Create ordered dimension labels (sort DimRaw by DimValue, with variant
  # grouping when present so e.g. all Integer levels appear before all Float
  # levels in the x-axis of each variant facet). For shared-baseline benches
  # (e.g. String Composition's A0 fanned into every variant), the same DimRaw
  # appears in multiple variants — unique() keeps each factor level once.
  if (is.numeric(df$DimValue)) {
    dim_order <- sort(unique(df$DimValue))
    if (has_variant) {
      # Order labels by (Variant, DimValue): e.g. I2, I3, ..., I10, F2, ..., F10.
      label_lookup <- df %>%
        distinct(DimRaw, DimValue, Variant) %>%
        arrange(Variant, DimValue)
      dim_labels <- unique(paste0(bench$dim_prefix, label_lookup$DimRaw))
    } else {
      raw_sorted <- df %>% distinct(DimRaw, DimValue) %>% arrange(DimValue) %>% pull(DimRaw)
      dim_labels <- paste0(bench$dim_prefix, raw_sorted)
    }
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
    group_by(Library, Variant, Operation, DimValue, DimLabel) %>%
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
    group_by(Library, Variant, Operation, DimLabel) %>%
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

  # Per-bench SW p-value distribution. Two facets (Deserialise / Serialise),
  # vertical line at α = 0.05. The reader's only takeaway should be "most
  # groups are non-normal, so non-parametric tests downstream"; per-cell
  # detail lives in normality/shapiro_results.csv.
  p_hist <- ggplot(shapiro_results, aes(x = sw_p_value)) +
    geom_histogram(bins = 20, fill = "#56B4E9", color = "white", alpha = 0.9) +
    geom_vline(xintercept = 0.05, linetype = "dashed",
               color = "#D55E00", linewidth = 0.6) +
    facet_wrap(~ Operation, ncol = 2) +
    labs(x = "Shapiro-Wilk p-value", y = "Group count") +
    theme_minimal(base_size = 14) +
    theme(strip.text = element_text(face = "bold"))

  # SW artifacts mirror the kruskal_wallis/ and cliffs_delta/ layout: histogram
  # + per-cell results CSV + Operation roll-up, all under stats/normality/
  # (alongside the qq/ subfolder).
  normality_dir <- file.path(plot_stats_dir, "normality")
  dir.create(normality_dir, showWarnings = FALSE, recursive = TRUE)
  ggsave(file.path(normality_dir, "shapiro_pvalue_distribution.png"),
         p_hist, width = 10, height = 4, dpi = 300)
  write_csv(shapiro_results,
            file.path(normality_dir, "shapiro_results.csv"))
  shapiro_summary <- summarise_with_total(
    shapiro_results, "Operation",
    n_groups       = n(),
    n_normal       = sum(normal, na.rm = TRUE),
    n_non_normal   = sum(!normal, na.rm = TRUE),
    pct_normal     = 100 * mean(normal, na.rm = TRUE),
    pct_non_normal = 100 * mean(!normal, na.rm = TRUE)
  )
  write_csv(shapiro_summary,
            file.path(normality_dir, "shapiro_summary.csv"))
  cat("  Saved: shapiro_wilk\n")

  # Accumulate per-bench SW results for the aggregate summary at the end.
  if (!exists("all_shapiro")) all_shapiro <- list()
  all_shapiro[[bench$name]] <- shapiro_results

  # ================================================================
  # 0a. QQ PLOTS (per Library × Operation × Variant)
  # ================================================================
  # Visual complement to the Shapiro-Wilk histogram. One PNG per
  # (Library × Operation × Variant) under stats/normality/qq/<lib>/,
  # faceted by sweep level. Mirrors factorial_analysis.R's QQ layout.
  qq_dir <- file.path(plot_stats_dir, "normality", "qq")

  for (lib in levels(df$Library)) {
    lib_qq_dir <- file.path(qq_dir, tolower(lib))
    dir.create(lib_qq_dir, showWarnings = FALSE, recursive = TRUE)

    for (op_lvl in levels(df$Operation)) {
      for (v_lvl in levels(df$Variant)) {
        sub <- df %>% filter(Library == lib, Operation == op_lvl, Variant == v_lvl)
        if (nrow(sub) == 0) next

        p_qq <- ggplot(sub, aes(sample = EnergyPerOp)) +
          stat_qq(size = 1.6, alpha = 0.7, color = "#56B4E9") +
          stat_qq_line(linewidth = 0.6, color = "#D55E00") +
          facet_wrap(~ DimLabel, scales = "free_y") +
          labs(x = "Theoretical Quantiles",
               y = "Sample Quantiles (Energy/Op)") +
          theme_minimal(base_size = 12) +
          theme(strip.text = element_text(face = "bold"))

        op_short <- ifelse(op_lvl == "Deserialize", "deser", "ser")
        v_suffix <- if (has_variant) sprintf("_%s", tolower(v_lvl)) else ""
        ggsave(file.path(lib_qq_dir, sprintf("qq_%s%s.png", op_short, v_suffix)),
               p_qq, width = 10, height = 6, dpi = 300)
      }
    }
  }
  cat("  Saved: qq plots\n")

  # ================================================================
  # 0b. ACROSS-LIBRARY KW (per cell) + CLIFF'S δ HELPERS
  # ================================================================
  # Across-libraries KW within each cell (Variant × DimLabel × Operation):
  # asks whether the five libraries differ at this fixed workload point.
  # Mirrors factorial_analysis.R's stats/kruskal_wallis/library/ pass and
  # backs the per-cell ranking claims in §5.2 — without rejection here, the
  # ranks reported per level would be indistinguishable from noise.
  kw_library_results <- df %>%
    group_by(Variant, DimLabel, Operation) %>%
    group_modify(~ {
      if (n_distinct(.x$Library) <= 1) {
        return(tibble(n_obs = nrow(.x),
                      n_libs = n_distinct(.x$Library),
                      kw_chi = NA_real_, kw_df = NA_real_, kw_p = NA_real_))
      }
      k <- kruskal.test(.x$EnergyPerOp ~ .x$Library)
      tibble(n_obs  = nrow(.x),
             n_libs = n_distinct(.x$Library),
             kw_chi = unname(k$statistic),
             kw_df  = unname(k$parameter),
             kw_p   = k$p.value)
    }) %>%
    ungroup() %>%
    mutate(Dimension   = bench$name,
           significant = !is.na(kw_p) & kw_p < 0.05)

  kw_library_dir <- file.path(plot_stats_dir, "kruskal_wallis")
  dir.create(kw_library_dir, showWarnings = FALSE, recursive = TRUE)
  write_csv(kw_library_results,
            file.path(kw_library_dir, "kw_library_results.csv"))

  kw_library_summary <- summarise_with_total(
    kw_library_results, "Operation",
    n_cells             = n(),
    n_significant       = sum(significant, na.rm = TRUE),
    n_not_significant   = sum(!significant, na.rm = TRUE),
    pct_significant     = 100 * mean(significant, na.rm = TRUE),
    pct_not_significant = 100 * mean(!significant, na.rm = TRUE)
  )
  write_csv(kw_library_summary,
            file.path(kw_library_dir, "kw_library_summary.csv"))

  if (!exists("all_kw_library")) all_kw_library <- list()
  all_kw_library[[bench$name]] <- kw_library_results
  cat("  Saved: kw_library (across-libs per cell)\n")

  # Cliff's δ between two samples.
  # Sign convention: δ = (#{y > x} − #{y < x}) / (m·n). δ > 0 means the *second*
  # argument is stochastically larger. This is the opposite of effsize's
  # cliff.delta(d, f) where δ > 0 means the *first* arg is larger; downstream
  # heatmap subtitles must match this convention.
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

  # Endpoint magnitude ratio: mean energy at the extreme sweep level divided
  # by mean energy at the baseline level. Well-defined for categorical and
  # percentage sweeps; reads as "going from baseline to extreme makes this
  # library Xx more (or less) expensive". Feeds the cross-dim ratio heatmap
  # in 01_isolation/.
  endpoint_ratio_per_group <- means %>%
    group_by(Library, Variant, Operation) %>%
    summarise(
      e_baseline    = first(MeanEnergy[DimValue == base_value]),
      e_extreme     = first(MeanEnergy[DimValue == extreme_value]),
      ratio_extreme = e_extreme / e_baseline,
      .groups = "drop"
    ) %>%
    select(Library, Variant, Operation, ratio_extreme)

  # Per-element endpoint ratio: E_ratio divided by dim_ratio, i.e.\ the
  # growth factor of energy-per-element from baseline to extreme. Defined
  # whenever the dimension counts elements (per_element_unit set). Equals 1
  # at perfectly linear scaling regardless of how wide the sweep range is;
  # >1 indicates super-linear, <1 sub-linear. When the sweep starts at 0
  # (String Composition's A0 = 0% density), the lowest positive level is
  # used as the per-element baseline so dim_ratio stays defined.
  if (!is.null(bench$per_element_unit) && is.numeric(df$DimValue)) {
    pe_base <- if (base_value > 0) base_value else {
      pos_levels <- dim_order[dim_order > 0]
      if (length(pos_levels) > 0) min(pos_levels) else NA_real_
    }
    if (!is.na(pe_base) && pe_base > 0) {
      dim_ratio <- extreme_value / pe_base
      per_element_endpoint <- means %>%
        group_by(Library, Variant, Operation) %>%
        summarise(
          e_pe_base = first(MeanEnergy[DimValue == pe_base]),
          e_extreme = first(MeanEnergy[DimValue == extreme_value]),
          per_element_ratio_extreme = (e_extreme / e_pe_base) / dim_ratio,
          .groups = "drop"
        ) %>%
        select(Library, Variant, Operation, per_element_ratio_extreme)
    } else {
      per_element_endpoint <- endpoint_ratio_per_group %>%
        mutate(per_element_ratio_extreme = NA_real_) %>%
        select(Library, Variant, Operation, per_element_ratio_extreme)
    }
  } else {
    per_element_endpoint <- endpoint_ratio_per_group %>%
      mutate(per_element_ratio_extreme = NA_real_) %>%
      select(Library, Variant, Operation, per_element_ratio_extreme)
  }

  # Adjacent-pair step data: per-step W-ratio, E-ratio, and per-element costs
  # at each end of the step. Written as its own CSV; feeds the local-ratio and
  # per-element-ratio heatmaps below.
  if (is.numeric(df$DimValue)) {
    local_steps <- means %>%
      filter(DimValue > 0) %>%
      group_by(Library, Variant, Operation) %>%
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
        # Per-element cost at each end of the step: energy divided by the
        # number of units the dimension counts (fields for width, objects for
        # size, characters for value_length, etc.).
        from_per_element = MeanEnergy   / DimValue,
        to_per_element   = to_MeanEnergy / to_DimValue
      ) %>%
      select(Library, Variant, Operation,
             from_level = DimValue, to_level   = to_DimValue,
             from_label = DimLabel, to_label   = to_DimLabel,
             from_E     = MeanEnergy, to_E     = to_MeanEnergy,
             from_per_element, to_per_element,
             W_ratio, E_ratio)
    write_csv(local_steps, file.path(plot_dir, "local_steps.csv"))
    cat("  Saved: local_steps\n")

    # Local per-element ratio heatmap: emitted only for "count-of-things"
    # dimensions that carry a per_element_unit; skipped for percentage-of-
    # substitution sweeps (Unicode, Escape, Numeric, Redundancy) where the
    # per-element semantic is not meaningful.
    if (!is.null(bench$per_element_unit)) {
      # Local per-element ratio heatmap: each adjacent transition's per-element
      # energy growth factor (= to_per_element / from_per_element, equivalently
      # E_ratio / W_ratio). At linear scaling all cells read 1.0x regardless of
      # the underlying W-step, so this view is roughly column-comparable.
      # Only emitted for count-based dims (Size, Depth, Width, ValueLength)
      # where the per-element semantic is meaningful.
      pe_plot <- local_steps %>%
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
          name   = "Per-element ratio",
          labels = function(x) sprintf("%.2fx", 10^x)
        ) +
        guides(fill = guide_colourbar(title.position = "top", title.hjust = 0.5,
                                      barwidth  = grid::unit(8, "cm"),
                                      barheight = grid::unit(0.4, "cm"))) +
        (if (has_variant)
           facet_grid(Operation ~ Variant, scales = "free_x", space = "free_x")
         else facet_wrap(~ Operation, ncol = 1)) +
        labs(x = NULL, y = NULL) +
        theme_minimal(base_size = 14) +
        theme(axis.text.y     = element_text(face = "bold"),
              axis.text.x     = element_text(angle = 30, hjust = 1),
              strip.text      = element_text(face = "bold"),
              legend.position = "bottom",
              panel.grid      = element_blank())

      pe_width <- if (has_variant) 12 else 10
      ggsave(file.path(plot_dir, "local_per_element_ratio_heatmap.png"),
             p, width = pe_width, height = 5.5, dpi = 300)
      cat("  Saved: local_per_element_ratio_heatmap\n")
    }
  }

  # Per-element endpoint ratio per (Library, Variant, Operation), tagged with
  # the dimension name. This is the only per-dim effect carried forward: it
  # feeds the cross-dim per-element ratio heatmap (RQ1 normalised view).
  effect_summary <- per_element_endpoint %>%
    mutate(Dimension = bench$name)

  # Accumulate for the cross-dim per-element ratio heatmap.
  if (!exists("all_effects")) all_effects <- list()
  all_effects[[bench$name]] <- effect_summary

  # ================================================================
  # 0c. CLIFF'S δ — PAIRWISE LIBRARIES (lib-pair δ at every cell)
  # ================================================================
  # Lib-pair δ at every (sweep level × variant) cell, written straight into
  # stats/cliffs_delta/ (no library/ subfolder). Variant is an extra fixed-other
  # axis: pairs are computed within a variant so I2..I10 never mix with F2..F10.
  # This is the only Cliff's δ artifact retained (it backs the per-level ranking
  # claims); the per-library level-pair and cross-dim endpoint δ families were
  # dropped as non-discriminating (δ saturates within a library across a sweep).
  # Per-dim results are also pooled into _aggregate/stats/ further below.

  cd_lib_dir <- file.path(plot_stats_dir, "cliffs_delta")
  dir.create(cd_lib_dir, showWarnings = FALSE, recursive = TRUE)

  lib_levels_iso <- levels(df$Library)
  lib_pairs_iso  <- combn(lib_levels_iso, 2, simplify = FALSE)
  pair_order_iso <- sapply(lib_pairs_iso, function(p) sprintf("%s vs %s", p[1], p[2]))

  # lib pairs at each (level × variant) cell
  cd_library_results <- tibble()
  for (op_lvl in levels(df$Operation)) {
    for (v_lvl in levels(df$Variant)) {
      levels_in_v <- df %>% filter(Variant == v_lvl) %>%
        pull(DimLabel) %>% droplevels() %>% levels()
      for (lv in levels_in_v) {
        sub_cell <- df %>%
          filter(Operation == op_lvl, Variant == v_lvl, DimLabel == lv)
        if (nrow(sub_cell) == 0) next
        for (pair in lib_pairs_iso) {
          a <- sub_cell %>% filter(Library == pair[1]) %>% pull(EnergyPerOp)
          b <- sub_cell %>% filter(Library == pair[2]) %>% pull(EnergyPerOp)
          if (length(a) < 2 || length(b) < 2) next
          d <- cliffs_delta(a, b)
          cd_library_results <- bind_rows(cd_library_results, tibble(
            LibA       = pair[1],
            LibB       = pair[2],
            comparison = sprintf("%s vs %s", pair[1], pair[2]),
            Operation  = op_lvl,
            DimLabel   = lv,
            Variant    = v_lvl,
            delta      = d,
            delta_abs  = abs(d),
            magnitude  = magnitude_tier(abs(d))
          ))
        }
      }
    }
  }
  if (nrow(cd_library_results) > 0) {
    cd_library_results <- cd_library_results %>%
      mutate(comparison = factor(comparison, levels = pair_order_iso),
             DimLabel   = factor(DimLabel, levels = levels(df$DimLabel)))

    write_csv(cd_library_results,
              file.path(cd_lib_dir, "cd_library_pairwise_results.csv"))

    # Pool for the cross-dim Cliff's δ summary (redundancy excluded downstream).
    if (!exists("all_cd_library")) all_cd_library <- list()
    all_cd_library[[bench$name]] <- cd_library_results %>%
      mutate(Dimension = bench$name, DimLabel = as.character(DimLabel))

    # Heatmap: operations as columns. base_size 14 + no on-figure title (LaTeX
    # caption carries it); the sign-convention subtitle is kept because the plot
    # is unreadable without it. Variant-bearing dimensions are written as one PNG
    # per variant (numeric -> Integer/Float = 2; string_composition ->
    # Unicode/Escape/UnicodeEscape = 3) rather than a single facet grid.
    make_cd_heatmap <- function(dat) {
      ggplot(dat, aes(x = DimLabel, y = comparison, fill = delta)) +
        geom_tile(color = "white", linewidth = 0.3) +
        geom_text(aes(label = sprintf("%.2f", delta)), size = 2.4) +
        scale_y_discrete(limits = rev(pair_order_iso)) +
        scale_fill_gradient2(low = "#56B4E9", mid = "#F5F5F5", high = "#D55E00",
                             midpoint = 0, name = "Cliff's delta",
                             limits = c(-1, 1)) +
        facet_wrap(~ Operation, nrow = 1) +
        labs(subtitle = "delta > 0: first library uses LESS energy",
             x = bench$dim_label, y = "Library Pair") +
        theme_minimal(base_size = 14) +
        theme(axis.text.x   = element_text(angle = 30, hjust = 1),
              strip.text    = element_text(face = "bold"),
              plot.subtitle = element_text(size = 10),
              panel.grid    = element_blank())
    }

    if (has_variant) {
      for (v_lvl in unique(cd_library_results$Variant)) {
        sub_v <- cd_library_results %>% filter(Variant == v_lvl)
        if (nrow(sub_v) == 0) next
        ggsave(file.path(cd_lib_dir, sprintf("cd_library_pairs_%s.png", tolower(v_lvl))),
               make_cd_heatmap(sub_v), width = 11, height = 6, dpi = 300)
      }
    } else {
      ggsave(file.path(cd_lib_dir, "cd_library_pairs.png"),
             make_cd_heatmap(cd_library_results), width = 11, height = 6, dpi = 300)
    }
  }

  cat("  Saved: cliffs_delta library-pair heatmap\n")

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
    (if (has_variant)
       facet_grid(Operation ~ Variant, scales = "free_y")
     else facet_wrap(~ Operation, ncol = 1, scales = "free_y")) +
    labs(x = bench$x_label, y = "Total Energy (μJ/op) [Package + DRAM]",
         color = "Library") +
    theme_minimal(base_size = 14) +
    theme(legend.position = "bottom",
          panel.grid.minor = element_blank(),
          strip.text = element_text(face = "bold"))

  scaling_width  <- if (has_variant) 12 else 10
  scaling_height <- if (has_variant) 9  else 9
  ggsave(file.path(plot_dir, "scaling.png"),
         p, width = scaling_width, height = scaling_height, dpi = 300)
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

  # Rank is computed within each (Operation, Variant, DimLabel) cell. Including
  # Variant matters for shared-baseline benches (String Composition's A0 is
  # fanned into every variant — without Variant in the grouping its 5 libraries
  # would be ranked against 15 entries instead of 5). For benches without a
  # variant axis the Variant column is constant "All", so this is a no-op.
  rank_ratio_data <- means %>%
    group_by(Operation, Variant, DimLabel) %>%
    mutate(
      Rank       = rank(MeanEnergy, ties.method = "min"),
      NormRatio  = MeanEnergy / min(MeanEnergy),
      RatioLabel = sprintf(ifelse(NormRatio >= 10, "(%.1fx)", "(%.2fx)"),
                           NormRatio)
    ) %>%
    ungroup()

  # Accumulate per-dim rank data for the cross-dim summary at the bottom.
  # Only the columns the aggregate actually uses are kept (DimValue is dropped
  # to keep the bind_rows portable across dims with different x types).
  if (!exists("all_rank_data")) all_rank_data <- list()
  all_rank_data[[bench$name]] <- rank_ratio_data %>%
    select(Library, Variant, Operation, Rank, NormRatio) %>%
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
    (if (has_variant)
       facet_grid(Operation ~ Variant, scales = "free_x", space = "free_x")
     else facet_wrap(~ Operation, ncol = 1)) +
    labs(x = bench$dim_label, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y    = element_text(face = "bold"),
          strip.text     = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid     = element_blank())
  rr_width  <- if (has_variant) 12 else 10
  ggsave(file.path(plot_dir, "heatmap_rank_ratio.png"),
         p, width = rr_width, height = 9, dpi = 300)
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

    # ================================================================
    # 3. ALLOCATION BAR CHART (both operations in one PNG)
    # ================================================================
    pd_alloc <- report %>% mutate(AllocKB = AllocBytes / 1024)

    p_alloc <- ggplot(pd_alloc, aes(x = DimLabel, y = AllocKB, fill = Library)) +
      geom_col(position = position_dodge(width = 0.8), width = 0.7) +
      scale_fill_manual(values = lib_colors) +
      facet_wrap(~ Operation, ncol = 2, scales = "free_y") +
      labs(x = bench$dim_label, y = "Allocated (KB/op)", fill = "Library") +
      theme_minimal(base_size = 14) +
      theme(legend.position = "bottom",
            strip.text      = element_text(face = "bold"),
            axis.text.x     = element_text(angle = 45, hjust = 1))
    ggsave(file.path(plot_dir, "alloc.png"),
           p_alloc, width = 14, height = 6, dpi = 300)
    cat("  Saved: alloc\n")

    # ================================================================
    # 4. GC COLLECTIONS BAR CHART (both operations in one PNG)
    # ================================================================
    gc_long <- report %>%
      select(Library, Operation, DimLabel, Gen0, Gen1) %>%
      mutate(across(c(Gen0, Gen1), ~ tidyr::replace_na(.x, 0))) %>%
      pivot_longer(cols = c(Gen0, Gen1),
                   names_to = "Generation", values_to = "Collections") %>%
      mutate(Generation = factor(Generation, levels = c("Gen1", "Gen0")))

    gen_colors <- c("Gen0" = "#56B4E9", "Gen1" = "#E69F00")

    p_gc <- ggplot(gc_long, aes(x = DimLabel, y = Collections, fill = Generation)) +
      geom_col(position = "stack", width = 0.7) +
      scale_fill_manual(values = gen_colors) +
      facet_grid(Operation ~ Library, scales = "free_y") +
      labs(x = bench$dim_label, y = "GC Collections / 1000 ops", fill = "Generation") +
      theme_minimal(base_size = 14) +
      theme(strip.text      = element_text(face = "bold"),
            legend.position = "bottom",
            axis.text.x     = element_text(angle = 45, hjust = 1, size = 9))
    ggsave(file.path(plot_dir, "gc.png"),
           p_gc, width = 14, height = 7, dpi = 300)
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

}

# ===========================================================================
# AGGREGATE NORMALITY SUMMARY (across all isolation dimensions)
# ===========================================================================
if (exists("all_shapiro") && length(all_shapiro) > 0) {
  agg_dir <- file.path(base_plot_dir, "_aggregate", "stats", "normality")
  dir.create(agg_dir, showWarnings = FALSE, recursive = TRUE)

  # Redundancy is deferred to Ch6 (Future Work); excluded from any cross-dim
  # aggregate so the chapter's "across the six dimensions" totals match what
  # §5.2 reports. Per-dim files under figures/isolation/redundancy_deprecated/ stay.
  all_sw <- bind_rows(all_shapiro[names(all_shapiro) != "redundancy_deprecated"])

  cat("\n=== Isolation Shapiro-Wilk Aggregate ===\n")
  cat(sprintf("Total groups tested: %d\n", nrow(all_sw)))
  cat(sprintf("Normal     (p >= 0.05): %d (%.1f%%)\n",
              sum(all_sw$normal, na.rm = TRUE),
              100 * mean(all_sw$normal, na.rm = TRUE)))
  cat(sprintf("Non-normal (p <  0.05): %d (%.1f%%)\n",
              sum(!all_sw$normal, na.rm = TRUE),
              100 * mean(!all_sw$normal, na.rm = TRUE)))

  # Per (Library, Operation) breakdown — quotable in the methodology chapter.
  # Carries both absolute counts (n_normal, n_non_normal) and percentages, plus
  # a grand-total row at the bottom so the chapter-wide headline number is in
  # the same file.
  by_lib_op <- summarise_with_total(
    all_sw, c("Library", "Operation"),
    n_groups       = n(),
    n_normal       = sum(normal, na.rm = TRUE),
    n_non_normal   = sum(!normal, na.rm = TRUE),
    pct_normal     = 100 * mean(normal, na.rm = TRUE),
    pct_non_normal = 100 * mean(!normal, na.rm = TRUE)
  )
  write_csv(by_lib_op, file.path(agg_dir, "shapiro_summary.csv"))
  write_csv(all_sw,    file.path(agg_dir, "shapiro_results.csv"))

  # Histogram of SW p-values across all isolation groups.
  p_hist <- ggplot(all_sw, aes(x = sw_p_value)) +
    geom_histogram(bins = 30, fill = "#56B4E9", color = "white", alpha = 0.9) +
    geom_vline(xintercept = 0.05, linetype = "dashed", color = "#D55E00", linewidth = 0.6) +
    annotate("text", x = 0.07, y = Inf, label = "alpha = 0.05",
             vjust = 2, hjust = 0, color = "#D55E00", size = 4) +
    labs(x = "Shapiro-Wilk p-value", y = "Group count") +
    theme_minimal(base_size = 14)
  ggsave(file.path(agg_dir, "shapiro_pvalue_distribution.png"),
         p_hist, width = 8, height = 5, dpi = 300)

  cat(sprintf("Saved aggregate to: %s\n", agg_dir))
}

# ===========================================================================
# AGGREGATE ACROSS-LIBRARIES KW (per-cell library equality) — RQ1
# ===========================================================================
# Pools the per-dim kw_library tables (excluding redundancy) and writes a
# matching cross-dim summary. Quotable as "KW rejects within-cell library
# equality in N/M cells" for the §5.2 intro.
if (exists("all_kw_library") && length(all_kw_library) > 0) {
  kw_lib_dir <- file.path(base_plot_dir, "_aggregate", "stats",
                          "kruskal_wallis")
  dir.create(kw_lib_dir, showWarnings = FALSE, recursive = TRUE)

  # Holm-Bonferroni across the within-cell KW family (one test per cell,
  # excluding redundancy). Mirrors the existing across-levels KW Holm step
  # so both pipelines (within-cell and across-levels) are FWER-controlled.
  # `significant` here uses kw_p_adj; per-dim files retain raw-p significance.
  all_kw_lib <- bind_rows(all_kw_library[names(all_kw_library) != "redundancy_deprecated"]) %>%
    mutate(
      kw_p_adj    = p.adjust(kw_p, method = "holm"),
      significant = !is.na(kw_p_adj) & kw_p_adj < 0.05
    )
  write_csv(all_kw_lib, file.path(kw_lib_dir, "kw_library_results.csv"))

  kw_lib_summary <- summarise_with_total(
    all_kw_lib, c("Dimension", "Operation"),
    n_cells             = n(),
    n_significant       = sum(significant, na.rm = TRUE),
    n_not_significant   = sum(!significant, na.rm = TRUE),
    pct_significant     = 100 * mean(significant, na.rm = TRUE),
    pct_not_significant = 100 * mean(!significant, na.rm = TRUE)
  )
  write_csv(kw_lib_summary, file.path(kw_lib_dir, "kw_library_summary.csv"))

  cat("\n=== Isolation Across-Library KW (within-cell, Holm-adjusted) Aggregate ===\n")
  cat(sprintf("Total cells tested: %d\n", nrow(all_kw_lib)))
  cat(sprintf("Significant     (p_adj <  0.05): %d (%.1f%%)\n",
              sum(all_kw_lib$significant, na.rm = TRUE),
              100 * mean(all_kw_lib$significant, na.rm = TRUE)))
  cat(sprintf("Non-significant (p_adj >= 0.05): %d (%.1f%%)\n",
              sum(!all_kw_lib$significant, na.rm = TRUE),
              100 * mean(!all_kw_lib$significant, na.rm = TRUE)))
  cat(sprintf("Saved aggregate KW (across libs) to: %s\n", kw_lib_dir))
}

# ===========================================================================
# AGGREGATE CROSS-DIM PER-ELEMENT RATIO HEATMAP (RQ1 — normalised view)
# ===========================================================================
# E_ratio / dim_ratio per (library, operation, dimension). Removes the sweep
# range from the magnitude reading: at perfectly linear scaling all cells
# read 1.0x regardless of whether the sweep covers 100x (Width) or 10000x
# (Size). String Composition variants are included by treating their density
# levels as counts of special characters in the 20-char baseline string
# (5% = 1 char, ..., 100% = 20 chars), with the 0% (A0) baseline skipped and
# U5/E5/UE5 used as the per-element reference instead (see string_composition
# bench config).
if (exists("all_effects") && length(all_effects) > 0) {
  pe_dir <- file.path(base_plot_dir, "_aggregate")
  dir.create(pe_dir, showWarnings = FALSE, recursive = TRUE)

  # Fold a row's Variant into its Dimension label (e.g. numeric -> numeric_float)
  # so Integer/Float and the String-Composition variants get their own columns.
  expand_variants <- function(df) {
    df %>%
      mutate(
        Dimension = ifelse(
          !is.na(Variant) & as.character(Variant) != "All",
          paste0(Dimension, "_", tolower(as.character(Variant))),
          Dimension
        )
      )
  }

  pe_dim_order  <- c("size", "depth", "width", "value_length",
                     "numeric_integer", "numeric_float",
                     "string_composition_unicode",
                     "string_composition_escape",
                     "string_composition_unicodeescape")
  pe_dim_labels <- c("Size", "Depth", "Width", "ValueLength",
                     "Numeric (Int)", "Numeric (Float)",
                     "StrComp (Uni)", "StrComp (Esc)", "StrComp (UE)")

  format_pe_ratio <- function(r) {
    case_when(
      is.na(r) ~ "—",
      r >= 10  ~ sprintf("%.1fx", r),
      r >= 1   ~ sprintf("%.2fx", r),
      TRUE     ~ sprintf("%.2fx", r)
    )
  }

  pe_data <- expand_variants(bind_rows(all_effects)) %>%
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
      name   = "Per-element ratio",
      labels = function(x) sprintf("%.2fx", 10^x)
    ) +
    guides(fill = guide_colourbar(title.position = "top", title.hjust = 0.5,
                                  barwidth  = grid::unit(12, "cm"),
                                  barheight = grid::unit(0.4, "cm"))) +
    facet_wrap(~ Operation, ncol = 1) +
    labs(x = NULL, y = NULL) +
    theme_minimal(base_size = 14) +
    theme(axis.text.y     = element_text(face = "bold"),
          axis.text.x     = element_text(angle = 30, hjust = 1),
          strip.text      = element_text(face = "bold"),
          legend.position = "bottom",
          panel.grid      = element_blank())

  ggsave(file.path(pe_dir, "cross_dim_per_element_ratios.png"),
         p, width = 11, height = 7.5, dpi = 300)
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
  rank_dir <- file.path(base_plot_dir, "_aggregate")
  dir.create(rank_dir, showWarnings = FALSE, recursive = TRUE)

  # Dimension ordering matches the §5.3.2 / §5.4.3 subsections. Numeric is
  # split into Int / Float since ranks within Int and within Float are
  # separate competitions; pooling would collapse two distinct rankings.
  dim_order_chap  <- c("size", "depth", "width", "value_length",
                       "numeric_integer", "numeric_float",
                       "string_composition_unicode",
                       "string_composition_escape",
                       "string_composition_unicodeescape")
  dim_labels_chap <- c("Size", "Depth", "Width", "ValueLength",
                       "Numeric (Int)", "Numeric (Float)",
                       "StrComp (Uni)", "StrComp (Esc)", "StrComp (UE)")

  # Reuse the same Variant expansion as for effects: when a rank row carries
  # a non-"All" Variant, fold the variant into the Dimension name so each
  # (Library, Operation, Dimension) group is one ranking.
  all_ranks <- bind_rows(all_rank_data) %>%
    mutate(
      Dimension = ifelse(
        !is.na(Variant) & as.character(Variant) != "All",
        paste0(Dimension, "_", tolower(as.character(Variant))),
        Dimension
      )
    )

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

# ===========================================================================
# AGGREGATE CROSS-DIM CLIFF'S δ — POOLED PAIRWISE + MAGNITUDE OVERVIEW (RQ1)
# ===========================================================================
# Pools every per-dim cd_library_pairwise_results.csv (excluding the deprecated
# redundancy sweep) into one table, plus a magnitude-tier overview so the
# ~1,000-row pooled table can be read at a glance: per library-pair counts and
# percentages of Romano tiers (L/M/S/N), with an "All" total row. The non-large
# rows are the statistically-tied rank comparisons discussed in §5.2.
if (exists("all_cd_library") && length(all_cd_library) > 0) {
  cd_agg_dir <- file.path(base_plot_dir, "_aggregate", "stats", "cliffs_delta")
  dir.create(cd_agg_dir, showWarnings = FALSE, recursive = TRUE)

  all_cd <- bind_rows(all_cd_library[names(all_cd_library) != "redundancy_deprecated"])
  write_csv(all_cd, file.path(cd_agg_dir, "cd_library_pairwise_results.csv"))

  cd_magnitude_summary <- summarise_with_total(
    all_cd, "comparison",
    n_cells       = n(),
    n_L           = sum(magnitude == "L", na.rm = TRUE),
    n_M           = sum(magnitude == "M", na.rm = TRUE),
    n_S           = sum(magnitude == "S", na.rm = TRUE),
    n_N           = sum(magnitude == "N", na.rm = TRUE),
    n_not_large   = sum(magnitude != "L", na.rm = TRUE),
    pct_large     = 100 * mean(magnitude == "L", na.rm = TRUE),
    pct_not_large = 100 * mean(magnitude != "L", na.rm = TRUE)
  )
  write_csv(cd_magnitude_summary,
            file.path(cd_agg_dir, "cd_magnitude_summary.csv"))

  total_cd <- tail(cd_magnitude_summary, 1)
  cat("\n=== Isolation Cliff's δ pooled (excl. redundancy) ===\n")
  cat(sprintf("Pairwise comparisons: %d\n", total_cd$n_cells))
  cat(sprintf("Large (|delta| >= 0.474): %d (%.1f%%)\n",
              total_cd$n_L, total_cd$pct_large))
  cat(sprintf("Not large (ties): %d (%.1f%%)\n",
              total_cd$n_not_large, total_cd$pct_not_large))
  cat(sprintf("Saved pooled Cliff's delta + magnitude overview to: %s\n", cd_agg_dir))
}

cat("\nAll done!\n")
