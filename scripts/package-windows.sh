#!/usr/bin/env bash
#
# Builds a self-contained Windows package for Boligmappa.
# The target Windows machine needs NOTHING installed (no .NET, Node, or IDE) —
# just copy the resulting publish/<rid> folder over and run start.bat.
#
# Requirements on THIS (build) machine: .NET 8 SDK + Node/npm.
# Usage: scripts/package-windows.sh [runtime-id]   (default: win-x64)

set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
RID="${1:-win-x64}"
OUT="$ROOT/publish/$RID"

echo "==> 1/4 Building frontend (production)"
( cd "$ROOT/frontend" && npm ci && npm run build )

echo "==> 2/4 Copying frontend into API wwwroot"
WWW="$ROOT/backend/Boligmappa.Api/wwwroot"
rm -rf "$WWW"; mkdir -p "$WWW"; cp -r "$ROOT/frontend/dist/." "$WWW/"

echo "==> 3/4 Publishing self-contained $RID (single file + bundled runtime)"
rm -rf "$OUT"
dotnet publish "$ROOT/backend/Boligmappa.Api/Boligmappa.Api.csproj" \
  -c Release -r "$RID" --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o "$OUT"

echo "==> 4/4 Writing start.bat launcher"
cat > "$OUT/start.bat" <<'BAT'
@echo off
echo Boligmappa kjorer paa http://localhost:5080
echo Lukk dette vinduet (eller trykk Ctrl+C) for aa stoppe.
start "" http://localhost:5080
Boligmappa.Api.exe
BAT

echo
echo "Done. Deliverable folder: $OUT"
echo "Copy it to Windows and double-click start.bat (or Boligmappa.Api.exe)."
