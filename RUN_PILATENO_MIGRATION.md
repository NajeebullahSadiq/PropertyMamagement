# Quick Guide: Apply Vehicle Number Fields String Migration

## For Development Environment

### Step 1: Apply Database Migration
Choose one of the following methods:

#### Method A: Direct SQL (Fastest)
```bash
# Connect to your PostgreSQL database and run:
psql -U your_username -d your_database_name

# Then execute:
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE TEXT USING "PermitNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE TEXT USING "PilateNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE TEXT USING "EnginNo"::TEXT;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE TEXT USING "ShasiNo"::TEXT;
```

#### Method B: Using SQL Script File
```bash
psql -U your_username -d your_database_name -f Backend/Scripts/change_pilateno_to_string.sql
```

#### Method C: Using EF Core (if configured)
```bash
cd Backend
dotnet ef database update
```

### Step 2: Rebuild Backend
```bash
cd Backend
dotnet build
```

### Step 3: Restart Backend Server
```bash
cd Backend
dotnet run
```

### Step 4: Rebuild Frontend
```bash
cd Frontend
npm install  # Only if needed
ng build
```

### Step 5: Test the Changes
1. Navigate to Vehicle module (ثبت مشخصات وسایط نقلیه)
2. Try entering alphanumeric values:
   - **Plate Number (نمبر پلیت)**: ABC-123, کابل-1234, 12-ABC-456
   - **Permit Number (نمبر جواز سیر)**: LIC-2024-001
   - **Engine Number (شماره انجین)**: ENG-ABC-12345, موتر-123
   - **Chassis Number (شماره شاسی)**: VIN-1HGBH41JXMN109186
3. Verify all numbers are saved and displayed correctly
4. Check the vehicle list and search functionality

## For Production Environment

### Step 1: Backup Database
```bash
pg_dump -U your_username -d your_database_name > backup_before_vehicle_string_change.sql
```

### Step 2: Apply Migration During Maintenance Window
```bash
psql -U your_username -d your_database_name -f Backend/Scripts/change_pilateno_to_string.sql
```

### Step 3: Deploy Updated Code
```bash
# Deploy backend
cd Backend
dotnet publish -c Release
# Copy published files to server

# Deploy frontend
cd Frontend
ng build --configuration production
# Copy dist files to web server
```

### Step 4: Restart Services
```bash
# Restart backend service
sudo systemctl restart prmis-backend

# Restart nginx (if needed)
sudo systemctl restart nginx
```

### Step 5: Verify Production
- Test vehicle creation with alphanumeric values for all fields
- Verify existing vehicles still display correctly
- Check vehicle search and filtering

## Verification Checklist

- [ ] Database columns changed to TEXT type (PermitNo, PilateNo, EnginNo, ShasiNo)
- [ ] Backend compiles without errors
- [ ] Frontend compiles without errors
- [ ] Can create new vehicle with alphanumeric plate number
- [ ] Can create new vehicle with alphanumeric engine number
- [ ] Can create new vehicle with alphanumeric chassis number
- [ ] Existing vehicles display correctly
- [ ] Vehicle search works with new string values
- [ ] Vehicle list displays all numbers correctly
- [ ] Print functionality works correctly

## Rollback (If Needed)

```sql
-- WARNING: This will fail if any numbers contain non-numeric characters
ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PermitNo" TYPE INTEGER USING "PermitNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "PilateNo" TYPE INTEGER USING "PilateNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "EnginNo" TYPE INTEGER USING "EnginNo"::INTEGER;

ALTER TABLE tr."VehiclesPropertyDetails" 
ALTER COLUMN "ShasiNo" TYPE INTEGER USING "ShasiNo"::INTEGER;
```

## Troubleshooting

### Issue: Migration fails with "cannot cast type text to integer"
**Solution:** Some existing data might already be text. Check the current column type:
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema = 'tr' 
  AND table_name = 'VehiclesPropertyDetails' 
  AND column_name IN ('PermitNo', 'PilateNo', 'EnginNo', 'ShasiNo');
```

### Issue: Frontend still shows numeric input only
**Solution:** Clear browser cache and rebuild frontend:
```bash
cd Frontend
rm -rf dist .angular/cache
ng build
```

### Issue: Backend validation errors
**Solution:** Ensure backend is rebuilt after model changes:
```bash
cd Backend
dotnet clean
dotnet build
```

## Support

For issues or questions, refer to:
- Full documentation: `PILATENO_STRING_CHANGE.md`
- Migration file: `Backend/Infrastructure/Migrations/Vehicle/20260202_ChangePlateNumberToString.cs`
- SQL script: `Backend/Scripts/change_pilateno_to_string.sql`
