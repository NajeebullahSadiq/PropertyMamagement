# Production Deployment Checklist - Company Module

## Overview
This guide covers deploying the company module to production with data migration from Access database.

## Prerequisites
- [ ] PostgreSQL database backup completed
- [ ] Access database file available: `Backend/data/files/mainform_records.json`
- [ ] Backend application stopped
- [ ] Database connection string configured in `appsettings.Production.json`

---

## Step 1: Create Database Tables

**Script**: `Backend/Scripts/company_module_clean_recreate.sql`

**What it does**:
- Drops existing company tables (if any)
- Creates 11 transaction tables in `org` schema
- Creates 5 audit tables in `log` schema
- Creates performance indexes
- Uses SERIAL for auto-increment IDs

**How to run**:
```bash
psql -U postgres -d PRMIS -f Backend/Scripts/company_module_clean_recreate.sql
```

**Expected output**:
```
✓ All company module tables dropped successfully
✓ All 11 company module tables created successfully
✓ All 5 company audit tables created successfully
✓ All performance indexes created successfully
```

**Verification**:
- Transaction tables: 11 / 11
- Audit tables: 5 / 5
- Indexes created: ~50+

---

## Step 2: Migrate Data from Access

**Tool**: `Backend/DataMigration/Program.cs`

**What it does**:
- Reads `mainform_records.json` (exported from Access)
- Migrates companies, owners, licenses, and cancellations
- Sets ProvinceId = 1 (Kabul) for all records
- Formats license numbers as KBL-00001, KBL-00002, etc.
- Uses singular table names (CompanyOwner, not CompanyOwners)

**How to run**:
```bash
cd Backend/DataMigration
dotnet run
```

**Expected output**:
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

**What gets migrated**:
- ✅ Company details (Title, TIN, ProvinceId=1)
- ✅ Owner information (Name, DOB, Electronic ID, Phone)
- ✅ Owner addresses (Province, District, Village)
- ✅ License details (Number, Dates, Office Address)
- ✅ Cancellation info (if applicable)

**Database IDs after migration**:
- CompanyDetails: IDs 1-7326
- CompanyOwner: IDs 1-7326
- LicenseDetails: IDs 1-7326
- CompanyCancellationInfo: IDs 1-2169

---

## Step 3: Fix Database Sequences ⚠️ **CRITICAL**

**Script**: `Backend/Scripts/fix_company_sequences.sql`

**Why this is needed**:
When you insert records with explicit IDs (like the migration does), PostgreSQL sequences don't automatically update. Without this fix:
- Sequences remain at 1
- Next insert tries to use ID 1
- But ID 1 already exists
- **Result**: ERROR: duplicate key value violates unique constraint

**What it does**:
- Updates CompanyDetails sequence to 7327 (next available)
- Updates CompanyOwner sequence to 7327
- Updates LicenseDetails sequence to 7327
- Updates other sequences based on data
- Verifies all sequences are correct

**How to run**:
```bash
psql -U postgres -d PRMIS -f Backend/Scripts/fix_company_sequences.sql
```

**Expected output**:
```
table_name                | next_id | is_called
--------------------------+---------+-----------
CompanyDetails            | 7326    | t
CompanyOwner              | 7326    | t
LicenseDetails            | 7326    | t
Guarantors                | 1       | f
CompanyCancellationInfo   | 2169    | t
CompanyAccountInfo        | 1       | f
```

**Verification**:
- `is_called = t` means next value will be `next_id + 1`
- `is_called = f` means next value will be `next_id` (for empty tables)
- New companies will get IDs starting from 7327

---

## Step 4: Deploy Backend Application

**Files to deploy**:
- `Backend/bin/Release/net9.0/publish/` (entire folder)
- `appsettings.Production.json` (with correct connection string)

**Configuration check**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=PRMIS;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

**Start application**:
```bash
cd Backend
dotnet WebAPIBackend.dll --environment=Production
```

**Verify startup**:
- No database errors in logs
- API responds to health check
- License number generator service loaded

---

## Step 5: Verification Tests

### Test 1: View Migrated Data
**Endpoint**: `GET /api/CompanyDetails`

**Expected**: List of 7,326 companies

**Sample response**:
```json
{
  "id": 1,
  "title": "شرکت املاک...",
  "provinceId": 1,
  "licenseNumber": "KBL-00001"
}
```

### Test 2: View Migrated Owner
**Endpoint**: `GET /api/CompanyOwner/1`

**Expected**: Owner details for company 1

**Sample response**:
```json
{
  "id": 1,
  "firstName": "محمد",
  "companyId": 1,
  "electronicNationalIdNumber": "123456789"
}
```

### Test 3: Create New Company
**Endpoint**: `POST /api/CompanyDetails`

**Body**:
```json
{
  "title": "Test Company",
  "tin": "12345"
}
```

**Expected**: 
- Success response with `id: 7327`
- No duplicate key error

### Test 4: Create New License
**Endpoint**: `POST /api/LicenseDetail`

**Body** (as Admin):
```json
{
  "companyId": 7327,
  "provinceId": 1,
  "issueDate": "1403-11-14",
  "areaId": 8,
  "officeAddress": "Kabul",
  "licenseType": "realEstate",
  "licenseCategory": "جدید",
  "calendarType": "hijriShamsi"
}
```

**Expected**:
- Success response
- License number auto-generated: `KBL-7327` or next available
- ProvinceId = 1

### Test 5: License Number Sequence
**Query database**:
```sql
SELECT "LicenseNumber", "ProvinceId" 
FROM org."LicenseDetails" 
WHERE "ProvinceId" = 1 
ORDER BY "Id" DESC 
LIMIT 5;
```

**Expected**:
- Migrated: KBL-00001 through KBL-07326
- New: KBL-07327, KBL-07328, etc.

---

## Rollback Plan

If something goes wrong:

### Option 1: Restore from Backup
```bash
psql -U postgres -d PRMIS < backup_before_migration.sql
```

### Option 2: Re-run Migration
```bash
# Step 1: Clean tables
psql -U postgres -d PRMIS -f Backend/Scripts/company_module_clean_recreate.sql

# Step 2: Migrate data
cd Backend/DataMigration
dotnet run

# Step 3: Fix sequences
psql -U postgres -d PRMIS -f Backend/Scripts/fix_company_sequences.sql
```

---

## Common Issues & Solutions

### Issue 1: Duplicate Key Error on New Company
**Symptom**: `ERROR: duplicate key value violates unique constraint "CompanyDetails_pkey"`

**Cause**: Sequences not updated after migration

**Solution**: Run `Backend/Scripts/fix_company_sequences.sql`

### Issue 2: License Number Empty
**Symptom**: New licenses have empty `licenseNumber` field

**Cause**: ProvinceId is null

**Solution**: 
- Admin users: Select province in UI
- Company Registrar: Ensure user has ProvinceId set

### Issue 3: Migration Shows 0 Records Created
**Symptom**: Migration completes but says "Companies created: 0"

**Cause**: Records already exist (duplicate run)

**Solution**: This is expected if re-running migration. Check database:
```sql
SELECT COUNT(*) FROM org."CompanyDetails";
```

---

## Post-Deployment Checklist

- [ ] All 7,326 companies visible in UI
- [ ] Owner information displays correctly
- [ ] License numbers show as KBL-00001 format
- [ ] Can create new company (gets ID 7327+)
- [ ] Can create new license (gets auto-generated number)
- [ ] Search functionality works
- [ ] Province filtering works for non-admin users
- [ ] License printing works for complete licenses
- [ ] Audit logs are being created

---

## Files Summary

### Required for Production:
1. ✅ `Backend/Scripts/company_module_clean_recreate.sql` - Creates tables
2. ✅ `Backend/DataMigration/Program.cs` - Migrates data
3. ✅ `Backend/Scripts/fix_company_sequences.sql` - Fixes sequences
4. ✅ `Backend/data/files/mainform_records.json` - Source data

### Optional (for troubleshooting):
- `Backend/Scripts/diagnose_id_generation.sql` - Check ID generation
- `Backend/Scripts/simple_check.sql` - Verify data
- `Backend/Scripts/delete_all_company_data.sql` - Clean slate (if needed)

---

## Support Contacts

If issues arise during deployment:
1. Check application logs: `Backend/logs/`
2. Check PostgreSQL logs
3. Review this checklist
4. Contact development team

---

## Success Criteria

✅ Migration complete when:
- 7,326 companies in database
- 7,326 owners in database
- 7,326 licenses in database
- All license numbers formatted as KBL-XXXXX
- New records get IDs starting from 7327
- No duplicate key errors
- License numbers auto-generate correctly
