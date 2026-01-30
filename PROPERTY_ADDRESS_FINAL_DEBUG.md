# Property Address Issue - Final Debugging

## Current Status
Property addresses are being sent in UPDATE requests but returning empty array in GET responses.

## Payload Analysis

### UPDATE Request Payload:
```json
{
  "id": 6,
  "propertyAddresses": [{
    "id": 0,
    "provinceId": 5,
    "districtId": 94,
    "PropertyDetailsId": 6,
    "village": "dfghjmk,"
  }]
}
```

### GET Response:
```json
{
  "id": 6,
  "pnumber": "PROP-20260127170516",
  "propertyAddresses": []  // ← EMPTY!
}
```

## Changes Made

### 1. Added `.Include()` to GetPropertyById
**File**: `Backend/Controllers/PropertyDetailsController.cs` (line 176)
```csharp
var Pro = await _context.PropertyDetails
    .Include(p => p.PropertyAddresses)
    .Where(x => x.Id.Equals(id))
    .ToListAsync();
```

### 2. Added Logging to UpsertPropertyAddressAsync
**File**: `Backend/Controllers/PropertyDetailsController.cs` (line 68-110)
- Logs when PropertyAddresses is null
- Logs when creating new address
- Logs when updating existing address
- Logs when save is successful

### 3. Added Logging to UpdatePropertyDetails
**File**: `Backend/Controllers/PropertyDetailsController.cs` (line 520-540)
- Logs property ID
- Logs PropertyAddresses count
- Logs address details (ProvinceId, DistrictId, Village, PropertyDetailsId)

## CRITICAL: Backend Must Be Restarted!

**The code changes will NOT take effect until you restart the backend service!**

### How to Restart:

#### Option 1: If running in Visual Studio
1. Stop debugging (Shift+F5)
2. Rebuild solution
3. Start debugging again (F5)

#### Option 2: If running via command line
```bash
# Stop the current process (Ctrl+C)
cd Backend
dotnet build WebAPIBackend.csproj
dotnet run
```

#### Option 3: If running as a service
```bash
# Stop the service
sudo systemctl stop prmis-backend

# Start the service
sudo systemctl start prmis-backend

# Check logs
sudo journalctl -u prmis-backend -f
```

## Testing Steps

### Step 1: Restart Backend
**IMPORTANT**: Restart the backend service first!

### Step 2: Update Property
1. Open property ID 6
2. Modify the address fields (province, district, village)
3. Click save/update
4. Watch the backend console

### Step 3: Check Backend Console Logs

**Expected output**:
```
UpdatePropertyDetails: Property ID=6
UpdatePropertyDetails: PropertyAddresses count=1
UpdatePropertyDetails: Address - ProvinceId=5, DistrictId=94, Village=dfghjmk,, PropertyDetailsId=6
UpsertPropertyAddressAsync: Processing address for property 6, ProvinceId=5, DistrictId=94, Village=dfghjmk,
UpsertPropertyAddressAsync: Creating new address for property 6
UpsertPropertyAddressAsync: Address saved successfully for property 6
```

**If you see**:
```
UpdatePropertyDetails: PropertyAddresses count=0
```
Then the address is NOT being sent from frontend.

**If you see**:
```
UpsertPropertyAddressAsync: PropertyAddresses is null for property 6
```
Then the request object doesn't have PropertyAddresses.

### Step 4: Check Database
Run this SQL query:
```sql
SELECT * FROM tr."PropertyAddresses" 
WHERE "PropertyDetailsId" = 6;
```

**Expected result**: One row with the address data.

**If no rows**: Address is not being saved. Check backend logs for errors.

### Step 5: Test GET Endpoint
After confirming address exists in database:

```
GET http://localhost:5143/api/PropertyDetails/6
```

**Expected response**:
```json
{
  "id": 6,
  "propertyAddresses": [{
    "id": 123,  // ← Should have a real ID, not 0
    "provinceId": 5,
    "districtId": 94,
    "propertyDetailsId": 6,
    "village": "dfghjmk,"
  }]
}
```

## Possible Issues

### Issue 1: Backend Not Restarted
**Symptom**: No console logs appear when updating property
**Solution**: Restart backend service

### Issue 2: Address Not Being Sent
**Symptom**: Console shows "PropertyAddresses count=0"
**Solution**: Check frontend code, verify propertyAddresses is in request payload

### Issue 3: Address Not Being Saved
**Symptom**: Console shows "Address saved successfully" but database has no rows
**Solution**: 
- Check for database errors in backend logs
- Verify database connection
- Check if SaveChangesAsync() is throwing an exception

### Issue 4: Address Saved But Not Retrieved
**Symptom**: Database has address but GET returns empty array
**Solution**:
- Verify backend was restarted after adding `.Include()`
- Check if there's a caching issue
- Try clearing browser cache

## Diagnostic SQL Queries

```sql
-- Check if address exists
SELECT * FROM tr."PropertyAddresses" 
WHERE "PropertyDetailsId" = 6;

-- Check all addresses
SELECT pa.*, pd."Pnumber"
FROM tr."PropertyAddresses" pa
LEFT JOIN tr."PropertyDetails" pd ON pa."PropertyDetailsId" = pd."Id"
ORDER BY pa."CreatedAt" DESC;

-- Check for duplicate addresses
SELECT "PropertyDetailsId", COUNT(*) as count
FROM tr."PropertyAddresses"
GROUP BY "PropertyDetailsId"
HAVING COUNT(*) > 1;

-- Delete all addresses for property 6 (if needed to start fresh)
DELETE FROM tr."PropertyAddresses" 
WHERE "PropertyDetailsId" = 6;
```

## Next Steps

1. ✅ Code changes applied
2. ⚠️ **RESTART BACKEND SERVICE** ← DO THIS NOW!
3. Update property with address
4. Check backend console logs
5. Check database
6. Test GET endpoint
7. Share results:
   - Backend console logs
   - Database query results
   - GET endpoint response

## Files Modified

1. ✅ `Backend/Controllers/PropertyDetailsController.cs`
   - Added `.Include(p => p.PropertyAddresses)` to GetPropertyById
   - Added logging to UpsertPropertyAddressAsync
   - Added logging to UpdatePropertyDetails

2. 📝 `Backend/Scripts/check_property_addresses.sql`
   - Diagnostic SQL queries

3. 📝 `PROPERTY_ADDRESS_FINAL_DEBUG.md`
   - This comprehensive debugging guide

## Status
🔍 **AWAITING BACKEND RESTART** - User must restart backend service and test again

---

**CRITICAL REMINDER**: The backend service MUST be restarted for code changes to take effect!
