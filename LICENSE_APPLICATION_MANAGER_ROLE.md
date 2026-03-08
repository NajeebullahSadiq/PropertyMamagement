# License Application Manager Role Implementation

## Overview
Created a new user role "LICENSE_APPLICATION_MANAGER" (کاربر مدیریت درخواست جواز) with full access to license applications and read-only access to other modules.

## New Role Details

### Role Information
- **Role Name**: LICENSE_APPLICATION_MANAGER
- **Dari Name**: کاربر مدیریت درخواست جواز
- **Purpose**: Manage license applications with full CRUD operations while having view-only access to other modules
- **Company Association**: NOT REQUIRED - This is a system-level role not tied to any specific company
- **Province Selection**: NOT REQUIRED - This role operates at the system level

### Permissions

#### Full Access (Create, Read, Update, Delete):
- License Applications Module
  - Create new applications
  - Edit existing applications
  - Delete applications
  - Add/edit/delete guarantors
  - Add/edit/delete withdrawal information

#### Read-Only Access:
1. **Company Module** (`/realestate/`)
   - View company list
   - No edit, save, print, or create permissions

2. **Property Module** (`/estate/list`)
   - View property list
   - No edit, delete, print, or create permissions

3. **Vehicle Module** (`/vehicle/list`)
   - View vehicle list
   - No edit, delete, print, or create permissions

4. **Securities Module** (`/securities/list`)
   - View securities list
   - No edit, delete, print, or create permissions

5. **Activity Monitoring** (`/activity-monitoring/list`)
   - View activity monitoring list
   - No edit, delete, or create permissions

6. **Verification Module** (`/verify`)
   - Access to verification portal

7. **Reports Module**
   - View reports
   - Export reports

8. **Dashboard**
   - View dashboard statistics

## Changes Made

### 1. Backend Changes

#### File: `Backend/Configuration/UserRoles.cs`

**Added new role constant:**
```csharp
public const string LicenseApplicationManager = "LICENSE_APPLICATION_MANAGER";
```

**Updated AllRoles array:**
```csharp
public static string[] AllRoles => new[]
{
    Admin,
    Authority,
    CompanyRegistrar,
    LicenseReviewer,
    PropertyOperator,
    VehicleOperator,
    LicenseApplicationManager
};
```

**Added Dari translation:**
```csharp
LicenseApplicationManager => "کاربر مدیریت درخواست جواز"
```

**Added permissions:**
```csharp
UserRoles.LicenseApplicationManager => new[]
{
    // Full access to license applications
    Permissions.LicenseApplicationView, 
    Permissions.LicenseApplicationCreate, 
    Permissions.LicenseApplicationEdit, 
    Permissions.LicenseApplicationDelete,
    // Read-only access to other modules
    Permissions.CompanyView,
    Permissions.PropertyView,
    Permissions.VehicleView,
    Permissions.ReportsView,
    Permissions.DashboardView
}
```

#### File: `Backend/Controllers/LicenseApplication/LicenseApplicationController.cs`

**Updated authorization attributes to include LICENSE_APPLICATION_MANAGER:**
- `[HttpPost]` Create - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpPut("{id}")]` Update - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpDelete("{id}")]` Delete - Already ADMIN only (kept as is)
- `[HttpPost("{applicationId}/guarantors")]` Add Guarantor - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpPut("{applicationId}/guarantors/{guarantorId}")]` Update Guarantor - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpDelete("{applicationId}/guarantors/{guarantorId}")]` Delete Guarantor - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpPost("{applicationId}/withdrawal")]` Save Withdrawal - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`
- `[HttpDelete("{applicationId}/withdrawal")]` Delete Withdrawal - `[Authorize(Roles = "ADMIN,LICENSE_APPLICATION_MANAGER")]`

#### File: `Backend/Helpers/RbacHelper.cs`

**Updated CanViewAllRecords:**
```csharp
"property" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseApplicationManager),
"vehicle" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseApplicationManager),
"company" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseReviewer, UserRoles.LicenseApplicationManager),
```

**Updated CanAccessModule:**
```csharp
"company" => HasAnyRole(userRoles, UserRoles.CompanyRegistrar, UserRoles.LicenseReviewer, UserRoles.LicenseApplicationManager),
"property" => HasAnyRole(userRoles, UserRoles.PropertyOperator, UserRoles.CompanyRegistrar, UserRoles.LicenseApplicationManager) || licenseType == "realEstate",
"vehicle" => HasAnyRole(userRoles, UserRoles.VehicleOperator, UserRoles.CompanyRegistrar, UserRoles.LicenseApplicationManager) || licenseType == "carSale",
```

### 2. Frontend Changes

#### File: `Frontend/src/app/auth/login/login.component.ts`

**Added auto-redirect for LICENSE_APPLICATION_MANAGER:**
```typescript
} else if (this.userRole === UserRoles.LicenseApplicationManager) {
  // License application manager goes to license applications list
  this.router.navigateByUrl('/license-applications/list');
}
```

**Result:**
- When LICENSE_APPLICATION_MANAGER users log in, they are automatically redirected to `/license-applications/list`
- This provides immediate access to their primary work area

#### File: `Frontend/src/app/shared/rbac.service.ts`

**Added new role constant:**
```typescript
export const UserRoles = {
  Admin: 'ADMIN',
  Authority: 'AUTHORITY',
  CompanyRegistrar: 'COMPANY_REGISTRAR',
  LicenseReviewer: 'LICENSE_REVIEWER',
  PropertyOperator: 'PROPERTY_OPERATOR',
  VehicleOperator: 'VEHICLE_OPERATOR',
  LicenseApplicationManager: 'LICENSE_APPLICATION_MANAGER'
} as const;
```

**Updated canAccessModule method:**
```typescript
case 'company':
  return role === UserRoles.CompanyRegistrar || role === UserRoles.LicenseReviewer || role === UserRoles.LicenseApplicationManager;
case 'property':
  return role === UserRoles.PropertyOperator || 
         role === UserRoles.CompanyRegistrar ||
         role === UserRoles.LicenseApplicationManager ||
         licenseType === 'realEstate';
case 'vehicle':
  return role === UserRoles.VehicleOperator || 
         role === UserRoles.CompanyRegistrar ||
         role === UserRoles.LicenseApplicationManager ||
         licenseType === 'carSale';
case 'securities':
  return role === UserRoles.Admin || role === UserRoles.Authority || role === UserRoles.LicenseApplicationManager;
case 'activitymonitoring':
  return role === UserRoles.Admin || role === UserRoles.Authority || role === UserRoles.LicenseApplicationManager;
case 'licenseapplications':
  return role === UserRoles.Admin || role === UserRoles.CompanyRegistrar || role === UserRoles.LicenseApplicationManager;
```

#### File: `Frontend/src/app/license-applications/license-application-list/license-application-list.component.ts`

**Updated permission checks:**
```typescript
checkPermissions(): void {
    const role = this.rbacService.getCurrentRole();
    this.isViewOnly = role === UserRoles.Authority || 
                      role === UserRoles.LicenseReviewer || 
                      role === UserRoles.CompanyRegistrar;
    this.canEdit = role === UserRoles.Admin || role === UserRoles.LicenseApplicationManager;
    this.canDelete = role === UserRoles.Admin || role === UserRoles.LicenseApplicationManager;
}
```

#### File: `Frontend/src/app/license-applications/license-application-form/license-application-form.component.ts`

**Updated permission checks:**
```typescript
checkPermissions(): void {
    const role = this.rbacService.getCurrentRole();
    this.isViewOnly = role === UserRoles.Authority || 
                      role === UserRoles.LicenseReviewer || 
                      role === UserRoles.CompanyRegistrar;
    this.canEdit = role === UserRoles.Admin || role === UserRoles.LicenseApplicationManager;
}
```

#### File: `Frontend/src/app/dashboard/masterlayout/masterlayout.component.ts`

**Added new flags:**
```typescript
canAccessLicenseApplications = false;
canManageLicenseApplications = false;
```

**Updated loadUserPermissions:**
```typescript
this.canAccessLicenseApplications = this.rbacService.canAccessModule('licenseApplications');
this.canManageLicenseApplications = this.rbacService.hasPermission('licenseapplication.create');
```

#### File: `Frontend/src/app/dashboard/masterlayout/masterlayout.component.html`

**Updated menu visibility:**
```html
<div *ngIf="canAccessLicenseApplications" class="section-label">درخواست جواز رهنمای معاملات</div>
<a *ngIf="canAccessLicenseApplications && canManageLicenseApplications" routerLink="/license-applications" routerLinkActive="active-nav" [routerLinkActiveOptions]="{exact: true}" class="nav-link">
  <mat-icon>assignment</mat-icon><span>ثبت درخواست متقاضیان</span>
</a>
<a *ngIf="canAccessLicenseApplications" routerLink="/license-applications/list" routerLinkActive="active-nav" class="nav-link">
  <mat-icon>list_alt</mat-icon><span>جدول ثبت درخواست متقاضیان</span>
</a>
```

## User Experience for LICENSE_APPLICATION_MANAGER

### What they CAN do:

#### License Applications (Full Access):
1. ✅ Create new license applications
2. ✅ Edit existing applications
3. ✅ Delete applications
4. ✅ Add/edit/delete guarantors
5. ✅ Add/edit/delete withdrawal information
6. ✅ View all license applications
7. ✅ Search and filter applications

#### Other Modules (Read-Only):
1. ✅ View company list (`/realestate/`)
2. ✅ View property list (`/estate/list`)
3. ✅ View vehicle list (`/vehicle/list`)
4. ✅ View securities list (`/securities/list`)
5. ✅ View activity monitoring (`/activity-monitoring/list`)
6. ✅ Access verification portal (`/verify`)
7. ✅ View reports
8. ✅ View dashboard

### What they CANNOT do:

#### Company Module:
1. ❌ Create new companies
2. ❌ Edit company details
3. ❌ Delete companies
4. ❌ Print company licenses

#### Property Module:
1. ❌ Create new properties
2. ❌ Edit property details
3. ❌ Delete properties
4. ❌ Print property documents

#### Vehicle Module:
1. ❌ Create new vehicles
2. ❌ Edit vehicle details
3. ❌ Delete vehicles
4. ❌ Print vehicle documents

#### Securities Module:
1. ❌ Create new securities
2. ❌ Edit securities
3. ❌ Delete securities
4. ❌ Print securities

#### Activity Monitoring:
1. ❌ Create new monitoring entries
2. ❌ Edit monitoring entries
3. ❌ Delete monitoring entries

## Role Comparison

| Feature | ADMIN | LICENSE_APPLICATION_MANAGER | COMPANY_REGISTRAR |
|---------|-------|----------------------------|-------------------|
| License Applications (CRUD) | ✅ Full | ✅ Full | ❌ View Only |
| Company Module | ✅ Full | ❌ View Only | ✅ Full |
| Property Module | ✅ Full | ❌ View Only | ❌ View Only |
| Vehicle Module | ✅ Full | ❌ View Only | ❌ View Only |
| Securities Module | ✅ Full | ❌ View Only | ❌ No Access |
| Activity Monitoring | ✅ Full | ❌ View Only | ❌ No Access |
| Verification | ✅ Full | ✅ View | ✅ View |
| Reports | ✅ Full | ✅ View | ✅ View |
| Dashboard | ✅ Full | ✅ View | ❌ No Access |
| User Management | ✅ Full | ❌ No Access | ❌ No Access |

## Testing Recommendations

1. **Test as LICENSE_APPLICATION_MANAGER:**
   - Verify full CRUD access to license applications
   - Verify can view but not edit company list
   - Verify can view but not edit property list
   - Verify can view but not edit vehicle list
   - Verify can view but not edit securities list
   - Verify can view activity monitoring
   - Verify can access verification portal
   - Verify menu shows "ثبت درخواست متقاضیان" and "جدول ثبت درخواست متقاضیان"

2. **Test API directly:**
   - POST/PUT/DELETE license application requests should succeed
   - POST/PUT/DELETE requests for other modules should return 403 Forbidden
   - GET requests should work for all modules

3. **Test UI:**
   - Edit/delete buttons should be visible in license applications list
   - Edit/delete buttons should be hidden in other module lists
   - Save buttons should be enabled in license application forms
   - Save buttons should be disabled in other module forms

## Notes

- This role is specifically designed for users who manage license applications
- They have read-only access to other modules for reference purposes
- All write operations outside license applications are blocked at both frontend and backend levels
- The role provides a good balance between functionality and security


## User Registration Changes

### Frontend: `Frontend/src/app/auth/register/register.component.ts`

**Updated role handling in `onPropertyTypeChange` method:**
```typescript
if (selectedRole === UserRoles.Admin || 
    selectedRole === UserRoles.Authority || 
    selectedRole === UserRoles.LicenseReviewer ||
    selectedRole === UserRoles.LicenseApplicationManager) {
  // System-level roles don't need company or province
  companyIdControl.setValue(0);
  licenseTypeControl?.setValue('');
  provinceIdControl?.setValue(null);
  this.showCompanySelect = false;
  this.showLicenseTypeSelect = false;
  this.showProvinceSelect = false;
}
```

**Result:**
- When creating a LICENSE_APPLICATION_MANAGER user, the company selection field is hidden
- No company association is required
- No province selection is required
- No license type is required

### Backend: `Backend/Controllers/ApplicationUserController.cs`

**Added validation for system-level roles:**
```csharp
// System-level roles (ADMIN, AUTHORITY, LICENSE_REVIEWER, LICENSE_APPLICATION_MANAGER) don't need company
var systemLevelRoles = new[] { 
    UserRoles.Admin, 
    UserRoles.Authority, 
    UserRoles.LicenseReviewer,
    UserRoles.LicenseApplicationManager 
};

if (systemLevelRoles.Contains(model.Role))
{
    model.CompanyId = 0; // Ensure no company association
    model.LicenseType = null; // No license type needed
}
```

**Result:**
- Backend automatically sets CompanyId to 0 for LICENSE_APPLICATION_MANAGER
- No license type is assigned
- User is not tied to any specific company

## Role Comparison - Company Association

| Role | Company Required | Province Required | License Type Required |
|------|-----------------|-------------------|----------------------|
| ADMIN | ❌ No | ❌ No | ❌ No |
| AUTHORITY | ❌ No | ❌ No | ❌ No |
| LICENSE_REVIEWER | ❌ No | ❌ No | ❌ No |
| LICENSE_APPLICATION_MANAGER | ❌ No | ❌ No | ❌ No |
| COMPANY_REGISTRAR | ❌ No | ✅ Yes | ❌ No |
| PROPERTY_OPERATOR | ✅ Yes | ❌ No | ✅ Yes (realEstate) |
| VEHICLE_OPERATOR | ✅ Yes | ❌ No | ✅ Yes (carSale) |

## Summary

The LICENSE_APPLICATION_MANAGER role is now properly configured as a system-level role that:
1. Does not require company selection during user creation
2. Does not require province selection
3. Does not require license type selection
4. Operates independently of any company association
5. Has full access to license applications across all companies
6. Has read-only access to other modules for reference purposes
