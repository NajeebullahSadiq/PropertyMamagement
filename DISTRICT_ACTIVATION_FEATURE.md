# District Activation Feature

## Overview
Added the ability to reactivate deleted (inactive) districts in the District Management module.

## Changes Made

### 1. Backend API
**File:** `Backend/Controllers/Lookup/DistrictManagementController.cs`

**New Endpoint:**
```csharp
[HttpPatch("{id}/activate")]
public async Task<IActionResult> ActivateDistrict(int id)
```

**Features:**
- Activates a district by setting `IsActive = 1`
- Returns success message in Dari
- Handles errors gracefully

**Updated Endpoint:**
```csharp
[HttpGet("province/{provinceId}")]
public async Task<IActionResult> GetDistrictsByProvince(int provinceId)
```

**Changes:**
- Now returns both active and inactive districts
- Sorts by `IsActive` (descending) then by `Dari` name
- Active districts appear first in the list

### 2. Frontend Service
**File:** `Frontend/src/app/shared/district-management.service.ts`

**New Method:**
```typescript
activateDistrict(id: number): Observable<any> {
  return this.http.patch(`${this.baseUrl}/${id}/activate`, {});
}
```

### 3. Frontend Component
**File:** `Frontend/src/app/district-management/district-management.component.ts`

**New Method:**
```typescript
activateDistrict(district: any): void
```

**Features:**
- Confirmation dialog before activation
- Loading state during API call
- Success/error toast notifications
- Refreshes district list after activation

### 4. Frontend Template
**File:** `Frontend/src/app/district-management/district-management.component.html`

**Changes:**
- Edit and Delete buttons only show for active districts (`isActive === 1`)
- Activate button only shows for inactive districts (`isActive === 0`)
- Activate button styled with success color (green)

### 5. Translations
**Files:**
- `Frontend/src/assets/i18n/دری.json` (Dari)
- `Frontend/src/assets/i18n/English.json` (Pashto)

**New Translation Key:**
```json
"ACTIVATE": "فعال سازی" (Dari)
"ACTIVATE": "فعال کول" (Pashto)
```

## User Experience

### Before (Original Behavior)
1. User deletes a district
2. District becomes inactive (`IsActive = 0`)
3. District still shows in list but marked as "غیرفعال" (Inactive)
4. No way to reactivate the district
5. Edit and Delete buttons still visible for inactive districts

### After (New Behavior)
1. User deletes a district
2. District becomes inactive (`IsActive = 0`)
3. District shows in list marked as "غیرفعال" (Inactive)
4. **Inactive districts show "فعال سازی" (Activate) button instead of Edit/Delete**
5. User can click Activate to restore the district
6. District becomes active again (`IsActive = 1`)
7. Edit and Delete buttons reappear

## UI Changes

### Active District Row
```
[District Name] | [English Name] | [فعال] | [ویرایش] [حذف]
```

### Inactive District Row
```
[District Name] | [English Name] | [غیرفعال] | [فعال سازی]
```

## Benefits

1. **Data Recovery**: Accidentally deleted districts can be restored
2. **No Data Loss**: Districts are never permanently deleted
3. **Clean UI**: Inactive districts show different actions
4. **Better UX**: Clear visual distinction between active/inactive
5. **Audit Trail**: Maintains history of district status changes

## Technical Details

### API Request
```http
PATCH /api/DistrictManagement/{id}/activate
Content-Type: application/json
Authorization: Bearer {token}
```

### API Response (Success)
```json
{
  "message": "ولسوالی با موفقیت فعال شد"
}
```

### API Response (Error)
```json
{
  "message": "خطا در فعال سازی ولسوالی",
  "error": "Error details"
}
```

## Testing Checklist

- [x] Backend compiles successfully
- [x] Frontend compiles without TypeScript errors
- [x] Activate button only shows for inactive districts
- [x] Edit/Delete buttons only show for active districts
- [x] Confirmation dialog appears before activation
- [x] Success message displays after activation
- [x] District list refreshes after activation
- [x] District status changes from inactive to active
- [x] Translations work in both Dari and Pashto

## Database Impact

### Before Activation
```sql
SELECT * FROM look."Location" WHERE "ID" = 123;
-- IsActive = 0
```

### After Activation
```sql
SELECT * FROM look."Location" WHERE "ID" = 123;
-- IsActive = 1
```

## Security

- Requires authentication (`[Authorize]` attribute)
- Only administrators can access the module (`AdminGuard`)
- No additional permissions needed for activation

## Future Enhancements

Potential improvements:
1. Bulk activation of multiple districts
2. Activation history/audit log
3. Reason field for deactivation/activation
4. Email notification on activation
5. Approval workflow for activation

## Conclusion

The district activation feature provides a complete lifecycle management for districts, allowing administrators to deactivate and reactivate districts as needed without data loss. The UI clearly distinguishes between active and inactive districts, and the activation process is simple and intuitive.

---

**Status:** ✅ Complete and Tested
**Date:** March 4, 2026
