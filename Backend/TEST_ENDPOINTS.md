# Test Endpoints After Conversion

## Application Status
✅ Build succeeded
✅ Database columns converted to TEXT
✅ View COALESCE statements fixed
✅ Application should be running on http://localhost:5143

## Endpoints to Test

### 1. Property Details
```
GET http://localhost:5143/api/PropertyDetails
```
**Expected**: List of properties with Price, RoyaltyAmount as strings

### 2. Property Details by ID
```
GET http://localhost:5143/api/PropertyDetails/1
```
**Expected**: Single property details

### 3. Property View
```
GET http://localhost:5143/api/PropertyDetails/GetView/1
```
**Expected**: Detailed property view with all related data

### 4. Vehicle Details
```
GET http://localhost:5143/api/VehiclesPropertyDetails
```
**Expected**: List of vehicles with Price, RoyaltyAmount as strings

### 5. Company Details
```
GET http://localhost:5143/api/CompanyDetails
```
**Expected**: List of companies with Tin as string

### 6. Dashboard - Estate Data
```
GET http://localhost:5143/api/Dashboard/GetEstateDashboardData
```
**Expected**: Dashboard statistics with totals

### 7. Dashboard - Vehicle Data
```
GET http://localhost:5143/api/Dashboard/GetVehicleDashboardData
```
**Expected**: Vehicle statistics with totals

### 8. Dashboard - Company Data
```
GET http://localhost:5143/api/Dashboard/GetCompanyDashboardData
```
**Expected**: Company statistics

## Testing with PowerShell

```powershell
# Test Property Details
Invoke-RestMethod -Uri "http://localhost:5143/api/PropertyDetails" -Method Get

# Test Dashboard
Invoke-RestMethod -Uri "http://localhost:5143/api/Dashboard/GetEstateDashboardData" -Method Get
```

## Testing with Browser

Simply open these URLs in your browser:
- http://localhost:5143/api/PropertyDetails
- http://localhost:5143/api/Dashboard/GetEstateDashboardData
- http://localhost:5143/api/Dashboard/GetVehicleDashboardData

## What to Verify

✅ **No Type Mismatch Errors**: Should not see "Reading as 'System.String' is not supported for fields having DataTypeName 'double precision'"

✅ **Data Returns Correctly**: All endpoints return data without errors

✅ **Price Values**: Price and RoyaltyAmount should be strings (e.g., "1000000" not 1000000)

✅ **Calculations Work**: Dashboard totals should be calculated correctly

✅ **CRUD Operations**: Try creating/updating a property to ensure writes work

## Common Issues

### Issue: "Cannot connect to database"
**Solution**: Check if PostgreSQL is running and connection string is correct

### Issue: "View does not exist"
**Solution**: The view should have been created automatically. If not, run the SQL script manually

### Issue: "Type mismatch error"
**Solution**: Some columns might not have been converted. Check with:
```sql
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_schema='tr' AND table_name='PropertyDetails' 
AND column_name IN ('Price', 'RoyaltyAmount', 'PArea', 'PNumber');
```

## Success Criteria

All these should be TRUE:
- [ ] Application starts without errors
- [ ] No view creation errors in logs
- [ ] GET /api/PropertyDetails returns data
- [ ] GET /api/Dashboard/GetEstateDashboardData returns statistics
- [ ] Price and RoyaltyAmount are strings in responses
- [ ] No type mismatch errors in application logs

## Next Steps After Testing

If all tests pass:
1. Test the frontend application
2. Verify all CRUD operations work
3. Test print functionality
4. Verify dashboard charts display correctly

If tests fail:
1. Check application logs for specific errors
2. Verify database column types with the SQL query above
3. Check if the view was created correctly
4. Review the error messages and fix accordingly
