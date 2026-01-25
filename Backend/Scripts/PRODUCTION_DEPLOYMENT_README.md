# Production Deployment: Numeric to Text Conversion

## Overview
This document describes the production deployment script for converting all numeric (double precision) columns to TEXT type in the PRMIS database.

## Script Location
`Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql`

## What This Script Does

### 1. Drops Dependent View
- Drops the `GetPrintType` view to avoid dependency errors during column type changes

### 2. Converts 20 Columns Across 6 Tables

#### PropertyDetails (2 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text

#### BuyerDetails (5 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text
- `HalfPrice`: double precision → text
- `SharePercentage`: double precision → text
- `ShareAmount`: double precision → text

#### SellerDetails (5 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text
- `HalfPrice`: double precision → text
- `SharePercentage`: double precision → text
- `ShareAmount`: double precision → text

#### VehiclesPropertyDetails (2 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text

#### VehiclesBuyerDetails (3 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text
- `HalfPrice`: double precision → text

#### VehiclesSellerDetails (3 columns)
- `Price`: double precision → text
- `RoyaltyAmount`: double precision → text
- `HalfPrice`: double precision → text

### 3. Recreates GetPrintType View
- Recreates the view with correct TEXT types
- Uses `COALESCE(pd."PNumber", '')` instead of `COALESCE(pd."PNumber", 0)`
- Uses lowercase column names: `pd."north"` instead of `pd."North"`

### 4. Verification Query
- Includes a verification query to confirm all columns are TEXT type

## How to Run in Production

### Option 1: Using pgAdmin (Recommended)
1. Open pgAdmin
2. Connect to your production database
3. Right-click on the database → Query Tool
4. Open the file: `Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql`
5. Click Execute (F5)
6. Review the verification query results at the bottom

### Option 2: Using psql Command Line
```bash
psql -U postgres -d PRMIS -f Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql
```

## Expected Results

After running the script, the verification query should show:

```
table_name                  | column_name      | data_type
----------------------------|------------------|----------
BuyerDetails                | HalfPrice        | text
BuyerDetails                | Price            | text
BuyerDetails                | RoyaltyAmount    | text
BuyerDetails                | ShareAmount      | text
BuyerDetails                | SharePercentage  | text
PropertyDetails             | Price            | text
PropertyDetails             | RoyaltyAmount    | text
SellerDetails               | HalfPrice        | text
SellerDetails               | Price            | text
SellerDetails               | RoyaltyAmount    | text
SellerDetails               | ShareAmount      | text
SellerDetails               | SharePercentage  | text
VehiclesBuyerDetails        | HalfPrice        | text
VehiclesBuyerDetails        | Price            | text
VehiclesBuyerDetails        | RoyaltyAmount    | text
VehiclesPropertyDetails     | Price            | text
VehiclesPropertyDetails     | RoyaltyAmount    | text
VehiclesSellerDetails       | HalfPrice        | text
VehiclesSellerDetails       | Price            | text
VehiclesSellerDetails       | RoyaltyAmount    | text
```

All 20 columns should show `data_type = 'text'`

## Post-Deployment

After running the script:

1. **Restart the application** to ensure the GetPrintType view is loaded correctly
2. **Test key functionality**:
   - Create a new property transaction
   - Create a new vehicle transaction
   - View dashboard reports
   - Print documents

## Rollback (If Needed)

If you need to rollback, you would need to:
1. Convert columns back to `double precision`
2. Recreate the view with numeric types

**Note**: Rollback is NOT recommended as the C# code has been updated to work with TEXT types.

## C# Code Changes

The following C# code has been updated to work with TEXT types:

### Models
- All numeric properties changed from `double?` to `string?`
- Files: PropertyDetail.cs, BuyerDetail.cs, SellerDetail.cs, VehiclesPropertyDetail.cs, etc.

### Controllers
- Changed `.HasValue` checks to `!string.IsNullOrWhiteSpace()` checks
- Added `double.TryParse()` for calculations
- Files: PropertyDetailsController.cs, VehiclesController.cs, DashboardController.cs, etc.

### DTOs
- All DTOs updated to use `string?` types
- Files in `Backend/src/Application/` folder

## Support

If you encounter any issues:
1. Check the error message in pgAdmin
2. Verify the database connection string
3. Ensure you have proper permissions to alter tables
4. Contact the development team

## Database Connection
- Server: localhost
- Database: PRMIS
- Username: postgres
- Password: Khan@223344
