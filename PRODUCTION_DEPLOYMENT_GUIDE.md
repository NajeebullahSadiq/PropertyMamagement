# Production Deployment Guide - Company Module

## Overview
This guide provides the exact steps to deploy the company module to production with the correct singular table names.

## Prerequisites
- PostgreSQL database server
- Access to production database
- `mainform_records.json` file with company data
- Backend application deployed

## Deployment Steps

### Step 1: Create Database Tables
Run the table creation script in your production PostgreSQL database:

```sql
Backend/Scripts/company_module_clean_recreate.sql
```

**What this does:**
- Drops any existing company tables (if re-running)
- Creates all 11 company transaction tables with **SINGULAR** names:
  - `org."CompanyDetail"`
  - `org."CompanyOwner"` ← **SINGULAR**
  - `org."CompanyOwnerAddress"` ← **SINGULAR**
  - `org."CompanyOwnerAddressHistory"` ← **SINGULAR**
  - `org."Guarantors"`
  - `org."Gaurantees"`
  - `org."LicenseDetails"`
  - `org."CompanyAccountInfo"`
  - `org."CompanyCancellationInfo"`
  - `org."Haqulemtyaz"`
  - `org."PeriodicForms"`
- Creates 5 audit tables in `log` schema
- Creates all necessary indexes and foreign keys

**Expected Output:**
```
✓ All company module tables dropped successfully
✓ All 11 company module tables created successfully
✓ All 5 company audit tables created successfully
✓ All performance indexes created successfully
========================================
Company Module Recreation Complete!
========================================
```

### Step 2: Run Data Migration
Copy the `mainform_records.json` file to the production server and run the migration:

```bash
cd Backend/DataMigration
dotnet run
```

**What this does:**
- Reads 7,329 records from `mainform_records.json`
- Inserts into **SINGULAR** table names:
  - `org."CompanyDetails"`
  - `org."CompanyOwner"` ← **SINGULAR**
  - `org."LicenseDetails"`
  - `org."CompanyCancellationInfo"`

**Expected Output:**
```
================================================================================
MIGRATION COMPLETED
================================================================================
Total records processed: 7329
Companies created: 7326
Owners created: 7326
Licenses created: 7326
Cancellations created: 2169
Records skipped: 3
Errors encountered: 0
================================================================================
```

### Step 3: Verify Data
Run these verification queries in PostgreSQL:

```sql
-- Check record counts
SELECT 'CompanyDetails' as table_name, COUNT(*) as records FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwner', COUNT(*) FROM org."CompanyOwner"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails";

-- Expected results:
-- CompanyDetails: 7326
-- CompanyOwner: 7326
-- LicenseDetails: 7326

-- Check a sample owner
SELECT "Id", "FirstName", "FatherName", "CompanyId" 
FROM org."CompanyOwner" 
LIMIT 5;
```

### Step 4: Start Backend Application
Start or restart the backend application:

```bash
cd Backend
dotnet run
```

Or if using systemd:
```bash
sudo systemctl restart prmis-backend
```

### Step 5: Test API Endpoints
Test the company owner API:

```bash
# Test getting owner for company 2
curl http://your-server:5143/api/CompanyOwner/2

# Expected: JSON array with owner information
```

## Important Notes

### Table Naming Convention
- ✅ **Database tables**: SINGULAR (`CompanyOwner`, `CompanyOwnerAddress`)
- ✅ **C# Models**: SINGULAR (`CompanyOwner`, `CompanyOwnerAddress`)
- ✅ **API Endpoints**: SINGULAR (`/api/CompanyOwner`)
- ✅ **Migration script**: Uses SINGULAR table names

### Why Singular Names?
- Matches business logic: One owner per company
- Consistent with C# naming conventions
- Cleaner and more intuitive

### Files Required for Production
1. `Backend/Scripts/company_module_clean_recreate.sql` - Table creation
2. `Backend/DataMigration/Program.cs` - Migration code
3. `Backend/DataMigration/Models.cs` - Data models
4. `Backend/data/files/mainform_records.json` - Source data
5. `Backend/DataMigration/DataMigration.csproj` - Project file

### Configuration
Ensure the connection string in `Backend/DataMigration/Program.cs` points to your production database:

```csharp
string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=postgres;Password=your_password";
```

## Troubleshooting

### If migration fails with "Company already exists"
The migration script skips existing companies. To re-run:

```sql
-- Delete all company data
DELETE FROM org."CompanyDetails" CASCADE;

-- Then re-run migration
cd Backend/DataMigration
dotnet run
```

### If API returns empty array for owners
1. Check table name in database:
   ```sql
   SELECT tablename FROM pg_tables WHERE schemaname = 'org' AND tablename LIKE '%Owner%';
   ```
   Should show: `CompanyOwner` (singular)

2. Check EF configuration in `Backend/Configuration/AppDbContext.cs`:
   ```csharp
   entity.ToTable("CompanyOwner", "org");  // Must be singular
   ```

3. Restart backend after any code changes

## Success Criteria
✅ All 11 tables created with singular names
✅ 7,326 companies migrated
✅ 7,326 owners migrated
✅ 7,326 licenses migrated
✅ API endpoint `/api/CompanyOwner/{id}` returns owner data
✅ Frontend can view and edit company information

## Rollback Plan
If deployment fails:

```sql
-- Drop all company tables
DROP TABLE IF EXISTS log."Licenseaudit" CASCADE;
DROP TABLE IF EXISTS log."Graunteeaudit" CASCADE;
DROP TABLE IF EXISTS log."Guarantorsaudit" CASCADE;
DROP TABLE IF EXISTS log."Companyowneraudit" CASCADE;
DROP TABLE IF EXISTS log."Companydetailsaudit" CASCADE;
DROP TABLE IF EXISTS org."PeriodicForms" CASCADE;
DROP TABLE IF EXISTS org."Haqulemtyaz" CASCADE;
DROP TABLE IF EXISTS org."CompanyCancellationInfo" CASCADE;
DROP TABLE IF EXISTS org."CompanyAccountInfo" CASCADE;
DROP TABLE IF EXISTS org."LicenseDetails" CASCADE;
DROP TABLE IF EXISTS org."Gaurantees" CASCADE;
DROP TABLE IF EXISTS org."Guarantors" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;
DROP TABLE IF EXISTS org."CompanyDetails" CASCADE;
```

Then restore from backup or re-run deployment steps.
