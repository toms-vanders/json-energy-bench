library(tidyverse)
library(car)

# --- Load data ---
args <- commandArgs(trailingOnly = FALSE)
script_path <- sub("--file=", "", args[grep("--file=", args)])
script_dir <- if (length(script_path) > 0) dirname(script_path) else "."
csv_path <- file.path(script_dir, "..", "BenchmarkArtifacts", "results",
                      "JsonBench.Benchmarks.Factorial.FactorialStringBench-measurements.csv")
plot_dir <- file.path(script_dir, "plots", "anova")
dir.create(plot_dir, showWarnings = FALSE, recursive = TRUE)
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

# Library colors
lib_colors <- c(
  "SpanJson"   = "#2196F3",
  "Utf8Json"   = "#4CAF50",
  "Jil"        = "#FF9800",
  "STJ"        = "#9C27B0",
  "Newtonsoft"  = "#F44336"
)

# ============================================================
# Run ANOVA per operation
# ============================================================
run_anova <- function(op_label) {
  cat("\n========================================\n")
  cat(op_label, "- 4-Factor ANOVA\n")
  cat("========================================\n\n")

  op_df <- df %>% filter(Operation == op_label)

  # Fit full factorial ANOVA (Type II for unbalanced robustness)
  model <- lm(EnergyPerOp ~ Library * Depth * Width * Content, data = op_df)
  aov_result <- Anova(model, type = 2)

  # Compute eta-squared (SS_effect / SS_total)
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

  # Print table
  print(aov_table %>% mutate(across(where(is.numeric), ~ round(., 4))), n = 30)

  # Save table to CSV
  write_csv(aov_table, file.path(plot_dir,
            paste0("anova_table_", tolower(gsub("ialize", "", op_label)), ".csv")))

  # --- Plot 1: Effect size bar chart (eta-squared) ---
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
      title = paste0(op_label, " - Variance Explained by Each Factor"),
      subtitle = "Eta-squared (%) from 4-factor ANOVA | Blue = significant (p < 0.05)",
      x = NULL,
      y = "Variance Explained (%)",
      fill = NULL
    ) +
    theme_minimal(base_size = 11) +
    theme(
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      legend.position = "bottom"
    )

  ggsave(file.path(plot_dir, paste0("eta_squared_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p1, width = 10, height = 6, dpi = 150)
  cat("Saved: eta_squared plot\n")

  # --- Plot 2: Main effects ---
  means_lib <- op_df %>%
    group_by(Library) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")
  means_depth <- op_df %>%
    group_by(Depth) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")
  means_width <- op_df %>%
    group_by(Width) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")
  means_content <- op_df %>%
    group_by(Content) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

  p_lib <- ggplot(means_lib, aes(x = Library, y = MeanEnergy, fill = Library)) +
    geom_col(width = 0.6) +
    scale_fill_manual(values = lib_colors) +
    labs(x = NULL, y = "Mean Energy (uJ/op)", title = "Library") +
    theme_minimal(base_size = 10) +
    theme(legend.position = "none",
          axis.text.x = element_text(angle = 30, hjust = 1),
          plot.title = element_text(face = "bold", hjust = 0.5))

  p_depth <- ggplot(means_depth, aes(x = Depth, y = MeanEnergy)) +
    geom_col(width = 0.6, fill = "#607D8B") +
    labs(x = NULL, y = NULL, title = "Depth") +
    theme_minimal(base_size = 10) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))

  p_width <- ggplot(means_width, aes(x = Width, y = MeanEnergy)) +
    geom_col(width = 0.6, fill = "#607D8B") +
    labs(x = NULL, y = NULL, title = "Width") +
    theme_minimal(base_size = 10) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))

  p_content <- ggplot(means_content, aes(x = Content, y = MeanEnergy)) +
    geom_col(width = 0.6, fill = "#607D8B") +
    labs(x = NULL, y = NULL, title = "Content") +
    theme_minimal(base_size = 10) +
    theme(plot.title = element_text(face = "bold", hjust = 0.5))

  p2 <- patchwork::wrap_plots(p_lib, p_depth, p_width, p_content, nrow = 1) +
    patchwork::plot_annotation(
      title = paste0(op_label, " - Main Effects"),
      subtitle = "Mean energy collapsed across all other factors",
      theme = theme(
        plot.title = element_text(size = 14, face = "bold"),
        plot.subtitle = element_text(size = 10, color = "grey40")
      )
    )

  ggsave(file.path(plot_dir, paste0("main_effects_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p2, width = 14, height = 5, dpi = 150)
  cat("Saved: main_effects plot\n")

  # --- Plot 3: Interaction - Library x Width ---
  int_lw <- op_df %>%
    group_by(Library, Width) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

  p3 <- ggplot(int_lw, aes(x = Width, y = MeanEnergy,
                            color = Library, group = Library)) +
    geom_line(linewidth = 1) +
    geom_point(size = 2.5) +
    scale_color_manual(values = lib_colors) +
    labs(
      title = paste0(op_label, " - Library x Width Interaction"),
      subtitle = "Non-parallel lines indicate interaction (rankings change with width)",
      x = "Width", y = "Mean Energy (uJ/op)", color = "Library"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      legend.position = "bottom"
    )

  ggsave(file.path(plot_dir, paste0("interaction_lib_width_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p3, width = 8, height = 6, dpi = 150)
  cat("Saved: interaction_lib_width plot\n")

  # --- Plot 4: Interaction - Library x Depth ---
  int_ld <- op_df %>%
    group_by(Library, Depth) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

  p4 <- ggplot(int_ld, aes(x = Depth, y = MeanEnergy,
                            color = Library, group = Library)) +
    geom_line(linewidth = 1) +
    geom_point(size = 2.5) +
    scale_color_manual(values = lib_colors) +
    labs(
      title = paste0(op_label, " - Library x Depth Interaction"),
      subtitle = "Non-parallel lines indicate interaction (rankings change with depth)",
      x = "Depth", y = "Mean Energy (uJ/op)", color = "Library"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      legend.position = "bottom"
    )

  ggsave(file.path(plot_dir, paste0("interaction_lib_depth_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p4, width = 8, height = 6, dpi = 150)
  cat("Saved: interaction_lib_depth plot\n")

  # --- Plot 5: Interaction - Library x Content ---
  int_lc <- op_df %>%
    group_by(Library, Content) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

  p5 <- ggplot(int_lc, aes(x = Content, y = MeanEnergy,
                            color = Library, group = Library)) +
    geom_line(linewidth = 1) +
    geom_point(size = 2.5) +
    scale_color_manual(values = lib_colors) +
    labs(
      title = paste0(op_label, " - Library x Content Interaction"),
      subtitle = "Non-parallel lines indicate interaction (rankings change with content type)",
      x = "Content Type", y = "Mean Energy (uJ/op)", color = "Library"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      legend.position = "bottom"
    )

  ggsave(file.path(plot_dir, paste0("interaction_lib_content_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p5, width = 8, height = 6, dpi = 150)
  cat("Saved: interaction_lib_content plot\n")

  # --- Plot 6: Interaction - Depth x Width (faceted by Library) ---
  int_dw <- op_df %>%
    group_by(Library, Depth, Width) %>%
    summarise(MeanEnergy = mean(EnergyPerOp), .groups = "drop")

  p6 <- ggplot(int_dw, aes(x = Width, y = MeanEnergy,
                            color = Depth, group = Depth)) +
    geom_line(linewidth = 0.9) +
    geom_point(size = 2) +
    facet_wrap(~ Library, scales = "free_y", nrow = 1) +
    labs(
      title = paste0(op_label, " - Depth x Width Interaction per Library"),
      subtitle = "Non-parallel lines = depth and width interact differently per library",
      x = "Width", y = "Mean Energy (uJ/op)", color = "Depth"
    ) +
    theme_minimal(base_size = 11) +
    theme(
      strip.text = element_text(face = "bold", size = 10),
      plot.title = element_text(size = 14, face = "bold"),
      plot.subtitle = element_text(size = 10, color = "grey40"),
      legend.position = "bottom"
    )

  ggsave(file.path(plot_dir, paste0("interaction_depth_width_",
         tolower(gsub("ialize", "", op_label)), ".png")),
         p6, width = 16, height = 5, dpi = 150)
  cat("Saved: interaction_depth_width plot\n")
}

run_anova("Deserialize")
run_anova("Serialize")

cat("\nDone!\n")
