#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

ALL=(
  normality_analysis
  kruskal_wallis_library
  cliffs_delta
  time_energy_correlation
  factorial_normalized
  isolation_analysis
)

usage() {
  cat <<EOF
Usage: [BENCH_VARIANT=Byte|String] $(basename "$0") [script ...]

Runs R analysis scripts. With no args, runs all. Names accepted with or
without .R suffix. BENCH_VARIANT is forwarded to each script (default: Byte).

Available:
$(printf '  %s\n' "${ALL[@]}")
EOF
}

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
  usage
  exit 0
fi

if [[ $# -eq 0 ]]; then
  TO_RUN=("${ALL[@]}")
else
  TO_RUN=()
  for arg in "$@"; do
    name="${arg%.R}"
    if [[ ! " ${ALL[*]} " =~ " ${name} " ]]; then
      echo "Unknown script: $arg" >&2
      usage >&2
      exit 2
    fi
    TO_RUN+=("$name")
  done
fi

if [[ -n "${BENCH_VARIANT:-}" ]]; then
  variant="$BENCH_VARIANT"
else
  echo "Select variant:"
  echo "  1) Byte  (default)"
  echo "  2) String"
  read -rp "> " choice
  case "${choice:-1}" in
    1|b|B|byte|Byte|"")   variant="Byte" ;;
    2|s|S|string|String)  variant="String" ;;
    *) echo "Invalid choice: $choice" >&2; exit 2 ;;
  esac
fi
echo "BENCH_VARIANT=$variant"

for name in "${TO_RUN[@]}"; do
  echo
  echo "===== $name ====="
  BENCH_VARIANT="$variant" Rscript "$name.R"
done
