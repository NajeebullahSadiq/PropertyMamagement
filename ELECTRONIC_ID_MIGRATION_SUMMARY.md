# Electronic National ID Migration Summary
## الیکټرونیکی تذکره - Electronic National ID Only

This document summarizes the changes made to remove all paper-based ID fields and keep only Electronic National ID throughout the entire system.

## Backend Changes

### 1. Database Migrations Created

#### Shared Module
- `Backend/Infrastructure/Migrations/Shared/20260118_RemovePaperIdFields.cs`
  - Removes `IdentityCardType` lookup table
  - Updates `CompanyOwner`: Removes `IdentityCardTypeId`, `Jild`, `Safha`, `SabtNumber`
  - Renames `IndentityCardNumber` → `ElectronicNationalIdNumber` (string, 50 chars)
  - Updates `Guarantor`: Same changes as CompanyOwner

#### Petition Writer License Module
- `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260118_RemovePaperIdFields_PetitionWriter.cs`
  - Removes `IdentityCardType` column
  - Removes `PaperIdNumber`, `PaperIdVolume`, `PaperIdPage`, `PaperIdRegNumber`
  - Renames `ElectronicIdNumber` → `ElectronicNationalIdNumber` (required field)

#### Property Module
- `Backend/Infrastructure/Migrations/Property/20260118_RemovePaperIdFields_Property.cs`
  - Updates `SellerDetail`, `BuyerDetail`, `WitnessDetail`
  - Removes `TazkiraType`, `TazkiraVolume`, `TazkiraPage`, `TazkiraNumber`
  - Renames `IndentityCardNumber` → `ElectronicNationalIdNumber`

#### Vehicle Module
- `Backend/Infrastructure/Migrations/Vehicle/20260118_RemovePaperIdFields_Vehicle.cs`
  - Updates `VehiclesSellerDetail`, `VehiclesBuyerDetail`, `VehiclesWitnessDetail`
  - Removes `TazkiraType`, `TazkiraVolume`, `TazkiraPage`, `TazkiraNumber`
  - Renames `IndentityCardNumber` → `ElectronicNationalIdNumber`

### 2. Model Updates

#### Company Module
- `Backend/Models/Company/CompanyOwner.cs`
  - Removed: `IdentityCardTypeId`, `Jild`, `Safha`, `SabtNumber`
  - Added: `ElectronicNationalIdNumber` (string)
  - Removed navigation: `IdentityCardType`

- `Backend/Models/Company/Guarantor.cs`
  - Removed: `IdentityCardTypeId`, `Jild`, `Safha`, `SabtNumber`
  - Added: `ElectronicNationalIdNumber` (string)
  - Removed navigation: `IdentityCardType`

#### Petition Writer License Module
- `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs`
  - Removed: `IdentityCardType`, `PaperIdNumber`, `PaperIdVolume`, `PaperIdPage`, `PaperIdRegNumber`
  - Changed: `ElectronicIdNumber` → `ElectronicNationalIdNumber` (required)

#### Vehicle Module
- `Backend/Models/Vehicles/VehiclesSellerDetail.cs`
- `Backend/Models/Vehicles/VehiclesBuyerDetail.cs`
- `Backend/Models/Vehicles/VehiclesWitnessDetail.cs`
  - Removed: `TazkiraType`, `TazkiraVolume`, `TazkiraPage`, `TazkiraNumber`
  - Changed: `IndentityCardNumber` → `ElectronicNationalIdNumber`

#### Property Module
- `Backend/Models/Property/SellerDetail.cs`
- `Backend/Models/Property/BuyerDetail.cs`
- `Backend/Models/Property/WitnessDetail.cs`
  - Removed: `TazkiraType`, `TazkiraVolume`, `TazkiraPage`, `TazkiraNumber`
  - Changed: `IndentityCardNumber` → `ElectronicNationalIdNumber`

### 3. Configuration Updates

- `Backend/Configuration/AppDbContext.cs`
  - Removed `DbSet<IdentityCardType>`
  - Removed `IdentityCardType` entity configuration
  - Removed foreign key relationships to `IdentityCardType` in `CompanyOwner` and `Guarantor`
  - Updated `PetitionWriterLicense` configuration to use `ElectronicNationalIdNumber`

- `Backend/Models/Lookup/IdentityCardType.cs` - **DELETED**

## Frontend Changes

### 1. Model Updates

#### Petition Writer License
- `Frontend/src/app/models/PetitionWriterLicense.ts`
  - Removed: `IdentityCardTypeEnum`, `IdentityCardTypes` constant
  - Removed: `identityCardType`, `electronicIdNumber`, `paperIdNumber`, `paperIdVolume`, `paperIdPage`, `paperIdRegNumber`
  - Added: `electronicNationalIdNumber` (required string)
  - Updated `PetitionWriterLicenseData` interface accordingly

#### Guarantor
- `Frontend/src/app/models/Guarantor.ts`
  - Removed: `identityCardTypeId`, `indentityCardNumber`, `jild`, `safha`, `sabtNumber`
  - Added: `electronicNationalIdNumber`

### 2. Components to Update

The following components have been updated to remove paper ID fields and use only `electronicNationalIdNumber`:

#### Petition Writer License Components ✓
- `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts` ✓
  - Removed: `showElectronicIdFields`, `showPaperIdFields`, `identityCardType` form control
  - Removed: `paperIdNumber`, `paperIdVolume`, `paperIdPage`, `paperIdRegNumber` form controls
  - Updated: Use `electronicNationalIdNumber` (required field)
  - Removed: `onIdentityCardTypeChange()` method

- `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html` ✓
  - Removed: Identity card type dropdown
  - Removed: All paper ID fields section
  - Updated: Show only `electronicNationalIdNumber` field with label "الیکټرونیکی تذکره نمبر"

- `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.ts` ✓
  - Removed: `getIdentityCardTypeName()` method
  - Removed: `IdentityCardTypes` import

- `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.html` ✓
  - Removed: Identity card type display
  - Removed: All paper ID fields display
  - Updated: Show only `electronicNationalIdNumber`

#### Company Owner Components ✓
- `Frontend/src/app/realestate/companyowner/companyowner.component.ts` ✓
  - Removed: `identityCardTypeId`, `jild`, `safha`, `sabtNumber` form controls
  - Removed: `onPropertyTypeChange()`, `isElectricIdSelected()` methods
  - Removed: `filteredIdTypes`, `IdTypes` properties
  - Updated: Use `electronicNationalIdNumber` (required field)

- `Frontend/src/app/realestate/companyowner/companyowner.component.html` ✓
  - Removed: Identity card type dropdown
  - Removed: Jild, Safha, SabtNumber fields
  - Updated: Show only `electronicNationalIdNumber` field

- `Frontend/src/app/realestate/companydetailsview/companydetailsview.component.html` ✓
  - Removed: Identity card type display
  - Removed: Jild, Safha, SabtNumber display
  - Updated: Show only `electronicNationalIdNumber`

#### Guarantor Components ✓
- `Frontend/src/app/realestate/guaranators/guaranators.component.ts` ✓
  - Removed: `identityCardTypeId`, `jild`, `safha`, `sabtNumber` form controls
  - Removed: `onPropertyTypeChange()` method
  - Removed: `IdTypes` property
  - Updated: Use `electronicNationalIdNumber` (required field)

- `Frontend/src/app/realestate/guaranators/guaranators.component.html` ✓
  - Removed: Identity card type dropdown
  - Removed: Jild, Safha, SabtNumber fields
  - Updated: Show only `electronicNationalIdNumber` field

#### Vehicle Components ✓
- `Frontend/src/app/vehicle/vehicle-submit/sellerdetail/sellerdetail.component.ts` ✓
- `Frontend/src/app/vehicle/vehicle-submit/sellerdetail/sellerdetail.component.html` ✓
- `Frontend/src/app/vehicle/vehicle-submit/buyerdetail/buyerdetail.component.ts` ✓
- `Frontend/src/app/vehicle/vehicle-submit/buyerdetail/buyerdetail.component.html` ✓
- `Frontend/src/app/vehicle/vehicle-submit/witnessdetail/witnessdetail.component.ts` (NOT UPDATED - witness component not found)
- `Frontend/src/app/vehicle/vehicle-submit/witnessdetail/witnessdetail.component.html` (NOT UPDATED - witness component not found)
  - Removed: `tazkiraType`, `tazkiraVolume`, `tazkiraPage`, `tazkiraNumber` form controls
  - Removed: `isPaperTazkira()` method
  - Updated: Use `electronicNationalIdNumber` (required field)

- `Frontend/src/app/vehicle/vehicledetailsview/vehicledetailsview.component.html` ✓
  - Removed: Tazkira type and paper ID fields display
  - Updated: Show only `electronicNationalIdNumber`

#### Property Components
- Similar updates needed for property seller, buyer, and witness components (NOT YET DONE)

### 3. Services to Update

- `Frontend/src/app/shared/compnaydetail.service.ts` ✓
  - Removed: `getIdentityTypes()` method

- `Frontend/src/app/shared/localization.service.ts` ✓
  - Removed: `identityCardTypes` array

### 4. Backend Request DTOs to Update

- `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs`
  - Remove paper ID fields
  - Update to use `ElectronicNationalIdNumber`

- Similar updates for other request DTOs

### 5. Controllers to Update

All controllers that handle ID fields need to be updated to work with the new field names:
- `Backend/Controllers/Companies/CompanyOwnerController.cs`
- `Backend/Controllers/Companies/GuaranatorController.cs`
- `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs`
- Vehicle and Property controllers

## Migration Steps

1. **Backup Database** before running migrations ✓
2. **Run Migrations** in this order:
   - Shared module migration (removes IdentityCardType table)
   - PetitionWriterLicense migration
   - Property migration
   - Vehicle migration

3. **Update Backend Code**: ✓
   - Models updated ✓
   - AppDbContext updated ✓
   - Update Controllers and Services (PENDING)
   - Update Request/Response DTOs (PENDING)

4. **Update Frontend Code**: ✓
   - Models updated ✓
   - Petition Writer License components updated ✓
   - Company Owner components updated ✓
   - Guarantor components updated ✓
   - Vehicle Seller/Buyer components updated ✓
   - Vehicle Details View updated ✓
   - Services updated ✓
   - Property components (PENDING)

5. **Testing**:
   - Test all forms with electronic ID only
   - Test view/display components
   - Test API endpoints
   - Test validation

## Status Summary

### Completed ✓
- Backend models updated for all modules
- Database migrations created for all modules
- AppDbContext configuration updated
- Frontend models updated (PetitionWriterLicense, Guarantor)
- Petition Writer License form and view components updated
- Company Owner components updated
- Guarantor components updated
- Vehicle Seller/Buyer components updated
- Vehicle Details View component updated
- Services updated (removed getIdentityTypes, identityCardTypes)

### Pending
- Property module frontend components (seller, buyer, witness)
- Backend Request DTOs updates
- Backend Controllers verification
- Database migration execution
- End-to-end testing

## Field Mapping Reference

### Old → New Field Names

| Module | Old Field Name | New Field Name |
|--------|---------------|----------------|
| Company Owner | IndentityCardNumber | ElectronicNationalIdNumber |
| Guarantor | IndentityCardNumber | ElectronicNationalIdNumber |
| Petition Writer | ElectronicIdNumber | ElectronicNationalIdNumber |
| Property (All) | IndentityCardNumber | ElectronicNationalIdNumber |
| Vehicle (All) | IndentityCardNumber | ElectronicNationalIdNumber |

### Removed Fields

- `IdentityCardTypeId` / `identityCardType`
- `Jild` / `jild` (Volume)
- `Safha` / `safha` (Page)
- `SabtNumber` / `sabtNumber` (Registration Number)
- `TazkiraType` / `tazkiraType`
- `TazkiraVolume` / `tazkiraVolume`
- `TazkiraPage` / `tazkiraPage`
- `TazkiraNumber` / `tazkiraNumber`
- `PaperIdNumber` / `paperIdNumber`
- `PaperIdVolume` / `paperIdVolume`
- `PaperIdPage` / `paperIdPage`
- `PaperIdRegNumber` / `paperIdRegNumber`

## UI Label

Use this label for the electronic ID field in all forms:
- **Pashto/Dari**: الیکټرونیکی تذکره نمبر
- **English**: Electronic National ID Number

## Notes

- The `ElectronicNationalIdNumber` field is now **required** in PetitionWriterLicense
- The field is **optional** in other modules (Company, Property, Vehicle)
- Field type changed from `double` to `string` with max length 50 characters
- All foreign key relationships to `IdentityCardType` table have been removed
