#!/bin/bash

# =====================================================
# Activity Monitoring Module Deployment Script
# =====================================================
# This script deploys only the Activity Monitoring module
# Safe for existing databases (uses IF NOT EXISTS)
# =====================================================

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Database connection parameters
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-prmis_db}"
DB_USER="${DB_USER:-postgres}"

# Script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo -e "${BLUE}=========================================="
echo "Activity Monitoring Module Deployment"
echo -e "==========================================${NC}"
echo ""
echo "Database: $DB_NAME"
echo "Host: $DB_HOST:$DB_PORT"
echo "User: $DB_USER"
echo ""

# Check if psql is available
if ! command -v psql &> /dev/null; then
    echo -e "${RED}Error: psql command not found. Please install PostgreSQL client.${NC}"
    exit 1
fi

# Prompt for password if not set
if [ -z "$PGPASSWORD" ]; then
    echo -e "${YELLOW}Enter database password:${NC}"
    read -s PGPASSWORD
    export PGPASSWORD
    echo ""
fi

# Test database connection
echo -e "${BLUE}Testing database connection...${NC}"
if ! psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "SELECT 1;" > /dev/null 2>&1; then
    echo -e "${RED}Error: Cannot connect to database.${NC}"
    echo "Please check your connection parameters and credentials."
    exit 1
fi
echo -e "${GREEN}✓ Database connection successful${NC}"
echo ""

# Deploy Activity Monitoring module
echo -e "${BLUE}Deploying Activity Monitoring module...${NC}"
if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$SCRIPT_DIR/11_ActivityMonitoring_Initial.sql"; then
    echo -e "${GREEN}✓ Activity Monitoring module deployed successfully${NC}"
else
    echo -e "${RED}✗ Error deploying Activity Monitoring module${NC}"
    exit 1
fi

echo ""
echo -e "${BLUE}=========================================="
echo "Deployment Summary"
echo -e "==========================================${NC}"
echo -e "${GREEN}✓ ActivityMonitoringRecords table created${NC}"
echo -e "${GREEN}✓ ActivityMonitoringComplaints table created${NC}"
echo -e "${GREEN}✓ ActivityMonitoringRealEstateViolations table created${NC}"
echo -e "${GREEN}✓ ActivityMonitoringPetitionWriterViolations table created${NC}"
echo -e "${GREEN}✓ All indexes and foreign keys created${NC}"
echo ""
echo -e "${GREEN}Deployment completed successfully!${NC}"

# Verify tables were created
echo ""
echo -e "${BLUE}Verifying table creation...${NC}"
TABLE_COUNT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -c "
    SELECT COUNT(*) 
    FROM information_schema.tables 
    WHERE table_schema = 'org' 
    AND table_name IN (
        'ActivityMonitoringRecords',
        'ActivityMonitoringComplaints',
        'ActivityMonitoringRealEstateViolations',
        'ActivityMonitoringPetitionWriterViolations'
    );
" | tr -d ' ')

if [ "$TABLE_COUNT" -eq "4" ]; then
    echo -e "${GREEN}✓ All 4 tables verified in database${NC}"
else
    echo -e "${YELLOW}⚠ Warning: Expected 4 tables, found $TABLE_COUNT${NC}"
fi

echo ""
echo -e "${GREEN}Done!${NC}"
