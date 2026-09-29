#!/usr/bin/env bash
set -euo pipefail

release_sha="${1:?Commit SHA is required}"
if [[ ! "$release_sha" =~ ^[0-9a-f]{40}$ ]]; then
  echo "Expected a full commit SHA" >&2
  exit 1
fi

cd "$(dirname "$0")"

worker_names="$(sudo k3s kubectl get nodes -o json | python3 -c '
import json
import sys

nodes = json.load(sys.stdin)["items"]
for ip in ("10.42.10.50", "10.42.11.84"):
    matching = [
        node for node in nodes
        if any(address["type"] == "InternalIP" and address["address"] == ip
               for address in node["status"]["addresses"])
    ]
    if len(matching) != 1:
        sys.exit(f"Expected one K3s node at {ip}, found {len(matching)}")
    node = matching[0]
    if not any(condition["type"] == "Ready" and condition["status"] == "True"
               for condition in node["status"]["conditions"]):
        sys.exit(f"K3s node at {ip} is not Ready")
    print(node["metadata"]["name"])
')"

while IFS= read -r node; do
  sudo k3s kubectl label node "$node" app-node=true --overwrite
done <<< "$worker_names"

sed "s/__IMAGE_TAG__/$release_sha/g" app.yaml | sudo k3s kubectl apply -f -
sudo k3s kubectl -n todo rollout status deployment/todo-backend --timeout=300s
sudo k3s kubectl -n todo rollout status deployment/todo-frontend --timeout=300s
