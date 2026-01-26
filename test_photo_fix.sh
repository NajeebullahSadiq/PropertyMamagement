#!/bin/bash

# Test script to verify photo fix deployment

echo "=========================================="
echo "Photo Fix Verification Test"
echo "=========================================="
echo ""

# Test 1: Check if Resources folder exists
echo "Test 1: Resources Folder"
if [ -d /var/www/prmis/backend/Resources/Documents/Company ]; then
    echo "✓ Resources folder exists"
    photo_count=$(ls /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | wc -l)
    echo "  Found $photo_count photo files"
    if [ $photo_count -gt 0 ]; then
        echo "  Sample files:"
        ls -lh /var/www/prmis/backend/Resources/Documents/Company/profile_*.jpg 2>/dev/null | head -3
    fi
else
    echo "✗ Resources folder NOT found"
    echo "  Run: sudo mkdir -p /var/www/prmis/backend/Resources/Documents/Company"
fi

echo ""

# Test 2: Check frontend deployment
echo "Test 2: Frontend Deployment"
if [ -f /var/www/prmis/frontend/index.html ]; then
    echo "✓ Frontend deployed"
    echo "  Main JS files:"
    ls -lh /var/www/prmis/frontend/main*.js 2>/dev/null | awk '{print "  " $9 " (" $5 ") - " $6 " " $7 " " $8}'
else
    echo "✗ Frontend NOT deployed"
fi

echo ""

# Test 3: Check database for photo paths
echo "Test 3: Database Photo Paths"
echo "Checking sample owner photos..."
sudo -u postgres psql -d PRMIS -c "
SELECT 
    \"Id\",
    \"FirstName\",
    \"FatherName\",
    \"PothoPath\",
    CASE 
        WHEN \"PothoPath\" LIKE 'Resources/%' THEN 'Has Resources prefix'
        ELSE 'Relative path'
    END as \"PathType\"
FROM org.\"CompanyOwner\"
WHERE \"PothoPath\" IS NOT NULL
LIMIT 5;
" 2>/dev/null || echo "Could not query database"

echo ""

# Test 4: Test direct file access
echo "Test 4: Direct File Access Test"
echo "Testing if a sample photo file is accessible..."

# Get a sample photo path from database
sample_path=$(sudo -u postgres psql -d PRMIS -t -c "
SELECT \"PothoPath\" 
FROM org.\"CompanyOwner\" 
WHERE \"PothoPath\" IS NOT NULL 
LIMIT 1;
" 2>/dev/null | xargs)

if [ -n "$sample_path" ]; then
    echo "Sample path from DB: $sample_path"
    
    # Construct full file path
    if [[ $sample_path == Resources/* ]]; then
        full_path="/var/www/prmis/backend/$sample_path"
    else
        full_path="/var/www/prmis/backend/Resources/$sample_path"
    fi
    
    echo "Full file path: $full_path"
    
    if [ -f "$full_path" ]; then
        echo "✓ File exists on server"
        ls -lh "$full_path"
        
        # Test URL
        if [[ $sample_path == Resources/* ]]; then
            test_url="http://103.132.98.92/api/$sample_path"
        else
            test_url="http://103.132.98.92/api/Resources/$sample_path"
        fi
        
        echo ""
        echo "Test URL: $test_url"
        echo "Testing HTTP access..."
        http_code=$(curl -s -o /dev/null -w "%{http_code}" "$test_url" 2>/dev/null || echo "000")
        
        if [ "$http_code" = "200" ]; then
            echo "✓ HTTP 200 OK - File is accessible"
        else
            echo "✗ HTTP $http_code - File NOT accessible"
        fi
    else
        echo "✗ File does NOT exist on server"
        echo "  Expected location: $full_path"
    fi
else
    echo "No photo paths found in database"
fi

echo ""
echo "=========================================="
echo "Verification Complete"
echo "=========================================="
echo ""
echo "Next Steps:"
echo "1. If Resources folder is missing, run: ./deploy_photo_fix.sh"
echo "2. If frontend is old, run: ./deploy_photo_fix.sh"
echo "3. Clear browser cache (Ctrl+Shift+R)"
echo "4. Test in browser"
echo ""
