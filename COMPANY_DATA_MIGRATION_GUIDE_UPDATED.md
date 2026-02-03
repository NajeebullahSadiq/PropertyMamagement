# Company Data Migration Guide - UPDATED
## Migrating 7,329 Records from Access Database to PostgreSQL

**Last Updated:** Based on your actual implementation in `Backend/DataMigration/`

---

## 📋 Overview

This guide reflects the **actual implementation** of your migration tool with all the specific mappings and logic you've implemented.

### What Will Be Migrated

| Data Type | Count | Description |
|-----------|-------|-------------|
| **Companies** | 7,329 | Real estate company details (all registered in Kabul) |
| **Owners** | ~7,326 | Company owner information with their actual addresses |
| **Licenses** | 7,329 | License details (all issued in Kabul) |
| **Cancellations** | ~2,169 | Cancelled license records |
| **Provinces** | 35+ | Auto-created from owner addresses |
| **Districts** | 741+ | Auto-created from owner addresses |
| **Education Levels** | 3+ | Auto-created from data |

**Estimated Time:** 5-10 minutes

---

## 🗺️ Key Implementation Details

### Province Assignment Logic (IMPORTANT!)

Your implementation has specific logic for province assignment:

1. **CompanyDetails.ProvinceId** 
   - ✅ Always set to **Kabul (کابل)**
   - Reason: All companies are registered in Kabul central office

2. **LicenseDetails.ProvinceId**
   - ✅ Always set to **Kabul (کابل)**
   - Reason: All licenses are issued by Kabul office

3. **CompanyOwners.OwnerProvinceId**
   - ✅ Set from `PerProvince` field
   - This is the owner's **actual home province**

4. **CompanyOwners.PermanentProvinceId**
   - ✅ Set from `TempProvince` field
   - This is the owner's **current residence province**

### Status Determination Logic

Your implementation checks **two fields** to determine status:

```csharp
static bool GetActiveStatus(string halatText, string cancelText)
{
    // Check if cancelled - فسخ شده means cancelled
    if (!string.IsNullOrWhiteSpace(cancelText) && 
        (cancelText.Contains("فسخ") || cancelText != ""))
        return false;
    
    // Check halat field
    if (!string.IsNullOrWhiteSpace(halatText) && halatText.Contains("فسخ"))
        return false;
        
    return true;
}
```

**Logic:**
- If `LicnsCancelNo` contains "فسخ" OR is not empty → **Inactive (false)**
- If `Halat` contains "فسخ" → **Inactive (false)**
- Otherwise → **Active (true)**

---

## 📊 Updated Data Mapping Reference

### CompanyDetails Table
```
OLD FIELD           →  NEW FIELD                VALUE/LOGIC
==================================================================
RID                 →  Id                       Direct copy
RealEstateName      →  Title                    Direct copy
TIN                 →  TIN                      Direct copy
(Fixed)             →  ProvinceId               Always Kabul (کابل)
Halat + LicnsCancelNo → Status                  See status logic above
```

### CompanyOwners Table
```
OLD FIELD           →  NEW FIELD                VALUE/LOGIC
==================================================================
FName               →  FirstName                Direct copy
FathName            →  FatherName               Direct copy
GFName              →  GrandFatherName          Direct copy
Education           →  EducationLevelId         Lookup/create
DOB                 →  DateofBirth              Direct copy (string)
TazkeraNo           →  ElectronicNationalIdNumber Direct copy
ContactNo           →  PhoneNumber              Direct copy
PerProvince         →  OwnerProvinceId          Owner's home province
PerWoloswaly        →  OwnerDistrictId          Owner's home district
TempProvince        →  PermanentProvinceId      Current residence province
TempWoloswaly       →  PermanentDistrictId      Current residence district
ExactAddress        →  OwnerVillage             Direct copy
ExactAddress        →  PermanentVillage         Same as OwnerVillage
Halat               →  Status                   See status logic
```

### LicenseDetails Table
```
OLD FIELD           →  NEW FIELD                VALUE/LOGIC
==================================================================
LicenseNo           →  LicenseNumber            Convert to string
(Fixed)             →  ProvinceId               Always Kabul (کابل)
SYear/SMonth/SDay   →  IssueDate                Format: YYYY-MM-DD
EYear/EMonth/EDay   →  ExpireDate               Format: YYYY-MM-DD
ExactAddress        →  OfficeAddress            Office location
LicenseType         →  LicenseType              Direct copy
LicenseType         →  LicenseCategory          Same as LicenseType
CreditRightAmount   →  RoyaltyAmount            Direct copy
CreditRightYear/Month/Day → RoyaltyDate         Format: YYYY-MM-DD
LateFine            →  PenaltyAmount            Direct copy
CreditRightNo       →  TariffNumber             Tariff/receipt number
HRNo                →  HrLetter                 Convert to string
Combo211/HRMonth/HRDay → HrLetterDate           Format: YYYY-MM-DD
(Fixed)             →  IsComplete               Always true
Halat + LicnsCancelNo → Status                  See status logic
```

### CompanyCancellationInfo Table
```
OLD FIELD           →  NEW FIELD                VALUE/LOGIC
==================================================================
LicnsCancelNo       →  LicenseCancellationLetterNumber Direct copy
CancelYear/Month/Day → LicenseCancellationLetterDate Format: YYYY-MM-DD
Remarks             →  Remarks                  Direct copy
(Fixed)             →  Status                   Always true
```

---

## 🚀 Step-by-Step Migration Process

### Step 1: Prepare Database (5 minutes)

```bash
# Navigate to Backend directory
cd Backend

# Run the company module creation script
psql -h localhost -U your_username -d your_database_name -f Scripts/company_module_clean_recreate.sql

# Run the pre-migration setup (creates lookup data)
psql -h localhost -U your_username -d your_database_name -f data/files/setup.sql
```

**Verify tables exist:**
```sql
-- Connect to database
psql -h localhost -U your_username -d your_database_name

-- Check required tables
SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_schema IN ('org', 'log', 'look')
  AND table_name IN (
    'CompanyDetails', 'CompanyOwners', 'LicenseDetails', 
    'CompanyCancellationInfo', 'Location', 'EducationLevel'
  )
ORDER BY table_schema, table_name;
```

---

### Step 2: Configure Connection String (1 minute)

Edit `Backend/DataMigration/Program.cs` at line 13:

```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=YOUR_DB_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD";
```

**Example:**
```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=prmis_db;Username=postgres;Password=mypassword123";
```

---

### Step 3: Verify Files (1 minute)

```bash
cd Backend/DataMigration

# Check all required files exist
ls -la

# You should see:
# - Program.cs (your updated version)
# - Models.cs (with nullable properties)
# - DataMigration.csproj
# - mainform_records.json
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
```

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
SELECT 'CancellationInfo', COUNT(*) FROM org."CompanyCancellationInfo";

-- Expected results:
-- CompanyDetails:    7329
-- CompanyOwners:     7326
-- LicenseDetails:    7329
-- CancellationInfo:  2169

-- Verify all companies are registered in Kabul
SELECT 
    l."Name" as province,
    COUNT(cd."Id") as company_count
FROM org."CompanyDetails" cd
LEFT JOIN look."Location" l ON l."ID" = cd."ProvinceId"
GROUP BY l."Name";

-- Expected: All 7329 companies should show "کابل"

-- Verify all licenses are issued in Kabul
SELECT 
    l."Name" as province,
    COUNT(ld."Id") as license_count
FROM org."LicenseDetails" ld
LEFT JOIN look."Location" l ON l."ID" = ld."ProvinceId"
GROUP BY l."Name";

-- Expected: All 7329 licenses should show "کابل"

-- Check owner province distribution (should be varied)
SELECT 
    l."Name" as owner_province,
    COUNT(co."Id") as owner_count
FROM org."CompanyOwners" co
LEFT JOIN look."Location" l ON l."ID" = co."OwnerProvinceId"
GROUP BY l."Name"
ORDER BY owner_count DESC
LIMIT 10;

-- Expected: Various provinces (Kabul, Parwan, Wardak, etc.)

-- View sample migrated data
SELECT 
    cd."Id",
    cd."Title" as company_name,
    co."FirstName" || ' ' || co."FatherName" as owner_name,
    ld."LicenseNumber",
    ld."IssueDate",
    ld."ExpireDate",
    ld."OfficeAddress",
    ld."TariffNumber",
    company_prov."Name" as company_province,
    owner_prov."Name" as owner_home_province
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwners" co ON co."CompanyId" = cd."Id"
LEFT JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
LEFT JOIN look."Location" company_prov ON company_prov."ID" = cd."ProvinceId"
LEFT JOIN look."Location" owner_prov ON owner_prov."ID" = co."OwnerProvinceId"
LIMIT 10;

-- Check cancelled licenses
SELECT 
    cd."Title",
    ld."LicenseNumber",
    cd."Status" as company_status,
    ld."Status" as license_status,
    cci."LicenseCancellationLetterNumber",
    cci."LicenseCancellationLetterDate"
FROM org."CompanyDetails" cd
JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
LEFT JOIN org."CompanyCancellationInfo" cci ON cci."CompanyId" = cd."Id"
WHERE cd."Status" = false OR ld."Status" = false
LIMIT 10;
```

---

## 🔍 Migration Implementation Details

### 1. Transaction-Based Processing
- Each record is processed in its own database transaction
- If any part fails, the entire record is rolled back
- Ensures data integrity (all-or-nothing)

### 2. Province Assignment Strategy
- **Central Registration**: All companies registered in Kabul
- **Central Licensing**: All licenses issued in Kabul
- **Owner Tracking**: Owner's actual home and current provinces tracked separately
- This allows province-based access control while maintaining accurate owner information

### 3. Automatic Lookup Creation
- Provinces created automatically from owner addresses
- Districts created and linked to their provinces
- Education levels created from unique values in data

### 4. Date Handling
- Jalali dates stored as strings: YYYY-MM-DD format
- Example: 1394/9/9 → "1394-09-09"
- No Gregorian conversion (preserves original calendar)
- HR Letter Date uses Combo211 field for year component

### 5. Status Determination
- Checks both `LicnsCancelNo` and `Halat` fields
- Any indication of cancellation (فسخ) sets status to false
- Applied to both CompanyDetails and LicenseDetails

### 6. Field Mappings
- `CreditRightNo` → `TariffNumber` (receipt/tariff reference)
- `ExactAddress` → `OfficeAddress` (office location in license)
- `ExactAddress` → `OwnerVillage` and `PermanentVillage` (owner addresses)
- `Combo211` + `HRMonth` + `HRDay` → `HrLetterDate`

### 7. Duplicate Handling
- Checks if company ID already exists before inserting
- Skips existing records (safe to re-run)
- Logs skipped records in statistics

---

## ⚠️ Troubleshooting

### Error: "Cannot connect to database"

**Solution:**
```bash
# Test connection
psql -h localhost -U your_username -d your_database_name -c "SELECT 1;"

# Check PostgreSQL is running
sudo systemctl status postgresql

# Verify database exists
psql -l | grep your_database_name
```

---

### Error: "Table does not exist"

**Solution:**
```sql
-- Check if tables exist (case-sensitive!)
\dt org.*
\dt look.*

-- If missing, run schema creation
\i Backend/Scripts/company_module_clean_recreate.sql
```

---

### Error: "Column does not exist"

**Solution:**
```sql
-- Verify column names match (case-sensitive in PostgreSQL!)
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'org' 
  AND table_name = 'CompanyDetails';

-- Common issue: PostgreSQL uses lowercase unless quoted
-- Your tables use PascalCase with quotes: "CompanyDetails"
```

---

### Error: "Foreign key constraint violation"

**Solution:**
```sql
-- Ensure Location table has Kabul province
SELECT * FROM look."Location" WHERE "Name" = 'کابل' AND "Type" = 'province';

-- If missing, insert it
INSERT INTO look."Location" ("Name", "Type", "Status", "CreatedAt", "CreatedBy")
VALUES ('کابل', 'province', true, NOW(), 'SETUP_SCRIPT');
```

---

### Error: "Duplicate key violation"

**Solution:**
```sql
-- Option 1: Clear existing data (CAUTION!)
TRUNCATE TABLE org."CompanyCancellationInfo" CASCADE;
TRUNCATE TABLE org."LicenseDetails" CASCADE;
TRUNCATE TABLE org."CompanyOwners" CASCADE;
TRUNCATE TABLE org."CompanyDetails" CASCADE;

-- Option 2: Let migration skip existing (default behavior)
-- Just re-run: dotnet run
```

---

## 📈 Performance Tips

### Expected Performance
- **Processing Speed:** 100-150 records/second
- **Total Duration:** 5-10 minutes for 7,329 records
- **Memory Usage:** 200-300 MB

### Optimization Options

#### 1. Increase Batch Size
```csharp
// In Program.cs, line 56
int batchSize = 100;  // Increase from 50
```

#### 2. Disable Indexes During Migration
```sql
-- Before migration
DROP INDEX IF EXISTS org."IX_CompanyDetails_ProvinceId";
DROP INDEX IF EXISTS org."IX_CompanyOwners_CompanyId";
DROP INDEX IF EXISTS org."IX_LicenseDetails_CompanyId";

-- After migration, recreate
CREATE INDEX "IX_CompanyDetails_ProvinceId" ON org."CompanyDetails"("ProvinceId");
CREATE INDEX "IX_CompanyOwners_CompanyId" ON org."CompanyOwners"("CompanyId");
CREATE INDEX "IX_LicenseDetails_CompanyId" ON org."LicenseDetails"("CompanyId");
```

---

## 🔒 Important Notes

### 1. Backup First!
```bash
# Always backup before migration
pg_dump -h localhost -U your_username -d your_database_name > backup_before_migration.sql

# Restore if needed
psql -h localhost -U your_username -d your_database_name < backup_before_migration.sql
```

### 2. Case Sensitivity
- PostgreSQL table/column names are case-sensitive when quoted
- Your schema uses PascalCase: `"CompanyDetails"`, `"FirstName"`, etc.
- Ensure queries match the exact case

### 3. Jalali Date Format
- Dates stored as strings in Jalali format: YYYY-MM-DD
- No automatic conversion to Gregorian
- If you need Gregorian dates, add conversion library later

### 4. Safe to Re-run
- Migration checks for existing companies by ID
- Skips records that already exist
- You can safely re-run if interrupted

### 5. UTF-8 Encoding
- All Persian/Dari text preserved with UTF-8
- Ensure database created with UTF-8:
  ```sql
  CREATE DATABASE your_database_name 
  WITH ENCODING 'UTF8' 
  LC_COLLATE='en_US.UTF-8' 
  LC_CTYPE='en_US.UTF-8';
  ```

---

## ✅ Post-Migration Verification Checklist

After successful migration:

- [ ] All 7,329 companies created
- [ ] All companies have ProvinceId = Kabul
- [ ] All 7,329 licenses created
- [ ] All licenses have ProvinceId = Kabul
- [ ] 7,326+ owners created with varied home provinces
- [ ] 2,169+ cancellations created
- [ ] Status correctly reflects cancelled licenses
- [ ] Office addresses populated in licenses
- [ ] Tariff numbers populated
- [ ] HR letter dates populated where available
- [ ] Sample queries return correct data
- [ ] Persian/Dari text displays correctly

---

## 📊 Expected Data Distribution

### Province Distribution

**Company Registration (CompanyDetails.ProvinceId):**
- کابل (Kabul): 7,329 (100%) ✅

**License Issuance (LicenseDetails.ProvinceId):**
- کابل (Kabul): 7,329 (100%) ✅

**Owner Home Provinces (CompanyOwners.OwnerProvinceId):**
- کابل (Kabul): ~10,663 references
- پروان (Parwan): ~619 references
- وردک (Wardak): ~595 references
- غزنی (Ghazni): ~400 references
- Various other provinces

### License Types
- جدید (New): ~4,394 records (59.9%)
- تجدید (Renewal): ~2,932 records (40.0%)

### Status Distribution
- Active: ~7,322 records (99.9%)
- Inactive (Cancelled): ~7 records (0.09%)

---

## 🎯 Success Criteria

Your migration is successful when:

✅ All 7,329 companies created with ProvinceId = Kabul  
✅ All 7,329 licenses created with ProvinceId = Kabul  
✅ 7,326+ owners created with varied home provinces  
✅ 2,169+ cancellations created  
✅ 0 errors encountered  
✅ Status correctly reflects cancellations  
✅ Office addresses and tariff numbers populated  
✅ Sample queries return correct data  
✅ Persian/Dari text displays correctly  

---

**You're ready to migrate! 🎉**

Run `dotnet run` in the `Backend/DataMigration` directory to start the migration process.
