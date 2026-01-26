#!/bin/bash

# Company Owner Photo 404 Fix - Deployment Script
# This script deploys the frontend fix and copies Resources folder

set -e  # Exit on error

echo "=========================================="
echo "Company Owner Photo 404 Fix - Deployment"
echo "=========================================="
echo ""

# Step 1: Copy Resources folder
echo "Step 1: Copying Resources folder..."
echo "Checking source location..."
if [ -d ~/PropertyMamagement/Backend/Resources/Documents/Company ]; then
    echo "✓ Source folder exists"
    ls -la ~/PropertyMamagement/Backend/Resources/Documents/Company/ | head -5
    
    echo ""
    echo "Copying Resources to deployment location..."
    sudo cp -r ~/PropertyMamagement/Backend/Resources/* /var/www/prmis/backend/Resources/
    
    echo "Setting permissions..."
    sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
    sudo chmod -R 755 /var/www/prmis/backend/Resources
    
    echo "✓ Resources copied successfully"
    echo ""
    echo "Verifying deployment location..."
    ls -la /var/www/prmis/backend/Resources/Documents/Company/ | head -5
else
    echo "⚠ Warning: Source Resources folder not found"
    echo "Skipping Resources copy..."
fi

echo ""
echo "=========================================="

# Step 2: Deploy Frontend
echo "Step 2: Deploying Frontend..."
cd ~/PropertyMamagement/Frontend

echo "Pulling latest code..."
git pull origin main

echo "Building production bundle..."
npx ng build --configuration production

echo "Deploying to /var/www/prmis/frontend..."
sudo rm -rf /var/www/prmis/frontend/*
sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/

echo "Setting permissions..."
sudo chown -R www-data:www-data /var/www/prmis/frontend
sudo chmod -R 755 /var/www/prmis/frontend

echo "✓ Frontend deployed successfully"
echo ""
echo "=========================================="

# Step 3: Verification
echo "Step 3: Verification"
echo ""
echo "Checking deployed files..."
echo "Frontend main.*.js files:"
ls -lh /var/www/prmis/frontend/main*.js 2>/dev/null || echo "No main.js files found"

echo ""
echo "Sample photo files in Resources:"
ls -lh /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -3 || echo "No photo files found"

echo ""
echo "=========================================="
echo "✓ Deployment Complete!"
echo "=========================================="
echo ""
echo "IMPORTANT: Users must clear browser cache!"
echo "  - Press Ctrl+Shift+R (hard refresh)"
echo "  - Or Ctrl+F5"
echo "  - Or clear browser cache manually"
echo ""
echo "Test the fix:"
echo "  1. Clear browser cache"
echo "  2. Go to Company Owner form"
echo "  3. Check if photo loads correctly"
echo "  4. Try printing license"
echo ""
echo "Expected URL format:"
echo "  http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg"
echo ""
