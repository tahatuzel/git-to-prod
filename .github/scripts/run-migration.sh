#!/usr/bin/env bash
set -euo pipefail

release_sha="${1:?Commit SHA is required}"
if [[ ! "$release_sha" =~ ^[0-9a-f]{40}$ ]]; then
  echo "Expected a full commit SHA" >&2
  exit 1
fi

IFS= read -r ConnectionStrings__DefaultConnection
if [[ -z "$ConnectionStrings__DefaultConnection" ]]; then
  echo "DB connection string is empty" >&2
  exit 1
fi
export ConnectionStrings__DefaultConnection
export ASPNETCORE_ENVIRONMENT=Production

migration_dir="/home/ec2-user/.git-to-prod/migration/$release_sha"
cd "$migration_dir"
chmod 700 ./efbundle
./efbundle

cd /home/ec2-user
rm -f "$migration_dir/efbundle" "$migration_dir/appsettings.json" "$migration_dir/run-migration.sh"
rmdir "$migration_dir"
