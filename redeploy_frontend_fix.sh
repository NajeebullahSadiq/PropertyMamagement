#!/bin/bash

echo "=========================================="
echo "Redeploying Frontend with HTML Fix"
echo "=========================================="
echo ""

cd ~/PropertyMamagement/Frontend

echo "Pulling latest code..."
git pull origin main

echo ""
echo "Building production bundle..."
npx ng build --configuration production

echo ""
echo "Deploying to /var/www/prmis/frontend..."
sudo rm -rf /var/www/prmis/frontend/*
sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/

echo "Setting permissions..."
sudo chown -R www-data:www-data /var/www/prmis/frontend
sudo chmod -R 755 /var/www/prmis/frontend

echo ""
echo "✓ Frontend redeployed!"
echo ""
echo "New main.js file:"
ls -lh /var/www/prmis/frontend/main*.js

echo ""
echo "=========================================="
echo "CRITICAL: Clear Browser Cache!"
echo "=========================================="
echo ""
echo "The fix is now deployed, but you MUST clear cache:"
echo ""
echo "1. Close browser completely"
echo "2. Reopen browser"
echo "3. Press Ctrl+Shift+Delete"
echo "4. Select 'Cached images and files'"
echo "5. Select 'All time'"
echo "6. Click 'Clear data'"
echo "7. Test again"
echo ""
echo "OR test in Incognito/Private window first"
echo ""
