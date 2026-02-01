# Company License Status Flow Implementation

## Overview
This implementation adds a status-based print restriction for the company module, similar to the real estate/property flow. The print button will be disabled until all required fields are completed.

## Changes Made

### 1. Backend Changes

#### Database Migration
- **File**: `Backend/Scripts/add_iscomplete_to_license.sql`
- **Action**: Run this script to add the `IsComplete` column to the `LicenseDetails` table
- **Command**: Execute the SQL script against your PostgreSQL database

#### Models
- **File**: `Backend/Models/Company/LicenseDetail.cs`
- Already has `IsComplete` property (bool)

#### Services
- **File**: `Backend/Services/CompanyService.cs`
- Added `UpdateLicenseCompletionStatusAsync(int companyId)` method
- This method checks if all required fields are filled:
  - Company has a title
  - Company has at least one owner with FirstName, FatherName, and ElectronicNationalIdNumber
  - Company has at least one license with LicenseNumber, IssueDate, ExpireDate, and OfficeAddress
  - Company has at least one guarantor with FirstName, FatherName, and ElectronicNationalIdNumber

#### Controllers Updated
1. **CompanyDetailsController.cs**
   - Updated `GetAll()` to include `isComplete` in the response
   - Updated `GetExpiredLicense()` to include `isComplete` in the response

2. **LicenseDetailController.cs**
   - Injected `ICompanyService`
   - Calls `UpdateLicenseCompletionStatusAsync()` after saving/updating license
   - Removed duplicate local method

3. **CompanyOwnerController.cs**
   - Injected `ICompanyService`
   - Calls `UpdateLicenseCompletionStatusAsync()` after saving/updating owner

4. **GuaranatorController.cs**
   - Injected `ICompanyService`
   - Calls `UpdateLicenseCompletionStatusAsync()` after saving/updating guarantor

### 2. Frontend Changes

#### Models
- **File**: `Frontend/src/app/models/companydetails.ts`
- Added `isComplete: boolean` to `companydetailsList` interface

#### Components
- **File**: `Frontend/src/app/realestate/realestatelist/realestatelist.component.html`
- Added "حالت" (Status) column to the table
- Shows status badge (تکمیل شده/ناقص) based on `isComplete`
- Print button is now disabled when `!property.isComplete`
- Shows "تکیمل ناشده" (Incomplete) for missing owner/guarantor fields

## How It Works

1. **When data is saved/updated**:
   - Any time a CompanyOwner, LicenseDetail, or Guarantor is saved or updated
   - The system automatically calls `UpdateLicenseCompletionStatusAsync()`
   - This method checks all required fields and updates the `IsComplete` flag

2. **In the list view**:
   - Each company shows its completion status
   - Incomplete records show red "ناقص" badge
   - Complete records show green "تکمیل شده" badge
   - Missing fields show "تکیمل ناشده" in red

3. **Print restriction**:
   - Print button is disabled (grayed out) when `isComplete = false`
   - Print button is enabled only when all required fields are filled

## Required Fields for Completion

A company license is considered complete when ALL of the following are present:

1. **Company Details**:
   - Title (not empty)

2. **Company Owner** (at least one):
   - FirstName
   - FatherName
   - ElectronicNationalIdNumber

3. **License Details** (at least one):
   - LicenseNumber
   - IssueDate
   - ExpireDate
   - OfficeAddress

4. **Guarantor** (at least one):
   - FirstName
   - FatherName
   - ElectronicNationalIdNumber

## Installation Steps

1. **Run the database migration**:
   ```bash
   psql -U postgres -d PRMIS -f Backend/Scripts/add_iscomplete_to_license.sql
   ```

2. **Rebuild the backend**:
   ```bash
   cd Backend
   dotnet build
   ```

3. **Restart the backend service**:
   ```bash
   dotnet run
   # or restart your service
   ```

4. **Rebuild the frontend** (if needed):
   ```bash
   cd Frontend
   npm run build
   ```

5. **Test the implementation**:
   - Create a new company with incomplete data
   - Verify the status shows as "ناقص" (Incomplete)
   - Verify the print button is disabled
   - Complete all required fields
   - Verify the status changes to "تکمیل شده" (Complete)
   - Verify the print button is now enabled

## Notes

- The status is automatically updated whenever any related data changes
- Existing records will be evaluated when the migration script runs
- The implementation follows the same pattern as the property/real estate module
- All changes are backward compatible
