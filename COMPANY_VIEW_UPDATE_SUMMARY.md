# Company Details View Update Summary

## Overview
Updated the company details view to reflect the current database schema by removing deprecated paper-based identity fields and adding the new electronic ID system and province-based license numbering.

## Changes Made

### Backend Changes - `Backend/Controllers/Companies/CompanyDetailsController.cs`

#### GetCompanyViewById Endpoint Updates:

**Added Fields:**
1. **License Details:**
   - `ProvinceId` - Province ID for license numbering
   - `ProvinceName` - Province name in Dari
   - `AreaId` - Area ID

**Removed Fields:**
- All paper-based identity fields that were removed in previous migrations:
  - `IdentityCardTypeName`
  - `IndentityCardNumber`
  - `Jild` (Volume)
  - `Safha` (Page)
  - `SabtNumber` (Registration number)

**Note:** Fields like `PetitionNumber`, `PetitionDate`, and company-level `PhoneNumber` were not added as they don't exist in the current CompanyDetail model. The owner's phone number is used instead.

### Frontend Changes - `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.html`

#### Company Basic Information Section:
**Removed:**
- نمبر درخواست (Petition Number)
- تاریخ درخواست (Petition Date)

**Changed:**
- شماره تماس (Phone Number) - now shows owner's phone number instead of company phone number

#### Company Owner Section:
**Removed:**
- نوع تذکره (Identity Card Type)
- شماره تذکره (Identity Card Number)
- جلد / صفحه / ثبت (Volume / Page / Registration)

**Added:**
- شماره تذکره الکترونیکی (Electronic National ID Number) - displayed in purple card

**Removed Address Section:**
- آدرس موقت (Temporary Address) - removed as it doesn't exist in CompanyOwner model

**Kept Address Sections:**
- آدرس اصلی مالک (Owner's Own Address)
- آدرس دایمی (Permanent Address)

#### License Details Section:
**Added:**
- ولایت (Province) - displayed between license number and license type
- Shows the province name in Dari where the license was issued

#### Guarantor Section:
**Removed:**
- شماره تذکره (Identity Card Number)
- جلد / صفحه / ثبت (Volume / Page / Registration)

**Added:**
- شماره تذکره الکترونیکی (Electronic National ID Number)

## Field Mapping

### Old Schema → New Schema

| Old Field | New Field | Notes |
|-----------|-----------|-------|
| IndentityCardNumber | ElectronicNationalIdNumber | Paper ID → Electronic ID |
| Jild, Safha, SabtNumber | (removed) | No longer needed with electronic IDs |
| IdentityCardType | (removed) | Only electronic IDs now |
| (none) | ProvinceId, ProvinceName | New province-based license numbering |
| Company.PhoneNumber | Owner.PhoneNumber | Uses owner's phone instead |
| Owner.TemporaryAddress | (removed) | Not in current schema |
| PetitionNumber, PetitionDate | (removed) | Not in current CompanyDetail model |

## Visual Changes

### Company Basic Information Card
- **Before:** Showed 6 fields including petition number and date
- **After:** Shows 4 fields (title, phone, TIN, status)
- Uses owner's phone number for contact information

### Owner Information Card
- **Before:** Showed 9 fields including paper ID details
- **After:** Shows 8 fields with electronic ID only
- Electronic ID is highlighted in purple card for emphasis
- Removed temporary address section

### License Information Card
- **Before:** 2 columns for license number and type
- **After:** 3 columns including province name
- Province field helps identify where license was issued

### Guarantor Cards
- **Before:** Showed paper ID details (volume/page/registration)
- **After:** Shows only electronic national ID number
- Cleaner, more modern appearance

## Benefits

1. **Data Accuracy:** View now matches actual database schema
2. **Modernization:** Removed paper-based ID system references
3. **Province Tracking:** Added province information for license tracking
4. **Consistency:** All sections now use electronic IDs consistently
5. **Cleaner UI:** Removed redundant and non-existent fields
6. **Simplified:** Removed fields that don't exist in current models

## Testing Checklist

- [ ] Company basic information displays correctly
- [ ] Owner information shows electronic ID instead of paper ID
- [ ] Owner phone number displays in company section
- [ ] License details include province name
- [ ] Guarantor information shows electronic ID
- [ ] Owner addresses (own and permanent) display correctly
- [ ] Temporary address section is removed
- [ ] Financial information displays correctly
- [ ] Cancellation information displays correctly
- [ ] Documents can be viewed and downloaded
- [ ] Print license button works
- [ ] No console errors
- [ ] No missing data warnings

## API Response Structure

```json
{
  "id": 1,
  "title": "Company Name",
  "tin": "123456",
  "docPath": "path/to/doc.pdf",
  "status": true,
  "owner": {
    "firstName": "Ahmad",
    "fatherName": "Mohammad",
    "grandFatherName": "Ali",
    "dateofBirth": "1990-01-01",
    "phoneNumber": "0700123456",
    "whatsAppNumber": "0700123456",
    "electronicNationalIdNumber": "1234567890123",
    "pothoPath": "path/to/photo.jpg",
    "educationLevelName": "لیسانس",
    "ownerProvinceName": "کابل",
    "ownerDistrictName": "ناحیه اول",
    "ownerVillage": "کارته سه",
    "permanentProvinceName": "کابل",
    "permanentDistrictName": "ناحیه دوم",
    "permanentVillage": "کارته چهار"
  },
  "license": {
    "licenseNumber": "KBL-0001",
    "provinceId": 1,
    "provinceName": "کابل",
    "licenseType": "Real Estate",
    "licenseCategory": "جدید",
    "issueDate": "2024-01-15",
    "expireDate": "2027-01-15",
    "officeAddress": "کابل، کارته سه",
    "areaId": 1,
    "areaName": "ساحه اول",
    "royaltyAmount": 5000,
    "royaltyDate": "2024-01-15",
    "tariffNumber": "TAR-001",
    "penaltyAmount": 0,
    "hrLetter": "HR-001",
    "hrLetterDate": "2024-01-10",
    "docPath": "path/to/license.pdf"
  },
  "guarantors": [
    {
      "firstName": "Hassan",
      "fatherName": "Ali",
      "grandFatherName": "Mohammad",
      "phoneNumber": "0700987654",
      "electronicNationalIdNumber": "9876543210987",
      "guaranteeTypeId": 1,
      "guaranteeTypeName": "قباله شرعی",
      "dateofGuarantee": "2024-01-15",
      "guaranteeDocNumber": "GUA-001",
      "courtName": "محکمه ابتدائیه",
      "collateralNumber": "COL-001",
      "permanentProvinceName": "کابل",
      "permanentDistrictName": "ناحیه اول",
      "paddressVillage": "کارته سه",
      "temporaryProvinceName": "کابل",
      "temporaryDistrictName": "ناحیه دوم",
      "taddressVillage": "کارته چهار"
    }
  ],
  "accountInfo": {
    "taxPaymentAmount": 10000,
    "taxPaymentDate": "2024-01-15",
    "settlementYear": "1403",
    "transactionCount": 50,
    "companyCommission": 25000,
    "settlementInfo": "تصفیه شده"
  },
  "cancellationInfo": null
}
```

## Files Modified

1. `Backend/Controllers/Companies/CompanyDetailsController.cs`
   - Updated GetCompanyViewById endpoint
   - Added province fields to license details
   - Removed deprecated paper ID fields
   - Removed non-existent fields (petition, company phone, temporary address)

2. `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.html`
   - Updated company basic section (removed petition fields)
   - Updated owner section (removed paper ID, added electronic ID)
   - Updated license section (added province field)
   - Updated guarantor section (removed paper ID, added electronic ID)
   - Removed temporary address section
   - Changed phone number to use owner's phone

## Migration Notes

- This update aligns with the electronic ID migration completed earlier
- No database changes required (already migrated)
- Only view layer updates
- Backward compatible with existing data
- Removed references to fields that don't exist in current models

## Related Documents

- `ELECTRONIC_ID_MIGRATION_SUMMARY.md` - Original electronic ID migration
- `PROVINCE_LICENSE_IMPLEMENTATION_COMPLETE.md` - Province-based license numbering
- `Backend/Infrastructure/Migrations/Company/20260118_RemoveCompanyDetailsFields.cs` - Field removal migration

## Support

If you encounter issues:
1. Check browser console for errors
2. Verify backend API returns correct data structure
3. Ensure all migrations have been applied
4. Check that electronic ID data exists in database
5. Verify province data is populated in Location table
6. Confirm owner has phone number populated

### Frontend Changes - `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.html`

#### Company Owner Section:
**Removed:**
- نوع تذکره (Identity Card Type)
- شماره تذکره (Identity Card Number)
- جلد / صفحه / ثبت (Volume / Page / Registration)

**Added:**
- شماره تذکره الکترونیکی (Electronic National ID Number) - displayed in purple card

#### License Details Section:
**Added:**
- ولایت (Province) - displayed between license number and license type
- Shows the province name in Dari where the license was issued

#### Guarantor Section:
**Removed:**
- شماره تذکره (Identity Card Number)
- جلد / صفحه / ثبت (Volume / Page / Registration)

**Added:**
- شماره تذکره الکترونیکی (Electronic National ID Number)

## Field Mapping

### Old Schema → New Schema

| Old Field | New Field | Notes |
|-----------|-----------|-------|
| IndentityCardNumber | ElectronicNationalIdNumber | Paper ID → Electronic ID |
| Jild, Safha, SabtNumber | (removed) | No longer needed with electronic IDs |
| IdentityCardType | (removed) | Only electronic IDs now |
| (none) | ProvinceId, ProvinceName | New province-based license numbering |
| (none) | PetitionNumber, PetitionDate | Company petition information |
| (none) | PhoneNumber (company level) | Company contact |

## Visual Changes

### Owner Information Card
- **Before:** Showed 9 fields including paper ID details
- **After:** Shows 8 fields with electronic ID only
- Electronic ID is highlighted in purple card for emphasis

### License Information Card
- **Before:** 2 columns for license number and type
- **After:** 3 columns including province name
- Province field helps identify where license was issued

### Guarantor Cards
- **Before:** Showed paper ID details (volume/page/registration)
- **After:** Shows only electronic national ID number
- Cleaner, more modern appearance

## Benefits

1. **Data Accuracy:** View now matches actual database schema
2. **Modernization:** Removed paper-based ID system references
3. **Province Tracking:** Added province information for license tracking
4. **Consistency:** All sections now use electronic IDs consistently
5. **Cleaner UI:** Removed redundant fields, improved readability

## Testing Checklist

- [ ] Company basic information displays correctly
- [ ] Owner information shows electronic ID instead of paper ID
- [ ] License details include province name
- [ ] Guarantor information shows electronic ID
- [ ] All addresses (owner, permanent, temporary) display correctly
- [ ] Financial information displays correctly
- [ ] Cancellation information displays correctly
- [ ] Documents can be viewed and downloaded
- [ ] Print license button works
- [ ] No console errors
- [ ] No missing data warnings

## API Response Structure

```json
{
  "id": 1,
  "title": "Company Name",
  "tin": "123456",
  "petitionNumber": "PET-001",
  "petitionDate": "2024-01-15",
  "phoneNumber": "0700123456",
  "docPath": "path/to/doc.pdf",
  "status": true,
  "owner": {
    "firstName": "Ahmad",
    "fatherName": "Mohammad",
    "grandFatherName": "Ali",
    "dateofBirth": "1990-01-01",
    "phoneNumber": "0700123456",
    "whatsAppNumber": "0700123456",
    "electronicNationalIdNumber": "1234567890123",
    "pothoPath": "path/to/photo.jpg",
    "educationLevelName": "لیسانس",
    "ownerProvinceName": "کابل",
    "ownerDistrictName": "ناحیه اول",
    "ownerVillage": "کارته سه",
    "permanentProvinceName": "کابل",
    "permanentDistrictName": "ناحیه دوم",
    "permanentVillage": "کارته چهار",
    "temporaryProvinceName": "کابل",
    "temporaryDistrictName": "ناحیه سوم",
    "temporaryVillage": "کارته پنج"
  },
  "license": {
    "licenseNumber": "KBL-0001",
    "provinceId": 1,
    "provinceName": "کابل",
    "licenseType": "Real Estate",
    "licenseCategory": "جدید",
    "issueDate": "2024-01-15",
    "expireDate": "2027-01-15",
    "officeAddress": "کابل، کارته سه",
    "areaId": 1,
    "areaName": "ساحه اول",
    "royaltyAmount": 5000,
    "royaltyDate": "2024-01-15",
    "tariffNumber": "TAR-001",
    "penaltyAmount": 0,
    "hrLetter": "HR-001",
    "hrLetterDate": "2024-01-10",
    "docPath": "path/to/license.pdf"
  },
  "guarantors": [
    {
      "firstName": "Hassan",
      "fatherName": "Ali",
      "grandFatherName": "Mohammad",
      "phoneNumber": "0700987654",
      "electronicNationalIdNumber": "9876543210987",
      "guaranteeTypeId": 1,
      "guaranteeTypeName": "قباله شرعی",
      "dateofGuarantee": "2024-01-15",
      "guaranteeDocNumber": "GUA-001",
      "courtName": "محکمه ابتدائیه",
      "collateralNumber": "COL-001",
      "permanentProvinceName": "کابل",
      "permanentDistrictName": "ناحیه اول",
      "paddressVillage": "کارته سه"
    }
  ],
  "accountInfo": {
    "taxPaymentAmount": 10000,
    "taxPaymentDate": "2024-01-15",
    "settlementYear": "1403",
    "transactionCount": 50,
    "companyCommission": 25000,
    "settlementInfo": "تصفیه شده"
  },
  "cancellationInfo": null
}
```

## Files Modified

1. `Backend/Controllers/Companies/CompanyDetailsController.cs`
   - Updated GetCompanyViewById endpoint
   - Added new fields to response
   - Removed deprecated fields

2. `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.html`
   - Updated owner section
   - Updated license section
   - Updated guarantor section
   - Removed paper ID fields
   - Added electronic ID fields
   - Added province field

## Migration Notes

- This update aligns with the electronic ID migration completed earlier
- No database changes required (already migrated)
- Only view layer updates
- Backward compatible with existing data

## Related Documents

- `ELECTRONIC_ID_MIGRATION_SUMMARY.md` - Original electronic ID migration
- `PROVINCE_LICENSE_IMPLEMENTATION_COMPLETE.md` - Province-based license numbering
- `Backend/Infrastructure/Migrations/Company/20260118_RemoveCompanyDetailsFields.cs` - Field removal migration

## Support

If you encounter issues:
1. Check browser console for errors
2. Verify backend API returns correct data structure
3. Ensure all migrations have been applied
4. Check that electronic ID data exists in database
5. Verify province data is populated in Location table
