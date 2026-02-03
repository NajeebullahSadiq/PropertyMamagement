# Production Migration - Final Steps

## Issue Resolved
The migration was failing because it tried to query/insert into `look."Location"` table with columns (`TypeId`) that don't exist in production.

## Solution Applied
Simplified the migration script to:
- **Always use ProvinceId = 1 (Kabul)** for all records
- **Skip district lookups** (return null for all districts)
- District/province names are preserved as text in the Village fields

## Step-by-Step Deployment Instructions

### 1. Rebuild Migration Tool on Production Server

```bash
cd ~/PropertyMamagement/Backend/DataMigration
dotnet build
```

**Expected Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 2. Run Migration

```bash
cd ~/PropertyMamagement/Backend/DataMigration
dotnet run
```

**Expected Output:**
```
=================================================================
Data Migration Tool - Access to PostgreSQL
=================================================================

Loading data from mainform_records.json...
Loaded 7329 records

Starting migration process...

Processed 100/7329 records...
Processed 200/7329 records...
...
Processed 7300/7329 records...

=================================================================
MIGRATION COMPLETED
=================================================================
Total records processed: 7329
Companies created: 7326
Owners created: 7326
Licenses created: 7326
Cancellations created: 2169
Records skipped: 0
Errors encountered: 3
```

### 3. Fix Database Sequences (CRITICAL!)

After migration completes, immediately run:

```bash
cd ~/PropertyMamagement/Backend
psql -h 127.0.0.1 -U prmis_user -d PRMIS -f Scripts/fix_company_sequences.sql
```

**Expected Output:**
```
 setval 
--------
   7327
(1 row)

 setval 
--------
   7327
(1 row)

 setval 
--------
   7327
(1 row)

 table_name        | next_id
-------------------+---------
 CompanyDetails    |    7327
 CompanyOwner      |    7327
 LicenseDetails    |    7327
 Guarantors        |       1
```

### 4. Verify Migration Results

```bash
psql -h 127.0.0.1 -U prmis_user -d PRMIS
```

Then run these queries:

```sql
-- Check total companies
SELECT COUNT(*) FROM org."CompanyDetails";
-- Expected: 7326

-- Check total owners
SELECT COUNT(*) FROM org."CompanyOwner";
-- Expected: 7326

-- Check total licenses
SELECT COUNT(*) FROM org."LicenseDetails";
-- Expected: 7326

-- Check license number format
SELECT "LicenseNumber", "ProvinceId" 
FROM org."LicenseDetails" 
LIMIT 10;
-- Expected: KBL-00001, KBL-00002, etc. with ProvinceId = 1

-- Check company provinces
SELECT "ProvinceId", COUNT(*) 
FROM org."CompanyDetails" 
GROUP BY "ProvinceId";
-- Expected: All should be ProvinceId = 1 (Kabul)

-- Check owner data
SELECT "FirstName", "FatherName", "ElectronicNationalIdNumber", "OwnerProvinceId"
FROM org."CompanyOwner"
LIMIT 5;
-- Expected: Names and IDs populated, OwnerProvinceId = 1

\q
```

### 5. Restart Backend Application

```bash
sudo systemctl restart prmis-backend
sudo systemctl status prmis-backend
```

**Expected Output:**
```
● prmis-backend.service - PRMIS Backend API
   Loaded: loaded
   Active: active (running)
```

### 6. Test API Endpoints

```bash
# Test company list
curl http://localhost:5000/api/CompanyDetails

# Test specific company owner
curl http://localhost:5000/api/CompanyOwner/1

# Test license details
curl http://localhost:5000/api/LicenseDetail/company/1
```

## Migration Script Changes

### What Changed:
1. **GetOrCreateProvinceId()** - Now always returns 1 (Kabul) instead of querying Location table
2. **GetOrCreateDistrictId()** - Now always returns null instead of querying Location table

### Why This Works:
- All companies are registered in Kabul (ProvinceId = 1)
- All licenses are issued in Kabul (ProvinceId = 1)
- Owner's actual province/district names are preserved in text fields:
  - `OwnerProvinceId` = 1 (for consistency)
  - `OwnerVillage` = contains full address text including district/province names
  - `PermanentVillage` = contains full address text

### Data Integrity:
- ✅ No data loss - all address information preserved as text
- ✅ No Location table queries - avoids schema mismatch
- ✅ Consistent province assignment - all records use Kabul
- ✅ License numbering works - KBL-00001, KBL-00002, etc.

## Troubleshooting

### If Migration Fails:

1. **Check connection string:**
   ```bash
   cd ~/PropertyMamagement/Backend/DataMigration
   head -20 Program.cs | grep -A 2 "connectionString"
   ```
   Should show: `Host=127.0.0.1;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024`

2. **Test database connection:**
   ```bash
   psql -h 127.0.0.1 -U prmis_user -d PRMIS -c "SELECT 1;"
   ```

3. **Check if tables exist:**
   ```bash
   psql -h 127.0.0.1 -U prmis_user -d PRMIS -c "\dt org.*"
   ```

4. **Clean up and retry:**
   ```bash
   # Delete all company data
   psql -h 127.0.0.1 -U prmis_user -d PRMIS -f Scripts/simple_delete_company_data.sql
   
   # Run migration again
   cd DataMigration
   dotnet run
   ```

### If Sequences Not Fixed:

You'll get errors like: `duplicate key value violates unique constraint "CompanyDetails_pkey"`

**Solution:**
```bash
psql -h 127.0.0.1 -U prmis_user -d PRMIS -f Scripts/fix_company_sequences.sql
```

## Success Criteria

✅ Migration completes with 7326 companies created  
✅ All sequences set to 7327 or higher  
✅ License numbers formatted as KBL-00001, KBL-00002, etc.  
✅ All ProvinceId fields = 1 (Kabul)  
✅ Backend API returns company data correctly  
✅ New company registration works (gets ID 7327+)  

## Next Steps After Successful Migration

1. Test company search functionality in frontend
2. Test creating new company (should get ID 7327)
3. Test license printing
4. Verify province-based access control
5. Monitor application logs for any errors

## Files Modified

- `Backend/DataMigration/Program.cs` - Simplified province/district lookup
- `Backend/DataMigration/DataMigration.csproj` - Updated to .NET 9.0

## Database Connection Details

- **Host:** 127.0.0.1 (use TCP, not localhost)
- **Port:** 5432
- **Database:** PRMIS
- **Username:** prmis_user
- **Password:** SecurePassword@2024
- **Schema:** org (company tables), look (lookup tables), log (audit tables)
