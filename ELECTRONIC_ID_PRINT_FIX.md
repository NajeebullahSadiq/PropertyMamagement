# Electronic National ID Number Print Fix

## Issue
The electronic Tazkira number (تذکره الکترونیکی) was not appearing in the company license print, showing as "—" instead.

## Root Cause
The `LicenseView` database view was not properly mapping the `ElectronicNationalIdNumber` field from the `CompanyOwner` table to the `IndentityCardNumber` field in the view.

## Solution
Recreate the `LicenseView` with the correct field mapping.

## Files

### SQL Script
**File:** `Backend/Scripts/fix_license_view_electronic_id.sql`

This script:
1. Drops the existing `LicenseView`
2. Recreates it with proper mapping: `co."ElectronicNationalIdNumber" AS "IndentityCardNumber"`
3. Verifies the fix by querying records with non-null electronic IDs

### Frontend (No Changes Needed)
The frontend is already correctly configured:
- **File:** `Frontend/src/app/print-license/print-license.component.html` (line 155)
- **Code:** `{{data.indentityCardNumber || '—'}}`

The field name `indentityCardNumber` (camelCase) correctly matches the view's `IndentityCardNumber` field after conversion.

## Deployment Steps

### 1. Run SQL Script
```bash
psql -U your_username -d PRMIS -f Backend/Scripts/fix_license_view_electronic_id.sql
```

Or execute in pgAdmin/your PostgreSQL client.

### 2. Verify Fix
After running the script:
1. Open a company license in the application
2. Click "Print" (چاپ)
3. Verify that the electronic Tazkira number now appears in the print

## Technical Details

### Database Structure
- **Table:** `org."CompanyOwner"`
- **Column:** `ElectronicNationalIdNumber` (VARCHAR/TEXT)

### View Mapping
- **View:** `public."LicenseView"`
- **Mapping:** `co."ElectronicNationalIdNumber" AS "IndentityCardNumber"`

### Frontend Data Flow
1. Backend returns view data with `IndentityCardNumber` field
2. TypeScript converts to camelCase: `indentityCardNumber`
3. HTML template displays: `{{data.indentityCardNumber}}`

## Note on Field Naming
The field name `IndentityCardNumber` contains a typo ("Indentity" instead of "Identity"). This is a legacy naming issue that exists throughout the codebase. While not ideal, we maintain consistency by keeping the same name to avoid breaking changes across multiple tables and views.

## Status

### ✅ Technical Fix: COMPLETE
The LicenseView has been successfully recreated with correct field mappings.

### ⚠️ Data Entry: ACTION REQUIRED
**The electronic ID field is NULL in the database for company ID 7.**

The view is now correctly configured, but the actual data needs to be entered. The user must:
1. Open the company owner form in the application
2. Navigate to the "معلومات تذکره" (ID Card Information) section
3. Enter the electronic Tazkira number in the field: **"الیکټرونیکی تذکره نمبر"**
4. Click "تغیر معلومات" (Update Information) to save

**See `ELECTRONIC_ID_DATA_ENTRY_GUIDE.md` for detailed step-by-step instructions.**

### Verification Query
```sql
-- Check if data exists for company 7
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "ElectronicNationalIdNumber",
    "CompanyId"
FROM org."CompanyOwner"
WHERE "CompanyId" = 7;
```

---

**Date:** January 25, 2026
**Issue Reporter:** User
**Fixed By:** Kiro AI Assistant
