# Property Verification Issue Fix

## Problem
Verification code `PRO-2026-D3XU8P` returns:
- `isValid: false`
- `status: "Invalid"`
- No seller/buyer information
- No property details

## Root Cause Analysis

The verification system works in two steps:
1. **Generate** verification code when property is created/completed
2. **Verify** the code when someone scans the QR code

The issue is that the verification code doesn't exist in the `DocumentVerifications` table, which means:
- The property was created but verification code was never generated
- The `/api/verification/generate` endpoint was never called for this property

## How Verification Should Work

### Step 1: Generate Verification Code
When a property document is completed, the system should call:
```
POST /api/verification/generate
{
  "documentId": 123,  // Property ID
  "documentType": "PropertyDocument"
}
```

This creates a record in `DocumentVerifications` table with:
- Unique verification code (e.g., PRO-2026-D3XU8P)
- Document ID and type
- Digital signature
- Document snapshot

### Step 2: Verify Document
When someone scans the QR code, they access:
```
GET /api/verification/verify/PRO-2026-D3XU8P
```

This:
1. Looks up the verification code in the database
2. Retrieves the property details
3. Loads seller and buyer information
4. Returns all data for display

## Diagnostic Steps

### 1. Check if Verification Code Exists
Run the SQL script: `Backend/Scripts/check_verification_code.sql`

```sql
SELECT * FROM org.DocumentVerifications 
WHERE VerificationCode = 'PRO-2026-D3XU8P';
```

**Expected Results:**
- If **empty**: Verification code was never generated
- If **found**: Check the DocumentId and DocumentType

### 2. Check Property Exists
```sql
SELECT * FROM org.PropertyDetails 
WHERE Id = [DocumentId from step 1];
```

### 3. Check Seller/Buyer Data
```sql
-- Check sellers
SELECT * FROM org.SellerDetails 
WHERE PropertyDetailsId = [PropertyId];

-- Check buyers
SELECT * FROM org.BuyerDetails 
WHERE PropertyDetailsId = [PropertyId];
```

## Solution

### Option 1: Generate Verification Code for Existing Property

If the property exists but verification code is missing:

1. Find the property ID that should have code `PRO-2026-D3XU8P`
2. Call the generate endpoint:

```bash
curl -X POST http://localhost:5143/api/verification/generate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "documentId": PROPERTY_ID,
    "documentType": "PropertyDocument"
  }'
```

### Option 2: Manually Insert Verification Code

If you know the property ID, you can manually create the verification record:

```sql
-- First, get property data
SELECT 
    pd.Id,
    pd.PNumber,
    pd.IssuanceNumber,
    sd.FirstName as SellerName,
    bd.FirstName as BuyerName
FROM org.PropertyDetails pd
LEFT JOIN org.SellerDetails sd ON sd.PropertyDetailsId = pd.Id
LEFT JOIN org.BuyerDetails bd ON bd.PropertyDetailsId = pd.Id
WHERE pd.Id = YOUR_PROPERTY_ID;

-- Then insert verification record
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
    YOUR_PROPERTY_ID,
    'PropertyDocument',
    'PLACEHOLDER_SIGNATURE',  -- Will be regenerated on first verify
    '{}',
    NOW(),
    'system',
    false
);
```

### Option 3: Automatic Generation on Property Completion

Update the property submission workflow to automatically generate verification codes.

**In PropertyDetailsController.cs**, add after property is marked complete:

```csharp
// After setting iscomplete = true
if (property.iscomplete == true)
{
    // Generate verification code
    var verificationService = HttpContext.RequestServices
        .GetRequiredService<IVerificationService>();
    
    var verificationResult = await verificationService
        .GetOrCreateVerificationAsync(
            property.Id, 
            "PropertyDocument", 
            userId
        );
    
    if (verificationResult.Success)
    {
        // Store verification code with property if needed
        // Or return it in the response
    }
}
```

## Frontend Integration

The frontend should:

1. **After property completion**, call generate endpoint
2. **Display QR code** with verification URL
3. **Show verification code** on printed document

Example in property component:

```typescript
async completeProperty(propertyId: number) {
  // Mark property as complete
  await this.propertyService.updateProperty(propertyId, { iscomplete: true });
  
  // Generate verification code
  const verification = await this.verificationService.generate({
    documentId: propertyId,
    documentType: 'PropertyDocument'
  });
  
  // Show QR code with verification.verificationUrl
  this.showQRCode(verification.verificationUrl);
}
```

## Testing the Fix

### 1. Create Test Property
```bash
# Create a property through the UI or API
# Note the property ID returned
```

### 2. Generate Verification Code
```bash
curl -X POST http://localhost:5143/api/verification/generate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "documentId": PROPERTY_ID,
    "documentType": "PropertyDocument"
  }'
```

### 3. Verify the Code
```bash
curl http://localhost:5143/api/verification/verify/PRO-2026-XXXXXX
```

**Expected Response:**
```json
{
  "isValid": true,
  "status": "Valid",
  "verificationCode": "PRO-2026-XXXXXX",
  "documentType": "PropertyDocument",
  "licenseNumber": "PROP-20260203...",
  "holderName": "SellerName - BuyerName",
  "sellerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "electronicNationalIdNumber": "...",
    "province": "...",
    "district": "..."
  },
  "buyerInfo": {
    "firstName": "...",
    "fatherName": "...",
    "electronicNationalIdNumber": "...",
    "province": "...",
    "district": "..."
  }
}
```

## Common Issues

### Issue 1: "Invalid" Status
**Cause**: Verification code doesn't exist in database
**Fix**: Generate verification code using Option 1 or 2 above

### Issue 2: No Seller/Buyer Info
**Cause**: Property has no seller/buyer records
**Fix**: Ensure seller and buyer are added to property before generating verification

### Issue 3: Signature Mismatch
**Cause**: Property data changed after verification code was generated
**Fix**: Regenerate verification code or revoke old one and create new

## Prevention

To prevent this issue in the future:

1. **Automatic Generation**: Generate verification codes automatically when property is marked complete
2. **Validation**: Don't allow property completion without seller/buyer data
3. **UI Feedback**: Show verification code and QR code immediately after generation
4. **Database Constraint**: Add check to ensure verification exists before printing

## Files Modified
- `Backend/Scripts/check_verification_code.sql` - Diagnostic SQL script
- `PROPERTY_VERIFICATION_FIX.md` - This documentation

## Next Steps

1. Run diagnostic SQL to check if verification code exists
2. If missing, use Option 1 to generate it
3. Test verification endpoint
4. Implement automatic generation (Option 3) to prevent future issues
