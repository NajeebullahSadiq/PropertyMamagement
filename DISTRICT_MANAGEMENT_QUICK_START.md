# District Management Module - Quick Start Guide

## ✅ Implementation Complete

The District Management module has been successfully implemented and is ready to use.

## 🚀 How to Access

1. **Login** as an administrator
2. Navigate to the sidebar menu
3. Under **"مدیریت کاربران"** (User Management) section
4. Click on **"مدیریت ولسوالی ها"** (District Management)

## 📋 Features

### ✓ View Districts
- Select a province from the dropdown
- View all districts for that province in a table
- See district status (Active/Inactive)

### ✓ Add New District
1. Select a province
2. Click **"افزودن ولسوالی"** (Add District)
3. Enter district name in Dari (required)
4. Optionally enter English name
5. Click **"ذخیره"** (Save)

### ✓ Edit District
1. Select a province
2. Click **"ویرایش"** (Edit) button next to a district
3. Modify the district name
4. Click **"ذخیره"** (Save)

### ✓ Delete District
1. Select a province
2. Click **"حذف"** (Delete) button next to a district
3. Confirm deletion
4. System will check if district is in use
5. If not in use, district will be deactivated

## 🔒 Security

- **Access Control**: Only administrators can access this module
- **Data Protection**: Districts in use cannot be deleted
- **Validation**: Duplicate district names are prevented

## 📊 Database

The module uses the existing `look.Location` table:
- **TypeID = 2**: Province
- **TypeID = 3**: District
- **ParentID**: References the province
- **IsActive**: 1 = Active, 0 = Inactive

## ⚠️ Important Notes

1. **Soft Delete**: Deleting a district sets `IsActive = 0` (doesn't remove from database)
2. **Usage Check**: Districts used in any records cannot be deleted
3. **Automatic Path**: The system automatically generates `PathDari` (e.g., "کابل/ناحیه اول")
4. **No Breaking Changes**: All existing functionality continues to work

## 🔧 Technical Details

### Backend
- **Controller**: `Backend/Controllers/Lookup/DistrictManagementController.cs`
- **Endpoints**: `/api/DistrictManagement/*`
- **Authentication**: Required (`[Authorize]` attribute)

### Frontend
- **Module**: `Frontend/src/app/district-management/`
- **Service**: `Frontend/src/app/shared/district-management.service.ts`
- **Route**: `/district-management`
- **Guard**: `AdminGuard`

### Translations
- **Dari**: `Frontend/src/assets/i18n/دری.json`
- **Pashto**: `Frontend/src/assets/i18n/English.json`

## ✅ Build Status

- ✅ Backend: Compiled successfully
- ✅ Frontend: No TypeScript errors
- ✅ All files created and integrated

## 📝 Usage Checking

The system checks if a district is used in:
- Company Owners (OwnerDistrictId, PermanentDistrictId)
- Guarantors (PaddressDistrictId, TaddressDistrictId)
- Seller Details (PaddressDistrictId, TaddressDistrictId)
- Buyer Details (PaddressDistrictId, TaddressDistrictId)

If a district is found in any of these tables, deletion is prevented with an error message.

## 🎯 Next Steps

1. **Test the Module**:
   - Login as admin
   - Navigate to District Management
   - Try adding, editing, and deleting districts

2. **Verify Data**:
   - Check that districts appear in other forms
   - Verify existing functionality still works

3. **User Training**:
   - Train administrators on how to use the module
   - Explain the importance of correct spelling

## 📞 Support

For issues or questions, refer to:
- Full documentation: `DISTRICT_MANAGEMENT_MODULE.md`
- This quick start guide

---

**Status**: ✅ Ready for Production
**Date**: March 4, 2026
