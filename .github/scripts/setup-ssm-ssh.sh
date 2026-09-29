#!/usr/bin/env bash
set -euo pipefail

: "${EC2_SSH_PRIVATE_KEY:?EC2_SSH_PRIVATE_KEY is required}"
: "${SSH_KNOWN_HOSTS:?SSH_KNOWN_HOSTS is required}"

aws --version
curl --fail --silent --show-error --location \
  https://s3.amazonaws.com/session-manager-downloads/plugin/latest/ubuntu_64bit/session-manager-plugin.deb \
  --output "$RUNNER_TEMP/session-manager-plugin.deb"
sudo dpkg --install "$RUNNER_TEMP/session-manager-plugin.deb"

mkdir -p "$HOME/.ssh"
chmod 700 "$HOME/.ssh"
printf '%s\n' "$EC2_SSH_PRIVATE_KEY" > "$HOME/.ssh/ec2_key"
printf '%s\n' "$SSH_KNOWN_HOSTS" > "$HOME/.ssh/known_hosts"
chmod 600 "$HOME/.ssh/ec2_key" "$HOME/.ssh/known_hosts"

cat > "$HOME/.ssh/config" <<'EOF'
Host i-*
    User ec2-user
    IdentityFile ~/.ssh/ec2_key
    IdentitiesOnly yes
    BatchMode yes
    StrictHostKeyChecking yes
    UserKnownHostsFile ~/.ssh/known_hosts
    ProxyCommand sh -c "aws ssm start-session --target %h --document-name AWS-StartSSHSession --parameters 'portNumber=%p'"
EOF
chmod 600 "$HOME/.ssh/config"
