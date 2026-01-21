# Activity Monitoring Module - Database Migration Guide

## Overview
This guide covers the database migration for the **Activity Monitoring of Real Estate Offices & Petition Writers** module.

## Files Included

### Migration Scripts
- `11_ActivityMonitoring_Initial.sql` - Main SQL migration script
- `deploy_activity_monitoring.sh` - Linux/Mac deployment script
- `deploy_activity_monitoring.bat` - Windows deployment script
- `rollback_activity_monitoring.sql` - Rollback script (removes all tables)

### Tables Created
1. **ActivityMonitoringRecords** - Main table (org schema)
2. **ActivityMonitoringComplaints** - Child table for complaints (org schema)
3. **ActivityMonitoringRealEstateViolations** - Child table for real estate violations (org schema)
4. **ActivityMonitoringPetitionWriterViolations** - Child table for petition writer violations (org schema)

## Prerequisites

- PostgreSQL 12 or higher
- Database user with CREATE TABLE permissions on `org` schema
- `psql` command-line tool installed (for deployment scripts)

## Deployment Methods

### Method 1: Using Deployment Script (Recommended)

#### Linux/Mac:
```bash
cd Backend/Scripts/Modules

# Set environment variables (optional)
export DB_HOST=localhost
export DB_PORT=5432
export DB_NAME=prmis_db
export DB_USER=postgres
export PGPASSWORD=your_password

# Make script executable
chmod +x deploy_activity_monitoring.sh

# Run deployment
./deploy_activity_monitoring.sh
```

#### Windows:
```cmd
cd Backend\Scripts\Modules

REM Set environment variables (optional)
set DB_HOST=localhost
set DB_PORT=5432
set DB_NAME=prmis_db
set DB_USER=postgres
set PGPASSWORD=your_password

REM Run deployment
deploy_activity_monitoring.bat
```

### Method 2: Using psql Directly

```bash
psql -h localhost -p 5432 -U postgres -d prmis_db -f 11_ActivityMonitoring_Initial.sql
```

### Method 3: Using Full Module Deployment

If deploying all modules:
```bash
cd Backend/Scripts/Modules
psql -h localhost -p 5432 -U postgres -d prmis_db -f deploy_all_modules.sql
```

### Method 4: Using Entity Framework (C#)

If using EF Core migrations:
```bash
cd Backend
dotnet ef database update
```

## Database Schema

### Main Table: ActivityMonitoringRecords

```sql
org.ActivityMonitoringRecords
├── Id (SERIAL PRIMARY KEY)
├── Financial Clearance Fields
│   ├── LicenseHolderName (VARCHAR(200) NOT NULL)
│   ├── TaxClearanceStatus (VARCHAR(100))
│   ├── TaxClearanceLetterNumber (VARCHAR(100))
│   ├── TaxClearanceDate (DATE)
│   └── PaidTaxAmount (DECIMAL(18,2))
├── Annual Report Fields
│   ├── ReportRegistrationDate (DATE)
│   ├── SaleDeedsCount (INTEGER)
│   ├── RentalDeedsCount (INTEGER)
│   ├── BaiUlWafaDeedsCount (INTEGER)
│   ├── VehicleTransactionDeedsCount (INTEGER)
│   ├── CancelledMixedTransactions (INTEGER)
│   ├── LostDeedsCount (INTEGER)
│   └── AnnualReportRemarks (VARCHAR(1000))
├── Inspection Fields
│   ├── InspectionDate (DATE)
│   ├── InspectedRealEstateOfficesCount (INTEGER)
│   ├── SealedOfficesCount (INTEGER)
│   ├── InspectedPetitionWritersCount (INTEGER)
│   └── ViolatingPetitionWritersCount (INTEGER)
└── Audit Fields
    ├── Status (BOOLEAN DEFAULT TRUE)
    ├── CreatedAt (TIMESTAMP)
    ├── CreatedBy (VARCHAR(50))
    ├── UpdatedAt (TIMESTAMP)
    └── UpdatedBy (VARCHAR(50))
```

### Child Tables

All child tables have:
- Foreign key to `ActivityMonitoringRecords.Id` with CASCADE DELETE
- Audit fields (CreatedAt, CreatedBy)
- Appropriate indexes for performance

## Indexes Created

### ActivityMonitoringRecords
- `IX_ActivityMonitoringRecords_LicenseHolderName`
- `IX_ActivityMonitoringRecords_TaxClearanceDate`
- `IX_ActivityMonitoringRecords_InspectionDate`
- `IX_ActivityMonitoringRecords_Status`
- `IX_ActivityMonitoringRecords_CreatedAt`

### ActivityMonitoringComplaints
- `IX_ActivityMonitoringComplaints_ActivityMonitoringRecordId`
- `IX_ActivityMonitoringComplaints_ComplaintSerialNumber`
- `IX_ActivityMonitoringComplaints_ComplainantName`
- `IX_ActivityMonitoringComplaints_ComplaintRegistrationDate`

### ActivityMonitoringRealEstateViolations
- `IX_ActivityMonitoringRealEstateViolations_ActivityMonitoringRecordId`
- `IX_ActivityMonitoringRealEstateViolations_ViolationSerialNumber`
- `IX_ActivityMonitoringRealEstateViolations_LicenseHolderName`
- `IX_ActivityMonitoringRealEstateViolations_ViolationDate`

### ActivityMonitoringPetitionWriterViolations
- `IX_ActivityMonitoringPetitionWriterViolations_ActivityMonitoringRecordId`
- `IX_ActivityMonitoringPetitionWriterViolations_ViolationSerialNumber`
- `IX_ActivityMonitoringPetitionWriterViolations_PetitionWriterName`
- `IX_ActivityMonitoringPetitionWriterViolations_ViolationDate`

## Verification

After deployment, verify the tables were created:

```sql
-- Check if tables exist
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name LIKE 'ActivityMonitoring%'
ORDER BY table_name;

-- Expected output:
-- ActivityMonitoringComplaints
-- ActivityMonitoringPetitionWriterViolations
-- ActivityMonitoringRecords
-- ActivityMonitoringRealEstateViolations

-- Check migration history
SELECT * FROM public."__EFMigrationsHistory" 
WHERE "MigrationId" = '20260121000001_ActivityMonitoring_Initial';
```

## Rollback

⚠️ **WARNING**: Rollback will delete all data in Activity Monitoring tables!

```bash
# Using psql
psql -h localhost -p 5432 -U postgres -d prmis_db -f rollback_activity_monitoring.sql

# Or manually
DROP TABLE IF EXISTS org."ActivityMonitoringPetitionWriterViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRealEstateViolations" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringComplaints" CASCADE;
DROP TABLE IF EXISTS org."ActivityMonitoringRecords" CASCADE;

DELETE FROM public."__EFMigrationsHistory" 
WHERE "MigrationId" = '20260121000001_ActivityMonitoring_Initial';
```

## Troubleshooting

### Issue: "relation already exists"
**Solution**: Tables already exist. This is safe - the script uses `IF NOT EXISTS`.

### Issue: "schema org does not exist"
**Solution**: Run the Shared module migration first:
```bash
psql -h localhost -p 5432 -U postgres -d prmis_db -f 01_Shared_Initial.sql
```

### Issue: "permission denied for schema org"
**Solution**: Grant permissions:
```sql
GRANT USAGE ON SCHEMA org TO your_user;
GRANT CREATE ON SCHEMA org TO your_user;
```

### Issue: Migration history conflict
**Solution**: Check existing migrations:
```sql
SELECT * FROM public."__EFMigrationsHistory" ORDER BY "MigrationId";
```

## Data Migration (if upgrading)

If you have existing data to migrate, create a data migration script:

```sql
-- Example: Migrate from old structure
INSERT INTO org."ActivityMonitoringRecords" (
    "LicenseHolderName",
    "TaxClearanceDate",
    "InspectionDate",
    "Status",
    "CreatedAt",
    "CreatedBy"
)
SELECT 
    old_license_holder,
    old_tax_date,
    old_inspection_date,
    TRUE,
    NOW(),
    'migration_script'
FROM old_monitoring_table
WHERE old_status = 'active';
```

## Performance Considerations

- All foreign keys have indexes for optimal JOIN performance
- Date fields are indexed for date-range queries
- Status field is indexed for filtering active records
- Consider partitioning if expecting > 1 million records

## Backup Recommendation

Before running migration on production:

```bash
# Backup entire database
pg_dump -h localhost -U postgres -d prmis_db -F c -f prmis_backup_$(date +%Y%m%d).dump

# Or backup just org schema
pg_dump -h localhost -U postgres -d prmis_db -n org -F c -f org_schema_backup_$(date +%Y%m%d).dump
```

## Support

For issues or questions:
1. Check the main migration README: `Backend/Infrastructure/Migrations/README.md`
2. Review migration execution order: `Backend/Infrastructure/Migrations/MIGRATION_EXECUTION_ORDER.md`
3. Check application logs for detailed error messages

## Migration Metadata

- **Migration ID**: 20260121000001_ActivityMonitoring_Initial
- **Module Number**: 11
- **Schema**: org
- **Dependencies**: None (standalone module)
- **EF Core Version**: 7.0.0
- **Date Created**: 2026-01-21

---

**Status**: ✅ Ready for deployment
**Tested**: PostgreSQL 12, 13, 14, 15
**Idempotent**: Yes (safe to run multiple times)
