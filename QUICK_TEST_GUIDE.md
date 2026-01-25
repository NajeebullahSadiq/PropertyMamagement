# Quick Test Guide - Province License Numbering

## ✅ Status: Implementation Complete

The missing `/api/SellerDetails/getProvinces` endpoint has been added and the system is ready to test.

## Quick Test Steps

### 1. Start Backend
```bash
cd Backend
dotnet run
```
Backend should start on: `http://localhost:5143`

### 2. Start Frontend
```bash
cd Frontend
npm start
```
Frontend should start on: `http://localhost:4200`

### 3. Test the Feature

#### Step-by-Step Test:
1. **Login** to the application
2. **Navigate** to Company Registration
3. **Fill** Company Details tab
4. **Go to** License Details tab
5. **Observe**: Province dropdown should be the first field
6. **Select** a province (e.g., "Kabul")
7. **Fill** other required fields:
   - Issue Date (تاریخ صدور)
   - Expire Date (تاریخ ختم) - auto-calculated
   - Area (ساحه)
   - Office Address (آدرس دفتر)
   - License Type (نوع جواز)
8. **Click** Save button
9. **Verify**: License number field should show auto-generated number like `KBL-0001`

### 4. Test Different Provinces

Try creating licenses for different provinces to verify unique numbering:

| Province | Expected Format | Example |
|----------|----------------|---------|
| Kabul | KBL-XXXX | KBL-0001, KBL-0002 |
| Kandahar | KHR-XXXX | KHR-0001, KHR-0002 |
| Herat | HRT-XXXX | HRT-0001, HRT-0002 |
| Balkh | BLK-XXXX | BLK-0001, BLK-0002 |

### 5. Verify in Database

```sql
-- Check generated license numbers
SELECT 
    l."Id",
    l."ProvinceId",
    loc."Name" as "ProvinceName",
    l."LicenseNumber",
    l."IssueDate",
    l."OfficeAddress"
FROM org."LicenseDetails" l
LEFT JOIN look."Location" loc ON l."ProvinceId" = loc."ID"
WHERE l."ProvinceId" IS NOT NULL
ORDER BY l."ProvinceId", l."LicenseNumber";
```

## Expected Behavior

### ✅ Success Indicators:
- Province dropdown loads with all provinces
- License number field is read-only
- After saving, license number appears automatically
- Each province has its own sequence (KBL-0001, KBL-0002, etc.)
- Different provinces have different codes (KBL vs KHR)

### ❌ Potential Issues:

#### Issue 1: Province dropdown is empty
**Cause**: No provinces in database
**Solution**: 
```sql
-- Check if provinces exist
SELECT * FROM look."Location" WHERE "TypeId" = 2;
```

#### Issue 2: 400 Error on getProvinces
**Cause**: Backend not running or endpoint not found
**Solution**: 
- Restart backend
- Check URL: `http://localhost:5143/api/SellerDetails/getProvinces`
- Verify endpoint exists in SellerDetailsController.cs

#### Issue 3: License number not generating
**Cause**: ProvinceId not being sent to backend
**Solution**: 
- Check browser console for errors
- Verify provinceId is selected in form
- Check network tab to see if provinceId is in request body

## API Testing (Optional)

### Test getProvinces Endpoint
```bash
curl http://localhost:5143/api/SellerDetails/getProvinces
```

Expected response:
```json
[
  {
    "id": 1,
    "name": "Kabul",
    "dari": "کابل",
    "pashto": "کابل",
    "typeId": 2,
    "parentId": null
  },
  // ... more provinces
]
```

### Test License Creation
```bash
curl -X POST http://localhost:5143/api/LicenseDetail \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "provinceId": 1,
    "issueDate": "1403-10-05",
    "expireDate": "1406-10-05",
    "areaId": 1,
    "officeAddress": "Test Address",
    "licenseType": "realEstate",
    "companyId": 1
  }'
```

Expected response:
```json
{
  "id": 123
}
```

Then query to see the generated license number:
```bash
curl http://localhost:5143/api/LicenseDetail/123
```

## Troubleshooting Commands

### Check Backend Logs
Look for any errors related to:
- LicenseNumberGenerator
- getProvinces endpoint
- Database queries

### Check Frontend Console
Open browser DevTools (F12) and look for:
- Network errors (400, 500)
- Console errors
- Failed API calls

### Verify Service Registration
```bash
# Search for service registration
grep -r "ILicenseNumberGenerator" Backend/Program.cs
```

Should show:
```csharp
builder.Services.AddScoped<WebAPIBackend.Services.ILicenseNumberGenerator, WebAPIBackend.Services.LicenseNumberGenerator>();
```

## Success Criteria

✅ All tests pass when:
1. Province dropdown loads successfully
2. License number auto-generates on save
3. Each province has unique sequence
4. License numbers follow format: `CODE-0001`
5. No errors in browser console
6. No errors in backend logs

## Next Steps After Testing

Once testing is successful:
1. ✅ Mark feature as complete
2. 📝 Document for users
3. 🎓 Train staff on new system
4. 🚀 Deploy to production
5. 📊 Monitor for issues

## Support

If you encounter issues during testing:
1. Check this guide first
2. Review backend logs
3. Check browser console
4. Verify database has province data
5. Ensure all migrations are applied
