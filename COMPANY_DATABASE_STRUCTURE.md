# Company Module Database Structure

## Overview
The Company module manages real estate company/license holder information, including company details, owners, guarantors, licenses, financial information, and cancellation records.

**Database Schemas:**
- `org` - Organization/Transaction tables
- `log` - Audit/History tables
- `look` - Lookup/Reference tables (shared)

---

## Transaction Tables (org schema)

### 1. CompanyDetails
**Purpose:** Main company/license holder information

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| Title | VARCHAR(500) | Company name/title |
| TIN | VARCHAR(50) | Tax Identification Number |
| ProvinceId | INTEGER (FK) | Province where company is registered (for access control) |
| DocPath | VARCHAR(500) | Document file path |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Has many: CompanyOwners, LicenseDetails, Guarantors, Gaurantees, CompanyAccountInfo, CompanyCancellationInfo, Haqulemtyaz, PeriodicForms
- References: Location (Province)

---

### 2. CompanyOwners
**Purpose:** Company owner/partner information

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| FirstName | VARCHAR(200) | Owner's first name |
| FatherName | VARCHAR(200) | Owner's father name |
| GrandFatherName | VARCHAR(200) | Owner's grandfather name |
| EducationLevelId | SMALLINT (FK) | Education level reference |
| DateofBirth | DATE | Date of birth |
| ElectronicNationalIdNumber | VARCHAR(50) | Electronic National ID (الیکټرونیکی تذکره) |
| PhoneNumber | VARCHAR(20) | Contact phone number |
| WhatsAppNumber | VARCHAR(20) | WhatsApp number |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| PothoPath | VARCHAR(500) | Photo file path |
| **Owner's Own Address (آدرس اصلی مالک)** | | |
| OwnerProvinceId | INTEGER (FK) | Owner's home province |
| OwnerDistrictId | INTEGER (FK) | Owner's home district |
| OwnerVillage | VARCHAR(500) | Owner's home village |
| **Permanent Address (آدرس دایمی)** | | |
| PermanentProvinceId | INTEGER (FK) | Current residence province |
| PermanentDistrictId | INTEGER (FK) | Current residence district |
| PermanentVillage | VARCHAR(500) | Current residence village |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails, EducationLevel
- Has many: CompanyOwnerAddresses, CompanyOwnerAddressHistory
- References: Location (multiple for addresses)

---

### 3. CompanyOwnerAddresses
**Purpose:** Legacy address table (kept for compatibility)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| AddressTypeId | INTEGER (FK) | Address type reference |
| ProvinceId | INTEGER (FK) | Province reference |
| DistrictId | INTEGER (FK) | District reference |
| Village | VARCHAR(500) | Village name |
| CompanyOwnerId | INTEGER (FK) | Reference to CompanyOwner |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyOwner, AddressType, Location (Province, District)

---

### 4. CompanyOwnerAddressHistory
**Purpose:** Track address changes over time

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| CompanyOwnerId | INTEGER (FK) | Reference to CompanyOwner |
| ProvinceId | INTEGER (FK) | Province reference |
| DistrictId | INTEGER (FK) | District reference |
| Village | VARCHAR(500) | Village name |
| AddressType | VARCHAR(50) | "Permanent" or "Current" |
| EffectiveFrom | TIMESTAMP | When this address became active |
| EffectiveTo | TIMESTAMP | When this address was replaced |
| IsActive | BOOLEAN | Currently active address |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyOwner, Location (Province, District)

---

### 5. Guarantors
**Purpose:** Guarantor/Collateral provider information (merged with guarantee details)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| **Personal Information** | | |
| FirstName | VARCHAR(200) | Guarantor's first name |
| FatherName | VARCHAR(200) | Guarantor's father name |
| GrandFatherName | VARCHAR(200) | Guarantor's grandfather name |
| ElectronicNationalIdNumber | VARCHAR(50) | Electronic National ID |
| PhoneNumber | VARCHAR(20) | Contact phone number |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| **Permanent Address** | | |
| PaddressProvinceId | INTEGER (FK) | Permanent address province |
| PaddressDistrictId | INTEGER (FK) | Permanent address district |
| PaddressVillage | VARCHAR(500) | Permanent address village |
| **Temporary Address** | | |
| TaddressProvinceId | INTEGER (FK) | Temporary address province |
| TaddressDistrictId | INTEGER (FK) | Temporary address district |
| TaddressVillage | VARCHAR(500) | Temporary address village |
| **Guarantee Information** | | |
| GuaranteeTypeId | INTEGER (FK) | Type of guarantee (Sharia Deed, Customary Deed, Cash) |
| PropertyDocumentNumber | BIGINT | Property document number |
| PropertyDocumentDate | DATE | Property document date |
| SenderMaktobNumber | VARCHAR(100) | Sender letter number |
| SenderMaktobDate | DATE | Sender letter date |
| AnswerdMaktobNumber | BIGINT | Response letter number |
| AnswerdMaktobDate | DATE | Response letter date |
| DateofGuarantee | DATE | Guarantee date |
| GuaranteeDocNumber | BIGINT | Guarantee document number |
| GuaranteeDate | DATE | Guarantee date |
| GuaranteeDocPath | VARCHAR(500) | Guarantee document file path |
| **Conditional: Sharia Deed (قباله شرعی)** | | |
| CourtName | VARCHAR(200) | Court name (محکمه نوم) |
| CollateralNumber | VARCHAR(100) | Collateral number (نمبر وثیقه) |
| **Conditional: Customary Deed (قباله عرفی)** | | |
| SetSerialNumber | VARCHAR(100) | Set serial number (نمبر سریال سټه) |
| GuaranteeDistrictId | INTEGER (FK) | District (ناحیه) |
| **Conditional: Cash (پول نقد)** | | |
| BankName | VARCHAR(200) | Bank name (بانک) |
| DepositNumber | VARCHAR(100) | Deposit number (نمبر اویز) |
| DepositDate | DATE | Deposit date (تاریخ اویز) |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails, GuaranteeType
- References: Location (multiple for addresses)

---

### 6. Gaurantees
**Purpose:** Legacy guarantee table (kept for compatibility)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| GuaranteeTypeId | INTEGER (FK) | Type of guarantee |
| PropertyDocumentNumber | BIGINT | Property document number |
| PropertyDocumentDate | DATE | Property document date |
| SenderMaktobNumber | VARCHAR(100) | Sender letter number |
| SenderMaktobDate | DATE | Sender letter date |
| AnswerdMaktobNumber | BIGINT | Response letter number |
| AnswerdMaktobDate | DATE | Response letter date |
| DateofGuarantee | DATE | Guarantee date |
| GuaranteeDocNumber | BIGINT | Guarantee document number |
| GuaranteeDate | DATE | Guarantee date |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| DocPath | VARCHAR(500) | Document file path |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails, GuaranteeType

---

### 7. LicenseDetails
**Purpose:** License/permit information with province-specific numbering

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| LicenseNumber | VARCHAR(50) | License number (format: PROVINCE_CODE-SEQUENTIAL) |
| ProvinceId | INTEGER (FK) | Province where license is issued |
| IssueDate | DATE | License issue date |
| ExpireDate | DATE | License expiration date |
| AreaId | INTEGER (FK) | Area/region reference |
| OfficeAddress | VARCHAR(500) | Office address |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| DocPath | VARCHAR(500) | Document file path |
| LicenseType | VARCHAR(100) | Type of license |
| **License Category (نوعیت جواز)** | | |
| LicenseCategory | VARCHAR(50) | جدید (New), تجدید (Renewal), مثنی (Duplicate) |
| RenewalRound | INTEGER | Renewal round number (دور تجدید) |
| **Financial Information** | | |
| RoyaltyAmount | DECIMAL(18,2) | Royalty/License fee amount (مبلغ حق‌الامتیاز) |
| RoyaltyDate | DATE | Royalty/License fee date (تاریخ حق‌الامتیاز) |
| TariffNumber | VARCHAR(100) | Tariff number |
| PenaltyAmount | DECIMAL(18,2) | Penalty amount (مبلغ جریمه) |
| PenaltyDate | DATE | Penalty date (تاریخ جریمه) |
| **HR Information** | | |
| HrLetter | VARCHAR(100) | HR letter reference (مکتوب قوای بشری) |
| HrLetterDate | DATE | HR letter date (تاریخ مکتوب قوای بشری) |
| **Status Fields** | | |
| IsComplete | BOOLEAN | All required fields filled (controls print) |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails, Area, Location (Province)

---

### 8. CompanyAccountInfo
**Purpose:** Company financial/tax information (مالیه)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| SettlementInfo | VARCHAR(500) | Account settlement info (نمرمكتوب تصفيه معلومات) |
| TaxPaymentAmount | DECIMAL(18,2) | Tax payment amount (تحويل ماليات) |
| SettlementYear | INTEGER | Settlement year (سال تصفيه مالية) |
| TaxPaymentDate | DATE | Tax payment date (تاريخ تحويل ماليات) |
| TransactionCount | INTEGER | Number of transactions (تعدادی معامله) |
| CompanyCommission | DECIMAL(18,2) | Company commission (كمیشن رهنما) |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails

---

### 9. CompanyCancellationInfo
**Purpose:** License cancellation/revocation information (فسخ / لغوه)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| LicenseCancellationLetterNumber | VARCHAR(100) | License cancellation letter number (نمبر مکتوب فسخ جواز) |
| RevenueCancellationLetterNumber | VARCHAR(100) | Revenue cancellation letter number (نمبر مکتوب فسخ عواید) |
| LicenseCancellationLetterDate | DATE | License cancellation date (تاریخ مکتوب فسخ جواز) |
| Remarks | VARCHAR(1000) | Remarks/Notes (ملاحظات) |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails

---

### 10. Haqulemtyaz
**Purpose:** Royalty/privilege fee information (حق الامتیاز)

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| FormNumber | INTEGER | Form number |
| FormDate | DATE | Form date |
| SubmissionFormNumber | INTEGER | Submission form number |
| SubmissionFormDate | DATE | Submission form date |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| DocPath | VARCHAR(500) | Document file path |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails

---

### 11. PeriodicForms
**Purpose:** Periodic reporting forms

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| ReferenceId | INTEGER (FK) | Form reference type |
| FormNumber | INTEGER | Form number |
| FormDate | DATE | Form date |
| SubmissionDate | DATE | Submission date |
| MaktobNumber | VARCHAR(100) | Letter number |
| MaktobDate | DATE | Letter date |
| DiagnosisNumber | INTEGER | Diagnosis number |
| Details | TEXT | Form details |
| DocPath | VARCHAR(500) | Document file path |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| Status | BOOLEAN | Active/Inactive status |
| CreatedAt | TIMESTAMP | Record creation timestamp |
| CreatedBy | VARCHAR(50) | User who created the record |

**Relationships:**
- Belongs to: CompanyDetails, FormsReference

---

## Audit Tables (log schema)

### 1. Companydetailsaudit
**Purpose:** Track changes to CompanyDetails

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| CompanyId | INTEGER (FK) | Reference to CompanyDetails |
| PropertyName | VARCHAR(100) | Field name that changed |
| OldValue | TEXT | Previous value |
| NewValue | TEXT | New value |
| UpdatedBy | VARCHAR(50) | User who made the change |
| UpdatedAt | TIMESTAMP | Change timestamp |

---

### 2. Companyowneraudit
**Purpose:** Track changes to CompanyOwners

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| OwnerId | INTEGER (FK) | Reference to CompanyOwner |
| PropertyName | VARCHAR(100) | Field name that changed |
| OldValue | TEXT | Previous value |
| NewValue | TEXT | New value |
| UpdatedBy | VARCHAR(50) | User who made the change |
| UpdatedAt | TIMESTAMP | Change timestamp |

---

### 3. Guarantorsaudit
**Purpose:** Track changes to Guarantors

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| GuarantorsId | INTEGER (FK) | Reference to Guarantor |
| PropertyName | VARCHAR(100) | Field name that changed |
| OldValue | TEXT | Previous value |
| NewValue | TEXT | New value |
| UpdatedBy | VARCHAR(50) | User who made the change |
| UpdatedAt | TIMESTAMP | Change timestamp |

---

### 4. Graunteeaudit
**Purpose:** Track changes to Gaurantees

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| GauranteeId | INTEGER (FK) | Reference to Gaurantee |
| PropertyName | VARCHAR(100) | Field name that changed |
| OldValue | TEXT | Previous value |
| NewValue | TEXT | New value |
| UpdatedBy | VARCHAR(50) | User who made the change |
| UpdatedAt | TIMESTAMP | Change timestamp |

---

### 5. Licenseaudit
**Purpose:** Track changes to LicenseDetails

| Column | Type | Description |
|--------|------|-------------|
| Id | SERIAL (PK) | Primary key |
| LicenseId | INTEGER (FK) | Reference to LicenseDetails |
| PropertyName | VARCHAR(100) | Field name that changed |
| OldValue | TEXT | Previous value |
| NewValue | TEXT | New value |
| UpdatedBy | VARCHAR(50) | User who made the change |
| UpdatedAt | TIMESTAMP | Change timestamp |

---

## Entity Relationships

```
CompanyDetails (1) ──┬── (Many) CompanyOwners
                     ├── (Many) LicenseDetails
                     ├── (Many) Guarantors
                     ├── (Many) Gaurantees
                     ├── (Many) CompanyAccountInfo
                     ├── (Many) CompanyCancellationInfo
                     ├── (Many) Haqulemtyaz
                     └── (Many) PeriodicForms

CompanyOwners (1) ───┬── (Many) CompanyOwnerAddresses
                     └── (Many) CompanyOwnerAddressHistory

Guarantors (1) ────── (Many) Guarantorsaudit
CompanyDetails (1) ── (Many) Companydetailsaudit
CompanyOwners (1) ─── (Many) Companyowneraudit
Gaurantees (1) ────── (Many) Graunteeaudit
LicenseDetails (1) ── (Many) Licenseaudit
```

---

## Key Features

### 1. Province-Based Access Control
- `CompanyDetails.ProvinceId` - Controls which users can access company data
- `LicenseDetails.ProvinceId` - Enables province-specific license numbering

### 2. License Management
- **License Categories:** جدید (New), تجدید (Renewal), مثنی (Duplicate)
- **Renewal Tracking:** RenewalRound field tracks renewal iterations
- **Financial Tracking:** Royalty amounts, penalties, tariff numbers
- **Completion Status:** IsComplete flag controls print functionality

### 3. Address Management
- **Multiple Address Types:** Owner's home address, permanent address
- **Address History:** CompanyOwnerAddressHistory tracks changes over time
- **Legacy Support:** CompanyOwnerAddresses maintained for compatibility

### 4. Guarantee Types
Three types with conditional fields:
- **Sharia Deed (قباله شرعی):** CourtName, CollateralNumber
- **Customary Deed (قباله عرفی):** SetSerialNumber, GuaranteeDistrictId
- **Cash (پول نقد):** BankName, DepositNumber, DepositDate

### 5. Comprehensive Audit Trail
- All major entities have corresponding audit tables
- Tracks field-level changes with old/new values
- Records user and timestamp for each change

### 6. Financial Management
- Tax payment tracking
- Transaction counts
- Company commission calculations
- Settlement year tracking

### 7. Cancellation Management
- License cancellation letter tracking
- Revenue cancellation tracking
- Remarks/notes for cancellation reasons

---

## Performance Indexes

All tables have indexes on:
- Primary keys (automatic)
- Foreign keys
- Frequently queried fields (Status, dates, names, IDs)
- Search fields (LicenseNumber, ElectronicNationalIdNumber, etc.)

---

## Data Validation Rules

1. **Required Fields:**
   - CompanyDetails: Title
   - CompanyOwners: FirstName, FatherName
   - Guarantors: FirstName, FatherName
   - CompanyAccountInfo: TaxPaymentAmount

2. **Numeric Constraints:**
   - All amounts must be non-negative
   - Transaction counts must be non-negative

3. **Date Logic:**
   - ExpireDate should be after IssueDate
   - EffectiveTo should be after EffectiveFrom

4. **Conditional Requirements:**
   - RenewalRound required when LicenseCategory = "تجدید"
   - Guarantee type-specific fields required based on GuaranteeTypeId

---

## Summary Statistics

- **Transaction Tables:** 11
- **Audit Tables:** 5
- **Total Tables:** 16
- **Total Indexes:** 50+
- **Schemas Used:** 3 (org, log, look)
