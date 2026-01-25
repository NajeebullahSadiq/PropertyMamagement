# Audit Table ID Auto-Increment Fix

## Problem
When updating company license information, the system threw an error:
```
23502: null value in column "Id" of relation "licenseaudit" violates not-null constraint
```

This occurred because the audit table's `Id` column was not configured as auto-increment in the database, even though Entity Framework was configured to generate values on add.

## Root Cause
All audit tables in the `log` schema were missing the auto-increment/serial configuration in PostgreSQL:
- `licenseaudit`
- `propertyaudit`
- `propertybuyeraudit`
- `propertyselleraudit`
- `vehicleaudit`
- `vehiclebuyeraudit`
- `vehicleselleraudit`
- `guarantorsaudit`
- `graunteeaudit`
- `companyowneraudit`
- `companydetailsaudit`

The Entity Framework configuration in `AppDbContext.cs` had `ValueGeneratedOnAdd()` only for `Licenseaudit`, but the database sequences were not properly created or linked.

## Solution

### 1. Database Fix (Required - Run First)
Execute the SQL script to fix all audit tables:
```bash
psql -U your_username -d your_database -f Backend/Scripts/fix_licenseaudit_id.sql
```

This script:
- Creates sequences for each audit table's Id column
- Sets the sequence to start from max(Id) + 1
- Links the sequence to the Id column as default value
- Makes the sequence owned by the column

### 2. Code Fix (Completed)
Updated `Backend/Configuration/AppDbContext.cs` to add `ValueGeneratedOnAdd()` to all audit entity configurations. This ensures Entity Framework knows these columns are auto-generated.

## Files Modified
1. `Backend/Scripts/fix_licenseaudit_id.sql` - Database fix script
2. `Backend/Configuration/AppDbContext.cs` - Added `ValueGeneratedOnAdd()` to all audit entities

## Testing
After running the SQL script:
1. Try updating a company license detail
2. The audit record should be created automatically with a generated Id
3. No more "null value in column Id" errors

## Prevention
- Always configure auto-increment columns in both:
  - Entity Framework (`.ValueGeneratedOnAdd()`)
  - Database (SERIAL or SEQUENCE with DEFAULT)
- Test audit functionality after creating new audit tables
