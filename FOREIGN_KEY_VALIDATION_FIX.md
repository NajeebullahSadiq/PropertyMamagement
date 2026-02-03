# Foreign Key Validation Fix for Vehicle Company Selection

## Problem
When Admin users tried to create vehicles, they encountered a database foreign key constraint violation:
```
23503: insert or update on table "VehiclesPropertyDetails" violates foreign key constraint "FK_VehiclesPropertyDetails_Company"
```

This error occurred because:
1. The selected `CompanyId` doesn't exist in the Company table
2. No validation was performed before attempting to save
3. The database has a foreign key constraint that enforces referential integrity

## Root Cause
The database has a foreign key constraint between `VehiclesPropertyDetails.CompanyId` and `CompanyDetails.Id`, but the application wasn't validating that the selected company exists before attempting to insert the record.

## Additional Issue Found
The role check was using incorrect role names:
- **Wrong**: `roles.Contains("Admin")` or `roles.Contains("SuperAdmin")`
- **Correct**: `roles.Contains(UserRoles.Admin)` which equals `"ADMIN"` (uppercase)

## Solution Implemented

### Backend Validation (VehiclesController.cs)
Added validation logic in the `SaveProperty()` method:

```csharp
// For Admin users - using correct role constant
if (roles.Contains(UserRoles.Admin) || roles.Contains(UserRoles.Authority))
{
    companyId = request.CompanyId;
    
    // Validate that the company exists if CompanyId is provided
    if (companyId.HasValue && companyId.Value > 0)
    {
        var companyExists = await _context.CompanyDetails.AnyAsync(c => c.Id == companyId.Value);
        if (!companyExists)
        {
            return BadRequest(new { message = $"شرکت با شناسه {companyId.Value} وجود ندارد" });
        }
    }
}
else
{
    // For regular users
    companyId = user.CompanyId;
    
    // Validate that the user has a company assigned
    if (!companyId.HasValue || companyId.Value == 0)
    {
        return BadRequest(new { message = "شما به هیچ شرکتی متصل نیستید" });
    }
}
```

### Role Constants Used
From `UserRoles.cs`:
- `UserRoles.Admin` = `"ADMIN"` (uppercase)
- `UserRoles.Authority` = `"AUTHORITY"` (view-only admin)
- `UserRoles.VehicleOperator` = `"VEHICLE_OPERATOR"`
- `UserRoles.PropertyOperator` = `"PROPERTY_OPERATOR"`

### Validation Benefits
1. **Early Detection**: Catches invalid CompanyId before database operation
2. **Clear Error Messages**: Provides user-friendly error messages in Dari/Pashto
3. **Prevents Database Errors**: Avoids cryptic foreign key constraint violations
4. **Better UX**: Users get immediate feedback about what went wrong
5. **Correct Role Checking**: Uses proper role constants instead of hardcoded strings

## Error Messages

### For Admin Users
- **Invalid Company**: "شرکت با شناسه {id} وجود ندارد" (Company with ID {id} does not exist)

### For Regular Users
- **No Company Assigned**: "شما به هیچ شرکتی متصل نیستید" (You are not connected to any company)

## Testing Checklist

### Admin User Tests
- [x] Role check now uses correct constant (UserRoles.Admin = "ADMIN")
- [ ] Select valid company → Vehicle creates successfully
- [ ] Select invalid/non-existent company → Clear error message
- [ ] Don't select company (null) → Appropriate handling
- [ ] Company dropdown only shows existing companies

### Regular User Tests
- [ ] User with assigned company → Vehicle creates successfully
- [ ] User without assigned company → Clear error message
- [ ] Company selector not visible to regular users

## Database Constraint
The foreign key constraint in the database:
```sql
ALTER TABLE tr."VehiclesPropertyDetails"
ADD CONSTRAINT "FK_VehiclesPropertyDetails_Company"
FOREIGN KEY ("CompanyId") REFERENCES tr."CompanyDetails"("Id");
```

This constraint ensures data integrity at the database level, and our application-level validation provides better user experience.

## Files Modified
1. `Backend/Controllers/Vehicles/VehiclesController.cs`

## Key Fixes
1. ✅ Changed role check from `"Admin"` to `UserRoles.Admin` (`"ADMIN"`)
2. ✅ Added company existence validation
3. ✅ Added user company assignment validation
4. ✅ Clear error messages in Dari/Pashto

## Next Steps for User
1. **Restart Backend**: The role check fix requires backend restart
2. **Test with Admin User**: Try creating vehicle again - should now recognize Admin role
3. **Verify Company Exists**: Ensure company ID 1 exists in the database

## Troubleshooting

### If error persists:
1. Check what companies are returned by the API:
   ```
   GET /api/CompanyDetails
   ```
2. Verify the selected company ID exists in the database:
   ```sql
   SELECT * FROM tr."CompanyDetails" WHERE "Id" = 1;
   ```
3. Check user's role in database:
   ```sql
   SELECT * FROM "AspNetUserRoles" WHERE "UserId" = '<user_id>';
   SELECT * FROM "AspNetRoles" WHERE "Id" = '<role_id>';
   ```

### Common Issues:
- **Role name mismatch**: Must use `UserRoles.Admin` constant, not hardcoded "Admin"
- **Empty company list**: No companies in database or user doesn't have permission to see them
- **Filtered companies**: Company list might be filtered by province/region
- **Soft-deleted companies**: Company exists but is marked as deleted
- **ID mismatch**: Frontend sending wrong ID format or value

## Status
✅ **Role Check Fixed** - Now uses correct UserRoles.Admin constant
✅ **Validation Added** - Backend now validates company existence before saving
⏳ **Testing Required** - Need to restart backend and test with Admin user

---

**Created**: February 3, 2026
**Updated**: February 3, 2026
**Related**: ADMIN_COMPANY_SELECTION_FIX.md
