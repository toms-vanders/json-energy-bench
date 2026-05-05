#!/usr/bin/env bash
# Revert benchmark clean-environment controls to Ubuntu defaults.
# Usage: sudo ./clean-env-down.sh

set -euo pipefail

if [ "$EUID" -ne 0 ]; then
  echo "ERROR: must run as root (sudo $0)" >&2
  exit 1
fi

echo "[1/5] Re-enabling SMT (Hyper-Threading)..."
echo on > /sys/devices/system/cpu/smt/control

echo "[2/5] Re-enabling Turbo Boost..."
echo 0 > /sys/devices/system/cpu/intel_pstate/no_turbo

echo "[3/5] Restoring frequency scaling (powersave, 800 MHz - 5 GHz)..."
cpupower frequency-set -g powersave >/dev/null
cpupower frequency-set -d 800MHz -u 5GHz >/dev/null

echo "[4/5] Re-enabling all C-states..."
cpupower idle-set -E >/dev/null

echo "[5/5] Restarting periodic system timers..."
systemctl start \
  apt-daily.timer apt-daily-upgrade.timer \
  motd-news.timer dpkg-db-backup.timer \
  fstrim.timer e2scrub_all.timer \
  systemd-tmpfiles-clean.timer

echo
echo "Defaults restored."
