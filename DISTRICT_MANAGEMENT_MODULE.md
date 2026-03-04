# District Management Module Implementation

## Overview
This document describes the implementation of the District Management module that allows administrators to add, edit, and delete districts for each province in the system.

## Implementation Date
March 4, 2026

## Features Implemented

### 1. Backend API Controller
**File:** `Backend/Controllers/Lookup/DistrictManagementController.cs`

**Endpoints:**
- `GET /api/DistrictManagement/provinces` - Get all provinces
- `GET /api/DistrictManagement/province/{provinceId}` - Get all districts for a specific province
- `GET /api/DistrictManagement/{id}` - Get a single district by ID
- `POST /api/DistrictManagement` - Create a new district
- `PUT /api/DistrictManagement/{id}` - Update an existing district
- `DELETE /api/DistrictManagement/{id}` - Soft delete a district (sets IsActive = 0)

**Features:**
- Province selection and validation
- District name validation (prevents duplicates within same province)
- Automatic path generation (Province/District)
- Soft delete with usage checking
- Checks if district is used in: CompanyOwners, Guarantors, SellerDetails, BuyerDetails
- Prevents deletion if district is in use

### 2. Frontend Module
**Location:** `Frontend/src/app/district-management/`

**Components:**
- `district-management.component.ts` - Main component logic
- `district-management.component.html` - UI template
- `district-management.component.scss` - Styling
- `district-management.module.ts` - Module definition

**Service:**
- `Frontend/src/app/shared/district-management.service.ts` - API communication service

**Features:**
- Province dropdown selection
- Districts list for selected province
- Add new district form
- Edit existing district
- Delete district with confirmation
- Real-time validation
- Loading states
- Error handling with toast notifications
- Bilingual support (Dari/Pashto)

### 3. Routing Configuration
**File:** `Frontend/src/app/app-routing.module.ts`

**Route:** `/district-management`
- Protected by `AdminGuard` (only administrators can access)
- Lazy loaded module
- Integrated with master layout

### 4. Navigation Menu
**File:** `Frontend/src/app/dashboard/masterlayout/masterlayout.component.html`

**Menu Item:**
- Added under "مدیریت کاربران" (User Management) section
- Icon: `location_city`
- Label: "مدیریت ولسوالی ها" (District Management)
- Only visible to administrators

### 5. Translations
**Files:**
- `Frontend/src/assets/i18n/دری.json` - Dari translations
- `Frontend/src/assets/i18n/English.json` - Pashto translations

**Translation Keys:**
```
DISTRICT_MANAGEMENT.TITLE
DISTRICT_MANAGEMENT.SELECT_PROVINCE
DISTRICT_MANAGEMENT.CHOOSE_PROVINCE
DISTRICT_MANAGEMENT.ADD_DISTRICT
DISTRICT_MANAGEMENT.EDIT_DISTRICT
DISTRICT_MANAGEMENT.ADD_NEW_DISTRICT
DISTRICT_MANAGEMENT.DISTRICT_NAME_DARI
DISTRICT_MANAGEMENT.DISTRICT_NAME_ENGLISH
DISTRICT_MANAGEMENT.DARI_REQUIRED
DISTRICT_MANAGEMENT.SAVE
DISTRICT_MANAGEMENT.CANCEL
DISTRICT_MANAGEMENT.DISTRICTS_LIST
DISTRICT_MANAGEMENT.NO_DISTRICTS
DISTRICT_MANAGEMENT.STATUS
DISTRICT_MANAGEMENT.ACTIVE
DISTRICT_MANAGEMENT.INACTIVE
DISTRICT_MANAGEMENT.ACTIONS
DISTRICT_MANAGEMENT.EDIT
DISTRICT_MANAGEMENT.DELETE
```

## Database Structure

### Location Table (look.Location)
The module uses the existing `Location` table with the following structure:

| Column | Type | Description |
|--------|------|-------------|
| ID | INTEGER | Primary key |
| Dari | VARCHAR(255) | District name in Dari (required) |
| Name | VARCHAR(255) | District name in English (optional) |
| ParentID | INTEGER | Reference to Province ID |
| TypeID | INTEGER | 2 = Province, 3 = District |
| IsActive | INTEGER | 1 = Active, 0 = Inactive |
| PathDari | VARCHAR(255) | Full path (e.g., "کابل/ناحیه اول") |

## User Workflow

### Adding a District
1. Administrator navigates to "مدیریت ولسوالی ها" from the menu
2. Selects a province from the dropdown
3. Clicks "افزودن ولسوالی" (Add District) button
4. Fills in the district name in Dari (required) and English (optional)
5. Clicks "ذخیره" (Save)
6. System validates and creates the district
7. Success message is displayed
8. District appears in the list

### Editing a District
1. Administrator selects a province
2. Clicks "ویرایش" (Edit) button next to a district
3. Modifies the district name
4. Clicks "ذخیره" (Save)
5. System validates and updates the district
6. Success message is displayed

### Deleting a District
1. Administrator selects a province
2. Clicks "حذف" (Delete) button next to a district
3. Confirms the deletion in the popup
4. System checks if district is in use
5. If not in use: District is soft deleted (IsActive = 0)
6. If in use: Error message is displayed
7. Success/error message is shown

## Security

### Access Control
- Only users with `ADMIN` role can access the module
- Protected by `AdminGuard` in routing
- Backend endpoints require authentication via `[Authorize]` attribute

### Validation
- Duplicate district names within same province are prevented
- Province existence is validated before creating districts
- District usage is checked before deletion

## Error Handling

### Backend Errors
- Province not found: 400 Bad Request
- District not found: 404 Not Found
- Duplicate district name: 400 Bad Request
- District in use (cannot delete): 400 Bad Request
- Server errors: 500 Internal Server Error

### Frontend Errors
- All errors are displayed using toast notifications
- Loading states prevent multiple submissions
- Form validation prevents invalid data submission

## Integration Points

### Existing Functionality
The module integrates seamlessly with existing functionality:
- All existing province/district dropdowns continue to work
- No changes required to existing forms
- District data is immediately available to all modules
- Soft delete ensures data integrity (districts in use cannot be deleted)

### Affected Modules
Districts are used in the following modules:
- Company Module (CompanyOwners, Guarantors)
- Property Module (SellerDetails, BuyerDetails)
- Vehicle Module (VehiclesSellerDetail, VehiclesBuyerDetail)
- License Applications (LicenseApplicationGuarantor)
- Petition Writer License (PetitionWriterLicense)

## Testing Recommendations

### Manual Testing
1. Test adding districts for different provinces
2. Test editing district names
3. Test deleting unused districts
4. Test attempting to delete districts in use
5. Test duplicate name validation
6. Test with different user roles (only admin should access)
7. Test language switching (Dari/Pashto)

### Data Validation
1. Verify PathDari is correctly generated
2. Verify IsActive flag is set correctly
3. Verify TypeID is always 3 for districts
4. Verify ParentID correctly references province

## Future Enhancements

### Potential Improvements
1. Bulk import/export of districts
2. District activation/deactivation toggle
3. Search and filter functionality
4. Pagination for large district lists
5. Audit trail for district changes
6. District merge functionality
7. Province-level district templates

## Notes

### Important Considerations
- The module uses soft delete (IsActive = 0) to maintain data integrity
- Districts in use cannot be deleted to prevent orphaned records
- The PathDari field is automatically generated and maintained
- All existing functionality remains unchanged
- The module is only accessible to administrators

### Maintenance
- Regular review of inactive districts
- Periodic cleanup of unused districts
- Monitor for spelling corrections needed
- Keep translations up to date

## Conclusion
The District Management module provides a user-friendly interface for administrators to manage districts without requiring direct database access. It maintains data integrity through validation and usage checking while providing a seamless experience for users.
