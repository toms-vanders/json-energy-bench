#!/usr/bin/env bash

set -euo pipefail

SCRIPT_NAME="$(basename "$0")"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../../../.." && pwd)"

usage() {
    cat <<EOF
Usage:
  Foreground:
    bash $SCRIPT_NAME <runs>

  Detached (survives SSH disconnect):
    sudo nohup bash $SCRIPT_DIR/$SCRIPT_NAME <runs> > $REPO_ROOT/logs/energy-diagnoser-clean-env-runs-smoke-\$(date '+%Y%m%d_%H%M%S').log 2>&1 &
    echo "PID: \$!"
EOF
}

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
    usage
    exit 0
fi

if [[ $# -lt 1 || ! "${1}" =~ ^[1-9][0-9]*$ ]]; then
    echo "ERROR: runs must be a positive integer." >&2
    usage
    exit 1
fi

# ── Paths ──────────────────────────────────────────────────────────────────────
BENCH_CMD="dotnet run -c Release -f net10.0 --project $REPO_ROOT/JsonBench/JsonBench.csproj -- --filter '*SmokeBenchByte*'"
RESULTS_SRC="$REPO_ROOT/BenchmarkArtifacts"
ARCHIVE_BASE="$REPO_ROOT/BenchmarkArtifactsArchive/Smoke"
LOG_FILE="$REPO_ROOT/logs/energy-diagnoser-clean-env-runs-smoke-$(date '+%Y%m%d_%H%M%S').log"

# ── Experiment parameters ──────────────────────────────────────────────────────
RUNS="$1"

# ── Helpers ───────────────────────────────────────────────────────────────────
log() { echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*" | tee -a "$LOG_FILE"; }

# ── Sanity checks ─────────────────────────────────────────────────────────────
mkdir -p "$ARCHIVE_BASE" "$(dirname "$LOG_FILE")"
command -v dotnet >/dev/null 2>&1 || { log "ERROR: dotnet not found in PATH"; exit 1; }

# ── Main experiment loop ───────────────────────────────────────────────────────
log "====== Experiment start ======"

for RUN in $(seq 1 "$RUNS"); do
    RUN_PADDED=$(printf "%02d" "$RUN")
    DEST_NAME="BenchmarkArtifacts_CleanEnv_EnergyDiagnoser_Run${RUN_PADDED}"
    DEST_PATH="$ARCHIVE_BASE/$DEST_NAME"

    log "── Run=${RUN_PADDED}/${RUNS} ──"

    # ── 1. Clean up any leftover results from a previous failed run ────────
    if [[ -d "$RESULTS_SRC" ]]; then
        log "WARNING: $RESULTS_SRC already exists – removing before run."
        rm -rf "$RESULTS_SRC"
    fi

    # ── 2. Run the benchmark (blocks until finished) ──────────────────────
    log "Starting benchmark..."
    eval "$BENCH_CMD"
    BENCH_EXIT=$?

    if [[ $BENCH_EXIT -ne 0 ]]; then
        log "ERROR: dotnet benchmark exited with code $BENCH_EXIT – aborting run."
        exit "$BENCH_EXIT"
    fi
    log "Benchmark finished (exit 0)."

    # ── 3. Archive results ────────────────────────────────────────────────
    if [[ ! -d "$RESULTS_SRC" ]]; then
        log "ERROR: Expected results directory $RESULTS_SRC not found after run."
        exit 1
    fi

    log "Archiving results → $DEST_PATH"
    mv "$RESULTS_SRC" "$DEST_PATH"
    log "Archive complete."

    # ── 4. Brief cooldown between runs ────────────────────────────────────
    if [[ $RUN -lt $RUNS ]]; then
        log "Cooling down for 10 s before next run..."
        sleep 10
    fi
done

log "====== Experiment complete. All results in $ARCHIVE_BASE ======"