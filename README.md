# Workload-Sensitive Energy Benchmarking of .NET JSON Libraries

Master's thesis project (MSc Computer Science, Aalborg University, Spring 2026) measuring the energy consumption of .NET JSON libraries during serialisation and deserialisation, and how energy rankings shift with the shape of the JSON workload.

📄 **Full report:** [Workload-Sensitive Energy Benchmarking of .NET JSON Libraries (PDF)](Workload_Sensitive_Energy_Benchmarking_of__NET_JSON_Libraries.pdf)

## Overview

Five library configurations are benchmarked:

| Configuration | Library |
|---|---|
| `STJRefGen` | System.Text.Json, reflection-based |
| `STJSrcGen` | System.Text.Json, source generator |
| `Newtonsoft` | Newtonsoft.Json 13.0.4 |
| `SpanJson` | SpanJson 4.2.1 |
| `Utf8Json` | Utf8Json 1.3.7 |

Energy is measured with Intel RAPL (CPU Package + DRAM domains) through a forked BenchmarkDotNet, on a controlled machine (SMT, Turbo Boost, and C-states disabled; fixed CPU frequency; benchmark pinned to a dedicated core). A deterministic JSON generator produces synthetic workloads that vary one dimension at a time:

- **Size** — top-level object count (10 – 100 000)
- **Depth** — nesting depth (1 – 40)
- **Width** — properties per object (2 – 200)
- **String length**, **numeric value length** (int & float), **string composition** (ASCII / Unicode / escape density), **redundancy**
- plus a size-normalised **factorial** Depth × Width × Content suite (48 configurations)

An exploratory part repeats a subset of the suite under `stress-ng` background load, using the Metrion energy-measurement tool for process-level attribution instead of machine-wide RAPL readings.

### Key findings

- Energy rankings are **workload-dependent**: no library is best everywhere, and rankings change across Width, String Length, Numeric Length, and String Composition.
- In general, `SpanJson` uses the least energy for deserialisation, `STJSrcGen` the least for serialisation, and `Newtonsoft.Json` usually the most.
- System.Text.Json source generation mainly benefits **serialisation** (generated write handlers); both variants share the same steady-state read path.
- Under the tested noisy conditions, Metrion largely preserved the rankings measured by the RAPL diagnoser (exploratory result).

## Repository layout

| Path | Contents |
|---|---|
| `JsonBench/` | BenchmarkDotNet suites: smoke test, seven per-dimension isolation sweeps, factorial grid |
| `JsonGenerator/` | Deterministic synthetic JSON generator (one workload dimension varied at a time) |
| `JsonBench.Tests/` | Round-trip tests validating the generator output and per-library serialisers |
| `BenchmarkDotNet.Energy/` | Git submodule — [BenchmarkDotNet fork](https://github.com/geovoda/BenchmarkDotNet.Energy) adding a RAPL `EnergyDiagnoser` (Package / Core / Uncore / DRAM / Psys via `/sys/class/powercap`, multi-socket aware) and a `MetrionEnergyProfiler` |
| `scripts/clean-env/` | Apply / verify / revert the clean-environment controls |
| `scripts/runs/` | Repeated-run drivers for the Metrion validation and environmental-effects experiments |
| `analysis/` | R pipeline producing the plots, tables, and statistics used in the report ([own README](analysis/README.md)) |
| `TestData/` | Generated JSON workloads |
| `BenchmarkArtifacts/` | Benchmark results consumed by the analysis scripts |

## Running the benchmarks

Requirements: Linux on an Intel CPU with RAPL (`/sys/class/powercap/intel-rapl`), .NET 10 SDK. Reading the RAPL `energy_uj` counters typically requires root.

```bash
git clone --recurse-submodules https://github.com/toms-vanders/json-energy-bench.git
cd json-energy-bench/JsonBench

dotnet run -c Release -- generate            # (re)generate TestData workloads
dotnet run -c Release -- --list flat         # list all benchmarks
dotnet run -c Release -- --filter '*Smoke*'  # run a suite by filter, e.g. *DepthIsolation*
```

For measurement-grade runs, apply the environment controls first (non-persistent, reapply per session):

```bash
sudo scripts/clean-env/clean-env-up.sh
scripts/clean-env/clean-env-check.sh
```

Results land in `BenchmarkArtifacts/`; `analysis/isolation_analysis.R` and `analysis/factorial_analysis.R` turn the measurement CSVs into figures and tables.

## Authors

Georgian Voda and Toms Vanders, supervised by Bent Thomsen and Søren Kejser Jensen — Department of Computer Science, Aalborg University.
