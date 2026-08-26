#!/bin/bash
set -euo pipefail

# CaresynX API - manual VPS deployment helper.
# Prefer the GitHub Actions workflow in .github/workflows/deploy-api.yml.
#
# Required:
#   VPS_HOST=your.vps.ip.or.domain
#
# Optional:
#   VPS_USER=ubuntu
#   VPS_PORT=22
#   KEY_PATH=$HOME/.ssh/caresynx-api-key.pem
#   VPS_API_DIR=/opt/caresynx/publish
#   VPS_API_SERVICE=caresynx-api
#   VPS_SERVICE_USER=www-data
#   VPS_SERVICE_GROUP=www-data

VPS_HOST="${VPS_HOST:?Set VPS_HOST to your VPS IP address or hostname.}"
VPS_USER="${VPS_USER:-ubuntu}"
VPS_PORT="${VPS_PORT:-22}"
KEY_PATH="${KEY_PATH:-$HOME/.ssh/caresynx-api-key.pem}"
VPS_API_DIR="${VPS_API_DIR:-/opt/caresynx/publish}"
VPS_API_SERVICE="${VPS_API_SERVICE:-caresynx-api}"
VPS_SERVICE_USER="${VPS_SERVICE_USER:-www-data}"
VPS_SERVICE_GROUP="${VPS_SERVICE_GROUP:-www-data}"
PUBLISH_DIR="${PUBLISH_DIR:-./publish}"

echo "Publishing CaresynX API..."
dotnet publish Scheduler.API/Scheduler.API.csproj -c Release -o "$PUBLISH_DIR"

echo "Preparing $VPS_USER@$VPS_HOST:$VPS_API_DIR..."
ssh -p "$VPS_PORT" -i "$KEY_PATH" -o StrictHostKeyChecking=no "$VPS_USER@$VPS_HOST" \
  "set -e;
   sudo mkdir -p '$VPS_API_DIR';
   sudo chown -R '$VPS_USER:$VPS_USER' '$VPS_API_DIR';
   sudo systemctl stop '$VPS_API_SERVICE' || true"

echo "Uploading API publish output..."
rsync -az --delete --no-times --no-perms \
  --exclude='.env' \
  --exclude='appsettings.Production.json' \
  --exclude='wwwroot/FileStorage/' \
  -e "ssh -p $VPS_PORT -i $KEY_PATH -o StrictHostKeyChecking=no" \
  "$PUBLISH_DIR/" "$VPS_USER@$VPS_HOST:$VPS_API_DIR/"

echo "Restarting $VPS_API_SERVICE..."
ssh -p "$VPS_PORT" -i "$KEY_PATH" -o StrictHostKeyChecking=no "$VPS_USER@$VPS_HOST" \
  "set -e;
   sudo chown -R '$VPS_SERVICE_USER:$VPS_SERVICE_GROUP' '$VPS_API_DIR';
   sudo systemctl daemon-reload;
   sudo systemctl restart '$VPS_API_SERVICE';
   sudo systemctl --no-pager --full status '$VPS_API_SERVICE'"

echo "Deployment complete."
