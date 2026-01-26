#!/bin/bash

echo "=========================================="
echo "Complete Photo Fix Script"
echo "=========================================="
echo ""

# Step 1: Copy Resources folder
echo "Step 1: Copying Resources folder..."
echo ""

if [ -d ~/PropertyMamagement/Backend/Resources ]; then
    echo "Source folder found. Copying..."
    sudo cp -r ~/PropertyMamagement/Backend/Resources/* /var/www/prmis/backend/Resources/
    sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
    sudo chmod -R 755 /var/www/prmis/backend/Resources
    echo "✓ Resources copied"
    
    # Verify
    photo_count=$(ls /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l)
    echo "  Photos in deployment: $photo_count"
else
    echo "⚠ Source Resources folder not found at ~/PropertyMamagement/Backend/Resources"
    echo "  Checking alternate location..."
    
    if [ -d ~/PropertyMamagement/Backend/wwwroot/Resources ]; then
        echo "  Found at wwwroot/Resources"
        sudo cp -r ~/PropertyMamagement/Backend/wwwroot/Resources/* /var/www/prmis/backend/Resources/
        sudo chown -R www-data:www-data /var/www/prmis/backend/Resources
        sudo chmod -R 755 /var/www/prmis/backend/Resources
        echo "✓ Resources copied from wwwroot"
    else
        echo "✗ Resources folder not found in either location"
        echo "  Please check where uploaded files are stored"
    fi
fi

echo ""
echo "=========================================="

# Step 2: Test file access
echo "Step 2: Testing file access..."
echo ""

sample_file=$(ls /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -1)
if [ -n "$sample_file" ]; then
    filename=$(basename "$sample_file")
    test_url="http://103.132.98.92/api/Resources/Documents/Company/$filename"
    echo "Testing URL: $test_url"
    
    http_code=$(curl -s -o /dev/null -w "%{http_code}" "$test_url" 2>/dev/null || echo "000")
    
    if [ "$http_code" = "200" ]; then
        echo "✓ HTTP 200 OK - Files are accessible!"
    else
        echo "✗ HTTP $http_code - Files NOT accessible"
        echo "  Backend may need restart..."
        echo "  Run: sudo systemctl restart prmis-backend"
    fi
else
    echo "⚠ No photo files found to test"
fi

echo ""
echo "=========================================="
echo "✓ COMPLETE!"
echo "=========================================="
echo ""
echo "CRITICAL NEXT STEP:"
echo ""
echo "You MUST clear your browser cache completely!"
echo ""
echo "Method 1 (Recommended):"
echo "  1. Press Ctrl+Shift+Delete"
echo "  2. Select 'Cached images and files'"
echo "  3. Select 'All time'"
echo "  4. Click 'Clear data'"
echo "  5. Close and reopen browser"
echo ""
echo "Method 2:"
echo "  1. Open DevTools (F12)"
echo "  2. Right-click the refresh button"
echo "  3. Select 'Empty Cache and Hard Reload'"
echo ""
echo "Method 3 (Nuclear option):"
echo "  - Close browser completely"
echo "  - Clear browser cache from settings"
echo "  - Reopen browser"
echo "  - Navigate to site"
echo ""
echo "After clearing cache, the URL should be:"
echo "  http://103.132.98.92/api/Resources/Documents/Company/profile_xxx.jpg"
echo ""
echo "NOT:"
echo "  http://103.132.98.92/api/Upload/view//api/Resources/..."
echo ""
