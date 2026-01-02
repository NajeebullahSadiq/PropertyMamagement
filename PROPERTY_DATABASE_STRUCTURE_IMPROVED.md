# Property Management Database Structure & Flow (IMPROVED)

## 📊 Complete Database Schema

### Core Property Table

**PropertyDetails** (`tr.PropertyDetails`)
- `Id` (PK)
- `Pnumber` - Property Number **(INDEXED)**
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
- **`Status` - Workflow Status (Draft/Pending/Approved/Completed/Cancelled) (INDEXED) ✨ NEW**
- **`VerifiedBy` - User who verified ✨ NEW**
- **`VerifiedAt` - Verification timestamp ✨ NEW**
- **`ApprovedBy` - User who approved ✨ NEW**
- **`ApprovedAt` - Approval timestamp ✨ NEW**
- `CreatedAt, CreatedBy` - Audit Fields **(CreatedBy INDEXED)**

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
- `Price, PriceText, RoyaltyAmount, HalfPrice` - Individual buyer amounts
- **`SharePercentage` - Ownership share percentage ✨ NEW**
- **`ShareAmount` - Ownership share amount ✨ NEW**
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
- **`SharePercentage` - Ownership share percentage ✨ NEW**
- **`ShareAmount` - Ownership share amount ✨ NEW**
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
- **`PaddressProvinceId` (FK) → Location ✨ NEW**
- **`PaddressDistrictId` (FK) → Location ✨ NEW**
- **`PaddressVillage` ✨ NEW**
- **`RelationshipToParties` - Relationship to buyer/seller ✨ NEW**
- **`WitnessType` - Transaction/Document witness ✨ NEW**
- `CreatedAt, CreatedBy`

---

### ✨ NEW: Property Ownership History

**PropertyOwnershipHistory** (`tr.PropertyOwnershipHistory`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails **(INDEXED)**
- `OwnerName` - Previous/Current Owner Name
- `OwnerFatherName` - Owner's Father Name
- `OwnershipStartDate` - When ownership started
- `OwnershipEndDate` - When ownership ended (null if current)
- `TransferDocumentPath` - Transfer document file path
- `Notes` - Additional notes
- `CreatedAt, CreatedBy`

**Purpose:** Track complete chain of ownership for property history

---

### ✨ NEW: Property Payment Tracking

**PropertyPayment** (`tr.PropertyPayment`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails **(INDEXED)**
- `PaymentDate` - Date of payment
- `AmountPaid` - Amount paid in this installment
- `PaymentMethod` - Cash/Check/Bank Transfer/etc
- `ReceiptNumber` - Payment receipt number
- `BalanceRemaining` - Remaining balance after this payment
- `Notes` - Additional notes
- `CreatedAt, CreatedBy`

**Purpose:** Track payment installments for properties with payment plans

---

### ✨ NEW: Property Valuation

**PropertyValuation** (`tr.PropertyValuation`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails **(INDEXED)**
- `ValuationDate` - Date of valuation
- `ValuationAmount` - Assessed property value
- `ValuatorName` - Name of valuator
- `ValuatorOrganization` - Valuation company/organization
- `ValuationDocumentPath` - Valuation report file path
- `Notes` - Additional notes
- `CreatedAt, CreatedBy`

**Purpose:** Track professional property valuations over time

---

### ✨ NEW: Property Document Management

**PropertyDocument** (`tr.PropertyDocument`)
- `Id` (PK)
- `PropertyDetailsId` (FK) → PropertyDetails **(INDEXED)**
- `DocumentCategory` - Deed/Title/Survey/Tax Certificate/Authorization/ID Proof/Other
- `FilePath` - Document file path
- `OriginalFileName` - Original uploaded filename
- `Description` - Document description
- `CreatedAt, CreatedBy`

**Purpose:** Categorized document management for better organization

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

## 🔄 Property Transaction Flow (IMPROVED)

### STEP 1: CREATE PROPERTY RECORD (Status: Draft)
- Property Number
- Property Type (House/Land/etc)
- Area + Unit Type
- Transaction Type (Sale/Rent)
- Price & Royalty
- Document Details
- Boundaries (N/S/E/W)
- Upload Documents (categorized)
- **Status automatically set to "Draft"**

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
- **Share Percentage/Amount (if multiple sellers)**

### STEP 4: ADD BUYER(S) - Multiple Allowed
- Personal Info (Name, Father, etc)
- Tazkira Details
- Permanent Address
- Temporary Address
- Role Type (Buyer/Agent)
- Documents (Photo, ID, Letters)
- TIN Number
- **Share Percentage/Amount (if multiple buyers)**
- Price Details (if split)
- Rent Dates (if lease)

### STEP 5: ADD WITNESS(ES) - Optional
- Personal Info
- Tazkira Details
- Contact Info
- **Address Information**
- **Relationship to Parties**
- **Witness Type**
- National ID Card

### STEP 6: ADD PAYMENT PLAN (Optional)
- Payment installments
- Payment dates
- Payment methods
- Receipt tracking

### STEP 7: ADD VALUATION (Optional)
- Valuation date
- Valuation amount
- Valuator details
- Valuation document

### STEP 8: SUBMIT FOR VERIFICATION (Status: Pending Review)
- User submits completed form
- **Status changes to "Pending Review"**
- Assigned to verifier

### STEP 9: VERIFICATION (Status: Approved/Rejected)
- Verifier reviews all details
- **VerifiedBy and VerifiedAt recorded**
- If approved: **Status → "Approved"**
- If rejected: **Status → "Rejected"** with notes

### STEP 10: FINAL APPROVAL (Status: Completed)
- Approver final review
- **ApprovedBy and ApprovedAt recorded**
- **Status → "Completed"**
- `iscomplete = true`
- `iseditable = false` (locked)
- Generate Print Document

### STEP 11: OWNERSHIP HISTORY (Automatic)
- Record ownership transfer in PropertyOwnershipHistory
- Track previous owner → new owner

### STEP 12: AUDIT TRAIL (Automatic)
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

3. **Update Property Status**
   - PropertyDetails.Status → "Cancelled"
   - Original record preserved

---

## 🔗 Entity Relationships (UPDATED)

```
PropertyDetails (1) ──────── (Many) BuyerDetails
PropertyDetails (1) ──────── (Many) SellerDetails
PropertyDetails (1) ──────── (Many) PropertyAddress
PropertyDetails (1) ──────── (Many) WitnessDetails
PropertyDetails (1) ──────── (Many) Propertyaudit
PropertyDetails (1) ──────── (1) PropertyCancellation
PropertyDetails (1) ──────── (Many) PropertyOwnershipHistory ✨ NEW
PropertyDetails (1) ──────── (Many) PropertyPayment ✨ NEW
PropertyDetails (1) ──────── (Many) PropertyValuation ✨ NEW
PropertyDetails (1) ──────── (Many) PropertyDocument ✨ NEW

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

WitnessDetails (Many) ──────── (1) Location (PaddressProvince) ✨ NEW
WitnessDetails (Many) ──────── (1) Location (PaddressDistrict) ✨ NEW
```

---

## 📝 Key Features

### Multiple Parties Support
- ✅ Multiple Sellers per property with share tracking
- ✅ Multiple Buyers per property with share tracking
- ✅ Multiple Witnesses per property with full details

### Workflow Management ✨ NEW
- **Draft** - Initial creation
- **Pending Review** - Submitted for verification
- **Approved** - Verified and approved
- **Completed** - Finalized transaction
- **Cancelled** - Cancelled transaction
- **Rejected** - Rejected during review

### Approval Tracking ✨ NEW
- Verification by authorized user
- Approval by authorized user
- Timestamp tracking for audit

### Ownership Tracking ✨ NEW
- Complete ownership history
- Chain of ownership
- Transfer documentation

### Payment Management ✨ NEW
- Installment tracking
- Payment method recording
- Balance calculation
- Receipt management

### Property Valuation ✨ NEW
- Professional valuations
- Multiple valuations over time
- Valuator tracking

### Document Management (IMPROVED)
- Categorized documents (Deed/Title/Survey/Tax/etc)
- Property documents
- Personal photos
- National ID cards
- Authorization letters
- Heirs letters
- Cancellation documents
- Valuation reports
- Payment receipts

### Address Types
- Permanent Address (Paddress)
- Temporary Address (Taddress)
- Property Location Address
- Witness Address ✨ NEW

### Transaction Types
- Sale/Purchase
- Rent/Lease (with start/end dates)
- Mortgage
- Others

### Audit Trail
- Complete change history
- Track who changed what and when
- Separate audit tables for each entity

### Performance Optimization ✨ NEW
- Indexed fields for faster queries:
  - PropertyDetails.Pnumber
  - PropertyDetails.Status
  - PropertyDetails.CreatedBy
  - All foreign keys

---

## 🎯 Improvements Summary

### ✅ HIGH PRIORITY (IMPLEMENTED)
1. ✅ Added `Status` field with workflow states
2. ✅ Added verification and approval tracking
3. ✅ Added share percentage/amount for buyers and sellers
4. ✅ Enhanced WitnessDetails with address and relationship

### ✅ MEDIUM PRIORITY (IMPLEMENTED)
5. ✅ Added PropertyOwnershipHistory table
6. ✅ Added PropertyPayment table for installments
7. ✅ Added PropertyDocument table with categorization

### ✅ LOW PRIORITY (IMPLEMENTED)
8. ✅ Added PropertyValuation table
9. ✅ Added database indexes for performance

---

## 🚀 Migration Instructions

To apply these improvements to your database:

```bash
# Navigate to Backend directory
cd Backend

# Add migration (already created)
# The migration file is: 20260115000000_ImprovePropertyManagementStructure.cs

# Apply migration to database
dotnet ef database update

# Verify migration
dotnet ef migrations list
```

**Note:** This migration is backward compatible and preserves all existing data.

---

## 📊 Database Size Impact

Estimated additional storage per property transaction:
- PropertyOwnershipHistory: ~200 bytes per ownership record
- PropertyPayment: ~150 bytes per payment
- PropertyValuation: ~200 bytes per valuation
- PropertyDocument: ~100 bytes per document (+ file size)
- New fields in existing tables: ~100 bytes total

**Total:** Minimal impact, approximately 1-2 KB per complete property transaction
