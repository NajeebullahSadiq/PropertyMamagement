# Petition Writer License Migration Guide

## Overview
This guide explains how to migrate Petition Writer License data from JSON files (years 1403 and 1404) into the PostgreSQL database.

## Prerequisites

1. **Database Setup**: Ensure the `PetitionWriterLicense` table exists in the database
2. **JSON Files**: Both files must be in the `Backend/DataMigration` directory:
   - `petition_1403_records.json`
   - `petition_1404_records.json`
3. **Connection String**: Set the database connection string (see below)

## Files Structure

```
Backend/DataMigration/
├── petition_1403_records.json          # 1403 records (138 entries)
├── petition_1404_records.json          # 1404 records (133 entries)
├── PetitionWriterModels.cs             # Data models
├── PetitionWriterMigration.cs          # Migration logic
└── Program.cs                          # Entry point
```

## Database Connection

### Option 1: Environment Variable (Recommended)
```bash
# Windows PowerShell
$env:MIGRATION_CONNECTION_STRING="Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=YourPassword"

# Linux/Mac
export MIGRATION_CONNECTION_STRING="Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=YourPassword"
```

### Option 2: Modify Code
Edit `PetitionWriterMigration.cs` line 13:
```csharp
private static string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=YourPassword";
```

## Running the Migration

### Step 1: Navigate to DataMigration Directory
```bash
cd Backend/DataMigration
```

### Step 2: Run the Migration

#### Option A: Petition Writer Only
```bash
dotnet run petitionwriter
```

#### Option B: All Migrations (Company + Securities + Petition Writer)
```bash
dotnet run all
```

#### Option C: Build and Run Separately
```bash
# Build
dotnet build

# Run
dotnet run petitionwriter
```

## What Gets Migrated

The migration creates records in the `org.PetitionWriterLicense` table with the following data:

### Personal Information
- Applicant Name (نام متقاضی)
- Father Name (نام پدر)
- Tazkera Jold & Page (جلد و صفحه تذکره)
- Electronic National ID Number (شماره تذکره الکترونیکی)
- Mobile Number (شماره تماس)

### Address Information
- **Permanent Address**: Province, District, Village
- **Current Address**: Province, District, Village/Nahia

### License Information
- License Number (شماره جواز)
- License Issue Date (تاریخ صدور)
- License Type (نوع جواز): جدید (New), تجدید (Renewal), or مثنی (Duplicate)
- Activity Location (محل فعالیت)
- Activity District/Nahia (ناحیه فعالیت)
- Relocation Info (معلومات انتقال)

### System Fields
- Province ID: Always set to 1 (Kabul)
- Status: Active (true)
- Created At: Current timestamp
- Created By: MIGRATION_SCRIPT_1403 or MIGRATION_SCRIPT_1404

## Data Mapping Details

### License Number
- Extracted from `LicenseNumber` field
- Handles both numeric and string formats
- Example: 1050, 1051, etc.

### Mobile Number
- Extracted from `MobileNumber` field
- Truncated to 20 characters if longer
- Handles numeric values (e.g., 783408079)

### Current Village/Nahia
- If numeric (e.g., 5), converts to "ناحیه 5"
- If text, uses as-is

### License Type Determination
- Checks `LicenseType_New` for "//" or "جدید" → "جدید"
- Checks `LicenseType_Renewal` for "//" or "تجدید" → "تجدید"
- Checks for "مثنی" → "مثنی"

### Date Parsing
- Format: DD/MM/YYYY (Persian calendar)
- Example: "5/1/1403" → 1403-01-05
- Invalid dates are stored as NULL

### Province & District Handling
- Kabul province always gets ID = 1
- Other provinces are looked up or created in `look.Location`
- Districts are looked up or created under their parent province

## Migration Statistics

After completion, you'll see:
```
=================================================================
PETITION WRITER MIGRATION COMPLETED
=================================================================
Total records processed: 271
Licenses created: 265
Records skipped: 6
Errors encountered: 0

Skipped Records:
  - Unknown (1403): Missing license number
  ...

=================================================================
```

## Common Issues & Solutions

### Issue 1: File Not Found
```
Warning: File 'petition_1403_records.json' not found. Skipping...
```
**Solution**: Ensure JSON files are in the `Backend/DataMigration` directory

### Issue 2: Connection Failed
```
Fatal Error: Connection refused
```
**Solution**: 
- Check PostgreSQL is running
- Verify connection string
- Check firewall settings

### Issue 3: Duplicate License Numbers
```
License 1050 already exists
```
**Solution**: This is normal - the migration skips existing licenses

### Issue 4: Invalid Date Format
Records with invalid dates will have NULL in the `LicenseIssueDate` field. Check the error log for details.

## Verification Queries

After migration, verify the data:

```sql
-- Count total licenses
SELECT COUNT(*) FROM org."PetitionWriterLicense";

-- Check by year
SELECT "CreatedBy", COUNT(*) 
FROM org."PetitionWriterLicense" 
GROUP BY "CreatedBy";

-- View sample records
SELECT "ApplicantName", "LicenseNumber", "LicenseIssueDate", "LicenseType"
FROM org."PetitionWriterLicense"
ORDER BY "LicenseNumber"
LIMIT 10;

-- Check for missing data
SELECT COUNT(*) FROM org."PetitionWriterLicense" 
WHERE "LicenseNumber" IS NULL OR "ApplicantName" IS NULL;

-- Check license types
SELECT "LicenseType", COUNT(*) 
FROM org."PetitionWriterLicense" 
GROUP BY "LicenseType";
```

## Rollback (If Needed)

To remove migrated data:

```sql
-- Remove all petition writer licenses from migration
DELETE FROM org."PetitionWriterLicense" 
WHERE "CreatedBy" LIKE 'MIGRATION_SCRIPT_%';

-- Or remove specific year
DELETE FROM org."PetitionWriterLicense" 
WHERE "CreatedBy" = 'MIGRATION_SCRIPT_1403';
```

## Data Quality Notes

1. **Missing License Numbers**: Records without license numbers are skipped
2. **Duplicate Prevention**: Existing licenses are not re-imported
3. **Province Handling**: All licenses are registered in Kabul (ProvinceId = 1)
4. **Date Validation**: Invalid dates are stored as NULL
5. **String Truncation**: Long fields are truncated to fit database constraints

## Support

For issues or questions:
1. Check the error log in the console output
2. Review the `Errors` and `Skipped` sections in the statistics
3. Verify the JSON file format matches the expected structure
4. Check database table structure matches the migration code

## Next Steps

After successful migration:
1. Verify data integrity using the verification queries
2. Test the Petition Writer License module in the application
3. Create backups of the migrated data
4. Document any data quality issues found
