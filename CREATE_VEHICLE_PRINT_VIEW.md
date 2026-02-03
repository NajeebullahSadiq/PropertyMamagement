# Create Vehicle Print View

## Problem
The `getVehiclePrintData` view doesn't exist in the database, causing errors when trying to print vehicle documents:
```
42P01: relation "getVehiclePrintData" does not exist
```

## Solution
Create the database view that combines vehicle details with seller, buyer, and witness information for printing.

## What the View Does
The `getVehiclePrintData` view:
- Combines data from multiple tables into a single view for printing
- Includes vehicle details (plate number, engine, chassis, price, etc.)
- Includes first seller information with addresses
- Includes first buyer information with addresses
- Includes up to 2 witnesses
- Joins with Location table to get province/district names in Dari

## Deployment Steps

### Step 1: Run the SQL Script
```bash
# Connect to your PostgreSQL database
psql -U your_username -d your_database_name

# Run the view creation script
\i Backend/Scripts/create_vehicle_print_view.sql
```

Or run directly in psql:
```sql
\i Backend/Scripts/create_vehicle_print_view.sql
```

### Step 2: Verify the View
```sql
-- Check if view exists
SELECT table_name 
FROM information_schema.views 
WHERE table_schema = 'public' 
AND table_name = 'getVehiclePrintData';

-- Test the view
SELECT * FROM "getVehiclePrintData" LIMIT 5;
```

### Step 3: Restart Backend
The backend is already configured to use this view, so just restart:
```bash
cd Backend
dotnet run
```

## View Structure

### Vehicle Information
- Id, PermitNo, PilateNo, TypeOfVehicle, Model
- EnginNo, ShasiNo, Color, Description
- Price, PriceText, HalfPrice, RoyaltyAmount
- CreatedAt

### Seller Information (First Seller)
- SellerFirstName, SellerFatherName
- SellerIndentityCardNumber
- SellerVillage, tSellerVillage
- SellerPhoto
- SellerProvince, SellerDistrict (Dari names)
- tSellerProvince, tSellerDistrict (Dari names)

### Buyer Information (First Buyer)
- BuyerFirstName, BuyerFatherName
- BuyerIndentityCardNumber
- BuyerVillage, tBuyerVillage
- BuyerPhoto
- BuyerProvince, BuyerDistrict (Dari names)
- tBuyerProvince, tBuyerDistrict (Dari names)

### Witness Information
- WitnessOneFirstName, WitnessOneFatherName, WitnessOneIndentityCardNumber
- WitnessTwoFirstName, WitnessTwoFatherName, WitnessTwoIndentityCardNumber

## Technical Details

### Why LATERAL Joins?
The view uses `LEFT JOIN LATERAL` to:
- Get only the first seller (if multiple sellers exist)
- Get only the first buyer (if multiple buyers exist)
- Get the first and second witness separately
- Maintain performance by limiting results at join time

### Location Joins
The view joins with `look."Location"` table to get:
- Province names in Dari
- District names in Dari
- Both permanent (P) and temporary (T) addresses

## Testing

### Test with Existing Vehicle
```sql
-- Get print data for a specific vehicle
SELECT * FROM "getVehiclePrintData" WHERE "Id" = 1;
```

### Test Print Endpoint
```bash
# Test the API endpoint
curl http://localhost:5143/api/Vehicles/GetPrintRecord/1
```

## Troubleshooting

### Error: "relation does not exist"
- Make sure you're connected to the correct database
- Check that the schema names are correct (tr, look)
- Verify all referenced tables exist

### Error: "column does not exist"
- Check that all column names match the actual database schema
- Verify Photo column is uppercase in VehiclesBuyerDetails and VehiclesSellerDetails

### No Data Returned
- Check that vehicles exist in VehiclesPropertyDetails
- Verify sellers, buyers, and witnesses are linked with correct PropertyDetailsId
- Check Location table has province/district data

## Files

### Created
- `Backend/Scripts/create_vehicle_print_view.sql` - View creation script

### Modified
- None (AppDbContext already has the view configured)

## Status
✅ **Script Created** - Ready to deploy
⏳ **Deployment Required** - Run the SQL script in your database

---

**Created**: February 3, 2026
**Purpose**: Enable vehicle document printing functionality
