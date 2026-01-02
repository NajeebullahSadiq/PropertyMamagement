# Property Management Database Structure & Flow

## 📊 Complete Database Schema

### Core Property Table

**PropertyDetails** (`tr.PropertyDetails`)
- `Id` (PK)
- `Pnumber` - Property Number
- `Parea` - Property Area
- `PunitTypeId` (FK) → PunitType
- `NumofFloor` - Number of Floors
- `NumofRooms` - Number of Rooms
- `PropertyTypeId` (FK) → PropertyType
- `CustomPropertyType` - Custom Type if not in list
- `Price` - Transaction Price
- `PriceText` - Price in Text
- `RoyaltyAmount` - Royalty/Tax Amount
- `TransactionTypeId` (FK) → TransactionType
- `Des` - Description
- `North, South, East, West` - Property Boundaries
- `DocumentType` - Type of Document
- `IssuanceNumber` - Document Issuance Number
- `IssuanceDate` - Document Issuance Date
- `SerialNumber` - Document Serial Number
- `TransactionDate` - Transaction Date
- `FilePath` - Main Document Path
- `PreviousDocumentsPath` - Previous Documents
- `ExistingDocumentsPath` - Existing Documents
- `iscomplete` - Is Transaction Complete
- `iseditable` - Is Editable
- `CreatedAt, CreatedBy` - Audit Fields

---

### Related Entities (1:Many)

**BuyerDetails** (`tr.BuyerDetails`) - MULTIPLE BUYERS SUPPORTED
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails
- `FirstName, FatherName, GrandFather`
- `IndentityCardNumber`
- `TazkiraType, TazkiraVolume, TazkiraPage, TazkiraNumber`
- `PhoneNumber`
- `PaddressProvinceId` (FK) → Location (Permanent Address)
- `PaddressDistrictId` (FK) → Location
- `PaddressVillage`
- `TaddressProvinceId` (FK) → Location (Temporary Address)
- `TaddressDistrictId` (FK) → Location
- `TaddressVillage`
- `Photo` - Photo Path
- `NationalIdCard` - National ID Card Path
- `RoleType` - "Buyer" or "Authorized Agent (Buyer)"
- `AuthorizationLetter` - Authorization Document Path
- `TaxIdentificationNumber` - TIN
- `AdditionalDetails`
- `Price, PriceText, RoyaltyAmount, HalfPrice`
- `TransactionType, TransactionTypeDescription`
- `RentStartDate, RentEndDate` - For Lease/Rent transactions
- `CreatedAt, CreatedBy`

**SellerDetails** (`tr.SellerDetails`) - MULTIPLE SELLERS SUPPORTED
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails
- `FirstName, FatherName, GrandFather`
- `IndentityCardNumber`
- `TazkiraType, TazkiraVolume, TazkiraPage, TazkiraNumber`
- `PhoneNumber`
- `PaddressProvinceId` (FK) → Location (Permanent Address)
- `PaddressDistrictId` (FK) → Location
- `PaddressVillage`
- `TaddressProvinceId` (FK) → Location (Temporary Address)
- `TaddressDistrictId` (FK) → Location
- `TaddressVillage`
- `Photo` - Photo Path
- `NationalIdCard` - National ID Card Path
- `RoleType` - "Seller" or "Authorized Agent (Seller)"
- `AuthorizationLetter` - Authorization Document Path
- `HeirsLetter` - Heirs Letter Path
- `TaxIdentificationNumber` - TIN
- `AdditionalDetails`
- `CreatedAt, CreatedBy`

**PropertyAddress** (`tr.PropertyAddress`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails
- `ProvinceId` - Property Location Province
- `DistrictId` - Property Location District
- `Village` - Property Location Village
- `CreatedAt, CreatedBy`

**WitnessDetails** (`tr.WitnessDetails`) - MULTIPLE WITNESSES
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails
- `FirstName, FatherName`
- `IndentityCardNumber`
- `TazkiraType, TazkiraVolume, TazkiraPage, TazkiraNumber`
- `PhoneNumber`
- `NationalIdCard` - National ID Card Path
- `CreatedAt, CreatedBy`

---

### Property Cancellation System

**PropertyCancellations** (`tr.PropertyCancellations`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails
- `CancellationDate`
- `CancellationReason`
- `CancelledBy`
- `Status` - Default: "Cancelled"
- `CreatedAt`

**PropertyCancellationDocuments** (`tr.PropertyCancellationDocuments`)
- `Id` (PK)
- `PropertyCancellationId` (FK) → PropertyCancellations
- `FilePath` - Document File Path
- `OriginalFileName`
- `CreatedAt, CreatedBy`

---

### Lookup Tables

**PropertyType** (`look.PropertyType`)
- `Id` (PK), `Name`, `Des`
- Examples: House, Apartment, Land, Commercial, Block

**TransactionType** (`look.TransactionType`)
- `Id` (PK), `Name`, `Des`
- Examples: Sale, Purchase, Rent/Lease, Mortgage

**PunitType** (`look.PunitType`)
- `Id` (PK), `Name`, `Des`
- Examples: Square Meter, Square Foot, Jerib, Acre

**Location** (`look.Location`)
- `Id` (PK), `Dari`, `Name`, `Code`, `ParentId`, `TypeId`, `Path`, `PathDari`, `IsActive`
- Hierarchical: Province → District

---

### Audit Trail System

**Propertyaudit** (`log.propertyaudit`)
- `Id` (PK)
- `PropertyId` (FK) → PropertyDetails
- `PropertyName` - Field Name Changed
- `OldValue` - Previous Value
- `NewValue` - New Value
- `UpdatedBy` - User Who Made Change
- `UpdatedAt` - Timestamp

**Propertybuyeraudit** (`log.propertybuyeraudit`)
- `Id` (PK)
- `BuyerId` (FK) → BuyerDetails
- `OldValue, NewValue`
- `UpdatedBy, UpdatedAt`

**Propertyselleraudit** (`log.propertyselleraudit`)
- `Id` (PK)
- `SellerId` (FK) → SellerDetails
- `OldValue, NewValue`
- `UpdatedBy, UpdatedAt`

---

## 🔄 Property Transaction Flow

### STEP 1: CREATE PROPERTY RECORD
- Property Number
- Property Type (House/Land/etc)
- Area + Unit Type
- Transaction Type (Sale/Rent)
- Price & Royalty
- Document Details
- Boundaries (N/S/E/W)
- Upload Documents

### STEP 2: ADD PROPERTY ADDRESS
- Province
- District
- Village

### STEP 3: ADD SELLER(S) - Multiple Allowed
- Personal Info (Name, Father, etc)
- Tazkira Details
- Permanent Address
- Temporary Address
- Role Type (Seller/Agent)
- Documents (Photo, ID, Letters)
- TIN Number

### STEP 4: ADD BUYER(S) - Multiple Allowed
- Personal Info (Name, Father, etc)
- Tazkira Details
- Permanent Address
- Temporary Address
- Role Type (Buyer/Agent)
- Documents (Photo, ID, Letters)
- TIN Number
- Price Details (if split)
- Rent Dates (if lease)

### STEP 5: ADD WITNESS(ES) - Optional
- Personal Info
- Tazkira Details
- Contact Info
- National ID Card

### STEP 6: COMPLETE & SUBMIT
- `iscomplete = true`
- `iseditable = false` (locked)
- Generate Print Document

### STEP 7: AUDIT TRAIL (Automatic)
- All changes tracked in audit tables

---

## 🚫 Property Cancellation Flow

1. **Create PropertyCancellation**
   - Link to PropertyDetailsId
   - Cancellation Date
   - Cancellation Reason
   - Cancelled By (User)
   - Status = "Cancelled"

2. **Upload Supporting Documents**
   - PropertyCancellationDocuments
   - Multiple documents allowed
   - Store file paths
   - Track original filenames

3. **Property marked as Cancelled** (Original record preserved)

---

## 🔗 Entity Relationships

```
PropertyDetails (1) ──────── (Many) BuyerDetails
PropertyDetails (1) ──────── (Many) SellerDetails
PropertyDetails (1) ──────── (Many) PropertyAddress
PropertyDetails (1) ──────── (Many) WitnessDetails
PropertyDetails (1) ──────── (Many) Propertyaudit
PropertyDetails (1) ──────── (1) PropertyCancellation
PropertyCancellation (1) ─── (Many) PropertyCancellationDocuments

PropertyDetails (Many) ───── (1) PropertyType
PropertyDetails (Many) ───── (1) TransactionType
PropertyDetails (Many) ───── (1) PunitType

BuyerDetails (Many) ───────── (1) Location (PaddressProvince)
BuyerDetails (Many) ───────── (1) Location (PaddressDistrict)
BuyerDetails (Many) ───────── (1) Location (TaddressProvince)
BuyerDetails (Many) ───────── (1) Location (TaddressDistrict)

SellerDetails (Many) ──────── (1) Location (PaddressProvince)
SellerDetails (Many) ──────── (1) Location (PaddressDistrict)
SellerDetails (Many) ──────── (1) Location (TaddressProvince)
SellerDetails (Many) ──────── (1) Location (TaddressDistrict)
```

---

## 📝 Key Features

### Multiple Parties Support
- ✅ Multiple Sellers per property
- ✅ Multiple Buyers per property
- ✅ Multiple Witnesses per property

### Role Types
- Direct Seller/Buyer
- Authorized Agent (with authorization letter)

### Document Management
- Property documents
- Personal photos
- National ID cards
- Authorization letters
- Heirs letters
- Cancellation documents

### Address Types
- Permanent Address (Paddress)
- Temporary Address (Taddress)
- Property Location Address

### Transaction Types
- Sale/Purchase
- Rent/Lease (with start/end dates)
- Mortgage
- Others

### Audit Trail
- Complete change history
- Track who changed what and when
- Separate audit tables for each entity
