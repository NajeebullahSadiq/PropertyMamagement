# Vehicle Number Fields Migration Summary

## Quick Overview
All vehicle identification number fields have been changed from integer to string type to support alphanumeric values.

## Fields Changed
1. **PilateNo** (نمبر پلیت) - Plate Number
2. **PermitNo** (نمبر جواز سیر) - Permit/License Number
3. **EnginNo** (شماره انجین) - Engine Number
4. **ShasiNo** (شماره شاسی) - Chassis Number

## Why This Change?
- Real-world vehicle numbers often contain letters and special characters
- Engine numbers typically include manufacturer codes (e.g., "ENG-ABC-12345")
- Chassis numbers (VIN) are alphanumeric (e.g., "1HGBH41JXMN109186")
- Plate numbers vary by region and may include letters (e.g., "کابل-1234", "ABC-123")

## What You Need to Do

### 1. Run Database Migration
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

Or use the script file:
```bash
psql -U your_username -d your_database_name -f Backend/Scripts/change_pilateno_to_string.sql
```

### 2. Rebuild and Restart
```bash
# Backend
cd Backend
dotnet build
dotnet run

# Frontend
cd Frontend
ng build
```

## What Changed in the UI
- Input fields now accept letters, numbers, and special characters
- No more numeric-only restriction
- Users can enter values like:
  - Plate: "ABC-123" or "کابل-1234"
  - Engine: "ENG-ABC-12345"
  - Chassis: "VIN-1HGBH41JXMN109186"

## Backward Compatibility
✅ Existing numeric data is automatically converted to strings
✅ No data loss
✅ All existing vehicles will continue to work

## Files Modified
- **Frontend**: 4 files (models, components)
- **Backend**: 4 files (models, controllers)
- **Database**: 1 migration script

## Documentation
- Full details: `PILATENO_STRING_CHANGE.md`
- Step-by-step guide: `RUN_PILATENO_MIGRATION.md`
- Migration file: `Backend/Infrastructure/Migrations/Vehicle/20260202_ChangePlateNumberToString.cs`
- SQL script: `Backend/Scripts/change_pilateno_to_string.sql`

## Testing
After deployment, test:
1. Create a new vehicle with alphanumeric values in all fields
2. Search for vehicles using the new alphanumeric values
3. View vehicle details
4. Print vehicle documents
5. Verify existing vehicles still display correctly

## Need Help?
Refer to the detailed documentation files or check the migration scripts for exact SQL commands.
