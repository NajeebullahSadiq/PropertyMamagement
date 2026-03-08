# License Application Permissions Update

## Overview
Restricted the "کاربر ثبت جواز رهنما" (COMPANY_REGISTRAR) role to have view-only access to license applications. Only ADMIN users can now create, edit, or delete license applications.

## Changes Made

### 1. Backend - Permission System Updates

#### File: `Backend/Configuration/UserRoles.cs`

**Added new permissions:**
- `LicenseApplicationView` - View license applications
- `LicenseApplicationCreate` - Create license applications
- `LicenseApplicationEdit` - Edit license applications
- `LicenseApplicationDelete` - Delete license applications

**Updated role permissions:**
- **ADMIN**: Full access (view, create, edit, delete)
- **AUTHORITY**: View-only access
- **COMPANY_REGISTRAR**: View-only access (removed create/edit/delete)
- **LICENSE_REVIEWER**: View-only access

#### File: `Backend/Controllers/LicenseApplication/LicenseApplicationController.cs`

**Added authorization attributes to restrict endpoints to ADMIN only:**
- `[HttpPost]` Create - ADMIN only
- `[HttpPut("{id}")]` Update - ADMIN only
- `[HttpDelete("{id}")]` Delete - ADMIN only (already had this)
- `[HttpPost("{applicationId}/guarantors")]` Add Guarantor - ADMIN only
- `[HttpPut("{applicationId}/guarantors/{guarantorId}")]` Update Guarantor - ADMIN only
- `[HttpDelete("{applicationId}/guarantors/{guarantorId}")]` Delete Guarantor - ADMIN only
- `[HttpPost("{applicationId}/withdrawal")]` Save Withdrawal - ADMIN only
- `[HttpDelete("{applicationId}/withdrawal")]` Delete Withdrawal - ADMIN only

**Read-only endpoints (accessible to all authorized users):**
- `[HttpGet]` GetAll - View list
- `[HttpGet("search")]` Search - Advanced search
- `[HttpGet("{id}")]` GetById - View details
- `[HttpGet("{applicationId}/guarantors")]` GetGuarantors - View guarantors
- `[HttpGet("{applicationId}/withdrawal")]` GetWithdrawal - View withdrawal info

### 2. Frontend - Permission Updates

#### File: `Frontend/src/app/shared/rbac.service.ts`

**Added new permission constants:**
```typescript
LicenseApplicationView: 'licenseapplication.view',
LicenseApplicationCreate: 'licenseapplication.create',
LicenseApplicationEdit: 'licenseapplication.edit',
LicenseApplicationDelete: 'licenseapplication.delete',
```

#### File: `Frontend/src/app/license-applications/license-application-list/license-application-list.component.ts`

**Updated permission checks:**
```typescript
checkPermissions(): void {
    const role = this.rbacService.getCurrentRole();
    this.isViewOnly = role === UserRoles.Authority || 
                      role === UserRoles.LicenseReviewer || 
                      role === UserRoles.CompanyRegistrar;
    this.canEdit = role === UserRoles.Admin;
    this.canDelete = role === UserRoles.Admin;
}
```

**Result:**
- COMPANY_REGISTRAR can view the list but cannot see edit/delete buttons
- Only ADMIN can see edit/delete buttons

#### File: `Frontend/src/app/license-applications/license-application-form/license-application-form.component.ts`

**Updated permission checks:**
```typescript
checkPermissions(): void {
    const role = this.rbacService.getCurrentRole();
    this.isViewOnly = role === UserRoles.Authority || 
                      role === UserRoles.LicenseReviewer || 
                      role === UserRoles.CompanyRegistrar;
    this.canEdit = role === UserRoles.Admin;
}
```

**Result:**
- All save buttons are disabled for COMPANY_REGISTRAR (using `[disabled]="isViewOnly"`)
- Application form save button disabled
- Guarantor form save button disabled
- Withdrawal form save button disabled

#### File: `Frontend/src/app/dashboard/masterlayout/masterlayout.component.ts`

**Added isAdmin flag:**
```typescript
isAdmin = false;

// In loadUserPermissions():
this.isAdmin = this.rbacService.isAdmin();
```

#### File: `Frontend/src/app/dashboard/masterlayout/masterlayout.component.html`

**Updated menu visibility:**
```html
<!-- Only ADMIN can see the form route -->
<a *ngIf="canAccessCompany && isAdmin" routerLink="/license-applications" 
   routerLinkActive="active-nav" [routerLinkActiveOptions]="{exact: true}" 
   class="nav-link">
  <mat-icon>assignment</mat-icon>
  <span>ثبت درخواست متقاضیان</span>
</a>

<!-- All company module users can see the list -->
<a *ngIf="canAccessCompany" routerLink="/license-applications/list" 
   routerLinkActive="active-nav" class="nav-link">
  <mat-icon>list_alt</mat-icon>
  <span>جدول ثبت درخواست متقاضیان</span>
</a>
```

**Result:**
- COMPANY_REGISTRAR can only see "جدول ثبت درخواست متقاضیان" (list view)
- "ثبت درخواست متقاضیان" (form) is hidden from COMPANY_REGISTRAR
- Only ADMIN can see both menu items

## User Experience for COMPANY_REGISTRAR

### What they CAN do:
1. ✅ View the license applications list
2. ✅ Search and filter applications
3. ✅ View application details (read-only)
4. ✅ View guarantor information (read-only)
5. ✅ View withdrawal information (read-only)

### What they CANNOT do:
1. ❌ Create new applications
2. ❌ Edit existing applications
3. ❌ Delete applications
4. ❌ Add/edit/delete guarantors
5. ❌ Add/edit/delete withdrawal information
6. ❌ Access the form route (http://localhost:4200/license-applications)

## Security Implementation

### Backend Security:
- All write operations (POST, PUT, DELETE) require ADMIN role
- Authorization is enforced at the API level using `[Authorize(Roles = "ADMIN")]`
- Even if frontend is bypassed, backend will reject unauthorized requests

### Frontend Security:
- UI elements (buttons, menu items) are hidden based on role
- Forms are disabled for non-admin users
- Route access is controlled by guards

## Testing Recommendations

1. **Test as ADMIN:**
   - Verify full access to create, edit, delete operations
   - Verify menu shows both "ثبت درخواست متقاضیان" and "جدول ثبت درخواست متقاضیان"

2. **Test as COMPANY_REGISTRAR:**
   - Verify can view list and details
   - Verify cannot see edit/delete buttons
   - Verify all save buttons are disabled
   - Verify "ثبت درخواست متقاضیان" menu item is hidden
   - Try to access http://localhost:4200/license-applications directly (should redirect or show empty form with disabled buttons)

3. **Test API directly:**
   - Try POST/PUT/DELETE requests as COMPANY_REGISTRAR (should return 403 Forbidden)
   - Verify GET requests work for all authorized users

## Notes

- The route http://localhost:4200/license-applications is not completely blocked at the routing level, but the form will be disabled for COMPANY_REGISTRAR users
- If you want to completely block the route, you would need to add a custom guard to the routing configuration
- All existing data and functionality remain unchanged
- This is a permission-only update with no database schema changes
