#!/usr/bin/env bash
# Verify benchmark clean-environment controls are applied.
# No sudo required. Exits 0 if all checks pass, 1 otherwise.
# Usage: ./clean-env-check.sh

fail=0

check() {
  local name="$1" expected="$2" actual="$3"
  if [ "$expected" = "$actual" ]; then
    printf "  OK   %-22s = %s\n" "$name" "$actual"
  else
    printf "  FAIL %-22s = %s (expected %s)\n" "$name" "$actual" "$expected"
    fail=1
  fi
}

echo "Clean-environment status:"

check "SMT active"            "0"           "$(cat /sys/devices/system/cpu/smt/active)"
check "Turbo disabled"        "1"           "$(cat /sys/devices/system/cpu/intel_pstate/no_turbo)"
check "Freq governor"         "performance" "$(cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_governor)"
check "Freq min (kHz)"        "3600000"     "$(cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_min_freq)"
check "Freq max (kHz)"        "3600000"     "$(cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_max_freq)"
check "C-state C1 disabled"   "1"           "$(cat /sys/devices/system/cpu/cpu0/cpuidle/state1/disable)"

for t in apt-daily.timer apt-daily-upgrade.timer motd-news.timer \
         dpkg-db-backup.timer fstrim.timer e2scrub_all.timer \
         systemd-tmpfiles-clean.timer; do
  state=$(systemctl is-active "$t" 2>/dev/null)
  if [ "$state" = "inactive" ] || [ "$state" = "failed" ] || [ -z "$state" ]; then
    printf "  OK   timer %-30s = stopped\n" "$t"
  else
    printf "  FAIL timer %-30s = %s\n" "$t" "$state"
    fail=1
  fi
done

echo
if [ "$fail" -eq 0 ]; then
  echo "All clean-env controls applied."
  exit 0
else
  echo "Some controls are NOT applied. Re-run clean-env-up.sh."
  exit 1
fi
