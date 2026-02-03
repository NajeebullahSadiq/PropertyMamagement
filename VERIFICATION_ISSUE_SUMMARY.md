# Property Verification Issue - Summary

## Problem
Verification code `PRO-2026-D3XU8P` returns invalid status with no seller/buyer information:

```json
{
  "isValid": false,
  "status": "Invalid",
  "verificationCode": "PRO-2026-D3XU8P",
  "documentType": "",
  "licenseNumber": null,
  "holderName": null,
  "sellerInfo": null,
  "buyerInfo": null
}
```

## Root Cause
The verification code either:
1. **Does not exist** in the `DocumentVerifications` table
2. **Property was deleted** after verification was created
3. **Signature mismatch** due to property data changes
4. **Missing seller/buyer data** in the property record

## Diagnostic Steps

### Quick Diagnosis
Run the comprehensive diagnostic script:
```bash
psql -U your_user -d your_database -f Backend/Scripts/diagnose_verification_PRO-2026-D3XU8P.sql
```

This will check:
- ✓ Verification code exists
- ✓ Property exists
- ✓ Seller data exists
- ✓ Buyer data exists
- ✓ Location data is valid
- ✓ Verification is not revoked

### Manual Check
```sql
-- Check if verification exists
SELECT * FROM org.DocumentVerifications 
WHERE VerificationCode = 'PRO-2026-D3XU8P';
```

## Solutions

### Solution 1: Regenerate from UI (Recommended)
1. Log in to the system
2. Go to Property List
3. Find the property (search by property number or names)
4. Click "View" or "Print"
5. The print page will automatically generate a new verification code
6. Verify the new code works

### Solution 2: Generate via API
```bash
# Get your authentication token
TOKEN="your_jwt_token"

# Find the property ID (from database or UI)
PROPERTY_ID=123

# Generate verification code
curl -X POST http://localhost:5143/api/verification/generate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"documentId\": $PROPERTY_ID,
    \"documentType\": \"PropertyDocument\"
  }"

# Response will include new verification code
# Test it:
curl http://localhost:5143/api/verification/verify/NEW_CODE_HERE
```

### Solution 3: Fix Missing Data
If property exists but has no seller/buyer:

1. Edit the property in the UI
2. Add seller details (name, national ID, address, etc.)
3. Add buyer details
4. Save the property
5. Regenerate verification code (Solution 1 or 2)

### Solution 4: Revoke and Regenerate
If property data was modified after verification:

```sql
-- Revoke old verification
UPDATE org.DocumentVerifications
SET IsRevoked = true,
    RevokedAt = NOW(),
    RevokedBy = 'admin',
    RevokedReason = 'Property data was modified, regenerating verification'
WHERE VerificationCode = 'PRO-2026-D3XU8P';
```

Then use Solution 1 or 2 to generate a new code.

## Testing the Fix

After applying any solution, test the verification:

```bash
curl http://localhost:5143/api/verification/verify/PRO-2026-D3XU8P
```

**Expected Success Response:**
```json
{
  "isValid": true,
  "status": "Valid",
  "verificationCode": "PRO-2026-D3XU8P",
  "documentType": "PropertyDocument",
  "licenseNumber": "PROP-20260203...",
  "holderName": "SellerFirstName - BuyerFirstName",
  "holderPhoto": "/path/to/seller/photo.jpg",
  "issueDate": "2026-02-03T...",
  "expiryDate": null,
  "companyTitle": null,
  "officeAddress": "ProvinceName, DistrictName",
  "sellerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "grandFatherName": "...",
    "electronicNationalIdNumber": "...",
    "phoneNumber": "...",
    "photo": "/path/to/photo.jpg",
    "province": "کابل",
    "district": "ناحیه اول",
    "village": "..."
  },
  "buyerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "grandFatherName": "...",
    "electronicNationalIdNumber": "...",
    "phoneNumber": "...",
    "photo": "/path/to/photo.jpg",
    "province": "کابل",
    "district": "ناحیه دوم",
    "village": "..."
  }
}
```

## How Verification Works

### 1. Generation (When Property is Printed)
```
User clicks "Print" 
  → Frontend calls GET /api/PropertyDetails/GetPrintRecord/{id}
  → Frontend calls POST /api/verification/generate
  → Backend creates DocumentVerification record
  → Returns verification code and URL
  → Frontend displays QR code on print page
```

### 2. Verification (When QR Code is Scanned)
```
User scans QR code
  → Opens URL: https://prmis.gov.af/verify/PRO-2026-XXXXXX
  → Frontend calls GET /api/verification/verify/PRO-2026-XXXXXX
  → Backend:
      1. Looks up verification code in database
      2. Checks if revoked
      3. Retrieves property details
      4. Loads seller and buyer information
      5. Verifies digital signature
      6. Returns all data
  → Frontend displays verification result
```

## Prevention

To prevent this issue in the future:

1. **Always complete property data** before printing
   - Add all seller details
   - Add all buyer details
   - Add property address
   - Mark property as complete

2. **Don't close print page** until QR code is visible
   - Wait for verification code to generate
   - Verify QR code is displayed
   - Test scan before distributing

3. **Don't modify property** after printing
   - If you must modify, regenerate verification code
   - Old verification codes become invalid after data changes

4. **Test verification** before distributing documents
   - Scan QR code yourself
   - Verify all data displays correctly
   - Check seller/buyer information is complete

## Files Created

1. **Backend/Scripts/check_verification_code.sql**
   - Basic SQL queries to check verification status

2. **Backend/Scripts/diagnose_verification_PRO-2026-D3XU8P.sql**
   - Comprehensive diagnostic script with detailed checks

3. **PROPERTY_VERIFICATION_FIX.md**
   - Detailed technical documentation

4. **PROPERTY_VERIFICATION_DEBUG.md**
   - Step-by-step debugging guide

5. **VERIFICATION_ISSUE_SUMMARY.md** (this file)
   - Quick reference summary

## Next Steps

1. **Run diagnostic script** to identify the exact problem
2. **Apply appropriate solution** based on diagnostic results
3. **Test verification** to confirm fix works
4. **Document the issue** for future reference

## Need Help?

If the issue persists after trying these solutions:

1. Check backend logs for errors
2. Verify database connection is working
3. Ensure all migrations have been run
4. Check that VerificationService is properly registered in DI container
5. Verify appsettings.json has correct Verification:BaseUrl

## Contact

For additional support, provide:
- Diagnostic script output
- Backend error logs
- Property ID
- Steps to reproduce the issue
