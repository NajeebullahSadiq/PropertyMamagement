#!/bin/bash

echo "=========================================="
echo "Photo Issue Diagnostic"
echo "=========================================="
echo ""

# 1. Check Resources folder locations
echo "1. Checking Resources folder locations..."
echo ""
echo "Development location (~/PropertyMamagement/Backend/Resources/):"
if [ -d ~/PropertyMamagement/Backend/Resources/Documents/Company ]; then
    echo "✓ EXISTS"
    photo_count=$(ls ~/PropertyMamagement/Backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l)
    echo "  Photo count: $photo_count"
    if [ $photo_count -gt 0 ]; then
        echo "  Sample files:"
        ls -lh ~/PropertyMamagement/Backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -3
    fi
else
    echo "✗ NOT FOUND"
fi

echo ""
echo "Deployment location (/var/www/prmis/backend/Resources/):"
if [ -d /var/www/prmis/backend/Resources/Documents/Company ]; then
    echo "✓ EXISTS"
    photo_count=$(ls /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l)
    echo "  Photo count: $photo_count"
    if [ $photo_count -gt 0 ]; then
        echo "  Sample files:"
        ls -lh /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -3
    fi
else
    echo "✗ NOT FOUND - THIS IS THE PROBLEM!"
    echo "  Creating directory..."
    sudo mkdir -p /var/www/prmis/backend/Resources/Documents/Company
fi

echo ""
echo "=========================================="

# 2. Check database photo paths
echo "2. Checking database photo paths..."
echo ""
sudo -u postgres psql -d PRMIS -c "
SELECT 
    \"Id\",
    \"FirstName\",
    \"PothoPath\",
    CASE 
        WHEN \"PothoPath\" LIKE 'Resources/%' THEN 'Full path (Resources/...)'
        WHEN \"PothoPath\" LIKE '/Resources/%' THEN 'Full path (/Resources/...)'
        ELSE 'Relative path'
    END as \"PathType\"
FROM org.\"CompanyOwner\"
WHERE \"PothoPath\" IS NOT NULL
ORDER BY \"Id\" DESC
LIMIT 5;
"

echo ""
echo "=========================================="

# 3. Test specific photo file
echo "3. Testing specific photo file..."
echo ""
sample_path="Resources/Documents/Company/profile_20260126_074257_233.jpg"
echo "Testing: $sample_path"
echo ""

# Check in development
dev_path=~/PropertyMamagement/Backend/$sample_path
echo "Development path: $dev_path"
if [ -f "$dev_path" ]; then
    echo "✓ EXISTS in development"
    ls -lh "$dev_path"
else
    echo "✗ NOT FOUND in development"
fi

echo ""

# Check in deployment
deploy_path=/var/www/prmis/backend/$sample_path
echo "Deployment path: $deploy_path"
if [ -f "$deploy_path" ]; then
    echo "✓ EXISTS in deployment"
    ls -lh "$deploy_path"
else
    echo "✗ NOT FOUND in deployment - NEEDS COPY!"
fi

echo ""
echo "=========================================="

# 4. Check frontend deployment
echo "4. Checking frontend deployment..."
echo ""
if [ -f /var/www/prmis/frontend/main.34a8f1cb36dd2bc8.js ]; then
    echo "✓ Frontend deployed (main.34a8f1cb36dd2bc8.js)"
    ls -lh /var/www/prmis/frontend/main.34a8f1cb36dd2bc8.js
else
    echo "✗ Frontend file not found"
fi

echo ""
echo "=========================================="
echo "DIAGNOSIS COMPLETE"
echo "=========================================="
echo ""

# Provide recommendations
echo "RECOMMENDATIONS:"
echo ""

if [ ! -d /var/www/prmis/backend/Resources/Documents/Company ] || [ $(ls /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l) -eq 0 ]; then
    echo "❌ CRITICAL: Resources folder missing or empty in deployment"
    echo "   FIX: Run these commands:"
    echo "   sudo cp -r ~/PropertyMamagement/Backend/Resources/* /var/www/prmis/backend/Resources/"
    echo "   sudo chown -R www-data:www-data /var/www/prmis/backend/Resources"
    echo "   sudo chmod -R 755 /var/www/prmis/backend/Resources"
    echo ""
fi

echo "⚠️  BROWSER CACHE: You MUST clear browser cache!"
echo "   - Press Ctrl+Shift+Delete"
echo "   - Select 'Cached images and files'"
echo "   - Click 'Clear data'"
echo "   OR"
echo "   - Press Ctrl+Shift+R (hard refresh)"
echo ""
