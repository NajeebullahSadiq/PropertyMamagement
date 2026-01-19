# Owner Address Field Changes Summary

## Changes Made

### 1. Made Owner Address Village Field Optional
- **Field**: `ګذر / قریه / ادرس بلاک` (Village/Street/Block Address) in the Owner's Own Address section
- **Change**: Removed the required validator, making this field optional
- **Location**: Company Owner form

### 2. Removed Temporary Address Section
- **Section Removed**: `آدرس موقت (اختیاری)` (Temporary Address - Optional)
- **Impact**: Completely removed from the company owner form, backend models, and database

## Files Modified

### Frontend Changes

1. **Frontend/src/app/realestate/companyowner/companyowner.component.ts**
   - Removed `temporaryDistrict` property
   - Removed `newTemporaryDistrict` property
   - Removed temporary address fields from form group
   - Removed `filterTemporaryDistricts()` method
   - Removed `filterNewTemporaryDistricts()` method
   - Removed temporary address getters
   - Removed temporary address from `currentAddressDisplay` interface
   - Removed temporary address handling in `addOwner()` and `updateOwner()` methods
   - Made `ownerVillage` field optional (removed required validator)

2. **Frontend/src/app/realestate/companyowner/companyowner.component.html**
   - Removed entire temporary address section
   - Removed temporary address from address change mode
   - Removed temporary address from current address display
   - Updated owner village field label to include "(اختیاری)" (optional)
   - Removed validation error display for owner village field

3. **Frontend/src/app/models/companyowner.ts**
   - Removed `temporaryProvinceId`, `temporaryDistrictId`, `temporaryVillage` properties
   - Removed `temporaryProvinceName`, `temporaryDistrictName` properties

### Backend Changes

1. **Backend/Models/Company/CompanyOwner.cs**
   - Removed `TemporaryProvinceId`, `TemporaryDistrictId`, `TemporaryVillage` properties
   - Removed `TemporaryProvince`, `TemporaryDistrict` navigation properties

2. **Backend/Models/RequestData/Company/CompanyOwnerData.cs**
   - Removed `TemporaryProvinceId`, `TemporaryDistrictId`, `TemporaryVillage` properties

3. **Backend/Controllers/Companies/CompanyOwnerController.cs**
   - Removed temporary address fields from GET endpoint
   - Removed temporary address assignment in POST endpoint
   - Removed temporary address assignment in PUT endpoint
   - Removed temporary address archiving logic from `ArchiveCurrentAddressToHistory()` method

4. **Backend/Controllers/Companies/LicenseDetailController.cs**
   - Removed temporary address fields from license view query
   - Removed temporary address columns from view creation SQL
   - Removed temporary address joins from view SQL

5. **Backend/Controllers/Companies/CompanyDetailsController.cs**
   - Removed temporary address fields from company details query

6. **Backend/Configuration/AppDbContext.cs**
   - Removed `TemporaryProvince` and `TemporaryDistrict` navigation property configurations

7. **Backend/Models/ViewModels/LicenseView.cs**
   - Removed `TemporaryProvinceName`, `TemporaryDistrictName`, `TemporaryVillage` properties

8. **Backend/src/Application/Company/DTOs/CompanyDetailDto.cs**
   - Removed `TemporaryProvinceName`, `TemporaryDistrictName`, `TemporaryVillage` properties from `CompanyOwnerViewDto`

9. **Backend/Configuration/DatabaseSeeder.cs**
   - Removed temporary address fields from LicenseView creation SQL
   - Removed temporary address joins from view SQL

10. **Backend/Migrations/AppDbContextModelSnapshot.cs**
    - Removed `TemporaryDistrictId`, `TemporaryProvinceId`, `TemporaryVillage` properties
    - Removed temporary address indexes
    - Removed temporary address navigation properties
    - Removed temporary address fields from LicenseView entity

### Database Migration

**Backend/Infrastructure/Migrations/Company/20260118_RemoveTemporaryAddressFields.cs**
- Created migration to drop temporary address columns from CompanyOwner table:
  - `TemporaryProvinceId`
  - `TemporaryDistrictId`
  - `TemporaryVillage`
- Includes rollback logic to restore columns if needed

## Database Changes

The migration will execute the following SQL:
```sql
ALTER TABLE org."CompanyOwner" DROP COLUMN "TemporaryProvinceId";
ALTER TABLE org."CompanyOwner" DROP COLUMN "TemporaryDistrictId";
ALTER TABLE org."CompanyOwner" DROP COLUMN "TemporaryVillage";
```

## Impact

1. **User Experience**: 
   - Simplified company owner form by removing unnecessary temporary address section
   - Made owner village field optional for more flexibility

2. **Data Model**: 
   - Cleaner data model without unused temporary address fields
   - Reduced database storage requirements

3. **API**: 
   - All API endpoints updated to exclude temporary address fields
   - Backward compatibility: Existing data will not be affected, only the columns will be removed

## Testing Recommendations

1. Test company owner creation without providing owner village
2. Test company owner creation with owner village
3. Test company owner update functionality
4. Test address change functionality
5. Verify license view displays correctly without temporary address
6. Run the migration on a test database first
7. Verify all company owner queries work correctly

## Migration Instructions

1. Backup your database before running the migration
2. Run the migration: `dotnet ef database update` or use your migration tool
3. Verify the columns have been removed from the CompanyOwner table
4. Test the application thoroughly

## Rollback

If you need to rollback these changes, the migration includes a `Down()` method that will restore the temporary address columns. However, any data that was in those columns will be lost.
