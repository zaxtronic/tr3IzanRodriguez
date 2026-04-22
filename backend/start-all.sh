#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$ROOT_DIR"

if [ ! -d node_modules ]; then
  echo "[start-all] Instalando dependencias..."
  npm install
fi

echo "[start-all] Levantando API (3001), GAME WS (3002) y GATEWAY (3000)..."

pids=()

cleanup() {
  echo
  echo "[start-all] Deteniendo servicios..."
  for pid in "${pids[@]:-}"; do
    if kill -0 "$pid" 2>/dev/null; then
      kill "$pid" 2>/dev/null || true
    fi
  done
  wait 2>/dev/null || true
  echo "[start-all] Servicios detenidos."
}

trap cleanup EXIT INT TERM

npm run start:api &
pids+=("$!")

npm run start:game &
pids+=("$!")

npm run start:gateway &
pids+=("$!")

echo "[start-all] OK. Presiona Ctrl+C para cerrar todo."

wait
