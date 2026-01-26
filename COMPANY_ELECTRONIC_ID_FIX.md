# Company Module Electronic ID Column Type Fix

## Problem
The `/api/CompanyDetails` endpoint was returning a 500 Internal Server Error with the following exception:

```
System.InvalidCastException: Reading as 'System.String' is not supported for fields having DataTypeName 'double precision'
```

## Root Cause
The `ElectronicNationalIdNumber` column in the Company module tables (`CompanyOwner` and `Guarantor`) was stored as `double precision` in PostgreSQL, but the C# models expected it to be a `string` type.

This type mismatch occurred because:
1. The column was initially created with the wrong data type
2. Electronic National ID numbers should be stored as text/varchar, not numeric types
3. The same issue was previously fixed in Property module tables but not in Company module tables

## Solution
Convert the `ElectronicNationalIdNumber` column from `double precision` to `VARCHAR(50)` in both:
- `org.CompanyOwner` table
- `org.Guarantor` table

## Deployment Steps

### On Production Server (Linux)

1. **Navigate to the scripts directory:**
   ```bash
   cd ~/PropertyMamagement/Backend/Scripts
   ```

2. **Make the deployment script executable:**
   ```bash
   chmod +x deploy_company_electronic_id_fix.sh
   ```

3. **Run the deployment script:**
   ```bash
   ./deploy_company_electronic_id_fix.sh
   ```

4. **When prompted, type `yes` to confirm**

5. **Restart the backend service:**
   ```bash
   sudo systemctl restart prmis-backend
   ```

6. **Verify the service is running:**
   ```bash
   sudo systemctl status prmis-backend
   ```

7. **Test the endpoint:**
   ```bash
   curl http://103.132.98.92/api/CompanyDetails
   ```

### Manual Deployment (Alternative)

If you prefer to run the SQL directly:

```bash
cd ~/PropertyMamagement/Backend/Scripts
psql -h localhost -U postgres -d prmis -f fix_company_electronic_id_columns.sql
sudo systemctl restart prmis-backend
```

### On Development (Windows)

1. **Navigate to the scripts directory:**
   ```cmd
   cd Backend\Scripts
   ```

2. **Run the deployment script:**
   ```cmd
   deploy_company_electronic_id_fix.bat
   ```

3. **When prompted, type `yes` to confirm**

## Verification

After applying the fix, verify the column types:

```sql
SELECT 
    table_schema,
    table_name,
    column_name,
    data_type,
    character_maximum_length
FROM information_schema.columns 
WHERE table_schema = 'org' 
AND table_name IN ('CompanyOwner', 'Guarantor')
AND column_name = 'ElectronicNationalIdNumber';
```

Expected result:
- `data_type`: `character varying`
- `character_maximum_length`: `50`

## Testing

1. **Test the main endpoint:**
   ```
   GET http://103.132.98.92/api/CompanyDetails
   ```
   Should return 200 OK with company list

2. **Test the expired licenses endpoint:**
   ```
   GET http://103.132.98.92/api/CompanyDetails/getexpired
   ```
   Should return 200 OK with expired license list

3. **Test the view endpoint:**
   ```
   GET http://103.132.98.92/api/CompanyDetails/GetView/{id}
   ```
   Should return 200 OK with detailed company information

## Files Modified/Created

1. **Backend/Scripts/fix_company_electronic_id_columns.sql** - SQL script to fix column types
2. **Backend/Scripts/deploy_company_electronic_id_fix.sh** - Linux deployment script
3. **Backend/Scripts/deploy_company_electronic_id_fix.bat** - Windows deployment script
4. **COMPANY_ELECTRONIC_ID_FIX.md** - This documentation

## Related Issues

This is similar to the fix previously applied to Property module tables in:
- `Backend/Scripts/fix_electronic_id_columns.sql`

The same pattern should be checked in other modules if similar errors occur.

## Rollback

If you need to rollback (not recommended as it will cause the error again):

```sql
ALTER TABLE org."CompanyOwner" 
ALTER COLUMN "ElectronicNationalIdNumber" TYPE double precision 
USING "ElectronicNationalIdNumber"::double precision;

ALTER TABLE org."Guarantor" 
ALTER COLUMN "ElectronicNationalIdNumber" TYPE double precision 
USING "ElectronicNationalIdNumber"::double precision;
```

## Notes

- The fix preserves all existing data
- NULL values are handled correctly
- The conversion is safe and will not lose data
- Electronic National ID numbers should always be stored as text to preserve leading zeros and handle non-numeric characters
