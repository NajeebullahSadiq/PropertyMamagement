#!/bin/bash
# PRMIS Backend Deployment Script
# This script preserves the Resources folder (uploaded files) during deployment

set -e

echo "=== PRMIS Backend Deployment ==="
echo "Starting deployment at $(date)"

# Configuration
BACKEND_DIR="/var/www/prmis/Backend"
DEPLOY_DIR="/var/www/prmis/backend"
RESOURCES_BACKUP="/tmp/prmis_resources_backup"

# Step 1: Pull latest code
echo ""
echo "Step 1: Pulling latest code..."
cd $BACKEND_DIR
git pull origin main

# Step 2: Build the application
echo ""
echo "Step 2: Building application..."
dotnet build WebAPIBackend.csproj --configuration Release

# Step 3: Publish the application
echo ""
echo "Step 3: Publishing application..."
dotnet publish WebAPIBackend.csproj --configuration Release --output ./publish

# Step 4: Backup Resources folder if it exists
echo ""
echo "Step 4: Backing up Resources folder..."
if [ -d "$DEPLOY_DIR/Resources" ]; then
    rm -rf $RESOURCES_BACKUP
    cp -r $DEPLOY_DIR/Resources $RESOURCES_BACKUP
    echo "Resources backed up to $RESOURCES_BACKUP"
else
    echo "No existing Resources folder to backup"
fi

# Step 5: Stop the service
echo ""
echo "Step 5: Stopping prmis-backend service..."
sudo systemctl stop prmis-backend

# Step 6: Clear old deployment (except Resources)
echo ""
echo "Step 6: Clearing old deployment..."
# Remove everything except Resources folder
find $DEPLOY_DIR -mindepth 1 -maxdepth 1 ! -name 'Resources' -exec rm -rf {} +

# Step 7: Copy new files
echo ""
echo "Step 7: Copying new files..."
cp -r ./publish/* $DEPLOY_DIR/

# Step 8: Restore Resources folder
echo ""
echo "Step 8: Restoring Resources folder..."
if [ -d "$RESOURCES_BACKUP" ]; then
    rm -rf $DEPLOY_DIR/Resources
    cp -r $RESOURCES_BACKUP $DEPLOY_DIR/Resources
    echo "Resources restored from backup"
elif [ ! -d "$DEPLOY_DIR/Resources" ]; then
    # Create empty Resources structure if it doesn't exist
    mkdir -p $DEPLOY_DIR/Resources/Documents/Property
    mkdir -p $DEPLOY_DIR/Resources/Documents/Vehicle
    mkdir -p $DEPLOY_DIR/Resources/Images
    echo "Created empty Resources folder structure"
fi

# Step 9: Set permissions
echo ""
echo "Step 9: Setting permissions..."
sudo chown -R www-data:www-data $DEPLOY_DIR
sudo chmod -R 755 $DEPLOY_DIR

# Step 10: Start the service
echo ""
echo "Step 10: Starting prmis-backend service..."
sudo systemctl start prmis-backend

# Step 11: Check status
echo ""
echo "Step 11: Checking service status..."
sleep 3
sudo systemctl status prmis-backend --no-pager

# Cleanup backup
rm -rf $RESOURCES_BACKUP

echo ""
echo "=== Deployment completed at $(date) ==="
