# Numeric to Text Conversion - Execution Checklist

## ✅ Completed Steps

- [x] Updated C# models to use `string?` instead of `double?`
- [x] Updated all controllers to handle string values
- [x] Updated DTOs to match model changes
- [x] Created SQL conversion script with view handling
- [x] Created documentation and instructions

## ⏳ Pending Steps - YOU NEED TO DO THESE

### Step 1: Backup Database (RECOMMENDED)
```bash
pg_dump -h localhost -U postgres -d PRMIS > PRMIS_backup_$(date +%Y%m%d_%H%M%S).sql
```

### Step 2: Run SQL Conversion Script in pgAdmin

1. Open pgAdmin
2. Connect to PostgreSQL server
3. Navigate to PRMIS database
4. Right-click → Query Tool
5. Open file: `Backend/Scripts/convert_double_to_text.sql`
6. Click Execute (▶) or press F5
7. Verify success messages in the Messages tab

**Expected Output:**
```
NOTICE:  Dropped dependent views
NOTICE:  Converted PropertyDetails.Price to text
NOTICE:  Converted PropertyDetails.RoyaltyAmount to text
NOTICE:  Converted PropertyDetails.PArea to text
NOTICE:  Converted PropertyDetails.PNumber to text
NOTICE:  Converted BuyerDetail.Price to text
NOTICE:  Converted BuyerDetail.RoyaltyAmount to text
NOTICE:  Converted BuyerDetail.HalfPrice to text
NOTICE:  Converted BuyerDetail.SharePercentage to text
NOTICE:  Converted BuyerDetail.ShareAmount to text
NOTICE:  Converted SellerDetail.SharePercentage to text
NOTICE:  Converted SellerDetail.ShareAmount to text
NOTICE:  Converted VehiclesPropertyDetail.Price to text
NOTICE:  Converted VehiclesPropertyDetail.RoyaltyAmount to text
NOTICE:  Converted CompanyDetail.Tin to text
NOTICE:  Converted LicenseDetail.LicenseNumber to text
NOTICE:  Converted PropertyValuation.ValuationAmount to text
NOTICE:  Converted PropertyPayment.AmountPaid to text
NOTICE:  Converted PropertyPayment.BalanceRemaining to text
NOTICE:  Recreated GetPrintType view
NOTICE:  ========================================
NOTICE:  Double precision to text conversion completed successfully!
NOTICE:  All numeric columns have been converted to text type.
NOTICE:  Views have been recreated.
NOTICE:  ========================================
```

### Step 3: Rebuild the Application

```bash
cd Backend
dotnet clean
dotnet build WebAPIBackend.csproj
```

**Expected Output:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Step 4: Run the Application

```bash
dotnet run --project WebAPIBackend.csproj
```

### Step 5: Test Endpoints

Test these endpoints in your browser or Postman:

1. **Property Details**
   - GET `http://localhost:5000/api/PropertyDetails`
   - GET `http://localhost:5000/api/PropertyDetails/1`

2. **Vehicle Details**
   - GET `http://localhost:5000/api/VehiclesPropertyDetails`

3. **Company Details**
   - GET `http://localhost:5000/api/CompanyDetails`

4. **Dashboard**
   - GET `http://localhost:5000/api/Dashboard/GetEstateDashboardData`
   - GET `http://localhost:5000/api/Dashboard/GetVehicleDashboardData`

### Step 6: Verify Data

Check that:
- [ ] No type mismatch errors
- [ ] Price values display correctly
- [ ] RoyaltyAmount calculations work
- [ ] Dashboard totals are accurate
- [ ] All CRUD operations work

## Troubleshooting

### If SQL script fails:
1. Check the error message in pgAdmin
2. Verify you're connected as postgres user
3. Ensure no other connections are using the database
4. Check if views exist: `SELECT * FROM information_schema.views WHERE table_name = 'GetPrintType';`

### If application build fails:
1. Check for syntax errors in controllers
2. Run `dotnet restore` first
3. Check the error messages carefully

### If endpoints return errors:
1. Check application logs
2. Verify database connection string
3. Test database connection: `psql -h localhost -U postgres -d PRMIS`
4. Check if columns were converted: 
   ```sql
   SELECT column_name, data_type 
   FROM information_schema.columns 
   WHERE table_schema='tr' AND table_name='PropertyDetails' 
   AND column_name IN ('Price', 'RoyaltyAmount', 'PArea', 'PNumber');
   ```

## Rollback (If Needed)

If something goes wrong:

1. **Restore database backup:**
   ```bash
   psql -h localhost -U postgres -d PRMIS < PRMIS_backup_YYYYMMDD_HHMMSS.sql
   ```

2. **Revert C# code changes** (use git):
   ```bash
   git checkout HEAD -- Backend/Models/
   git checkout HEAD -- Backend/Controllers/
   git checkout HEAD -- Backend/src/Application/
   ```

## Success Criteria

✅ All these should be true:
- [ ] SQL script executed without errors
- [ ] Application builds successfully
- [ ] Application runs without crashes
- [ ] All endpoints return data
- [ ] No type mismatch errors in logs
- [ ] Price and RoyaltyAmount values are correct
- [ ] Dashboard calculations are accurate

## Files Reference

- **SQL Script**: `Backend/Scripts/convert_double_to_text.sql`
- **Instructions**: `Backend/Scripts/RUN_CONVERSION_INSTRUCTIONS.md`
- **Summary**: `Backend/NUMERIC_TO_TEXT_CONVERSION_SUMMARY.md`
- **This Checklist**: `Backend/CONVERSION_CHECKLIST.md`

## Current Status

**YOU ARE HERE** → Step 2: Run SQL Conversion Script in pgAdmin

Once you complete Step 2, proceed to Steps 3-6 to verify everything works correctly.
