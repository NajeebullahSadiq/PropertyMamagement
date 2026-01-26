# Company Details API 500 Error - FINAL FIX

## Problem
The `/api/CompanyDetails` endpoint was returning a 500 Internal Server Error with the exception:
```
System.InvalidCastException: Reading as 'System.String' is not supported for fields having DataTypeName 'double precision'
```

## Root Cause
After extensive investigation, we found that the `LicenseNumber` column in the `LicenseDetails` table was stored as `double precision` in PostgreSQL, but the C# model expected a `string` type.

## Investigation History
1. Initially suspected `ElectronicNationalIdNumber` in `CompanyOwner` table - Fixed ✓
2. Checked `Guarantors` table - Already correct ✓
3. Ran comprehensive diagnostic to find ALL `double precision` columns in `org` schema
4. **Found the culprit**: `LicenseDetails.LicenseNumber` was `double precision`

## Solution
Convert `LicenseNumber` from `double precision` to `TEXT` in the `LicenseDetails` table.

## Deployment Steps

### Quick Deploy (Recommended)
```bash
cd ~/PropertyMamagement
git pull
chmod +x Backend/Scripts/deploy_license_number_fix.sh
./Backend/Scripts/deploy_license_number_fix.sh
```

### Manual Deploy
```bash
cd ~/PropertyMamagement
git pull

# Apply the database fix
sudo -u postgres psql -d PRMIS -f Backend/Scripts/fix_license_number_type.sql

# Restart backend service
sudo systemctl restart prmis-backend

# Test the endpoint
curl http://103.132.98.92/api/CompanyDetails
```

## Verification
After deployment, you should see:
- ✓ JSON data returned from the API (not 500/502 error)
- ✓ `LicenseNumber` column type is now `text` in database
- ✓ No more `double precision` columns in the `org` schema

## Files Changed
- `Backend/Scripts/fix_license_number_type.sql` - Database fix script
- `Backend/Scripts/deploy_license_number_fix.sh` - Automated deployment script
- `COMPANY_DETAILS_LICENSE_NUMBER_FIX.md` - This documentation

## Technical Details
- **Table**: `org.LicenseDetails`
- **Column**: `LicenseNumber`
- **Old Type**: `double precision`
- **New Type**: `TEXT`
- **C# Model**: `Backend/Models/Company/LicenseDetail.cs` expects `string?`

## Status
✅ Fix created and ready to deploy
