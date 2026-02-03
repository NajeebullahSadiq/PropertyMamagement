# Property Verification Debug Guide

## Issue Summary
Verification code `PRO-2026-D3XU8P` returns:
```json
{
  "isValid": false,
  "status": "Invalid",
  "verificationCode": "PRO-2026-D3XU8P",
  "documentType": "",
  "sellerInfo": null,
  "buyerInfo": null
}
```

## Step-by-Step Debugging

### Step 1: Check if Verification Code Exists in Database

Run this SQL query:

```sql
-- Check verification record
SELECT 
    Id,
    VerificationCode,
    DocumentId,
    DocumentType,
    IsRevoked,
    RevokedReason,
    CreatedAt,
    CreatedBy,
    DigitalSignature
FROM org.DocumentVerifications
WHERE VerificationCode = 'PRO-2026-D3XU8P';
```

**Possible Results:**

#### A) No rows returned
**Meaning**: Verification code was never generated
**Cause**: 
- Property was created but print page was never opened
- Error occurred during verification generation
- User closed print page before verification was generated

**Fix**: 
1. Find the property that should have this code
2. Open the print page again to regenerate the code
3. Or manually generate using API

#### B) Row exists but IsRevoked = true
**Meaning**: Verification was revoked
**Fix**: Check RevokedReason and decide if you need to create a new verification

#### C) Row exists and IsRevoked = false
**Meaning**: Verification exists but something else is wrong
**Continue to Step 2**

### Step 2: Check Property Data

If verification exists, check the property:

```sql
-- Get the DocumentId from Step 1, then:
SELECT 
    pd.Id,
    pd.PNumber,
    pd.Parea,
    pd.Price,
    pd.PropertyTypeId,
    pd.TransactionTypeId,
    pd.iscomplete,
    pd.iseditable,
    pd.CreatedAt,
    pd.CreatedBy,
    pd.CompanyId,
    pd.IssuanceNumber,
    pd.IssuanceDate
FROM org.PropertyDetails pd
WHERE pd.Id = [DocumentId from Step 1];
```

**Check:**
- Does the property exist?
- Is `iscomplete` = true?
- Does it have valid data?

### Step 3: Check Seller Data

```sql
SELECT 
    sd.Id,
    sd.FirstName,
    sd.FatherName,
    sd.GrandFather,
    sd.ElectronicNationalIdNumber,
    sd.PhoneNumber,
    sd.Photo,
    sd.PaddressProvinceId,
    sd.PaddressDistrictId,
    sd.PaddressVillage,
    sd.PropertyDetailsId
FROM org.SellerDetails sd
WHERE sd.PropertyDetailsId = [PropertyId from Step 2];
```

**Expected**: At least one seller record

### Step 4: Check Buyer Data

```sql
SELECT 
    bd.Id,
    bd.FirstName,
    bd.FatherName,
    bd.GrandFather,
    bd.ElectronicNationalIdNumber,
    bd.PhoneNumber,
    bd.Photo,
    bd.PaddressProvinceId,
    bd.PaddressDistrictId,
    bd.PaddressVillage,
    bd.PropertyDetailsId
FROM org.BuyerDetails bd
WHERE bd.PropertyDetailsId = [PropertyId from Step 2];
```

**Expected**: At least one buyer record

### Step 5: Check Location Data

```sql
-- Check if province/district IDs are valid
SELECT 
    l.Id,
    l.Name,
    l.Dari,
    l.ParentId
FROM org.Locations l
WHERE l.Id IN (
    SELECT DISTINCT PaddressProvinceId FROM org.SellerDetails WHERE PropertyDetailsId = [PropertyId]
    UNION
    SELECT DISTINCT PaddressDistrictId FROM org.SellerDetails WHERE PropertyDetailsId = [PropertyId]
    UNION
    SELECT DISTINCT PaddressProvinceId FROM org.BuyerDetails WHERE PropertyDetailsId = [PropertyId]
    UNION
    SELECT DISTINCT PaddressDistrictId FROM org.BuyerDetails WHERE PropertyDetailsId = [PropertyId]
);
```

## Common Problems and Solutions

### Problem 1: Verification Code Not Found

**Symptoms**: Step 1 returns no rows

**Solution A - Regenerate from UI**:
1. Log in to the system
2. Navigate to property list
3. Find the property (search by property number or seller/buyer name)
4. Click "View" or "Print"
5. The print page will automatically generate a new verification code

**Solution B - Generate via API**:
```bash
# Get authentication token first
TOKEN="your_jwt_token_here"

# Find property ID (you'll need to search your database or UI)
PROPERTY_ID=123

# Generate verification code
curl -X POST http://localhost:5143/api/verification/generate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{
    \"documentId\": $PROPERTY_ID,
    \"documentType\": \"PropertyDocument\"
  }"
```

**Solution C - Manual Database Insert** (Last resort):
```sql
-- Only use if you can't access the UI or API
-- Replace PROPERTY_ID with actual property ID

INSERT INTO org.DocumentVerifications (
    VerificationCode,
    DocumentId,
    DocumentType,
    DigitalSignature,
    DocumentSnapshot,
    CreatedAt,
    CreatedBy,
    IsRevoked
) VALUES (
    'PRO-2026-D3XU8P',
    PROPERTY_ID,
    'PropertyDocument',
    'TEMP_SIGNATURE_WILL_BE_REGENERATED',
    '{}',
    NOW(),
    'system',
    false
);
```

### Problem 2: Property Has No Seller/Buyer

**Symptoms**: Step 3 or 4 returns no rows

**Root Cause**: Property was created but seller/buyer were never added

**Solution**:
1. Edit the property in the UI
2. Add seller details
3. Add buyer details
4. Save and complete the property
5. Generate new verification code

### Problem 3: Signature Mismatch

**Symptoms**: 
- Verification exists
- Property exists
- Seller/buyer exist
- But still returns "Invalid"

**Root Cause**: Property data was modified after verification code was generated

**Solution**:
```sql
-- Revoke old verification
UPDATE org.DocumentVerifications
SET IsRevoked = true,
    RevokedAt = NOW(),
    RevokedBy = 'system',
    RevokedReason = 'Property data was modified'
WHERE VerificationCode = 'PRO-2026-D3XU8P';
```

Then regenerate verification code using Solution A or B from Problem 1.

### Problem 4: Location Data Missing

**Symptoms**: Seller/buyer exist but province/district names are null

**Root Cause**: Invalid location IDs or missing location data

**Solution**:
```sql
-- Check for invalid location references
SELECT 
    'Seller' as Source,
    sd.Id,
    sd.PaddressProvinceId,
    sd.PaddressDistrictId
FROM org.SellerDetails sd
LEFT JOIN org.Locations lp ON lp.Id = sd.PaddressProvinceId
LEFT JOIN org.Locations ld ON ld.Id = sd.PaddressDistrictId
WHERE sd.PropertyDetailsId = [PropertyId]
  AND (
    (sd.PaddressProvinceId IS NOT NULL AND lp.Id IS NULL)
    OR (sd.PaddressDistrictId IS NOT NULL AND ld.Id IS NULL)
  )
UNION ALL
SELECT 
    'Buyer' as Source,
    bd.Id,
    bd.PaddressProvinceId,
    bd.PaddressDistrictId
FROM org.BuyerDetails bd
LEFT JOIN org.Locations lp ON lp.Id = bd.PaddressProvinceId
LEFT JOIN org.Locations ld ON ld.Id = bd.PaddressDistrictId
WHERE bd.PropertyDetailsId = [PropertyId]
  AND (
    (bd.PaddressProvinceId IS NOT NULL AND lp.Id IS NULL)
    OR (bd.PaddressDistrictId IS NOT NULL AND ld.Id IS NULL)
  );
```

If invalid IDs found, update them:
```sql
-- Update with valid location IDs
UPDATE org.SellerDetails
SET PaddressProvinceId = [valid_province_id],
    PaddressDistrictId = [valid_district_id]
WHERE Id = [seller_id];
```

## Quick Fix Script

If you just want to fix this specific verification code quickly:

```sql
-- 1. Find if verification exists
DO $$
DECLARE
    v_doc_id INT;
    v_exists BOOLEAN;
BEGIN
    -- Check if verification exists
    SELECT DocumentId INTO v_doc_id
    FROM org.DocumentVerifications
    WHERE VerificationCode = 'PRO-2026-D3XU8P'
    LIMIT 1;
    
    IF v_doc_id IS NULL THEN
        RAISE NOTICE 'Verification code PRO-2026-D3XU8P does not exist in database';
        RAISE NOTICE 'You need to regenerate it from the print page';
    ELSE
        RAISE NOTICE 'Verification exists for Property ID: %', v_doc_id;
        
        -- Check if property exists
        SELECT EXISTS(SELECT 1 FROM org.PropertyDetails WHERE Id = v_doc_id) INTO v_exists;
        IF NOT v_exists THEN
            RAISE NOTICE 'ERROR: Property % does not exist!', v_doc_id;
        ELSE
            RAISE NOTICE 'Property exists';
            
            -- Check seller
            SELECT EXISTS(SELECT 1 FROM org.SellerDetails WHERE PropertyDetailsId = v_doc_id) INTO v_exists;
            IF NOT v_exists THEN
                RAISE NOTICE 'WARNING: No seller data found';
            ELSE
                RAISE NOTICE 'Seller data exists';
            END IF;
            
            -- Check buyer
            SELECT EXISTS(SELECT 1 FROM org.BuyerDetails WHERE PropertyDetailsId = v_doc_id) INTO v_exists;
            IF NOT v_exists THEN
                RAISE NOTICE 'WARNING: No buyer data found';
            ELSE
                RAISE NOTICE 'Buyer data exists';
            END IF;
        END IF;
    END IF;
END $$;
```

## Testing After Fix

After applying any fix, test the verification:

```bash
curl http://localhost:5143/api/verification/verify/PRO-2026-D3XU8P
```

**Expected successful response**:
```json
{
  "isValid": true,
  "status": "Valid",
  "verificationCode": "PRO-2026-D3XU8P",
  "documentType": "PropertyDocument",
  "licenseNumber": "PROP-...",
  "holderName": "SellerName - BuyerName",
  "sellerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "grandFatherName": "...",
    "electronicNationalIdNumber": "...",
    "phoneNumber": "...",
    "province": "...",
    "district": "...",
    "village": "..."
  },
  "buyerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "grandFatherName": "...",
    "electronicNationalIdNumber": "...",
    "phoneNumber": "...",
    "province": "...",
    "district": "...",
    "village": "..."
  }
}
```

## Prevention

To prevent this issue in the future:

1. **Always complete property data** before printing
2. **Don't close print page** until QR code is visible
3. **Check verification code** is displayed on print page
4. **Test verification** by scanning QR code before distributing document

## Files Created
- `Backend/Scripts/check_verification_code.sql` - Diagnostic queries
- `PROPERTY_VERIFICATION_FIX.md` - Detailed fix documentation
- `PROPERTY_VERIFICATION_DEBUG.md` - This debugging guide
