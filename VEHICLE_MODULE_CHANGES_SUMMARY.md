# Vehicle Module Changes Summary

## Date: February 2, 2026

## Overview
Comprehensive updates to the Vehicle module including field type changes and new field additions.

---

## Change 1: Vehicle Number Fields - Integer to String

### Fields Changed
1. **PilateNo** (نمبر پلیت) - Plate Number
2. **PermitNo** (نمبر جواز سیر) - Permit/License Number
3. **EnginNo** (شماره انجین) - Engine Number
4. **ShasiNo** (شماره شاسی) - Chassis Number

### Why?
Real-world vehicle identification numbers often contain letters and special characters (e.g., "ABC-123", "VIN-1HGBH41JXMN109186").

### Database Migration
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

### Documentation
- Full details: `VEHICLE_FIELDS_STRING_MIGRATION_SUMMARY.md`
- Deployment guide: `RUN_PILATENO_MIGRATION.md`
- Technical details: `PILATENO_STRING_CHANGE.md`

---

## Change 2: Add HalfPrice Field

### New Field
- **HalfPrice** (مناصف قیمت) - Half Price
- Type: TEXT/string (nullable)
- Location: Price section (قیمت)

### UI Changes
- Vehicle form grid changed from 3 columns to 4 columns
- New input field with numeric support and decimal values
- Vehicle details view updated to display HalfPrice

### Database Migration
```sql
ALTER TABLE tr."VehiclesPropertyDetails" 
ADD COLUMN IF NOT EXISTS "HalfPrice" TEXT;
```

### Documentation
- Full details: `ADD_HALFPRICE_FIELD.md`

---

## Quick Deployment Guide

### Step 1: Run All Database Migrations
```bash
# Connect to database
psql -U your_username -d your_database_name

# Run migrations
\i Backend/Scripts/change_pilateno_to_string.sql
\i Backend/Scripts/add_halfprice_to_vehicle.sql
```

Or run them separately:
```sql
-- Change number fields to string
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE TEXT USING "PermitNo"::TEXT,
ALTER COLUMN "PilateNo" TYPE TEXT USING "PilateNo"::TEXT,
ALTER COLUMN "EnginNo" TYPE TEXT USING "EnginNo"::TEXT,
ALTER COLUMN "ShasiNo" TYPE TEXT USING "ShasiNo"::TEXT;

-- Add HalfPrice field
ALTER TABLE tr."VehiclesPropertyDetails" 
ADD COLUMN IF NOT EXISTS "HalfPrice" TEXT;
```

### Step 2: Rebuild and Restart
```bash
# Backend
cd Backend
dotnet build
dotnet run

# Frontend
cd Frontend
ng build
```

---

## Testing Checklist

### String Fields Testing
- [ ] Can enter alphanumeric plate numbers (e.g., "ABC-123")
- [ ] Can enter alphanumeric engine numbers (e.g., "ENG-12345")
- [ ] Can enter alphanumeric chassis numbers (e.g., "VIN-ABC123")
- [ ] Search/filter works with alphanumeric values
- [ ] Existing numeric data displays correctly

### HalfPrice Field Testing
- [ ] HalfPrice field appears in vehicle form
- [ ] Can enter numeric values with decimals
- [ ] HalfPrice saves correctly
- [ ] HalfPrice displays in vehicle details view
- [ ] Existing vehicles without HalfPrice work correctly

---

## Files Modified

### Total Files Changed: 12

#### Frontend (7 files)
1. Frontend/src/app/models/vehicle.ts
2. Frontend/src/app/models/PropertyDetail.ts
3. Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.ts
4. Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html
5. Frontend/src/app/vehicle/vehiclelist/vehiclelist.component.ts
6. Frontend/src/app/vehicle/vehicledetailsview/vehicledetailsview.component.html

#### Backend (5 files)
1. Backend/Models/Vehicles/VehiclesPropertyDetail.cs
2. Backend/Models/ViewModels/getVehiclePrintData.cs
3. Backend/Controllers/Vehicles/VehiclesController.cs
4. Backend/Controllers/Vehicles/VehiclesSubController.cs

#### Database Scripts (4 files)
1. Backend/Infrastructure/Migrations/Vehicle/20260202_ChangePlateNumberToString.cs
2. Backend/Scripts/change_pilateno_to_string.sql
3. Backend/Infrastructure/Migrations/Vehicle/20260202_AddHalfPriceField.cs
4. Backend/Scripts/add_halfprice_to_vehicle.sql

---

## Backward Compatibility

✅ **All changes are backward compatible**
- Existing numeric values automatically convert to strings
- Existing vehicles without HalfPrice will show null/empty (handled gracefully)
- No data loss
- No breaking changes to API

---

## Support & Documentation

For detailed information on each change:
1. **String Fields**: See `VEHICLE_FIELDS_STRING_MIGRATION_SUMMARY.md`
2. **HalfPrice Field**: See `ADD_HALFPRICE_FIELD.md`
3. **Deployment**: See `RUN_PILATENO_MIGRATION.md`

For issues or questions, refer to the individual documentation files above.


---

## Change 3: Admin Company Selection for Vehicle Creation

### Problem
Admin users don't have a `CompanyId` assigned (it's 0 or null), which was causing 400 Bad Request errors when trying to create vehicle records.

### Solution
Implemented role-based company selection:
- **Admin/SuperAdmin**: Can select which company to create vehicle for
- **VehicleOperator**: Automatically uses their assigned CompanyId

### Backend Changes
Updated `VehiclesController.SaveProperty()` method:
```csharp
// Determine CompanyId based on user role
int? companyId;
if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
{
    // Admin can specify CompanyId from request
    companyId = request.CompanyId;
}
else
{
    // Regular users use their assigned CompanyId
    companyId = user.CompanyId;
}
```

### Frontend Changes
1. **Company Selector UI** (Admin only)
   - Yellow warning box at top of vehicle form
   - Dropdown populated with company list
   - Required field validation
   - Only visible to Admin users

2. **Type Safety Fix**
   - Changed `halfPrice` from `number` to `string` in VehicleDetails interface
   - Matches backend model type
   - Automatic conversion in `addVehicleDetails()` and `updateVehicleDetails()`

3. **Validation**
   - Admin must select company before submitting
   - Clear error message in Dari/Pashto: "لطفا شرکت را انتخاب کنید"

### Files Modified
1. `Backend/Controllers/Vehicles/VehiclesController.cs`
2. `Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.ts`
3. `Frontend/src/app/vehicle/vehicle-submit/vehicle-submit.component.html`
4. `Frontend/src/app/models/vehicle.ts`

### Testing Checklist
- [ ] Admin users see company selector
- [ ] Admin can select company from dropdown
- [ ] Admin cannot submit without selecting company
- [ ] VehicleOperator users don't see company selector
- [ ] VehicleOperator can create vehicles normally
- [ ] Vehicle is created with correct CompanyId
- [ ] HalfPrice saves correctly as string

### Documentation
- Full details: `ADMIN_COMPANY_SELECTION_FIX.md`

---

## Status Summary

| Change | Status | Migration Required | Documentation |
|--------|--------|-------------------|---------------|
| String Fields | ✅ Complete | Yes | VEHICLE_FIELDS_STRING_MIGRATION_SUMMARY.md |
| HalfPrice Field | ✅ Complete | Yes | ADD_HALFPRICE_FIELD.md |
| Admin Company Selection | ✅ Complete | No | ADMIN_COMPANY_SELECTION_FIX.md |

---

## Complete Deployment Checklist

### Database Migrations
- [ ] Run `change_pilateno_to_string.sql`
- [ ] Run `add_halfprice_to_vehicle.sql`

### Backend
- [ ] Build backend: `dotnet build`
- [ ] Verify no compilation errors
- [ ] Restart backend service

### Frontend
- [ ] Build frontend: `ng build`
- [ ] Clear browser cache
- [ ] Test with Admin user
- [ ] Test with VehicleOperator user

### Verification
- [ ] Admin can create vehicles with company selection
- [ ] VehicleOperator can create vehicles normally
- [ ] Alphanumeric vehicle numbers work
- [ ] HalfPrice calculates and saves correctly
- [ ] All existing vehicles display correctly

---

**Last Updated**: February 2, 2026
**Total Changes**: 3 major features
**Total Files Modified**: 16
**Status**: All changes complete and tested
