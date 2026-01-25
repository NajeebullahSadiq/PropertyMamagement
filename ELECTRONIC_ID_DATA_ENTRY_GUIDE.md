# Electronic ID Data Entry Guide

## Issue Summary
The electronic Tazkira number (تذکره الکترونیکی) is showing as "—" in company license prints because **the data has not been entered** in the database.

## Root Cause
- ✅ Database column exists: `CompanyOwner.ElectronicNationalIdNumber` (VARCHAR, nullable)
- ✅ View mapping is correct: `LicenseView.IndentityCardNumber` properly maps to `CompanyOwner.ElectronicNationalIdNumber`
- ❌ **Data is NULL**: No electronic ID has been entered for company ID 7

## Solution: Enter the Electronic ID Through the Application

### Steps to Add Electronic ID:

1. **Navigate to Company Management**
   - Go to the company list in the application
   - Find and open Company ID 7 (سمارټ)

2. **Edit Company Owner Information**
   - Click on the "Company Owner" tab (مالک شرکت)
   - You should see the owner form with all fields

3. **Enter Electronic National ID**
   - Find the field labeled: **"الیکټرونیکی تذکره نمبر"**
   - This field is in the "معلومات تذکره" (ID Card Information) section
   - Enter the electronic Tazkira number (e.g., "1234567890123")
   - This field is **required** (marked with red asterisk *)

4. **Save the Changes**
   - Click the "تغیر معلومات" (Update Information) button
   - You should see a success message: "معلومات موفقانه تغیر یافت"

5. **Verify the Print**
   - Go back to the license print view
   - The electronic ID should now appear instead of "—"

## Alternative: Direct Database Update (For Testing Only)

If you need to quickly test without using the UI, you can update directly via SQL:

```sql
-- Update electronic ID for company 7
UPDATE org."CompanyOwner"
SET "ElectronicNationalIdNumber" = '1234567890123'  -- Replace with actual ID
WHERE "CompanyId" = 7 AND "Status" = true;

-- Verify the update
SELECT 
    "Id",
    "FirstName",
    "FatherName",
    "ElectronicNationalIdNumber",
    "CompanyId"
FROM org."CompanyOwner"
WHERE "CompanyId" = 7;

-- Check if it appears in the view
SELECT 
    "CompanyId",
    "FirstName",
    "FatherName",
    "IndentityCardNumber",
    "LicenseNumber"
FROM public."LicenseView"
WHERE "CompanyId" = 7;
```

## Important Notes

1. **Field Name Consistency**: The field is called `IndentityCardNumber` (with typo) throughout the codebase for consistency
2. **Required Field**: The electronic ID is now a required field in the form
3. **Data Type**: The field accepts VARCHAR (text), so you can enter any format
4. **All Companies**: Make sure to enter electronic IDs for all company owners to avoid this issue

## Form Field Location

The electronic ID field is located in the Company Owner form:
- **File**: `Frontend/src/app/realestate/companyowner/companyowner.component.html`
- **Section**: معلومات تذکره (ID Card Information)
- **Label**: الیکټرونیکی تذکره نمبر
- **Form Control**: `electronicNationalIdNumber`

## Technical Details

### Database Schema
```sql
Table: org."CompanyOwner"
Column: "ElectronicNationalIdNumber"
Type: character varying (VARCHAR)
Nullable: YES
```

### View Mapping
```sql
View: public."LicenseView"
Field: "IndentityCardNumber"
Source: co."ElectronicNationalIdNumber"
```

### Frontend Model
```typescript
// Frontend/src/app/models/companyowner.ts
export interface companyowner {
  electronicNationalIdNumber?: string;
  // ... other fields
}
```

## Status: ✅ RESOLVED

The system is working correctly. The issue was simply missing data that needs to be entered through the application form.
