# Print Record Type Mismatch Fix

## Issue
When trying to retrieve print records via the `/api/PropertyDetails/GetPrintRecord/{id}` endpoint, the application was throwing type mismatch errors:

1. First error: `Reading as 'System.Int32' is not supported for fields having DataTypeName 'text'`
2. After initial fix: `Reading as 'System.String' is not supported for fields having DataTypeName 'integer'`

## Root Cause
The `GetPrintType` view model had field type mismatches with the actual database view schema:

**Initial State:**
- `PNumber` was `int` but database has `text` (wrapped in COALESCE)
- `PArea` was `int` but database has `text`
- `NumofRooms` was `int?` and database has `integer` (correct)

**After First Fix (incorrect):**
- Changed all three to `string?`, but `NumofRooms` is actually `integer` in the database

The confusion arose because:
1. `PNumber` and `PArea` were converted to `text` in the PropertyDetails table to support Dari/Pashto numerals
2. `NumofRooms` remained as `integer` in the PropertyDetails table
3. The GetPrintType view passes these through directly, so the types must match

## Solution Implemented

### Final Correct Field Types in GetPrintType Model
Updated the fields in `Backend/Models/ViewModels/GetPrintType.cs` to match the actual database view schema:

**Correct Configuration:**
```csharp
public string? PNumber { get; set; }    // text in DB (COALESCE makes it text)
public string? PArea { get; set; }      // text in DB
public int? NumofRooms { get; set; }    // integer in DB
```

## Database View Schema Reference

From the `GetPrintType` view definition in `PRODUCTION_convert_numeric_to_text.sql`:
```sql
SELECT
    pd."Id",
    COALESCE(pd."PNumber", '') as "PNumber",  -- text (COALESCE with string)
    COALESCE(pd."PArea", '') as "PArea",      -- text (COALESCE with string)
    pd."NumofRooms",                          -- integer (passed through)
    ...
```

## Why This Fix Works

1. **Type Alignment**: The C# model now correctly matches the PostgreSQL view schema
2. **Mixed Types Supported**: The model correctly handles both text and integer fields from the view
3. **Npgsql Compatibility**: The Npgsql driver can now correctly read each field with its proper type
4. **Nullable Support**: Using nullable types (`string?`, `int?`) allows for null values

## Related Context

This fix is part of a broader data type standardization effort:
- Some numeric fields (`PNumber`, `PArea`) were converted to `text` to support localized numerals
- Other numeric fields (`NumofRooms`, `NumofFloor`) remained as `integer` because they don't need localization
- View models must match the actual database view schema, not the source table schema

## Testing
To verify the fix:
1. Create a property record with all required details including number of rooms
2. Navigate to the print view or call `/api/PropertyDetails/GetPrintRecord/{id}`
3. Verify that the print record loads successfully without type mismatch errors
4. Confirm that property number, area, and number of rooms all display correctly

## Files Modified
- `Backend/Models/ViewModels/GetPrintType.cs`
- `Backend/Scripts/check_getprinttype_view_schema.sql` (diagnostic script created)

## Related Fixes
- `NUMERIC_TO_TEXT_CONVERSION_SUMMARY.md` - Original numeric to text conversion
- `DATA_TYPE_FIX_SUMMARY.md` - Related data type fixes
- `BUYER_FORM_FIELD_MAPPING_FIX.md` - Similar type conversion for buyer forms
