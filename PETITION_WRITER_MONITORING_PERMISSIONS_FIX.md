# Petition Writer Monitoring Permissions Fix

## Issue
The "ثبت نظارت بر فعالیت عریضه نویسان" (Petition Writer Monitoring) module permissions were not appearing in the UI permission management interface, even though they existed in the backend and database.

## Root Cause
The `role-permissions.component.ts` file was missing:
1. A permission group for Petition Writer Monitoring
2. Permission labels for the petition writer monitoring permissions

## Changes Made

### 1. Frontend/src/app/users/role-permissions/role-permissions.component.ts

#### Added Petition Writer Monitoring Permission Group
```typescript
{
  label: 'نظارت بر فعالیت عریضه نویسان',
  icon: 'fa-clipboard-check',
  keys: ['petitionwritermonitoring.view', 'petitionwritermonitoring.create', 
         'petitionwritermonitoring.edit', 'petitionwritermonitoring.delete']
}
```

#### Renamed Activity Monitoring Group Label
Changed from: `'نظارت بر فعالیت‌ها'` (Activity Monitoring)
Changed to: `'نظارت بر فعالیت دفاتر رهنمای معاملات'` (Transaction Office Activity Monitoring)

#### Added Petition Writer Monitoring Permission Labels
```typescript
'petitionwritermonitoring.view': 'مشاهده نظارت بر فعالیت عریضه نویسان',
'petitionwritermonitoring.create': 'ثبت نظارت بر فعالیت عریضه نویسان',
'petitionwritermonitoring.edit': 'ویرایش نظارت بر فعالیت عریضه نویسان',
'petitionwritermonitoring.delete': 'حذف نظارت بر فعالیت عریضه نویسان'
```

#### Updated Activity Monitoring Permission Labels
Changed from generic labels to specific ones:
- `'مشاهده نظارت فعالیت‌ها'` → `'مشاهده نظارت بر فعالیت دفاتر رهنما'`
- `'ثبت نظارت فعالیت‌ها'` → `'ثبت نظارت بر فعالیت دفاتر رهنما'`
- `'ویرایش نظارت فعالیت‌ها'` → `'ویرایش نظارت بر فعالیت دفاتر رهنما'`
- `'حذف نظارت فعالیت‌ها'` → `'حذف نظارت بر فعالیت دفاتر رهنما'`

### 2. Frontend/src/app/dashboard/masterlayout/masterlayout.component.html

#### Removed Hardcoded Role Check
Removed hardcoded role comparison in user profile display:
```html
<!-- Before -->
{{ userRoleDari || (userRole == 'ADMIN' ? 'مدیر سیستم' : 'کاربر') }}

<!-- After -->
{{ userRoleDari || 'کاربر' }}
```

The `userRoleDari` is already properly set by `rbacService.getRoleDari()` which handles all role translations including ADMIN.

## Verification

### Backend Support (Already Exists)
✅ Permissions defined in `Backend/Configuration/UserRoles.cs`:
- `PetitionWriterMonitoringView`
- `PetitionWriterMonitoringCreate`
- `PetitionWriterMonitoringEdit`
- `PetitionWriterMonitoringDelete`

✅ Controller exists: `Backend/Controllers/PetitionWriterMonitoring/PetitionWriterMonitoringController.cs`

✅ Database table exists: `org.PetitionWriterMonitoringRecords`

✅ Route guard exists: `PetitionWriterMonitoringGuard` in `Frontend/src/app/auth/authSetting/role.guard.ts`

✅ RBAC service support exists: `canAccessModule('petitionWriterMonitoring')` in `Frontend/src/app/shared/rbac.service.ts`

✅ Navigation already exists in masterlayout (lines 179-187)

### What Was Missing
❌ Permission group in UI (FIXED)
❌ Permission labels in UI (FIXED)

## Testing Steps

1. Login as ADMIN
2. Navigate to "مدیریت کاربران" → "صلاحیت‌های نقش‌ها"
3. Select any role (e.g., ACTIVITY_MONITORING_MANAGER)
4. Verify two separate permission groups now appear:
   - "نظارت بر فعالیت دفاتر رهنمای معاملات" (Transaction Office Activity Monitoring)
   - "نظارت بر فعالیت عریضه نویسان" (Petition Writer Monitoring)
5. Assign petition writer monitoring permissions to a role
6. Login as a user with that role
7. Verify the navigation menu shows "نظارت بر فعالیت عریضه نویسان" section

## Impact
- Users can now properly assign petition writer monitoring permissions through the UI
- Clear distinction between transaction office monitoring and petition writer monitoring
- All existing backend functionality is now accessible through proper permission management
