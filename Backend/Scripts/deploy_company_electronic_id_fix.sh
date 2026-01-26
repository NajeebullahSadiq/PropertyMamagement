#!/bin/bash

# Deploy Company Electronic ID Column Type Fix
# This script fixes the ElectronicNationalIdNumber column type in Company module tables

echo "=========================================="
echo "Company Electronic ID Column Type Fix"
echo "=========================================="
echo ""

# Database connection details
DB_NAME="prmis"
DB_USER="postgres"
DB_HOST="localhost"

echo "This script will:"
echo "1. Convert ElectronicNationalIdNumber from double precision to VARCHAR(50) in CompanyOwner table"
echo "2. Convert ElectronicNationalIdNumber from double precision to VARCHAR(50) in Guarantor table"
echo ""
echo "Database: $DB_NAME"
echo "User: $DB_USER"
echo "Host: $DB_HOST"
echo ""
read -p "Do you want to continue? (yes/no): " confirm

if [ "$confirm" != "yes" ]; then
    echo "Deployment cancelled."
    exit 0
fi

echo ""
echo "Applying fix..."
psql -h $DB_HOST -U $DB_USER -d $DB_NAME -f fix_company_electronic_id_columns.sql

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "Fix applied successfully!"
    echo "=========================================="
    echo ""
    echo "Please restart the backend service:"
    echo "  sudo systemctl restart prmis-backend"
    echo ""
else
    echo ""
    echo "=========================================="
    echo "ERROR: Fix failed!"
    echo "=========================================="
    echo "Please check the error messages above."
    exit 1
fi
