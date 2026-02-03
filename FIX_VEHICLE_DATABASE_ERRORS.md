# Fix Vehicle Database Errors

## Date: February 2, 2026

## Errors Encountered

### Error 1: Column "Photo" does not exist
```
Npgsql.PostgresException: 42703: column v.Photo does not exist
Hint: Perhaps you meant to reference the column "v.photo".
```

**Cause**: PostgreSQL case-sensitivity issue. The database has `photo` (lowercase) but the C# model mapping was set to `Photo` (uppercase).

### Error 2: Columns HalfPrice, Price, RoyaltyAmount in wrong tables
The query was trying to select `HalfPrice`, `Price`, and `RoyaltyAmount` from `VehiclesSellerDetails` and `VehiclesBuyerDetails` tables, but these columns don't exist there. They only exist in the main `VehiclesPropertyDetails` table.

## Root Causes

1. **Photo Column Case**: The database column is `photo` (lowercase) but AppDbContext was mapping to `"Photo"` (uppercase)
2. **Wrong Model Fields**: The `VehiclesSellerDetail` and `VehiclesBuyerDetail` models incorrectly had `Price`, `RoyaltyAmount`, and `HalfPrice` fields that don't exist in the database

## Fixes Applied

### Fix 1: Remove Invalid Fields from Models

#### VehiclesSellerDetail.cs
**Removed**:
- `public string? Price { get; set; }`
- `public string? RoyaltyAmount { get; set; }`
- `public string? HalfPrice { get; set; }`

These fields should only exist in `VehiclesPropertyDetails`, not in seller/buyer details.

#### VehiclesBuyerDetail.cs
**Removed**:
- `public string? Price { get; set; }`
- `public string? RoyaltyAmount { get; set; }`
- `public string? HalfPrice { get; set; }`

### Fix 2: Photo Column Mapping

Updated AppDbContext to map C# `Photo` property to database `photo` column (lowercase).

**File**: `Backend/Configuration/AppDbContext.cs`

**Changes**:
```csharp
// VehiclesBuyerDetail
entity.Property(e => e.Photo).HasColumnName("photo");

// VehiclesSellerDetail
entity.Property(e => e.Photo).HasColumnName("photo");
```

## Deployment Instructions

### Step 1: Rebuild Backend
```bash
cd Backend
dotnet clean
dotnet build
```

### Step 2: Restart Backend
```bash
cd Backend
dotnet run
```

**Note**: No database changes needed! The database already has the correct lowercase `photo` column.

## Verification

### Check Database Columns
```sql
-- Verify photo column exists (lowercase)
SELECT column_name, data_type, table_name
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name IN ('VehiclesBuyerDetails', 'VehiclesSellerDetails', 'VehiclesWitnessDetails')
  AND column_name = 'photo';

-- Should return rows with column_name = 'photo' (lowercase)
```

### Check Price Fields Location
```sql
-- Verify Price, HalfPrice, RoyaltyAmount only in main table
SELECT column_name, table_name
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name LIKE 'Vehicles%'
  AND column_name IN ('Price', 'HalfPrice', 'RoyaltyAmount')
ORDER BY table_name, column_name;

-- Should only show these columns in VehiclesPropertyDetails
```

## Testing Checklist

- [ ] Backend compiles without errors
- [ ] Can view vehicle details without database errors
- [ ] Photo field displays correctly for sellers
- [ ] Photo field displays correctly for buyers
- [ ] Price fields only accessed from main vehicle table
- [ ] No "column does not exist" errors in logs

## Why This Happened

### Price Fields in Wrong Tables
The models were likely copied from the property/estate module where buyer and seller details DO have their own price fields (because in real estate, each party might have different prices for their portion). However, in the vehicle module, there's only ONE price for the entire vehicle transaction, so these fields should only be in the main `VehiclesPropertyDetails` table.

### Photo Column Case
PostgreSQL stores unquoted identifiers in lowercase. The database column was created as `photo` (lowercase), but the AppDbContext was mapping to `"Photo"` (uppercase). The fix is to update the mapping to use lowercase `"photo"` to match the database.

## Data Model Clarification

### VehiclesPropertyDetails (Main Table)
- Contains: Price, PriceText, HalfPrice, RoyaltyAmount
- These are the transaction-level fields

### VehiclesSellerDetail / VehiclesBuyerDetail
- Contains: Personal information, addresses, documents, photo
- Does NOT contain: Price fields (these are in the main table)
- Photo column: `photo` (lowercase in database)

## Files Modified

1. **Backend/Models/Vehicles/VehiclesSellerDetail.cs** - Removed Price, RoyaltyAmount, HalfPrice
2. **Backend/Models/Vehicles/VehiclesBuyerDetail.cs** - Removed Price, RoyaltyAmount, HalfPrice
3. **Backend/Configuration/AppDbContext.cs** - Changed Photo mapping from `"Photo"` to `"photo"` for VehiclesBuyerDetail and VehiclesSellerDetail

## Related Issues

If you encounter similar errors with other columns, check:
1. Column case sensitivity (PostgreSQL lowercase vs C# PascalCase)
2. Column existence in the correct table
3. Model fields matching actual database schema
4. AppDbContext column name mappings

## Prevention

To prevent similar issues in the future:
1. Always check actual database column names (use `\d table_name` in psql)
2. Verify model fields match database schema
3. Don't copy fields between models without checking database structure
4. Use HasColumnName() in AppDbContext when database column names differ from C# property names
5. PostgreSQL stores unquoted identifiers in lowercase - be aware of this when mapping
