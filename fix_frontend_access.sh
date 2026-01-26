#!/bin/bash

echo "=========================================="
echo "Frontend Access Recovery"
echo "=========================================="
echo ""

# Check if frontend files exist
echo "Checking frontend deployment..."
if [ -f /var/www/prmis/frontend/index.html ]; then
    echo "✓ Frontend files exist"
    ls -lh /var/www/prmis/frontend/ | head -10
else
    echo "✗ Frontend files MISSING!"
    echo ""
    echo "Redeploying from build..."
    
    cd ~/PropertyMamagement/Frontend
    
    # Check if dist folder exists
    if [ -d dist/property-registeration-mis ]; then
        echo "✓ Build folder found, copying..."
        sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
        sudo chown -R www-data:www-data /var/www/prmis/frontend
        sudo chmod -R 755 /var/www/prmis/frontend
        echo "✓ Files copied"
    else
        echo "✗ Build folder not found, rebuilding..."
        npx ng build --configuration production
        sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/
        sudo chown -R www-data:www-data /var/www/prmis/frontend
        sudo chmod -R 755 /var/www/prmis/frontend
        echo "✓ Rebuilt and deployed"
    fi
fi

echo ""
echo "Verifying deployment..."
if [ -f /var/www/prmis/frontend/index.html ]; then
    echo "✓ index.html exists"
    file_count=$(ls /var/www/prmis/frontend/ | wc -l)
    echo "✓ Total files/folders: $file_count"
    
    echo ""
    echo "Main files:"
    ls -lh /var/www/prmis/frontend/*.js 2>/dev/null | head -5
    
    echo ""
    echo "✓ Frontend should now be accessible at:"
    echo "  http://103.132.98.92"
else
    echo "✗ Still missing files!"
    echo ""
    echo "Manual fix needed:"
    echo "  cd ~/PropertyMamagement/Frontend"
    echo "  npx ng build --configuration production"
    echo "  sudo cp -r dist/property-registeration-mis/* /var/www/prmis/frontend/"
    echo "  sudo chown -R www-data:www-data /var/www/prmis/frontend"
    echo "  sudo chmod -R 755 /var/www/prmis/frontend"
fi

echo ""
echo "=========================================="
