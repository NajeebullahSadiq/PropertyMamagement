# Database Migration Guide: Add Property Fields to Buyer/Seller Details

## Migration Name
`20251207081500_AddPropertyFieldsToBuyerAndSellerDetails`

## Overview
This migration adds 5 new columns to 4 different tables to support the relocation of property transaction fields from PropertyDetails to individual buyer/seller records.

## Changes Made

### Tables Modified
1. **BuyerDetails** (schema: `tr`)
2. **SellerDetails** (schema: `tr`)
3. **VehiclesBuyerDetails** (schema: `tr`)
4. **VehiclesSellerDetails** (schema: `tr`)

### New Columns Added (to each table)
- `PropertyTypeId` (integer, nullable)
- `Price` (double precision, nullable)
- `PriceText` (text, nullable)
- `RoyaltyAmount` (double precision, nullable)
- `HalfPrice` (double precision, nullable)

## Data Types
- **PropertyTypeId**: `integer` - References the PropertyType lookup table
- **Price**: `double precision` - Stores numeric price value
- **PriceText**: `text` - Stores price in text/words format
- **RoyaltyAmount**: `double precision` - Stores royalty/commission amount
- **HalfPrice**: `double precision` - Auto-calculated field (50% of Price)

## Migration Features
✅ **Safe Migration**: Uses PostgreSQL DO blocks to check if columns exist before adding
✅ **Idempotent**: Can be run multiple times without errors
✅ **Reversible**: Full rollback support in the Down() method
✅ **No Data Loss**: Existing data is preserved; only new nullable columns are added

## How to Apply

### Option 1: Using Entity Framework CLI (Recommended)
```bash
cd Backend
dotnet ef database update
```

### Option 2: Using Package Manager Console
```powershell
Update-Database
```

### Option 3: Manual SQL Execution
If needed, you can execute the migration manually by running the SQL commands in the migration file directly against your PostgreSQL database.

## Rollback Instructions

If you need to rollback this migration:

### Using Entity Framework CLI
```bash
cd Backend
dotnet ef database update 20251206074103_EnableMultipleSellersAndBuyers
```

### Using Package Manager Console
```powershell
Update-Database -Migration EnableMultipleSellersAndBuyers
```

## Verification

After applying the migration, verify the columns were created:

```sql
-- Check BuyerDetails table
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
AND table_name = 'BuyerDetails' 
AND column_name IN ('PropertyTypeId', 'Price', 'PriceText', 'RoyaltyAmount', 'HalfPrice');

-- Check SellerDetails table
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
AND table_name = 'SellerDetails' 
AND column_name IN ('PropertyTypeId', 'Price', 'PriceText', 'RoyaltyAmount', 'HalfPrice');

-- Check VehiclesBuyerDetails table
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
AND table_name = 'VehiclesBuyerDetails' 
AND column_name IN ('PropertyTypeId', 'Price', 'PriceText', 'RoyaltyAmount', 'HalfPrice');

-- Check VehiclesSellerDetails table
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
AND table_name = 'VehiclesSellerDetails' 
AND column_name IN ('PropertyTypeId', 'Price', 'PriceText', 'RoyaltyAmount', 'HalfPrice');
```

## Related Changes

This migration is part of the "Move Property Fields to Buyer Form" feature implementation:

### Backend Models Updated
- `BuyerDetail.cs` - Added 5 new properties
- `SellerDetail.cs` - Added 5 new properties
- `VehiclesBuyerDetail.cs` - Added 5 new properties
- `VehiclesSellerDetail.cs` - Added 5 new properties

### Frontend Components Updated
- `PropertydetailsComponent` - Removed price/property type fields
- `BuyerdetailComponent` (Estate) - Added new fields with half-price calculation
- `BuyerdetailComponent` (Vehicle) - Added new fields with half-price calculation

## Important Notes

1. **Nullable Columns**: All new columns are nullable to maintain backward compatibility
2. **No Foreign Key Constraints**: PropertyTypeId is not enforced as a foreign key in the migration (can be added separately if needed)
3. **HalfPrice Calculation**: This field is calculated in the application layer; the database stores the value
4. **Existing Data**: All existing records will have NULL values for the new columns until data is populated through the application

## Troubleshooting

### Migration Fails with "Column already exists"
This is safe and expected if the migration has already been applied. The migration includes checks to prevent duplicate column creation.

### Connection Issues
Ensure your database connection string in `appsettings.json` is correct and the database server is accessible.

### Permission Issues
Ensure your database user has ALTER TABLE permissions on the `tr` schema.

## Support
For issues or questions about this migration, refer to the feature implementation documentation or contact the development team.
