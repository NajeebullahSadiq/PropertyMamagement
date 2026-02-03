# Company Data Migration Guide
## Migrating 7,329 Records from Access Database to PostgreSQL

---

## 📋 Overview

This guide will help you migrate **7,329 company/license records** from your old Access database (exported as JSON) to the new PostgreSQL database structure.

### What Will Be Migrated

| Data Type | Count | Description |
|-----------|-------|-------------|
| **Companies** | 7,329 | Real estate company details |
| **Owners** | ~7,326 | Company owner information (99.96%) |
| **Licenses** | 7,329 | License details with dates and fees |
| **Cancellations** | ~2,169 | Cancelled license records (29.6%) |
| **Provinces** | 35 | Auto-created from data |
| **Districts** | 741 | Auto-created from data |
| **Education Levels** | 3+ | Auto-created from data |

**Estimated Time:** 5-10 minutes

---

## 🔧 Prerequisites

### 1. Software Requirements

- ✅ **.NET 8.0 SDK** or later
  - Download: https://dotnet.microsoft.com/download
  - Verify: `dotnet --version`

- ✅ **PostgreSQL 13+** running and accessible
  - Verify: `psql --version`
  - Check running: `pg_isready`

### 2. Database Requirements

- ✅ Database created with all schemas (`org`, `log`, `look`)
- ✅ All tables from `COMPANY_DATABASE_STRUCTURE.md` created
- ✅ Run the company module creation script: `Backend/Scripts/company_module_clean_recreate.sql`

### 3. Files Required

Located in `Backend/data/files/`:
- ✅ `mainform_records.json` - Your data (already exists)
- ✅ `Program.cs` - Migration logic
- ✅ `Models.cs` - Data models
- ✅ `DataMigration.csproj` - Project file
- ✅ `setup.sql` - Pre-migration setup

---

## 📊 Data Mapping Reference

### Old Access Database → New PostgreSQL Structure

#### CompanyDetails Table
```
OLD FIELD           →  NEW FIELD                TYPE
==========================================================
RID                 →  Id                       INTEGER (PK)
RealEstateName      →  Title                    VARCHAR(500)
TIN                 →  TIN                      VARCHAR(50)
PerProvince         →  ProvinceId               INTEGER (FK)
Halat               →  Status                   BOOLEAN
```

#### CompanyOwners Table
```
OLD FIELD           →  NEW FIELD                TYPE
==========================================================
FName               →  FirstName                VARCHAR(200)
FathName            →  FatherName               VARCHAR(200)
GFName              →  GrandFatherName          VARCHAR(200)
Education           →  EducationLevelId         SMALLINT (FK)
DOB                 →  DateofBirth              DATE
TazkeraNo           →  ElectronicNationalIdNumber VARCHAR(50)
ContactNo           →  PhoneNumber              VARCHAR(20)
PerProvince         →  OwnerProvinceId          INTEGER (FK)
PerWoloswaly        →  OwnerDistrictId          INTEGER (FK)
TempProvince        →  PermanentProvinceId      INTEGER (FK)
TempWoloswaly       →  PermanentDistrictId      INTEGER (FK)
ExactAddress        →  OwnerVillage, PermanentVillage VARCHAR(500)
```

#### LicenseDetails Table
```
OLD FIELD           →  NEW FIELD                TYPE
==========================================================
LicenseNo           →  LicenseNumber            VARCHAR(50)
SYear/SMonth/SDay   →  IssueDate                DATE
EYear/EMonth/EDay   →  ExpireDate               DATE
LicenseType         →  LicenseType, LicenseCategory VARCHAR(100)
CreditRightAmount   →  RoyaltyAmount            DECIMAL(18,2)
CreditRightYear/Month/Day → RoyaltyDate         DATE
LateFine            →  PenaltyAmount            DECIMAL(18,2)
HRNo                →  HrLetter                 VARCHAR(100)
```

#### CompanyCancellationInfo Table
```
OLD FIELD           →  NEW FIELD                TYPE
==========================================================
LicnsCancelNo       →  LicenseCancellationLetterNumber VARCHAR(100)
CancelYear/Month/Day → LicenseCancellationLetterDate DATE
Remarks             →  Remarks                  VARCHAR(1000)
```

---

## 🚀 Step-by-Step Migration Process

### Step 1: Prepare Database (5 minutes)

#### Option A: Run the complete setup script

```bash
# Navigate to Backend directory
cd Backend

# Run the company module creation script
psql -h localhost -U your_username -d your_database_name -f Scripts/company_module_clean_recreate.sql

# Run the pre-migration setup
psql -h localhost -U your_username -d your_database_name -f data/files/setup.sql
```

#### Option B: Verify manually

```sql
-- Connect to database
psql -h localhost -U your_username -d your_database_name

-- Verify schemas exist
\dn

-- Expected output:
--   org
--   log
--   look

-- Verify all required tables exist
SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_schema IN ('org', 'log', 'look')
  AND table_name IN (
    'companydetails', 'companyowners', 'licensedetails', 
    'companycancellationinfo', 'location', 'educationlevel'
  )
ORDER BY table_schema, table_name;

-- Should return 6 tables minimum
```

---

### Step 2: Configure Connection String (1 minute)

Edit `Backend/data/files/Program.cs` at line 13:

```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=YOUR_DB_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD";
```

**Replace with your actual values:**
- `YOUR_DB_NAME` → Your PostgreSQL database name (e.g., `prmis_db`)
- `YOUR_USERNAME` → Your PostgreSQL username (e.g., `postgres`)
- `YOUR_PASSWORD` → Your PostgreSQL password

**Example:**
```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=prmis_db;Username=postgres;Password=mypassword123";
```

---

### Step 3: Prepare Migration Project (2 minutes)

```bash
# Create migration directory
mkdir -p Backend/DataMigration
cd Backend/DataMigration

# Copy all required files
cp ../data/files/Program.cs .
cp ../data/files/Models.cs .
cp ../data/files/DataMigration.csproj .
cp ../data/files/mainform_records.json .

# Verify files are present
ls -la

# Expected output:
# DataMigration.csproj
# Program.cs
# Models.cs
# mainform_records.json
```

---

### Step 4: Build the Migration Tool (1 minute)

```bash
# Restore NuGet packages
dotnet restore

# Build the project
dotnet build

# Expected output:
# Build succeeded.
#     0 Warning(s)
#     0 Error(s)
```

**If you get errors:**
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Check that all files are in the correct directory
- Verify DataMigration.csproj has correct package references

---

### Step 5: Run the Migration (5-10 minutes)

```bash
# Run the migration
dotnet run
```

**Expected Console Output:**

```
=================================================================
Data Migration Tool - Access to PostgreSQL
=================================================================

Loading data from mainform_records.json...
Loaded 7329 records

Starting migration process...

Processed 100/7329 records...
Processed 200/7329 records...
Processed 300/7329 records...
...
Processed 7300/7329 records...

================================================================================
MIGRATION COMPLETED
================================================================================
Total records processed: 7329
Companies created: 7329
Owners created: 7326
Licenses created: 7329
Cancellations created: 2169
Records skipped: 0
Errors encountered: 0

================================================================================
```

---

### Step 6: Verify Migration (2 minutes)

```sql
-- Connect to database
psql -h localhost -U your_username -d your_database_name

-- Check record counts
SELECT 
    'CompanyDetails' as table_name, 
    COUNT(*) as count 
FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org."CompanyOwners"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails"
UNION ALL
SELECT 'CancellationInfo', COUNT(*) FROM org."CompanyCancellationInfo"
UNION ALL
SELECT 'Provinces', COUNT(*) FROM look."Location" WHERE "Type" = 'province'
UNION ALL
SELECT 'Districts', COUNT(*) FROM look."Location" WHERE "Type" = 'district';

-- Expected results:
-- CompanyDetails:    7329
-- CompanyOwners:     7326
-- LicenseDetails:    7329
-- CancellationInfo:  2169
-- Provinces:         35
-- Districts:         741

-- View sample migrated data
SELECT 
    cd."Id",
    cd."Title" as company_name,
    co."FirstName" || ' ' || co."FatherName" as owner_name,
    ld."LicenseNumber",
    ld."IssueDate",
    ld."ExpireDate",
    l."Name" as province
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwners" co ON co."CompanyId" = cd."Id"
LEFT JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
LEFT JOIN look."Location" l ON l."ID" = cd."ProvinceId"
LIMIT 10;

-- Check for any missing relationships
SELECT 
    cd."Id", 
    cd."Title",
    CASE WHEN co."Id" IS NULL THEN 'Missing Owner' ELSE 'OK' END as owner_status,
    CASE WHEN ld."Id" IS NULL THEN 'Missing License' ELSE 'OK' END as license_status
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwners" co ON co."CompanyId" = cd."Id"
LEFT JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
WHERE co."Id" IS NULL OR ld."Id" IS NULL
LIMIT 10;

-- Province distribution
SELECT 
    l."Name" as province,
    COUNT(cd."Id") as company_count
FROM org."CompanyDetails" cd
LEFT JOIN look."Location" l ON l."ID" = cd."ProvinceId"
GROUP BY l."Name"
ORDER BY company_count DESC
LIMIT 10;
```

---

## 🔍 Migration Details

### How the Migration Works

1. **Transaction-Based Processing**
   - Each record is processed in its own database transaction
   - If any part fails, the entire record is rolled back
   - Ensures data integrity (all-or-nothing)

2. **Automatic Lookup Creation**
   - Provinces are created automatically if they don't exist
   - Districts are created and linked to provinces
   - Education levels are created from unique values

3. **Date Handling**
   - Jalali dates (Solar Hijri) are stored as strings in YYYY-MM-DD format
   - Example: 1394/9/9 → "1394-09-09"
   - No Gregorian conversion (preserves original dates)

4. **Status Mapping**
   - "جدید" (New) → Status = true (Active)
   - "فسخ شده" (Cancelled) → Status = false (Inactive)

5. **Duplicate Handling**
   - Checks if company ID already exists
   - Skips existing records (safe to re-run)

---

## ⚠️ Troubleshooting

### Error: "File 'mainform_records.json' not found"

**Solution:**
```bash
# Make sure the JSON file is in the DataMigration folder
cp Backend/data/files/mainform_records.json Backend/DataMigration/
```

---

### Error: "Cannot connect to database"

**Solution:**
```bash
# Test PostgreSQL connection
psql -h localhost -U your_username -d your_database_name -c "SELECT 1;"

# If fails, check:
# 1. PostgreSQL is running
sudo systemctl status postgresql

# 2. Database exists
psql -l | grep your_database_name

# 3. User has permissions
psql -h localhost -U your_username -d postgres -c "\du"
```

---

### Error: "Table does not exist"

**Solution:**
```sql
-- Verify all tables exist
\dt org.*
\dt look.*

-- If missing, run the schema creation scripts
\i Backend/Scripts/company_module_clean_recreate.sql
\i Backend/data/files/setup.sql
```

---

### Error: "Duplicate key violation"

**Solution:**
```sql
-- Option 1: Clear existing data (CAUTION: Deletes all data!)
TRUNCATE TABLE org."CompanyCancellationInfo" CASCADE;
TRUNCATE TABLE org."LicenseDetails" CASCADE;
TRUNCATE TABLE org."CompanyOwners" CASCADE;
TRUNCATE TABLE org."CompanyDetails" CASCADE;

-- Option 2: Let migration skip existing records (default behavior)
-- Just re-run: dotnet run
```

---

### Error: "Foreign key constraint violation"

**Solution:**
```sql
-- Ensure lookup tables exist and have data
SELECT COUNT(*) FROM look."Location";
SELECT COUNT(*) FROM look."EducationLevel";

-- If empty, run setup script
\i Backend/data/files/setup.sql
```

---

### Error: "Out of memory"

**Solution:**
```csharp
// In Program.cs, reduce batch size (line 33)
int batchSize = 25;  // Reduce from 50 to 25
```

---

## 📈 Performance Tips

### Expected Performance

- **Processing Speed:** 100-150 records/second
- **Total Duration:** 5-10 minutes for 7,329 records
- **Memory Usage:** 200-300 MB
- **Database Size:** ~15-20 MB after migration

### Optimization Options

#### 1. Increase Batch Size (Faster, More Memory)
```csharp
// In Program.cs, line 33
int batchSize = 100;  // Increase from 50
```

#### 2. Parallel Processing (Much Faster, Use with Caution)
```csharp
// Replace the for loop in MigrateAllRecords with:
await Parallel.ForEachAsync(records, 
    new ParallelOptions { MaxDegreeOfParallelism = 4 },
    async (record, ct) => await MigrateRecord(record));
```

#### 3. Disable Indexes During Migration (Faster Inserts)
```sql
-- Before migration
DROP INDEX IF EXISTS org.idx_company_province;
DROP INDEX IF EXISTS org.idx_owner_company;
DROP INDEX IF EXISTS org.idx_license_company;

-- After migration, recreate indexes
CREATE INDEX idx_company_province ON org."CompanyDetails"("ProvinceId");
CREATE INDEX idx_owner_company ON org."CompanyOwners"("CompanyId");
CREATE INDEX idx_license_company ON org."LicenseDetails"("CompanyId");
```

---

## 🔒 Important Notes

### 1. Backup First!

**Always create a backup before migration:**
```bash
# Backup entire database
pg_dump -h localhost -U your_username -d your_database_name > backup_before_migration.sql

# Restore if needed
psql -h localhost -U your_username -d your_database_name < backup_before_migration.sql
```

### 2. Jalali Date Format

- Dates are stored in Jalali (Solar Hijri) format: YYYY-MM-DD
- Example: 1394-09-09 (not converted to Gregorian)
- If you need Gregorian dates, add a conversion library

### 3. Safe to Re-run

- Migration checks for existing companies by ID
- Skips records that already exist
- You can safely re-run if interrupted

### 4. UTF-8 Encoding

- All Persian/Dari text is preserved with UTF-8 encoding
- Ensure your database is created with UTF-8:
  ```sql
  CREATE DATABASE your_database_name 
  WITH ENCODING 'UTF8' 
  LC_COLLATE='en_US.UTF-8' 
  LC_CTYPE='en_US.UTF-8';
  ```

### 5. Transaction Isolation

- Each record is processed in its own transaction
- Failures don't affect other records
- Provides ACID guarantees

---

## ✅ Post-Migration Checklist

After successful migration:

- [ ] Verify record counts match expected values
- [ ] Check sample data for accuracy
- [ ] Verify all relationships (foreign keys)
- [ ] Test with your .NET API
- [ ] Connect React frontend
- [ ] Create database backup
- [ ] Document any data issues found
- [ ] Update application configuration

---

## 📊 Migration Statistics

### Data Distribution (Expected)

```
Province Distribution (Top 10):
├─ کابل (Kabul):        ~10,663 references
├─ پروان (Parwan):         ~619 references
├─ وردک (Wardak):          ~595 references
├─ غزنی (Ghazni):          ~400 references
├─ کاپیسا (Kapisa):        ~355 references
├─ لوگر (Logar):           ~352 references
├─ پنجشیر (Panjshir):      ~268 references
├─ لغمان (Laghman):        ~258 references
├─ پکتیا (Paktia):         ~244 references
└─ ننگرهار (Nangarhar):    ~156 references

License Types:
├─ جدید (New):         ~4,394 records (59.9%)
└─ تجدید (Renewal):    ~2,932 records (40.0%)

Status Distribution:
├─ Active (جدید):      ~7,322 records (99.9%)
└─ Inactive (فسخ):        ~7 records (0.09%)
```

---

## 🎯 Success Criteria

Your migration is successful when:

✅ All 7,329 companies created  
✅ 7,326+ owners created (99.96%)  
✅ 7,329 licenses created (100%)  
✅ 2,169+ cancellations created (29.6%)  
✅ 0 errors encountered  
✅ Sample queries return correct data  
✅ All foreign key relationships valid  
✅ Persian/Dari text displays correctly  

---

## 📞 Support

If you encounter issues:

1. Check console error messages
2. Review PostgreSQL logs: `tail -f /var/log/postgresql/*.log`
3. Verify connection string is correct
4. Ensure all prerequisite tables exist
5. Check that JSON file is valid and complete

---

## 📚 Additional Resources

- **Database Structure:** See `COMPANY_DATABASE_STRUCTURE.md`
- **Migration Flow:** See `Backend/data/files/MIGRATION_FLOW.md`
- **Quick Start:** See `Backend/data/files/QUICKSTART.md`
- **Detailed README:** See `Backend/data/files/README.md`

---

**You're ready to migrate! 🎉**

Run `dotnet run` in the `Backend/DataMigration` directory to start the migration process.
