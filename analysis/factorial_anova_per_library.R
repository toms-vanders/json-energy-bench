library(tidyverse)
library(car)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialStringBench-measurements.csv")
base_plot_dir <- file.path(script_dir, "plots", "anova")
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

# Convert to factors
df <- df %>%
  mutate(
    Content   = factor(Content, levels = c("T", "N", "B"),
                       labels = c("Textual", "Numeric", "Boolean")),
    Library   = factor(Library, levels = c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")),
    Operation = factor(Operation, levels = c("Deser", "Ser"),
                       labels = c("Deserialize", "Serialize")),
    Depth     = factor(Depth),
    Width     = factor(Width)
  )

# ============================================================
# Per-library 3-factor ANOVA (Depth x Width x Content)
# ============================================================
run_per_library <- function(lib_name) {
  lib_dir <- file.path(base_plot_dir, tolower(lib_name))
  dir.create(lib_dir, showWarnings = FALSE, recursive = TRUE)

  lib_df <- df %>% filter(Library == lib_name)

  for (op_label in c("Deserialize", "Serialize")) {
    op_short <- tolower(gsub("ialize", "", op_label))
    op_df <- lib_df %>% filter(Operation == op_label)

    cat("\n--- ", lib_name, " | ", op_label, " ---\n")

    # Fit 3-factor ANOVA
    model <- lm(EnergyPerOp ~ Depth * Width * Content, data = op_df)
    aov_result <- Anova(model, type = 2)

    # Compute eta-squared
    ss <- aov_result$`Sum Sq`
    ss_total <- sum(ss)
    eta_sq <- ss / ss_total
    aov_table <- tibble(
      Factor = rownames(aov_result),
      SumSq  = ss,
      Df     = aov_result$Df,
      Fvalue = aov_result$`F value`,
      Pvalue = aov_result$`Pr(>F)`,
      EtaSq  = eta_sq
    )

    print(aov_table %>% mutate(across(where(is.numeric), ~ round(., 4))), n = 20)

    # Save CSV
    write_csv(aov_table, file.path(lib_dir, paste0("anova_table_", op_short, ".csv")))

    # --- Plot 1: Eta-squared bar chart ---
    effect_data <- aov_table %>%
      filter(Factor != "Residuals") %>%
      mutate(
        Factor = fct_reorder(Factor, EtaSq),
        Significant = ifelse(!is.na(Pvalue) & Pvalue < 0.05, "p < 0.05", "n.s.")
      )

    p1 <- ggplot(effect_data, aes(x = Factor, y = EtaSq * 100, fill = Significant)) +
      geom_col(width = 0.7) +
      geom_text(aes(label = sprintf("%.1f%%", EtaSq * 100)),
                hjust = -0.1, size = 3) +
      coord_flip() +
      scale_fill_manual(values = c("p < 0.05" = "#2196F3", "n.s." = "#BDBDBD")) +
      scale_y_continuous(expand = expansion(mult = c(0, 0.15))) +
      labs(
        title = paste0(lib_name, " - ", op_label, " - Variance Explained"),
        subtitle = "Eta-squared (%) from 3-factor ANOVA (Depth x Width x Content)",
        x = NULL, y = "Variance Explained (%)", fill = NULL
      ) +
      theme_minimal(base_size = 11) +
      theme(
        plot.title = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        legend.position = "bottom"
      )

    ggsave(file.path(lib_dir, paste0("eta_squared_", op_short, ".png")),
           p1, width = 9, height = 5, dpi = 150)

    # --- Plot 2: Main effects ---
    means_depth <- op_df %>%
      group_by(Depth) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")
    means_width <- op_df %>%
      group_by(Width) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")
    means_content <- op_df %>%
      group_by(Content) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

    p_d <- ggplot(means_depth, aes(x = Depth, y = MeanEnergy)) +
      geom_col(width = 0.6, fill = "#607D8B") +
      labs(x = "Depth", y = "Mean Energy (uJ/op)", title = "Depth") +
      theme_minimal(base_size = 10) +
      theme(plot.title = element_text(face = "bold", hjust = 0.5))

    p_w <- ggplot(means_width, aes(x = Width, y = MeanEnergy)) +
      geom_col(width = 0.6, fill = "#607D8B") +
      labs(x = "Width", y = NULL, title = "Width") +
      theme_minimal(base_size = 10) +
      theme(plot.title = element_text(face = "bold", hjust = 0.5))

    p_c <- ggplot(means_content, aes(x = Content, y = MeanEnergy)) +
      geom_col(width = 0.6, fill = "#607D8B") +
      labs(x = "Content", y = NULL, title = "Content") +
      theme_minimal(base_size = 10) +
      theme(plot.title = element_text(face = "bold", hjust = 0.5))

    p2 <- patchwork::wrap_plots(p_d, p_w, p_c, nrow = 1) +
      patchwork::plot_annotation(
        title = paste0(lib_name, " - ", op_label, " - Main Effects"),
        subtitle = "Mean energy collapsed across other factors"
      )

    ggsave(file.path(lib_dir, paste0("main_effects_", op_short, ".png")),
           p2, width = 12, height = 4.5, dpi = 150)

    # --- Plot 3: Interaction - Depth x Width ---
    int_dw <- op_df %>%
      group_by(Depth, Width) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

    p3 <- ggplot(int_dw, aes(x = Width, y = MeanEnergy,
                              color = Depth, group = Depth)) +
      geom_line(linewidth = 1) +
      geom_point(size = 2.5) +
      labs(
        title = paste0(lib_name, " - ", op_label, " - Depth x Width Interaction"),
        subtitle = "Non-parallel lines indicate interaction",
        x = "Width", y = "Mean Energy (uJ/op)", color = "Depth"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        plot.title = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40"),
        legend.position = "bottom"
      )

    ggsave(file.path(lib_dir, paste0("interaction_depth_width_", op_short, ".png")),
           p3, width = 8, height = 6, dpi = 150)

    # --- Plot 4: Interaction - Depth x Content ---
    int_dc <- op_df %>%
      group_by(Depth, Content) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

    p4 <- ggplot(int_dc, aes(x = Content, y = MeanEnergy,
                              color = Depth, group = Depth)) +
      geom_line(linewidth = 1) +
      geom_point(size = 2.5) +
      labs(
        title = paste0(lib_name, " - ", op_label, " - Depth x Content Interaction"),
        x = "Content Type", y = "Mean Energy (uJ/op)", color = "Depth"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        plot.title = element_text(size = 14, face = "bold"),
        legend.position = "bottom"
      )

    ggsave(file.path(lib_dir, paste0("interaction_depth_content_", op_short, ".png")),
           p4, width = 8, height = 6, dpi = 150)

    # --- Plot 5: Interaction - Width x Content ---
    int_wc <- op_df %>%
      group_by(Width, Content) %>%
      summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

    p5 <- ggplot(int_wc, aes(x = Content, y = MeanEnergy,
                              color = Width, group = Width)) +
      geom_line(linewidth = 1) +
      geom_point(size = 2.5) +
      labs(
        title = paste0(lib_name, " - ", op_label, " - Width x Content Interaction"),
        x = "Content Type", y = "Mean Energy (uJ/op)", color = "Width"
      ) +
      theme_minimal(base_size = 11) +
      theme(
        plot.title = element_text(size = 14, face = "bold"),
        legend.position = "bottom"
      )

    ggsave(file.path(lib_dir, paste0("interaction_width_content_", op_short, ".png")),
           p5, width = 8, height = 6, dpi = 150)

    cat("Saved all plots for", lib_name, op_label, "\n")
  }
}

for (lib in c("SpanJson", "Utf8Json", "Jil", "STJ", "Newtonsoft")) {
  run_per_library(lib)
}

cat("\nDone!\n")
