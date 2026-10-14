#!/usr/bin/env bash
# Lists the project files modified on a given day (default: today), e.g.  docs/changed-files.sh 2026-10-14
# Skips build output and vendored libraries. Run it from anywhere.
day="${1:-$(date +%F)}"
next="$(date -d "$day + 1 day" +%F)"
root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$root" || exit 1
find . -type f \
  -not -path '*/bin/*' -not -path '*/obj/*' -not -path './.git/*' \
  -not -path '*/wwwroot/lib/bootstrap/*' -not -path '*/wwwroot/lib/jquery*' \
  -newermt "$day 00:00:00" ! -newermt "$next 00:00:00" | sed 's|^\./||' | sort
