# Analysis

R pipeline that turns BenchmarkDotNet measurement CSVs into the plots, tables,
and statistical summaries used in the thesis. Two scripts, both run
independently; outputs land under `figures/`.

## How to run

```bash
Rscript analysis/isolation_analysis.R   # per-dimension sweeps + cross-dim aggregates
Rscript analysis/factorial_analysis.R   # 48-cell Depth × Width × Content grid
```

Source data is read from `../BenchmarkArtifacts/results/` (not in this folder).

## Layout

```
figures/
├── factorial/                 (descriptive plots + CSVs for the 48-cell grid)
│   ├── stats/                 (Shapiro-Wilk → Kruskal-Wallis → Cliff's δ)
│   │   ├── normality/
│   │   ├── kruskal_wallis/{library, per_library}/
│   │   └── cliffs_delta/{library, per_library/<lib>}/
│   └── tables/                (LaTeX: factorial_energy_summary, report_friendly)
└── isolation/
    ├── _aggregate/            (cross-dimension summary plots + stats)
    │   ├── cross_dim_*.png/.csv
    │   └── stats/             (Holm-adjusted KW, δ magnitude tiers, SW)
    └── <dim>/                 (one folder per isolation dimension)
        ├── *.png, *.csv       (scaling, rank-ratio, alloc/GC, local steps)
        ├── stats/             (same shape as factorial/stats/)
        └── tables/            (LaTeX: <dim>_endpoint_energy_time, report_friendly)
```

Per-dimension folders (`depth/`, `width/`, `size/`, `value_length/`, `numeric/`,
`string_composition/`, `redundancy/`) all share the same internal layout, so
once you know one you know all seven.

## Conventions

- **Colours.** Library identity uses the Okabe-Ito palette (colourblind-safe);
  diverging heatmaps use sky-blue → grey → vermilion; sequential rank heatmaps
  use the Wistia 5-stop palette. Defined at the top of each script.
- **Energy.** All "energy" columns are Package + DRAM, in µJ/op.
- **Statistical pipeline.** Shapiro-Wilk for normality → Kruskal-Wallis for
  significance → Cliff's δ for effect size. Romano (2006) tiers (N/S/M/L).
- **`stats/` layout.** Within any `stats/` folder, `library/` holds tests
  *across* libraries at fixed workload; `per_library/` holds tests *within*
  each library across workload levels.
