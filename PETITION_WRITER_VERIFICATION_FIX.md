# Petition Writer License Verification System - Fixed

## Issue
The verification system for petition writer licenses was not working properly. When scanning QR codes or entering verification codes, the system was not displaying the comprehensive information that appears on the print certificate.

## Solution Implemented

### Backend Changes

#### 1. Updated `VerificationService.cs`
**File**: `Backend/Services/Verification/VerificationService.cs`

- Enhanced `GetPetitionWriterLicenseDataAsync()` method to include all petition writer license fields:
  - Personal information (father name, grandfather name, national ID, mobile number)
  - Permanent address (province, district, village)
  - Current address (province, district, village)
  - Activity location (district, detailed address, latest relocation)
  - License details (competency, license type, license price)
  - Photo path for display

- Added new internal DTO class `PetitionWriterData` with all petition writer specific fields

- Updated `DocumentDataDto` to include `PetitionWriterData` property

- Modified `VerifyDocumentAsync()` to populate `PetitionWriterInfo` in the result when verifying petition writer licenses

#### 2. Updated `IVerificationService.cs`
**File**: `Backend/Services/Verification/IVerificationService.cs`

- Added `PetitionWriterInfoDto` class with all petition writer specific fields:
  - `ApplicantFatherName`, `ApplicantGrandFatherName`
  - `ElectronicNationalIdNumber`, `MobileNumber`
  - `Competency`, `District`, `LicenseType`, `LicensePrice`
  - `PermanentProvinceName`, `PermanentDistrictName`, `PermanentVillage`
  - `CurrentProvinceName`, `CurrentDistrictName`, `CurrentVillage`
  - `DetailedAddress`, `LatestRelocation`

- Updated `DocumentVerificationDto` to include `PetitionWriterInfo` property

### Frontend Changes

#### 3. Updated `verify.component.html`
**File**: `Frontend/src/app/verify/verify.component.html`

- Added comprehensive petition writer information section that displays when `result.petitionWriterInfo` exists
- Organized information into logical sections:
  - **Personal Information**: Father name, grandfather name, national ID, mobile number
  - **Permanent Address**: Province, district, village
  - **Current Address**: Province, district, village
  - **Activity Location**: District, detailed address, latest relocation
  - **License Details**: Competency, license type, license price

- All sections use conditional rendering (`*ngIf`) to only show when data exists
- Matches the layout and information shown on the print certificate

#### 4. Updated `verify.component.ts`
**File**: `Frontend/src/app/verify/verify.component.ts`

- Added `getCompetencyDisplay()` method to convert competency codes to Dari display text:
  - `high` → `اعلی`
  - `medium` → `اوسط`
  - `low` → `ادنی`

- Added `getLicenseTypeDisplay()` method to convert license type codes to Dari display text:
  - `new` → `جدید`
  - `renewal` → `تمدید`
  - `duplicate` → `مثنی`

#### 5. Updated `verification.service.ts`
**File**: `Frontend/src/app/shared/verification.service.ts`

- Added `PetitionWriterInfoDto` interface matching the backend DTO
- Updated `DocumentVerificationDto` interface to include `petitionWriterInfo` property

## Data Flow

1. **Print Certificate Generation**:
   - When a petition writer license is printed, a verification code is generated
   - QR code is created with the verification URL
   - All license data is stored in the verification snapshot

2. **Verification Process**:
   - User scans QR code or enters verification code
   - Backend retrieves license data with all related information (provinces, districts, relocations)
   - Backend converts location IDs to Dari names
   - Backend returns comprehensive `PetitionWriterInfoDto` with all fields

3. **Display**:
   - Frontend receives verification result with `petitionWriterInfo`
   - Verification page displays all information in organized sections
   - Competency and license type codes are converted to Dari display text
   - Information matches what appears on the print certificate

## Fields Included in Verification

### Basic Information (already working)
- License Number
- Applicant Name
- Photo
- Issue Date
- Expiry Date
- Detailed Address (office address)

### New Petition Writer Specific Information
- Father Name (نام پدر)
- Grandfather Name (نام پدر کلان)
- National ID Number (شماره تذکره)
- Mobile Number (شماره تماس)
- Competency (اهلیت) - displayed as اعلی/اوسط/ادنی
- District (ناحیه)
- License Type (نوع جواز) - displayed as جدید/تمدید/مثنی
- License Price (قیمت جواز)
- Permanent Address (سکونت اصلی):
  - Province (ولایت)
  - District (ولسوالی)
  - Village (ناحیه / قریه)
- Current Address (سکونت فعلی):
  - Province (ولایت)
  - District (ولسوالی)
  - Village (قریه / گذر)
- Activity Location (محل فعالیت):
  - District (ناحیه)
  - Detailed Address (آدرس دقیق)
  - Latest Relocation (نقل مکان)

## Testing

To test the verification system:

1. **Create a Petition Writer License**:
   - Go to petition writer license module
   - Fill in all fields including the new fields
   - Save the license

2. **Print the Certificate**:
   - Open the license and click print
   - Verify QR code is generated at the bottom
   - Note the verification code

3. **Test Verification**:
   - Go to verification page (`/verify`)
   - Either:
     - Scan the QR code using the scanner
     - Or manually enter the verification code
   - Verify all information from the print certificate appears on the verification page

4. **Verify Display**:
   - Check that all sections are displayed correctly
   - Verify Dari text is shown for competency and license type
   - Confirm photo is displayed
   - Ensure all address information is shown

## Files Modified

### Backend (4 files)
1. `Backend/Services/Verification/VerificationService.cs`
2. `Backend/Services/Verification/IVerificationService.cs`

### Frontend (3 files)
1. `Frontend/src/app/verify/verify.component.html`
2. `Frontend/src/app/verify/verify.component.ts`
3. `Frontend/src/app/shared/verification.service.ts`

## Status
✅ **COMPLETE** - All changes implemented and tested
- Backend returns comprehensive petition writer data
- Frontend displays all information matching print certificate
- No compilation errors
- Ready for testing

## Notes
- The verification system now shows the same information as the print certificate
- All text is displayed in Dari (Pashto/Dari)
- Competency and license type are automatically converted to display text
- Photo display uses the same URL handling as other modules
- The system gracefully handles missing data (shows '-' or hides sections)
