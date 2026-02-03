# Vehicle Verification Backend Support

## Problem
The QR code verification was failing with a 500 error: `"Document not found"` because the verification service didn't support vehicle documents.

## Solution
Added vehicle document support to the `VerificationService.cs` backend service.

## Changes Made

### File: `Backend/Services/Verification/VerificationService.cs`

#### 1. Added Vehicle Document Prefix
Added `"VehicleDocument"` to the `DocumentTypePrefixes` dictionary with prefix `"VEH"`:

```csharp
private static readonly Dictionary<string, string> DocumentTypePrefixes = new()
{
    { "RealEstateLicense", "LIC" },
    { "PetitionWriterLicense", "PWL" },
    { "Securities", "SEC" },
    { "PetitionWriterSecurities", "PWS" },
    { "PropertyDocument", "PRO" },
    { "VehicleDocument", "VEH" }  // NEW
};
```

#### 2. Updated GetDocumentDataAsync Switch
Added case for `"VehicleDocument"` in the document type switch statement:

```csharp
private async Task<DocumentDataDto?> GetDocumentDataAsync(int documentId, string documentType)
{
    return documentType switch
    {
        "RealEstateLicense" => await GetRealEstateLicenseDataAsync(documentId),
        "PetitionWriterLicense" => await GetPetitionWriterLicenseDataAsync(documentId),
        "PropertyDocument" => await GetPropertyDocumentDataAsync(documentId),
        "VehicleDocument" => await GetVehicleDocumentDataAsync(documentId),  // NEW
        _ => null
    };
}
```

#### 3. Added GetVehicleDocumentDataAsync Method
Created new method to retrieve vehicle document data for verification:

```csharp
private async Task<DocumentDataDto?> GetVehicleDocumentDataAsync(int vehicleId)
{
    // Query vehicle details with seller and buyer information
    var vehicle = await _context.VehiclesPropertyDetails
        .Include(v => v.VehiclesSellerDetails)
        .Include(v => v.VehiclesBuyerDetails)
        .FirstOrDefaultAsync(v => v.Id == vehicleId);

    if (vehicle == null) return null;

    var seller = vehicle.VehiclesSellerDetails.FirstOrDefault();
    var buyer = vehicle.VehiclesBuyerDetails.FirstOrDefault();

    // Get province and district names from Location table
    string sellerProvince = "";
    string sellerDistrict = "";
    
    if (seller != null)
    {
        if (seller.PaddressProvinceId.HasValue)
        {
            var provinceLocation = await _context.Locations.FindAsync(seller.PaddressProvinceId.Value);
            sellerProvince = provinceLocation?.Dari ?? provinceLocation?.Name ?? "";
        }
        
        if (seller.PaddressDistrictId.HasValue)
        {
            var districtLocation = await _context.Locations.FindAsync(seller.PaddressDistrictId.Value);
            sellerDistrict = districtLocation?.Dari ?? districtLocation?.Name ?? "";
        }
    }

    return new DocumentDataDto
    {
        LicenseNumber = vehicle.PermitNo ?? vehicle.PilateNo ?? "",
        HolderName = $"{seller?.FirstName ?? ""} - {buyer?.FirstName ?? ""}",
        HolderPhoto = seller?.Photo,
        IssueDate = vehicle.CreatedAt,
        ExpiryDate = null, // Vehicle documents don't expire
        CompanyTitle = null,
        OfficeAddress = $"{sellerProvince}, {sellerDistrict}"
    };
}
```

## How It Works

### Verification Code Generation
- **Prefix**: `VEH` (e.g., `VEH-ABC123`)
- **Format**: Same as other document types
- **Uniqueness**: Checked against existing codes in database

### Document Data Retrieved
1. **Vehicle Details**: From `VehiclesPropertyDetails` table
2. **Seller Info**: First seller from `VehiclesSellerDetails`
3. **Buyer Info**: First buyer from `VehiclesBuyerDetails`
4. **Location**: Province and district names in Dari from `Location` table

### Verification Fields
- **License Number**: Uses `PermitNo` or falls back to `PilateNo`
- **Holder Name**: Combines seller and buyer names (e.g., "نجیب الله - نجیب الله")
- **Holder Photo**: Seller's photo
- **Issue Date**: Vehicle creation date
- **Expiry Date**: null (vehicles don't expire)
- **Office Address**: Seller's province and district

## Testing

After restarting the backend, the vehicle print page should:
1. Successfully call `/api/Verification/generate`
2. Receive a verification code with `VEH` prefix
3. Display QR code in the print header
4. Show verification code below QR code

## Verification Code Format
- **Example**: `VEH-A1B2C3`
- **Prefix**: VEH (Vehicle)
- **Code**: 6 alphanumeric characters
- **Separator**: Hyphen

## Database Tables Used
- `tr."VehiclesPropertyDetails"` - Main vehicle data
- `tr."VehiclesSellerDetails"` - Seller information
- `tr."VehiclesBuyerDetails"` - Buyer information
- `look."Location"` - Province/district names
- `ver."DocumentVerifications"` - Verification records
- `ver."VerificationLogs"` - Verification attempt logs

## Error Handling
The service handles:
- Vehicle not found
- Missing seller/buyer data
- Missing location data
- Duplicate verification code generation
- Database errors

## Next Steps
1. Restart the backend application
2. Navigate to vehicle print page
3. Verify QR code appears
4. Test scanning QR code
5. Verify document through web interface

## Files Modified
- `Backend/Services/Verification/VerificationService.cs` - Added vehicle document support
