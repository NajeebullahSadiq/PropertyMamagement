# Add Activity Location Field to License

## Summary
Added a new optional text field "محل فعالیت" (Activity Location) to the license section of the company module. This field is also displayed in the print license view under "واقع ناحیه".

## Changes Made

### 1. Database
- **File**: `Backend/Scripts/add_activity_location_to_license.sql`
- Added `ActivityLocation` TEXT column to `org."LicenseDetails"` table
- Column is nullable (optional field)

### 2. Backend Models
- **LicenseDetail.cs**: Added `ActivityLocation` property
- **LicenseDetailData.cs**: Added `ActivityLocation` property
- **LicenseDtos.cs**: Added `ActivityLocation` to both CreateDto and UpdateDto

### 3. Backend Controllers
- **LicenseDetailController.cs**:
  - Updated POST method to handle `ActivityLocation`
  - Updated PUT method to handle `ActivityLocation`
  - Updated SQL queries in `getById` to include `ActivityLocation`
  - Updated SQL queries in PUT method to include `ActivityLocation`
  - **Updated `GetLicenseView` method**: Modified to return `ActivityLocation` as `ownerVillage` in the print data response (with fallback to actual OwnerVillage if ActivityLocation is not set)
  
- **CompanyDetailsController.cs**:
  - Updated `GetView` method to include `ActivityLocation` in License selection

### 4. Frontend Model
- **LicenseDetail.ts**: Added `activityLocation?: string` property

### 5. Frontend Component
- **licensedetails.component.html**:
  - Added new input field for "محل فعالیت" after "محل انتقال"
  - Field is marked as optional with "(اختیاری)" label
  
- **licensedetails.component.ts**:
  - Added `activityLocation` form control to `licenseForm`
  - Added `activityLocation` to prepopulation logic

### 6. Print License View
- **Backend**: The `GetLicenseView` endpoint now returns `ActivityLocation` as `ownerVillage` in the response
- **Frontend**: The print template already displays `ownerVillage` in the "واقع ناحیه" field, so it will automatically show the ActivityLocation value
- **Fallback**: If ActivityLocation is not set, it falls back to the owner's village from CompanyOwner table

## Field Details
- **Label**: محل فعالیت (Activity Location)
- **Type**: Text input
- **Required**: No (Optional)
- **Position**: Between "محل انتقال" and "آدرس دقیق دفتر" in the license form
- **Print Display**: Shows in "واقع ناحیه" field on the printed license

## Deployment Steps

### Production Database
Run this SQL script:
```sql
ALTER TABLE org."LicenseDetails"
ADD COLUMN IF NOT EXISTS "ActivityLocation" TEXT;

COMMENT ON COLUMN org."LicenseDetails"."ActivityLocation" IS 'محل فعالیت - Activity Location';
```

### Backend
- Backend code already updated and builds successfully
- Restart the backend service after deployment

### Frontend
- Frontend code already updated
- Rebuild and deploy the Angular application

## Testing
1. Open a company record
2. Navigate to the license tab
3. Verify the new "محل فعالیت" field appears between "محل انتقال" and "آدرس دقیق دفتر"
4. Enter a value (e.g., "کابل، ناحیه اول") and save
5. Reload the page and verify the value persists
6. Click the print button
7. Verify the ActivityLocation value appears in the "واقع ناحیه" field on the printed license
8. Test with an empty ActivityLocation to verify it falls back to the owner's village

## Notes
- Field is optional and can be left empty
- No validation rules applied
- Field is included in all license CRUD operations
- Field is included in the company view API response
- **Print behavior**: The ActivityLocation value takes priority over the owner's village in the print view
- If ActivityLocation is empty, the system falls back to displaying the owner's village from the CompanyOwner table
