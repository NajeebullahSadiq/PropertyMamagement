# Property Module - PNumber Null Constraint Violation Fix

## Issue
When updating property details, the system throws an error:
```
Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes.
Npgsql.PostgresException: 23502: null value in column "PNumber" of relation "PropertyDetails" violates not-null constraint
```

## Root Cause

### Database Schema
The `PNumber` column in the `tr.PropertyDetails` table is defined as NOT NULL in the database.

### Code Issue
In the `UpdatePropertyDetails` method (`PropertyDetailsController.cs` line 577):
1. The method uses `SetValues(request)` to copy all values from the request to the existing entity
2. The frontend doesn't send the `PNumber` value in update requests
3. This causes `PNumber` to be set to null
4. Database rejects the update because `PNumber` cannot be null

### Why This Happens
- `PNumber` is auto-generated when a property is first created
- The frontend form doesn't include `PNumber` field (it's read-only/system-generated)
- When updating, the request object has `PNumber = null`
- `SetValues()` copies this null value, overwriting the existing PNumber

## Solution Implemented

### Backend Fix
**File**: `Backend/Controllers/PropertyDetailsController.cs`

Modified the `UpdatePropertyDetails` method to preserve the `PNumber` value:

```csharp
// Store the original values of the CreatedBy and CreatedOn properties
var createdBy = existingProperty.CreatedBy;
var createdAt = existingProperty.CreatedAt;
var pNumber = existingProperty.Pnumber; // Preserve PNumber

// Update the entity with the new values
_context.Entry(existingProperty).CurrentValues.SetValues(request);

// Restore the original values of the CreatedBy and CreatedOn properties
existingProperty.CreatedBy = createdBy;
existingProperty.CreatedAt = createdAt;
existingProperty.Pnumber = pNumber; // Restore PNumber
```

## How It Works

### Before Fix
1. User updates property details
2. Frontend sends request without `PNumber`
3. Backend copies all values from request → `PNumber` becomes null
4. Database rejects update → Error thrown

### After Fix
1. User updates property details
2. Frontend sends request without `PNumber`
3. Backend preserves existing `PNumber` value before copying
4. Backend restores `PNumber` after copying
5. Database accepts update → Success ✅

## Similar Fields Protected

The same pattern is used for other system-managed fields:
- `CreatedBy` - User who created the record
- `CreatedAt` - Timestamp when record was created
- `Pnumber` - System-generated property number (NOW FIXED)

These fields should never be modified during updates, so they are preserved.

## Testing Checklist

- [x] Backend compiles without errors
- [ ] Create new property record → PNumber is generated
- [ ] Update property details (change area, rooms, etc.)
- [ ] Verify update succeeds without error
- [ ] Verify PNumber remains unchanged after update
- [ ] Check database to confirm PNumber is preserved

## Database Schema Note

The `PNumber` column constraint in the database:
```sql
ALTER TABLE tr."PropertyDetails" 
ALTER COLUMN "PNumber" SET NOT NULL;
```

This constraint is correct and should remain. The fix ensures the application respects this constraint.

## Alternative Solutions Considered

### Option 1: Make PNumber Nullable in Database
- **Rejected**: PNumber is a required field for property identification
- Would require migration and could break existing queries

### Option 2: Always Send PNumber from Frontend
- **Rejected**: PNumber is system-generated and shouldn't be editable
- Would require frontend changes and expose system field

### Option 3: Preserve PNumber in Backend (CHOSEN)
- **Selected**: Minimal change, follows existing pattern
- Consistent with how CreatedBy/CreatedAt are handled
- No frontend changes needed

## Files Modified

1. `Backend/Controllers/PropertyDetailsController.cs`
   - Modified `UpdatePropertyDetails` method to preserve `PNumber`

## Related Issues

This fix also resolves the seller address prepopulation issue indirectly:
- Users couldn't update property details due to PNumber error
- This prevented property address from being saved
- Which caused the "ثبت آدرس ملکیت" button to find no address

## Status
✅ **COMPLETE** - Ready for testing and deployment
