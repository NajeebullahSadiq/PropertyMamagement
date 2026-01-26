# Quick Fix for Company Details 500 Error

## Problem
`/api/CompanyDetails` returns 500 error: "Reading as 'System.String' is not supported for fields having DataTypeName 'double precision'"

## Quick Fix (Copy & Paste)

```bash
# 1. Navigate to scripts directory
cd ~/PropertyMamagement/Backend/Scripts

# 2. Run the fix
psql -h localhost -U postgres -d prmis -f fix_company_electronic_id_columns.sql

# 3. Restart backend
sudo systemctl restart prmis-backend

# 4. Check status
sudo systemctl status prmis-backend

# 5. Test the endpoint
curl http://103.132.98.92/api/CompanyDetails
```

## What This Does
Converts `ElectronicNationalIdNumber` column from `double precision` to `VARCHAR(50)` in:
- `org.CompanyOwner` table
- `org.Guarantor` table

## Expected Result
The `/api/CompanyDetails` endpoint should now return 200 OK with company data instead of 500 error.

---

For detailed information, see: **COMPANY_ELECTRONIC_ID_FIX.md**
