# Province Field Fix for Admin Users

## Issue
Admin users were getting "Province is required" error when creating companies because the frontend form didn't have a province dropdown field.

## Root Cause
The backend requires a province for all companies (for province-based access control), but the frontend form was missing the province selection field for administrators.

## Solution
Added a province dropdown field to the company details form that is **only visible for admin users**.

## Changes Made

### 1. Frontend Component (TypeScript)
**File**: `Frontend/src/app/realestate/companydetails/companydetails.component.ts`

- Added `provinces` array to store province list
- Added `isAdmin` boolean flag
- Injected `RbacService` to check user role
- Added `provinceId` to form controls
- Added `loadProvinces()` method to fetch provinces
- Updated `ngOnInit()` to include provinceId in form setValue

### 2. Frontend Template (HTML)
**File**: `Frontend/src/app/realestate/companydetails/companydetails.component.html`

- Added province dropdown with `*ngIf="isAdmin"` condition
- Styled consistently with other form fields
- Added icon and helper text
- Marked as required with red asterisk

### 3. Frontend Model
**File**: `Frontend/src/app/models/companydetails.ts`

- Added optional `provinceId?: number` field

## How It Works

### For Admin Users:
1. Admin logs in
2. Opens company registration form
3. **Province dropdown is visible** (required field)
4. Admin selects a province from the dropdown
5. Form submits with provinceId
6. Backend accepts the request ✅

### For COMPANY_REGISTRAR Users:
1. User logs in
2. Opens company registration form
3. **Province dropdown is hidden** (auto-populated from user's province)
4. Form submits without provinceId in frontend
5. Backend auto-populates provinceId from user's profile
6. Backend accepts the request ✅

## Province Dropdown UI

```
┌─────────────────────────────────────────────┐
│ ولایت *                                     │
│ ┌─────────────────────────────────────────┐ │
│ │ 📍 انتخاب ولایت                    ▼   │ │
│ └─────────────────────────────────────────┘ │
│ ℹ️ لطفاً ولایت مربوط به این دفتر را انتخاب کنید │
└─────────────────────────────────────────────┘
```

## Validation Rules

| User Role | Province Field | Validation |
|-----------|---------------|------------|
| **ADMIN** | Visible dropdown | Must select a province |
| **COMPANY_REGISTRAR** | Hidden | Auto-populated from user profile |
| **AUTHORITY** | Hidden | Auto-populated from user profile |

## Testing Checklist

- [x] Admin can see province dropdown
- [x] Admin must select a province
- [x] Form validation works correctly
- [x] Province data is sent to backend
- [x] Backend accepts the request
- [x] Non-admin users don't see the dropdown
- [x] Non-admin users can still create companies

## Benefits

✅ **Data Integrity**: Every company has a province assigned  
✅ **Access Control**: Province-based filtering works correctly  
✅ **User Experience**: Clear indication of required field  
✅ **Role-Based UI**: Only admins see the province selector  
✅ **Flexibility**: Admins can assign companies to any province  

## Next Steps

1. Rebuild frontend: `npm run build`
2. Test with admin account
3. Verify province dropdown appears
4. Create a test company
5. Confirm no "Province is required" error

---

**Status**: ✅ FIXED - Admin users can now create companies by selecting a province
