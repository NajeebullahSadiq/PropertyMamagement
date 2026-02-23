# License Number Format Update - 8 Digits

## Changes Made

### 1. Backend Code Updated
- Updated `Backend/Services/LicenseNumberGenerator.cs`
- Changed format from 4 digits to 8 digits
- New format: `PROVINCE_CODE-00000001` (e.g., `KBL-00007418`)

### 2. Database Migration Script Created
- File: `Backend/Scripts/update_license_numbers_to_8_digits.sql`
- Updates all existing license numbers to 8-digit format

## How to Apply Changes

### Step 1: Backup Database
```bash
# Create a backup before running the update
pg_dump -U your_username -d your_database > backup_before_license_update.sql
```

### Step 2: Run the SQL Script
```bash
# Connect to your PostgreSQL database
psql -U your_username -d your_database -f Backend/Scripts/update_license_numbers_to_8_digits.sql
```

Or run it directly in your database tool (pgAdmin, DBeaver, etc.)

### Step 3: Restart Backend Server
```bash
# Stop the current backend server
# Then restart it to load the new code
cd Backend
dotnet run
```

## What Gets Updated

### Tables Affected:
1. `LicenseDetail` - Company licenses
2. `PetitionWriterLicense` - Petition writer licenses  
3. `LicenseApplication` - License applications

### Format Examples:
- Before: `KBL-7418`
- After: `KBL-00007418`

- Before: `HRT-123`
- After: `HRT-00000123`

- Before: `KHR-99999`
- After: `KHR-00099999`

## Verification

After running the script, you can verify the changes:

```sql
-- Check company licenses
SELECT "Id", "LicenseNumber", "CompanyId"
FROM "LicenseDetail"
WHERE "LicenseNumber" IS NOT NULL
ORDER BY "Id" DESC
LIMIT 20;

-- Check petition writer licenses
SELECT "Id", "LicenseNumber", "FirstName", "LastName"
FROM "PetitionWriterLicense"
WHERE "LicenseNumber" IS NOT NULL
ORDER BY "Id" DESC
LIMIT 20;
```

## Important Notes

1. The SQL script uses a transaction (BEGIN/COMMIT) for safety
2. Only license numbers with less than 8 digits will be updated
3. License numbers already in 8-digit format will not be changed
4. The script includes verification queries to show the results
5. New licenses created after the backend restart will automatically use 8-digit format

## Rollback (if needed)

If you need to rollback, restore from your backup:
```bash
psql -U your_username -d your_database < backup_before_license_update.sql
```
