#!/bin/bash

echo "=========================================="
echo "Nginx and Resources Fix"
echo "=========================================="
echo ""

# Step 1: Restart nginx to pick up frontend files
echo "Step 1: Restarting nginx..."
sudo systemctl restart nginx
sleep 2

if sudo systemctl is-active --quiet nginx; then
    echo "✓ Nginx is running"
else
    echo "✗ Nginx failed to start!"
    echo "Checking nginx configuration..."
    sudo nginx -t
    exit 1
fi

echo ""
echo "Step 2: Checking Resources folder location..."
echo ""

# Check where Resources should be according to nginx config
echo "According to nginx config, Resources should be at:"
echo "  /var/www/prmis/storage/Resources/"
echo ""

# Check if storage directory exists
if [ ! -d /var/www/prmis/storage ]; then
    echo "Creating storage directory..."
    sudo mkdir -p /var/www/prmis/storage
fi

# Check if Resources exist in backend location
if [ -d ~/PropertyMamagement/Backend/Resources ]; then
    echo "✓ Source Resources found at ~/PropertyMamagement/Backend/Resources"
    
    echo "Copying to /var/www/prmis/storage/Resources/..."
    sudo cp -r ~/PropertyMamagement/Backend/Resources /var/www/prmis/storage/
    sudo chown -R www-data:www-data /var/www/prmis/storage/Resources
    sudo chmod -R 755 /var/www/prmis/storage/Resources
    
    echo "✓ Resources copied to storage location"
    
    # Verify
    photo_count=$(ls /var/www/prmis/storage/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l)
    echo "  Photos in storage: $photo_count"
else
    echo "⚠ Source Resources not found"
fi

echo ""
echo "Step 3: Testing frontend access..."
http_code=$(curl -s -o /dev/null -w "%{http_code}" http://103.132.98.92/ 2>/dev/null || echo "000")

if [ "$http_code" = "200" ]; then
    echo "✓ Frontend is accessible (HTTP 200)"
else
    echo "✗ Frontend returned HTTP $http_code"
    echo ""
    echo "Checking nginx error log..."
    sudo tail -20 /var/log/nginx/prmis_error.log
fi

echo ""
echo "Step 4: Testing photo access..."
sample_file=$(ls /var/www/prmis/storage/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -1)
if [ -n "$sample_file" ]; then
    filename=$(basename "$sample_file")
    test_url="http://103.132.98.92/api/Resources/Documents/Company/$filename"
    echo "Testing: $test_url"
    
    http_code=$(curl -s -o /dev/null -w "%{http_code}" "$test_url" 2>/dev/null || echo "000")
    
    if [ "$http_code" = "200" ]; then
        echo "✓ Photos are accessible (HTTP 200)"
    else
        echo "✗ Photos returned HTTP $http_code"
    fi
else
    echo "⚠ No photo files found to test"
fi

echo ""
echo "=========================================="
echo "✓ Fix Complete!"
echo "=========================================="
echo ""
echo "Frontend should now be accessible at:"
echo "  http://103.132.98.92"
echo ""
echo "If still not working, check:"
echo "  sudo systemctl status nginx"
echo "  sudo tail -50 /var/log/nginx/prmis_error.log"
echo ""
