# Fix Province ID and License Number Format

## Changes Required

### 1. Province ID
- **Current:** ProvinceId is looked up from Location table (varies)
- **Required:** ProvinceId = **1** for all companies and licenses

### 2. License Number Format
- **Current:** Plain numbers (e.g., "123", "1234", "12345")
- **Required:** **KBL-00000{number}** format (e.g., "KBL-00001", "KBL-00123", "KBL-01234")

---

## Step 1: Update Existing Records

Run this SQL script to fix all existing records:

```bash
psql -h localhost -U postgres -d PRMIS -f Backend/Scripts/fix_province_and_license_format.sql
```

### What It Does:
1. ✅ Sets all `CompanyDetails.ProvinceId` to **1**
2. ✅ Sets all `LicenseDetails.ProvinceId` to **1**
3. ✅ Formats all license numbers as **KBL-00001**, **KBL-00123**, etc.

### Expected Output:
```
NOTICE:  Starting fix for Province IDs and License Numbers...
NOTICE:   
NOTICE:  ✓ Updated 7327 companies to ProvinceId = 1
NOTICE:  ✓ Updated 7327 licenses to ProvinceId = 1 and formatted license numbers
NOTICE:   
NOTICE:  ========================================
NOTICE:  Fix Complete!
NOTICE:  ========================================
```

---

## Step 2: Complete Migration with Fixed Code

The migration code has been updated to:
1. Use **ProvinceId = 1** (hardcoded) instead of looking up Kabul
2. Format license numbers as **KBL-{5-digit-padded-number}**

Run the migration to process the last 2 records:

```bash
cd Backend/DataMigration
dotnet build
dotnet run
```

### Code Changes:

#### Before:
```csharp
// Looked up Kabul province
int? kabulProvinceId = await GetOrCreateProvinceId("کابل", conn, transaction);

// Plain license number
cmd.Parameters.AddWithValue("licensenumber", record.LicenseNo.ToString());
```

#### After:
```csharp
// Hardcoded ProvinceId = 1
int kabulProvinceId = 1;

// Formatted license number: KBL-00001, KBL-00123, etc.
string formattedLicenseNumber = $"KBL-{record.LicenseNo.ToString().PadLeft(5, '0')}";
cmd.Parameters.AddWithValue("licensenumber", formattedLicenseNumber);
```

---

## Verification Queries

After running the fix script and migration:

### 1. Check Province IDs:
```sql
-- All companies should have ProvinceId = 1
SELECT 
    "ProvinceId",
    COUNT(*) as count
FROM org."CompanyDetails"
GROUP BY "ProvinceId";

-- Expected: All 7329 records with ProvinceId = 1
```

### 2. Check License Province IDs:
```sql
-- All licenses should have ProvinceId = 1
SELECT 
    "ProvinceId",
    COUNT(*) as count
FROM org."LicenseDetails"
GROUP BY "ProvinceId";

-- Expected: All 7329 records with ProvinceId = 1
```

### 3. Check License Number Format:
```sql
-- All licenses should have KBL- prefix
SELECT 
    "LicenseNumber",
    "IssueDate",
    "ExpireDate"
FROM org."LicenseDetails"
ORDER BY "Id"
LIMIT 20;

-- Expected format: KBL-00001, KBL-00002, ..., KBL-01234, etc.
```

### 4. Verify License Number Pattern:
```sql
-- Count licenses with correct format
SELECT 
    CASE 
        WHEN "LicenseNumber" LIKE 'KBL-%' THEN 'Correct Format (KBL-)'
        ELSE 'Incorrect Format'
    END as format_status,
    COUNT(*) as count
FROM org."LicenseDetails"
GROUP BY format_status;

-- Expected: All 7329 with "Correct Format (KBL-)"
```

### 5. Sample Data View:
```sql
-- View sample companies with formatted license numbers
SELECT 
    cd."Id",
    cd."Title",
    cd."ProvinceId" as company_province_id,
    ld."LicenseNumber",
    ld."ProvinceId" as license_province_id,
    ld."IssueDate",
    ld."ExpireDate"
FROM org."CompanyDetails" cd
JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
ORDER BY cd."Id"
LIMIT 20;

-- Expected:
-- - company_province_id = 1 for all
-- - license_province_id = 1 for all
-- - LicenseNumber format: KBL-00001, KBL-00002, etc.
```

---

## License Number Format Examples

| Original | Formatted |
|----------|-----------|
| 1 | KBL-00001 |
| 12 | KBL-00012 |
| 123 | KBL-00123 |
| 1234 | KBL-01234 |
| 12345 | KBL-12345 |

---

## Why ProvinceId = 1?

Setting ProvinceId to 1 (instead of looking up Kabul) ensures:
1. ✅ **Consistency** - All records use the same ID
2. ✅ **Performance** - No lookup queries needed
3. ✅ **Simplicity** - Hardcoded value is faster and more reliable
4. ✅ **Compatibility** - Works even if Location table changes

---

## Why KBL- Prefix?

The KBL- prefix (Kabul) provides:
1. ✅ **Province Identification** - Clearly shows license is from Kabul
2. ✅ **Uniqueness** - Prevents conflicts with other provinces
3. ✅ **Consistency** - Matches your system's license numbering scheme
4. ✅ **Readability** - Easy to identify and search

---

## Files Updated

1. ✅ `Backend/Scripts/fix_province_and_license_format.sql` - SQL fix script
2. ✅ `Backend/DataMigration/Program.cs` - Migration code updated
   - InsertCompanyDetails: Uses ProvinceId = 1
   - InsertLicenseDetails: Uses ProvinceId = 1 and formats license numbers

---

## Execution Steps

### Step 1: Fix Existing Records
```bash
psql -h localhost -U postgres -d PRMIS -f Backend/Scripts/fix_province_and_license_format.sql
```

### Step 2: Rebuild Migration
```bash
cd Backend/DataMigration
dotnet build
```

### Step 3: Complete Migration
```bash
dotnet run
```

### Step 4: Verify Results
```sql
-- Connect to database
psql -h localhost -U postgres -d PRMIS

-- Run verification queries above
```

---

## Expected Final State

After completing all steps:

| Table | Field | Value |
|-------|-------|-------|
| CompanyDetails | ProvinceId | 1 (all 7,329 records) |
| LicenseDetails | ProvinceId | 1 (all 7,329 records) |
| LicenseDetails | LicenseNumber | KBL-00001 to KBL-99999 format |

---

## Success Criteria

✅ All 7,329 companies have ProvinceId = 1  
✅ All 7,329 licenses have ProvinceId = 1  
✅ All 7,329 licenses have KBL-00000 format  
✅ No lookup queries for province  
✅ Consistent data across all records  

---

**Ready to execute!** 🚀

Run the SQL script first, then complete the migration.
