#!/bin/bash

# Deploy LicenseNumber Type Fix
# This fixes the 500 error on /api/CompanyDetails endpoint
# Root cause: LicenseNumber was double precision instead of TEXT

echo "=========================================="
echo "Deploying LicenseNumber Type Fix"
echo "=========================================="
echo ""

# Apply the fix
echo "Step 1: Converting LicenseNumber from double precision to TEXT..."
sudo -u postgres psql -d PRMIS -f Backend/Scripts/fix_license_number_type.sql

echo ""
echo "Step 2: Restarting backend service..."
sudo systemctl restart prmis-backend

echo ""
echo "Step 3: Waiting for service to start (5 seconds)..."
sleep 5

echo ""
echo "Step 4: Testing the endpoint..."
curl http://103.132.98.92/api/CompanyDetails

echo ""
echo ""
echo "=========================================="
echo "Deployment Complete!"
echo "=========================================="
echo ""
echo "If you see JSON data above (not 500/502 error), the fix worked!"
echo ""
