#!/usr/bin/env bash
set -euo pipefail

umask 077
secret_file="$(mktemp)"
trap 'rm -f "$secret_file"' EXIT
cat > "$secret_file"
if [[ ! -s "$secret_file" ]]; then
  echo "DB connection string is empty" >&2
  exit 1
fi

sudo k3s kubectl -n todo create secret generic todo-db \
  --from-file="connectionString=$secret_file" \
  --dry-run=client -o yaml | sudo k3s kubectl apply -f -
