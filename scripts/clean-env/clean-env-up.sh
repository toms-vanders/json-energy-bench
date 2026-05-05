#!/usr/bin/env bash
# Apply benchmark clean-environment controls.
# Settings are non-persistent; re-apply at the start of each session.
# Usage: sudo ./clean-env-up.sh

set -euo pipefail

if [ "$EUID" -ne 0 ]; then
  echo "ERROR: must run as root (sudo $0)" >&2
  exit 1
fi

echo "[1/5] Disabling SMT (Hyper-Threading)..."
echo off > /sys/devices/system/cpu/smt/control

echo "[2/5] Disabling Turbo Boost..."
echo 1 > /sys/devices/system/cpu/intel_pstate/no_turbo

echo "[3/5] Locking CPU frequency at 3.6 GHz (performance governor)..."
cpupower frequency-set -g performance >/dev/null
cpupower frequency-set -d 3.6GHz -u 3.6GHz >/dev/null

echo "[4/5] Disabling all C-states..."
cpupower idle-set -D 0 >/dev/null

echo "[5/5] Stopping periodic system timers..."
systemctl stop \
  apt-daily.timer apt-daily-upgrade.timer \
  motd-news.timer dpkg-db-backup.timer \
  fstrim.timer e2scrub_all.timer \
  systemd-tmpfiles-clean.timer

echo
echo "Clean environment applied."
echo "Run clean-env-check.sh to verify."
