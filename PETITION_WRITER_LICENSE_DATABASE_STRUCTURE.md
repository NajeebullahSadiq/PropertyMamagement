# Petition Writer License Module - Complete Database Structure

## Overview
This document provides the complete database structure for the Petition Writer License module, including all tables, fields, relationships, and business logic.

---

## Database Schema: `org` (Organization)

### Table 1: `PetitionWriterLicenses`
**Purpose**: Main table storing petition writer license information

#### Columns

| Column Name | Data Type | Max Length | Nullable | Default | Description |
|------------|-----------|------------|----------|---------|-------------|
| `Id` | INTEGER | - | NO | AUTO | Primary Key (Auto-increment) |
| `LicenseNumber` | VARCHAR | 50 | NO | - | Unique license number (Format: PROVINCE_CODE-SEQUENTIAL) |
| `ProvinceId` | INTEGER | - | YES | NULL | Province for license numbering (FK to shared.Locations) |
| `ApplicantName` | VARCHAR | 200 | NO | - | نام و تخلص - Full name of petition writer |
| `ApplicantFatherName` | VARCHAR | 200 | YES | NULL | نام پدر - Father's name |
| `ApplicantGrandFatherName` | VARCHAR | 200 | YES | NULL | نام پدر کلان - Grandfather's name |
| `MobileNumber` | VARCHAR | 20 | YES | NULL | شماره تماس - Mobile phone number |
| `Competency` | VARCHAR | 50 | YES | NULL | اهلیت - Competency level (high/medium/low) |
| `ElectronicNationalIdNumber` | VARCHAR | 50 | NO | - | شماره تذکره الکترونیکی - Electronic National ID |
| `PermanentProvinceId` | INTEGER | - | YES | NULL | سکونت اصلی - ولایت (FK to shared.Locations) |
| `PermanentDistrictId` | INTEGER | - | YES | NULL | سکونت اصلی - ولسوالی (FK to shared.Locations) |
| `PermanentVillage` | VARCHAR | 500 | YES | NULL | سکونت اصلی - ناحیه/قریه |
| `CurrentProvinceId` | INTEGER | - | YES | NULL | سکونت فعلی - ولایت (FK to shared.Locations) |
| `CurrentDistrictId` | INTEGER | - | YES | NULL | سکونت فعلی - ولسوالی (FK to shared.Locations) |
| `CurrentVillage` | VARCHAR | 500 | YES | NULL | سکونت فعلی - قریه/گذر |
| `DetailedAddress` | VARCHAR | 1000 | YES | NULL | آدرس دقیق محل فعالیت - Detailed activity address |
| `PicturePath` | VARCHAR | 500 | YES | NULL | عکس - Photo file path |
| `BankReceiptNumber` | VARCHAR | 100 | YES | NULL | نمبر رسید بانکی - Bank receipt number |
| `BankReceiptDate` | DATE | - | YES | NULL | تاریخ رسید بانکی - Bank receipt date |
| `District` | VARCHAR | 200 | YES | NULL | ناحیه - District/Area for activity |
| `LicenseType` | VARCHAR | 50 | YES | NULL | نوع جواز - License type (new/renewal/duplicate) |
| `LicensePrice` | DECIMAL | - | YES | NULL | قیمت جواز - License price in AFN |
| `LicenseIssueDate` | DATE | - | YES | NULL | تاریخ صدور جواز - License issue date |
| `LicenseExpiryDate` | DATE | - | YES | NULL | تاریخ ختم جواز - License expiry date |
| `LicenseStatus` | INTEGER | - | NO | 1 | Status: 1=Active, 2=Cancelled, 3=Withdrawn |
| `CancellationDate` | DATE | - | YES | NULL | تاریخ لغو - Cancellation date |
| `Status` | BOOLEAN | - | NO | TRUE | Soft delete flag (TRUE=Active, FALSE=Deleted) |
| `CreatedAt` | TIMESTAMP | - | YES | NULL | Record creation timestamp |
| `CreatedBy` | VARCHAR | 50 | YES | NULL | User ID who created the record |
| `UpdatedAt` | TIMESTAMP | - | YES | NULL | Last update timestamp |
| `UpdatedBy` | VARCHAR | 50 | YES | NULL | User ID who last updated the record |

#### Indexes
- **PRIMARY KEY**: `Id`
- **UNIQUE INDEX**: `LicenseNumber`
- **INDEX**: `ApplicantName`
- **INDEX**: `LicenseStatus`
- **INDEX**: `ProvinceId`

#### Foreign Keys
- `ProvinceId` → `shared.Locations(Id)` - Province for license numbering
- `PermanentProvinceId` → `shared.Locations(Id)` - Permanent address province
- `PermanentDistrictId` → `shared.Locations(Id)` - Permanent address district
- `CurrentProvinceId` → `shared.Locations(Id)` - Current address province
- `CurrentDistrictId` → `shared.Locations(Id)` - Current address district

---

### Table 2: `PetitionWriterRelocations`
**Purpose**: Tracks relocation history when petition writer changes activity location

#### Columns

| Column Name | Data Type | Max Length | Nullable | Default | Description |
|------------|-----------|------------|----------|---------|-------------|
| `Id` | INTEGER | - | NO | AUTO | Primary Key (Auto-increment) |
| `PetitionWriterLicenseId` | INTEGER | - | NO | - | FK to PetitionWriterLicenses |
| `NewActivityLocation` | VARCHAR | 500 | NO | - | محل فعالیت جدید - New activity location |
| `RelocationDate` | DATE | - | YES | NULL | تاریخ نقل مکان - Date of relocation |
| `Remarks` | VARCHAR | 1000 | YES | NULL | ملاحظات - Additional remarks |
| `CreatedAt` | TIMESTAMP | - | YES | NULL | Record creation timestamp |
| `CreatedBy` | VARCHAR | 50 | YES | NULL | User ID who created the record |

#### Indexes
- **PRIMARY KEY**: `Id`
- **INDEX**: `PetitionWriterLicenseId`
- **INDEX**: `RelocationDate`

#### Foreign Keys
- `PetitionWriterLicenseId` → `org.PetitionWriterLicenses(Id)` ON DELETE CASCADE

---

## Business Logic & Rules

### License Number Generation
- **Format**: `{PROVINCE_CODE}-{SEQUENTIAL_NUMBER}`
- **Example**: `KBL-001`, `HRT-045`, `KDR-123`
- **Province Codes**: Defined in `shared.Locations` table
- **Sequential Number**: Auto-incremented per province
- **Uniqueness**: Enforced by unique index

### License Types & Pricing
| License Type | Dari | English | Price (AFN) | Expiry Calculation |
|-------------|------|---------|-------------|-------------------|
| `new` | جدید | New | 168 | +1 year from issue date |
| `renewal` | تمدید | Renewal | 85 | +1 year from issue date |
| `duplicate` | مثنی | Duplicate | 85 | Manual entry required |

### Competency Levels
| Code | Dari | English |
|------|------|---------|
| `high` | اعلی | High |
| `medium` | اوسط | Medium |
| `low` | ادنی | Low |

### License Status Values
| Status | Description | Dari |
|--------|-------------|------|
| 1 | Active | فعال |
| 2 | Cancelled | لغو شده |
| 3 | Withdrawn | انصراف |

### Date Handling
- All dates stored as `DATE` type (DateOnly in C#)
- Frontend supports both Solar (Hijri Shamsi) and Gregorian calendars
- Conversion handled by `DateConversionHelper` service
- Display format: `YYYY/MM/DD`

### Auto-Calculations

#### 1. License Price
- Automatically calculated based on `LicenseType`
- جدید (new) → 168 AFN
- تمدید (renewal) → 85 AFN
- مثنی (duplicate) → 85 AFN

#### 2. License Expiry Date
- **New License (جدید)**: Issue Date + 1 year
- **Renewal (تمدید)**: Issue Date + 1 year
- **Duplicate (مثنی)**: Manual entry (no auto-calculation)

---

## Relationships

### One-to-Many Relationships
1. **PetitionWriterLicenses → PetitionWriterRelocations**
   - One license can have multiple relocations
   - Cascade delete: When license is deleted, all relocations are deleted

2. **Locations → PetitionWriterLicenses (Province)**
   - One province can have many licenses
   - Used for license numbering

3. **Locations → PetitionWriterLicenses (Permanent Address)**
   - Province and District for permanent address

4. **Locations → PetitionWriterLicenses (Current Address)**
   - Province and District for current address

---

## API Endpoints

### Base URL: `/api/PetitionWriterLicense`

#### 1. Get All Licenses (Paginated)
```
GET /api/PetitionWriterLicense?pageNumber=1&pageSize=10
```
**Response**: Paginated list with metadata

#### 2. Get License by ID
```
GET /api/PetitionWriterLicense/{id}
```
**Response**: Complete license details with related data

#### 3. Create License
```
POST /api/PetitionWriterLicense
Content-Type: application/json

{
  "applicantName": "string",
  "applicantFatherName": "string",
  "electronicNationalIdNumber": "string",
  "mobileNumber": "string",
  "competency": "high|medium|low",
  "licenseType": "new|renewal|duplicate",
  "licensePrice": 168,
  "licenseIssueDate": "2026-02-11",
  "licenseExpiryDate": "2027-02-11",
  ...
}
```

#### 4. Update License
```
PUT /api/PetitionWriterLicense/{id}
```

#### 5. Delete License (Soft Delete)
```
DELETE /api/PetitionWriterLicense/{id}
```

#### 6. Update License Status
```
PUT /api/PetitionWriterLicense/{id}/status
```

#### 7. Get Relocations
```
GET /api/PetitionWriterLicense/{id}/relocations
```

#### 8. Add Relocation
```
POST /api/PetitionWriterLicense/{id}/relocations
```

#### 9. Update Relocation
```
PUT /api/PetitionWriterLicense/relocations/{id}
```

#### 10. Delete Relocation
```
DELETE /api/PetitionWriterLicense/relocations/{id}
```

---

## Verification System Integration

### Verification Code Generation
- **Document Type**: `PetitionWriterLicense`
- **Code Prefix**: `PWL`
- **Format**: `PWL-YYYY-XXXXXX` (e.g., `PWL-2026-A1B2C3`)
- **QR Code**: Generated on print, contains verification URL
- **Verification URL**: `https://prmis.gov.af/verify/{code}`

### Verification Data Returned
When a petition writer license is verified, the following data is returned:
- Basic Info: License number, name, photo, issue/expiry dates
- Personal Info: Father name, grandfather name, national ID, mobile
- Permanent Address: Province, district, village
- Current Address: Province, district, village
- Activity Location: District, detailed address, latest relocation
- License Details: Competency, license type, license price

---

## File Storage

### Photo Storage
- **Base Path**: `Resources/Documents/PetitionWriter/`
- **Naming Convention**: `{LicenseId}_{Timestamp}_{OriginalFileName}`
- **Allowed Formats**: JPG, JPEG, PNG
- **Max Size**: 5MB
- **Display**: Used in print certificate and verification page

---

## Print Certificate

### Certificate Layout
- **Format**: A4 size, portrait orientation
- **Sections**:
  1. Header: Ministry info, national emblem, logo, photo
  2. Main Table: 6-column layout with all license details
  3. Bottom Table: License validity, competency assessment, authority signature
  4. Footer: QR code and verification code

### Certificate Features
- Watermark with license details
- Guilloche pattern border
- Ornate corner decorations
- Professional styling with blue color scheme
- QR code for verification
- Verification code display

---

## Security & Authorization

### Role-Based Access Control (RBAC)
- **Admin**: Full access to all operations
- **Province User**: Access limited to their province licenses
- **View Only**: Read-only access

### Province-Based Filtering
- Users can only see/edit licenses from their assigned province
- Enforced at middleware level (`ProvinceAuthorizationMiddleware`)
- Admin users bypass province restrictions

### Audit Trail
- All create/update operations logged with user ID and timestamp
- `CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt` fields maintained
- Soft delete preserves historical data

---

## Migration Scripts

### Initial Setup
```sql
-- Located at: Backend/Scripts/petition_writer_module_clean_recreate.sql
-- Creates both tables with all constraints and indexes
```

### Field Additions
```sql
-- Located at: Backend/Infrastructure/Migrations/PetitionWriterLicense/
-- 20260209_AddNewFieldsToPetitionWriter.cs
-- Adds: MobileNumber, Competency, District, LicenseType, LicensePrice, 
--       LicenseExpiryDate, DetailedAddress
```

### Field Removals
```sql
-- ActivityLocation field removed (replaced by DetailedAddress)
-- Migration script: Backend/Scripts/remove_activity_location_field.sql
```

---

## Frontend Components

### Main Components
1. **petition-writer-license-list**: List view with search and pagination
2. **petition-writer-license-form**: Create/Edit form with 3 tabs
3. **petition-writer-license-view**: Read-only detail view
4. **print-petition-writer-license**: Print certificate component

### Form Tabs
1. **Tab 1**: Personal information and addresses
2. **Tab 2**: Financial info and license details
3. **Tab 3**: Status management (cancel/withdraw)

### Features
- Auto-calculation of license price based on type
- Auto-calculation of expiry date (+1 year for new/renewal)
- Calendar type selection (Solar/Gregorian)
- Photo upload with preview
- Relocation history management
- Print certificate with QR code
- Verification system integration

---

## Data Validation

### Required Fields
- `LicenseNumber` (auto-generated if not provided)
- `ApplicantName`
- `ElectronicNationalIdNumber`

### Field Constraints
- `LicenseNumber`: Unique, max 50 characters
- `ApplicantName`: Max 200 characters
- `ElectronicNationalIdNumber`: Max 50 characters
- `MobileNumber`: Max 20 characters
- `Competency`: Must be 'high', 'medium', or 'low'
- `LicenseType`: Must be 'new', 'renewal', or 'duplicate'
- `LicensePrice`: Positive decimal value
- `LicenseStatus`: Must be 1, 2, or 3

### Business Rules
- License number must be unique across all licenses
- Expiry date must be after issue date
- Cancellation date required when status is Cancelled (2)
- Photo file size must not exceed 5MB
- Relocation date cannot be in the future

---

## Sample Data

### Sample License Record
```json
{
  "id": 1,
  "licenseNumber": "KBL-001",
  "provinceId": 1,
  "applicantName": "احمد شاه احمدی",
  "applicantFatherName": "محمد شاه",
  "applicantGrandFatherName": "عبدالله",
  "mobileNumber": "0700123456",
  "competency": "high",
  "electronicNationalIdNumber": "1234567890123",
  "permanentProvinceId": 1,
  "permanentDistrictId": 101,
  "permanentVillage": "خیرخانه",
  "currentProvinceId": 1,
  "currentDistrictId": 102,
  "currentVillage": "شهرنو",
  "detailedAddress": "ناحیه دهم، سرک اول، کوچه سوم",
  "picturePath": "Resources/Documents/PetitionWriter/1_20260211_photo.jpg",
  "bankReceiptNumber": "REC-2026-001",
  "bankReceiptDate": "2026-02-10",
  "district": "ناحیه دهم",
  "licenseType": "new",
  "licensePrice": 168,
  "licenseIssueDate": "2026-02-11",
  "licenseExpiryDate": "2027-02-11",
  "licenseStatus": 1,
  "status": true,
  "createdAt": "2026-02-11T10:30:00",
  "createdBy": "admin"
}
```

### Sample Relocation Record
```json
{
  "id": 1,
  "petitionWriterLicenseId": 1,
  "newActivityLocation": "ناحیه پانزدهم، سرک دوم",
  "relocationDate": "2026-06-15",
  "remarks": "نقل مکان به دفتر جدید",
  "createdAt": "2026-06-15T14:20:00",
  "createdBy": "admin"
}
```

---

## Database Queries

### Get Active Licenses by Province
```sql
SELECT * FROM org."PetitionWriterLicenses"
WHERE "ProvinceId" = 1 
  AND "LicenseStatus" = 1 
  AND "Status" = TRUE
ORDER BY "LicenseNumber";
```

### Get Licenses Expiring Soon (within 30 days)
```sql
SELECT * FROM org."PetitionWriterLicenses"
WHERE "LicenseExpiryDate" BETWEEN CURRENT_DATE AND CURRENT_DATE + INTERVAL '30 days'
  AND "LicenseStatus" = 1
  AND "Status" = TRUE;
```

### Get License with Full Details
```sql
SELECT 
  pwl.*,
  pp.name_dari as permanent_province,
  pd.name_dari as permanent_district,
  cp.name_dari as current_province,
  cd.name_dari as current_district
FROM org."PetitionWriterLicenses" pwl
LEFT JOIN shared."Locations" pp ON pwl."PermanentProvinceId" = pp."Id"
LEFT JOIN shared."Locations" pd ON pwl."PermanentDistrictId" = pd."Id"
LEFT JOIN shared."Locations" cp ON pwl."CurrentProvinceId" = cp."Id"
LEFT JOIN shared."Locations" cd ON pwl."CurrentDistrictId" = cd."Id"
WHERE pwl."Id" = 1;
```

### Get Relocation History
```sql
SELECT * FROM org."PetitionWriterRelocations"
WHERE "PetitionWriterLicenseId" = 1
ORDER BY "RelocationDate" DESC;
```

---

## Performance Considerations

### Indexes
- Primary key on `Id` for fast lookups
- Unique index on `LicenseNumber` for uniqueness enforcement
- Index on `ApplicantName` for search operations
- Index on `LicenseStatus` for filtering
- Index on `ProvinceId` for province-based queries

### Optimization Tips
1. Use pagination for list views (default: 10 records per page)
2. Include only necessary fields in SELECT queries
3. Use JOIN instead of multiple queries for related data
4. Cache location data (provinces/districts) in frontend
5. Implement lazy loading for relocation history

---

## Backup & Recovery

### Backup Strategy
- Daily automated backups of entire database
- Transaction log backups every hour
- Retention period: 30 days

### Recovery Procedures
1. Restore from latest backup
2. Apply transaction logs if needed
3. Verify data integrity
4. Update sequences if necessary

---

## Future Enhancements

### Planned Features
1. Digital signature integration
2. Online payment gateway for license fees
3. SMS notifications for expiry reminders
4. Mobile app for license verification
5. Bulk import/export functionality
6. Advanced reporting and analytics
7. Integration with national ID verification system

---

## Support & Maintenance

### Common Issues
1. **License number not generating**: Check province configuration
2. **Photo not displaying**: Verify file path and permissions
3. **Date conversion errors**: Ensure calendar type is specified
4. **Verification not working**: Check verification service configuration

### Maintenance Tasks
- Weekly: Review and archive old records
- Monthly: Analyze performance and optimize queries
- Quarterly: Update location data if needed
- Annually: Review and update license pricing

---

## Contact Information

For technical support or questions about the Petition Writer License module:
- **System**: Property Registration Management Information System (PRMIS)
- **Ministry**: Ministry of Justice - Afghanistan
- **Department**: Directorate of Real Estate Guides and Petition Writers

---

**Document Version**: 1.0  
**Last Updated**: February 11, 2026  
**Status**: Complete and Production-Ready
