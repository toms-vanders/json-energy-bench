#!/usr/bin/env bash
# noisy-runs-smoke.sh
# Runs json-energy-bench SmokeBenchByte under various stress-ng CPU loads (0/25/50/75/100 %)
# and archives results after each run. At noise level 0, stress-ng is skipped entirely.
#
# Usage (foreground):
#   bash noisy-runs-smoke.sh
#
# Usage (detached – survives SSH disconnect):
#   sudo nohup bash scripts/runs/smoke/noisy-runs-smoke.sh > /home/test/json-energy-bench/logs/noisy-runs-smoke-$(date '+%Y%m%d_%H%M%S').log 2>&1 &
#   echo "PID: $!"

set -euo pipefail

SCRIPT_NAME="$(basename "$0")"

usage() {
    cat <<EOF
Usage:
  Foreground:
    bash $SCRIPT_NAME

  Detached (survives SSH disconnect):
    sudo nohup bash $SCRIPT_NAME > /home/test/json-energy-bench/logs/noisy-runs-smoke-\$(date '+%Y%m%d_%H%M%S').log 2>&1 &
    echo "PID: \$!"
EOF
}

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
    usage
    exit 0
fi

# ── Paths ──────────────────────────────────────────────────────────────────────
BENCH_CMD="dotnet run -c Release -f net10.0 --project /home/test/json-energy-bench/JsonBench/JsonBench.csproj -- --filter '*SmokeBenchByte*'"
RESULTS_SRC="/home/test/json-energy-bench/BenchmarkArtifacts"
ARCHIVE_BASE="/home/test/json-energy-bench/BenchmarkArtifactsArchive/Smoke"
LOG_FILE="/home/test/json-energy-bench/logs/noisy-runs-smoke-$(date '+%Y%m%d_%H%M%S').log"

# ── Experiment parameters ──────────────────────────────────────────────────────
NOISE_LEVELS=(0 25 50 75 100)   # stress-ng --cpu-load values; 0 = no stress-ng
RUNS_PER_LEVEL=5                # repetitions per noise level

# ── Helpers ───────────────────────────────────────────────────────────────────
log() { echo "[$(date '+%Y-%m-%d %H:%M:%S')] $*" | tee -a "$LOG_FILE"; }

cleanup_stress() {
    if kill -0 "${STRESS_PID:-}" 2>/dev/null; then
        log "Stopping stress-ng (PID $STRESS_PID)..."
        kill "$STRESS_PID" 2>/dev/null || true
        wait "$STRESS_PID" 2>/dev/null || true
        log "stress-ng stopped."
    fi
}

# Always stop stress-ng if the script exits for any reason
trap cleanup_stress EXIT

# ── Sanity checks ─────────────────────────────────────────────────────────────
mkdir -p "$ARCHIVE_BASE" "$(dirname "$LOG_FILE")"
command -v stress-ng >/dev/null 2>&1 || { log "ERROR: stress-ng not found in PATH"; exit 1; }
command -v dotnet    >/dev/null 2>&1 || { log "ERROR: dotnet not found in PATH";    exit 1; }

# ── Main experiment loop ───────────────────────────────────────────────────────
log "====== Experiment start ======"

for NOISE in "${NOISE_LEVELS[@]}"; do
    for RUN in $(seq 1 "$RUNS_PER_LEVEL"); do
        RUN_PADDED=$(printf "%02d" "$RUN")
        DEST_NAME="BenchmarkArtifacts_UncleanEnv${NOISE}_Metrion1000msRaw_ActualStage_Run${RUN_PADDED}"
        DEST_PATH="$ARCHIVE_BASE/$DEST_NAME"

        log "── Noise=${NOISE}%  Run=${RUN_PADDED}/${RUNS_PER_LEVEL} ──"

        # ── 1. Clean up any leftover results from a previous failed run ────────
        if [[ -d "$RESULTS_SRC" ]]; then
            log "WARNING: $RESULTS_SRC already exists – removing before run."
            rm -rf "$RESULTS_SRC"
        fi

        # ── 2. Optionally start stress-ng in the background ───────────────────
        STRESS_PID=""
        if [[ $NOISE -gt 0 ]]; then
            log "Starting stress-ng at ${NOISE}% CPU load..."
            stress-ng --cpu 0 --cpu-load "$NOISE" &
            STRESS_PID=$!
            log "stress-ng PID: $STRESS_PID"
            # Give stress-ng a moment to ramp up before the benchmark starts
            sleep 2
        else
            log "Noise level 0 – skipping stress-ng."
        fi

        # ── 3. Run the benchmark (blocks until finished) ──────────────────────
        log "Starting benchmark..."
        eval "$BENCH_CMD"
        BENCH_EXIT=$?

        if [[ $BENCH_EXIT -ne 0 ]]; then
            log "ERROR: dotnet benchmark exited with code $BENCH_EXIT – aborting run."
            cleanup_stress
            exit "$BENCH_EXIT"
        fi
        log "Benchmark finished (exit 0)."

        # ── 4. Stop stress-ng (no-op when STRESS_PID is empty) ───────────────
        cleanup_stress
        STRESS_PID=""          # prevent double-kill on next iteration

        # ── 5. Archive results ────────────────────────────────────────────────
        if [[ ! -d "$RESULTS_SRC" ]]; then
            log "ERROR: Expected results directory $RESULTS_SRC not found after run."
            exit 1
        fi

        log "Archiving results → $DEST_PATH"
        mv "$RESULTS_SRC" "$DEST_PATH"
        log "Archive complete."

        # ── 6. Brief cooldown between runs ────────────────────────────────────
        if [[ $RUN -lt $RUNS_PER_LEVEL || $NOISE -ne ${NOISE_LEVELS[-1]} ]]; then
            log "Cooling down for 10 s before next run..."
            sleep 10
        fi
    done
done

log "====== Experiment complete. All results in $ARCHIVE_BASE ======"