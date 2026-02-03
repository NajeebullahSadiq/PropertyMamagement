# Add HalfPrice Field to Vehicle Module

## Overview
Added a new field "HalfPrice" (مناصف قیمت) to the vehicle price section in the ثبت مشخصات وسایط نقلیه (Vehicle Details Registration) tab.

## Date
February 2, 2026

## Changes Made

### 1. Frontend Changes

#### Models Updated
- **Frontend/src/app/models/vehicle.ts**
  - Added `halfPrice: number` to `VehicleDetails` interface

- **Frontend/src/app/models/PropertyDetail.ts**
  - Added `halfPrice: number` to `VehiclesDetailsList` interface

#### Components Updated
- **Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.ts**
  - Added `halfPrice: ['']` to form initialization
  - Added `halfPrice` to form setValue in ngOnInit
  - Added `get halfPrice()` getter method

- **Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html**
  - Changed grid from `md:grid-cols-3` to `md:grid-cols-4` to accommodate new field
  - Added HalfPrice input field with label "مناصف قیمت"
  - Added icon `fa-divide` for the field
  - Field accepts numeric input with decimal support

- **Frontend/src/app/vehicle/vehicledetailsview/vehicledetailsview.component.html**
  - Changed grid from `md:grid-cols-3` to `md:grid-cols-4`
  - Added HalfPrice display card with blue gradient styling
  - Displays formatted currency value

### 2. Backend Changes

#### Models Updated
- **Backend/Models/Vehicles/VehiclesPropertyDetail.cs**
  - Added `public string? HalfPrice { get; set; }`

- **Backend/Models/ViewModels/getVehiclePrintData.cs**
  - Added `public string? HalfPrice { get; set; }`

#### Controllers Updated
- **Backend/Controllers/Vehicles/VehiclesController.cs**
  - **SaveProperty (POST)**: Added `HalfPrice = request.HalfPrice` assignment
  - **GetAll**: Added `p.HalfPrice` to select statement
  - **GetVehicleViewById**: Added `vehicle.HalfPrice` to result object
  - **GetPrintRecordById**: Added `data.HalfPrice` to result object
  - **UpdateVehicleDetails (PUT)**: Automatically includes HalfPrice via SetValues

### 3. Database Changes

#### Migration Files Created
- **Backend/Infrastructure/Migrations/Vehicle/20260202_AddHalfPriceField.cs**
  - EF Core migration to add HalfPrice column
  - Includes Up and Down methods for rollback

- **Backend/Scripts/add_halfprice_to_vehicle.sql**
  - SQL script to add HalfPrice column
  - Can be run directly on the database

#### Migration Script Content
```sql
ALTER TABLE tr."VehiclesPropertyDetails" 
ADD COLUMN IF NOT EXISTS "HalfPrice" TEXT;
```

## Field Details

### Field Name
- **Database Column**: `HalfPrice`
- **C# Property**: `HalfPrice`
- **TypeScript Property**: `halfPrice`
- **Label (Dari)**: مناصف قیمت
- **Label (English)**: Half Price

### Field Type
- **Database**: TEXT (nullable)
- **Backend**: string? (nullable)
- **Frontend**: number

### Field Behavior
- **Auto-calculated**: Yes (Price ÷ 2)
- **User Input**: No (read-only/disabled field)
- **Calculation**: Automatically updates when price changes
- **Display**: Shows calculated value in disabled input field

### Calculation Logic
```typescript
halfPrice = price / 2
```

When the user enters or changes the price:
1. Price is parsed to a number
2. Half price is calculated as price divided by 2
3. The halfPrice field is automatically updated
4. The field is displayed as read-only (disabled)

### Field Position
Located in the قیمت (Price) section, displayed as the 4th field in a 4-column grid:
1. قیمت به عدد (Price in Numbers)
2. قیمت به حروف (Price in Words)
3. مناصف قیمت (Half Price) - **NEW**
4. حق‌العمل رهنمای معاملات (Transaction Guide Fee)

## Deployment Instructions

### Step 1: Apply Database Migration

#### Option A: Using SQL Script (Recommended)
```bash
psql -U your_username -d your_database_name -f Backend/Scripts/add_halfprice_to_vehicle.sql
```

#### Option B: Using EF Core
```bash
cd Backend
dotnet ef database update
```

#### Option C: Manual SQL
```sql
ALTER TABLE tr."VehiclesPropertyDetails" 
ADD COLUMN IF NOT EXISTS "HalfPrice" TEXT;
```

### Step 2: Rebuild Backend
```bash
cd Backend
dotnet build
```

### Step 3: Rebuild Frontend
```bash
cd Frontend
ng build
```

### Step 4: Restart Services
```bash
# Backend
cd Backend
dotnet run

# Or restart service
sudo systemctl restart prmis-backend
```

## Testing Checklist

- [ ] Database column added successfully
- [ ] Backend compiles without errors
- [ ] Frontend compiles without errors
- [ ] HalfPrice field appears in vehicle form (disabled/read-only)
- [ ] HalfPrice automatically calculates when price is entered
- [ ] HalfPrice updates when price is changed
- [ ] HalfPrice shows correct value (price ÷ 2)
- [ ] HalfPrice value is saved when creating new vehicle
- [ ] HalfPrice value is saved when updating existing vehicle
- [ ] HalfPrice displays correctly in vehicle details view
- [ ] HalfPrice appears in vehicle list (if applicable)
- [ ] HalfPrice is included in print/export functionality
- [ ] Existing vehicles without HalfPrice display correctly (null handling)
- [ ] Field is properly disabled and cannot be manually edited

## UI Changes

### Vehicle Form (ثبت مشخصات وسایط نقلیه)
**Before**: 3-column grid with Price, Price Text, and Royalty Amount
**After**: 4-column grid with Price, Price Text, **Half Price (auto-calculated)**, and Royalty Amount

**Behavior**:
- User enters price in "قیمت به عدد" field
- Half Price automatically calculates and displays (price ÷ 2)
- Half Price field is disabled (read-only, gray background)
- Similar to how "حق‌العمل رهنمای معاملات" (Royalty Amount) works

### Vehicle Details View
**Before**: 3-column grid for price information
**After**: 4-column grid with Half Price displayed in blue gradient card

## Rollback Instructions

### Using EF Core
```bash
cd Backend
dotnet ef database update 20260202_ChangePlateNumberToString
```

### Using SQL
```sql
ALTER TABLE tr."VehiclesPropertyDetails" 
DROP COLUMN IF EXISTS "HalfPrice";
```

## Files Modified

### Frontend (5 files)
1. Frontend/src/app/models/vehicle.ts
2. Frontend/src/app/models/PropertyDetail.ts
3. Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.ts
4. Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html
5. Frontend/src/app/vehicle/vehicledetailsview/vehicledetailsview.component.html

### Backend (3 files)
1. Backend/Models/Vehicles/VehiclesPropertyDetail.cs
2. Backend/Models/ViewModels/getVehiclePrintData.cs
3. Backend/Controllers/Vehicles/VehiclesController.cs

### Database Scripts (2 files)
1. Backend/Infrastructure/Migrations/Vehicle/20260202_AddHalfPriceField.cs
2. Backend/Scripts/add_halfprice_to_vehicle.sql

## Notes
- The field is optional (nullable) to maintain compatibility with existing records
- The field accepts decimal values for precise pricing
- The field is displayed with currency formatting in the view
- The field is included in all CRUD operations automatically
- Audit logging will track changes to this field
- The field is included in print/export functionality

## Related Changes
This change is part of the vehicle module enhancements along with:
- Vehicle number fields changed to string type (PilateNo, PermitNo, EnginNo, ShasiNo)
- See: `VEHICLE_FIELDS_STRING_MIGRATION_SUMMARY.md`
