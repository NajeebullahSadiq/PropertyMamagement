# Vehicle Number Fields Type Change: Integer to String

## Overview
Changed the vehicle identification number fields in the Vehicle module from integer/number type to string/text type to allow alphanumeric values:
- `PilateNo` (نمبر پلیت) - Plate Number
- `PermitNo` (نمبر جواز سیر) - Permit/License Number  
- `EnginNo` (شماره انجین) - Engine Number
- `ShasiNo` (شماره شاسی) - Chassis Number

## Date
February 2, 2026

## Changes Made

### 1. Frontend Changes

#### Models Updated
- **Frontend/src/app/models/vehicle.ts**
  - Changed `permitNo: number` → `permitNo: string`
  - Changed `pilateNo: number` → `pilateNo: string`
  - Changed `enginNo: number` → `enginNo: string`
  - Changed `shasiNo: number` → `shasiNo: string`

- **Frontend/src/app/models/PropertyDetail.ts**
  - Changed all four fields from `number` to `string` in `VehiclesDetailsList` interface

#### Components Updated
- **Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html**
  - Removed `appNumericInput` directive from `pilateNo` input field
  - Removed `appNumericInput` directive from `enginNo` input field
  - Removed `appNumericInput` directive from `shasiNo` input field
  - All inputs now accept alphanumeric characters

- **Frontend/src/app/vehicle/vehiclelist/vehiclelist.component.ts**
  - Updated `filterProperties()` method to handle string values
  - Removed unnecessary `.toString()` calls
  - Added null checks for all string fields

### 2. Backend Changes

#### Models Updated
- **Backend/Models/Vehicles/VehiclesPropertyDetail.cs**
  - Changed `public int PermitNo` → `public string? PermitNo`
  - Changed `public int PilateNo` → `public string? PilateNo`
  - Changed `public int? EnginNo` → `public string? EnginNo`
  - Changed `public int? ShasiNo` → `public string? ShasiNo`

- **Backend/Models/ViewModels/getVehiclePrintData.cs**
  - Changed `public int PermitNo` → `public string? PermitNo`
  - Changed `public int PilateNo` → `public string? PilateNo`
  - Changed `public int EnginNo` → `public string? EnginNo`
  - Changed `public int ShasiNo` → `public string? ShasiNo`

#### DTOs
- **Backend/src/Application/Vehicle/DTOs/VehicleDetailDto.cs**
  - Already had `string?` type for these fields (no change needed)

#### Controllers Updated
- **Backend/Controllers/Vehicles/VehiclesSubController.cs**
  - Updated validation logic in completeness check
  - Changed from `vehicleDetails.PilateNo == 0` to `string.IsNullOrWhiteSpace(vehicleDetails.PilateNo)`

### 3. Database Changes

#### Migration Files Created
- **Backend/Infrastructure/Migrations/Vehicle/20260202_ChangePlateNumberToString.cs**
  - EF Core migration to alter column types from integer to text
  - Includes Up and Down methods for rollback capability
  - Handles all four fields

- **Backend/Scripts/change_pilateno_to_string.sql**
  - SQL script to alter column types
  - Can be run directly on the database

#### Migration Script Content
```sql
-- Change PermitNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE TEXT USING "PermitNo"::TEXT;

-- Change PilateNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE TEXT USING "PilateNo"::TEXT;

-- Change EnginNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE TEXT USING "EnginNo"::TEXT;

-- Change ShasiNo from INTEGER to TEXT
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE TEXT USING "ShasiNo"::TEXT;
```

## Deployment Instructions

### Option 1: Using EF Core Migration (Recommended for Development)
```bash
cd Backend
dotnet ef database update
```

### Option 2: Using SQL Script (Recommended for Production)
```bash
psql -h hostname -U username -d database_name -f Backend/Scripts/change_pilateno_to_string.sql
```

### Option 3: Manual SQL Execution
Connect to your PostgreSQL database and run:
```sql
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE TEXT USING "PermitNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE TEXT USING "PilateNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE TEXT USING "EnginNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE TEXT USING "ShasiNo"::TEXT;
```

## Testing Checklist

- [ ] Vehicle form accepts alphanumeric plate numbers (e.g., "ABC-123", "کابل-1234")
- [ ] Vehicle form accepts alphanumeric engine numbers (e.g., "ENG-12345", "موتر-123")
- [ ] Vehicle form accepts alphanumeric chassis numbers (e.g., "CH-ABC-123")
- [ ] Vehicle list displays all numbers correctly
- [ ] Vehicle search/filter works with string values
- [ ] Vehicle details view shows all numbers correctly
- [ ] Print functionality displays all numbers correctly
- [ ] Existing numeric values are preserved after migration
- [ ] Backend validation accepts string values
- [ ] API endpoints return string values for all number fields

## Rollback Instructions

If you need to revert this change:

### Using EF Core
```bash
cd Backend
dotnet ef database update 20260131_AddVehicleWitnessFields
```

### Using SQL
```sql
-- WARNING: This will lose data if numbers contain non-numeric characters
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE INTEGER USING "PermitNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE INTEGER USING "PilateNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE INTEGER USING "EnginNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE INTEGER USING "ShasiNo"::INTEGER;
```

## Impact Analysis

### Breaking Changes
- None for existing numeric data (automatically converted to string)
- Frontend now accepts alphanumeric input instead of numeric only

### Benefits
- Supports international vehicle identification formats
- Allows special characters and letters in all vehicle numbers
- More flexible for different vehicle registration systems
- Aligns with real-world vehicle identification formats
- Engine numbers often contain letters (e.g., manufacturer codes)
- Chassis numbers (VIN) are typically alphanumeric

### Data Migration
- Existing integer values are automatically converted to strings
- No data loss during migration
- PostgreSQL `USING` clause handles the conversion

## Files Modified

### Frontend (4 files)
1. Frontend/src/app/models/vehicle.ts
2. Frontend/src/app/models/PropertyDetail.ts
3. Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html
4. Frontend/src/app/vehicle/vehiclelist/vehiclelist.component.ts

### Backend (4 files)
1. Backend/Models/Vehicles/VehiclesPropertyDetail.cs
2. Backend/Models/ViewModels/getVehiclePrintData.cs
3. Backend/Controllers/Vehicles/VehiclesSubController.cs
4. Backend/Infrastructure/Migrations/Vehicle/20260202_ChangePlateNumberToString.cs

### Database Scripts (1 file)
1. Backend/Scripts/change_pilateno_to_string.sql

## Notes
- The DTOs in `Backend/src/Application/Vehicle/DTOs/VehicleDetailDto.cs` already had the correct string type
- Audit tables store values as strings, so no changes needed there
- The change is backward compatible with existing numeric data
- Engine numbers (EnginNo) and chassis numbers (ShasiNo) are now also strings for better flexibility
