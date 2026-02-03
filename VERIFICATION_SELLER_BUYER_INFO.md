# Verification Response with Seller and Buyer Information

## Overview
Enhanced the verification system to return detailed seller and buyer information for Property and Vehicle documents, instead of just a combined name.

## Changes Made

### 1. Updated DTOs - `Backend/Services/Verification/IVerificationService.cs`

#### Added Seller and Buyer DTOs
```csharp
public class SellerInfoDto
{
    public string? FirstName { get; set; }
    public string? FatherName { get; set; }
    public string? GrandFatherName { get; set; }
    public string? ElectronicNationalIdNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Photo { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Village { get; set; }
}

public class BuyerInfoDto
{
    public string? FirstName { get; set; }
    public string? FatherName { get; set; }
    public string? GrandFatherName { get; set; }
    public string? ElectronicNationalIdNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Photo { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Village { get; set; }
}
```

#### Updated DocumentVerificationDto
Added two new properties:
```csharp
public class DocumentVerificationDto
{
    // ... existing properties ...
    
    // Seller information (for Property and Vehicle documents)
    public SellerInfoDto? SellerInfo { get; set; }
    
    // Buyer information (for Property and Vehicle documents)
    public BuyerInfoDto? BuyerInfo { get; set; }
}
```

### 2. Updated Internal Data Structures - `Backend/Services/Verification/VerificationService.cs`

#### Added Internal Seller and Buyer Data Classes
```csharp
internal class SellerData
{
    public string? FirstName { get; set; }
    public string? FatherName { get; set; }
    public string? GrandFatherName { get; set; }
    public string? ElectronicNationalIdNumber { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Photo { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Village { get; set; }
}

internal class BuyerData
{
    // Same structure as SellerData
}
```

#### Updated DocumentDataDto
```csharp
internal class DocumentDataDto
{
    // ... existing properties ...
    
    public SellerData? Seller { get; set; }
    public BuyerData? Buyer { get; set; }
}
```

### 3. Updated VerifyDocumentAsync Method
Added code to populate seller and buyer information in the verification response:

```csharp
// Populate seller and buyer information (for Property and Vehicle documents)
if (currentData.Seller != null)
{
    result.SellerInfo = new SellerInfoDto
    {
        FirstName = currentData.Seller.FirstName,
        FatherName = currentData.Seller.FatherName,
        GrandFatherName = currentData.Seller.GrandFatherName,
        ElectronicNationalIdNumber = currentData.Seller.ElectronicNationalIdNumber,
        PhoneNumber = currentData.Seller.PhoneNumber,
        Photo = currentData.Seller.Photo,
        Province = currentData.Seller.Province,
        District = currentData.Seller.District,
        Village = currentData.Seller.Village
    };
}

if (currentData.Buyer != null)
{
    result.BuyerInfo = new BuyerInfoDto
    {
        // Same mapping as seller
    };
}
```

### 4. Updated GetPropertyDocumentDataAsync Method
Enhanced to retrieve and populate detailed seller and buyer information:

```csharp
// Prepare seller data
SellerData? sellerData = null;
if (seller != null)
{
    string sellerProvince = "";
    string sellerDistrict = "";
    
    // Get province and district names from Location table
    if (seller.PaddressProvinceId.HasValue)
    {
        var prov = await _context.Locations.FindAsync(seller.PaddressProvinceId.Value);
        sellerProvince = prov?.Dari ?? prov?.Name ?? "";
    }
    
    if (seller.PaddressDistrictId.HasValue)
    {
        var dist = await _context.Locations.FindAsync(seller.PaddressDistrictId.Value);
        sellerDistrict = dist?.Dari ?? dist?.Name ?? "";
    }
    
    sellerData = new SellerData
    {
        FirstName = seller.FirstName,
        FatherName = seller.FatherName,
        GrandFatherName = seller.GrandFather,
        ElectronicNationalIdNumber = seller.ElectronicNationalIdNumber,
        PhoneNumber = seller.PhoneNumber,
        Photo = seller.Photo,
        Province = sellerProvince,
        District = sellerDistrict,
        Village = seller.PaddressVillage
    };
}

// Similar code for buyer data
```

### 5. Updated GetVehicleDocumentDataAsync Method
Enhanced to retrieve and populate detailed seller and buyer information (same pattern as property).

## API Response Structure

### Before (Old Response)
```json
{
  "isValid": true,
  "status": "Valid",
  "verificationCode": "VEH-ABC123",
  "documentType": "VehicleDocument",
  "licenseNumber": "232",
  "holderName": "نجیب الله - نجیب الله",
  "holderPhoto": "Resources/Documents/Profile/profile.jpg",
  "issueDate": "2026-02-03T05:09:22.62926",
  "companyTitle": null,
  "officeAddress": "کابل, ناحیه سوم"
}
```

### After (New Response)
```json
{
  "isValid": true,
  "status": "Valid",
  "verificationCode": "VEH-ABC123",
  "documentType": "VehicleDocument",
  "licenseNumber": "232",
  "holderName": "نجیب الله - نجیب الله",
  "holderPhoto": "Resources/Documents/Profile/profile.jpg",
  "issueDate": "2026-02-03T05:09:22.62926",
  "companyTitle": null,
  "officeAddress": "کابل, ناحیه سوم",
  "sellerInfo": {
    "firstName": "نجیب الله",
    "fatherName": "صادق",
    "grandFatherName": "صدبر",
    "electronicNationalIdNumber": "1400030063636",
    "phoneNumber": "0744444444",
    "photo": "Resources/Documents/Profile/profile_20260203_050123_020.jpg",
    "province": "کابل",
    "district": "ناحیه سوم",
    "village": "ناحیه سوم"
  },
  "buyerInfo": {
    "firstName": "نجیب الله",
    "fatherName": "صادق",
    "grandFatherName": "صدبر",
    "electronicNationalIdNumber": "1400030063636",
    "phoneNumber": "0700363636",
    "photo": "Resources/Documents/Profile/profile_20260203_051035_273.jpg",
    "province": "هرات",
    "district": "ناحیه دوم",
    "village": "چهارمن"
  }
}
```

## Benefits

### 1. Detailed Information
- Full seller details (name, father name, grandfather name, ID, phone, photo, address)
- Full buyer details (same structure)
- Province and district names in Dari

### 2. Better Verification
- Verifiers can see complete information about both parties
- Photos of both seller and buyer
- Contact information for verification purposes
- Complete address details

### 3. Consistent with Company Licenses
- Company licenses show company information
- Property/Vehicle documents now show seller and buyer information
- Each document type has appropriate detailed information

### 4. Backward Compatible
- Old fields (`holderName`, `holderPhoto`) still present
- New fields (`sellerInfo`, `buyerInfo`) are optional
- Existing verification pages will continue to work

## Document Types

### With Seller/Buyer Info
- **PropertyDocument**: Property transactions
- **VehicleDocument**: Vehicle transactions

### Without Seller/Buyer Info (Company-based)
- **RealEstateLicense**: Company license
- **PetitionWriterLicense**: Individual license
- **Securities**: Company securities
- **PetitionWriterSecurities**: Individual securities

## Testing

### Test Property Verification
1. Create a property transaction
2. Generate QR code
3. Scan QR code or visit verification URL
4. Verify response includes `sellerInfo` and `buyerInfo` objects

### Test Vehicle Verification
1. Create a vehicle transaction
2. Generate QR code
3. Scan QR code or visit verification URL
4. Verify response includes `sellerInfo` and `buyerInfo` objects

### Test Company License Verification
1. Generate QR code for company license
2. Verify response does NOT include `sellerInfo` or `buyerInfo`
3. Verify response includes `companyTitle` and `officeAddress`

## Files Modified

1. `Backend/Services/Verification/IVerificationService.cs` - Added DTOs
2. `Backend/Services/Verification/VerificationService.cs` - Updated methods

## Next Steps

1. Restart backend application
2. Test verification for property documents
3. Test verification for vehicle documents
4. Update frontend verification page to display seller/buyer info (if needed)
