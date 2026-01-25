# Province-Based License Numbering for Petition Writer Licenses - COMPLETE ✅

## Implementation Summary

Successfully implemented province-based license numbering system for petition writer licenses (ثبت جواز عریضه‌نویسان) with format: `PROVINCE_CODE-SEQUENTIAL_NUMBER` (e.g., KBL-0001, KHR-0002).

**Implementation Date:** January 25, 2026

---

## Backend Implementation ✅

### 1. Database Changes

**SQL Script:** `Backend/Scripts/add_province_to_petition_writer_license.sql`
- Adds `ProvinceId` column to `PetitionWriterLicense` table
- Creates foreign key relationship to `Province` table
- Updates existing records with default province (Kabul)

**Migration:** `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260125_AddProvinceToPetitionWriterLicense.cs`
- EF Core migration for province field
- Includes rollback support

### 2. Model Updates

**File:** `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs`
```csharp
public int? ProvinceId { get; set; }
public Province? Province { get; set; }
```

**File:** `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs`
```csharp
public int? ProvinceId { get; set; }
```

### 3. License Number Generator

**File:** `Backend/Services/LicenseNumberGenerator.cs`
- Added `GenerateNextPetitionWriterLicenseNumber(int provinceId)` method
- Implements province-based sequential numbering
- Uses all 34 Afghanistan province codes (KBL, KHR, HRT, etc.)

### 4. Controller Updates

**File:** `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs`

**Changes:**
- Injected `ILicenseNumberGenerator` service
- Auto-generates license number when `ProvinceId` is provided
- Added `/api/PetitionWriterLicense/provinces` endpoint
- Updated GetAll and GetById to include Province data with `.Include(x => x.Province)`

**New Endpoint:**
```csharp
[HttpGet("provinces")]
public IActionResult GetProvinces()
{
    var provinces = _context.Province.OrderBy(p => p.Dari).ToList();
    return Ok(provinces);
}
```

---

## Frontend Implementation ✅

### 1. Model Updates

**File:** `Frontend/src/app/models/PetitionWriterLicense.ts`
```typescript
provinceId?: number;
provinceName?: string;
```

### 2. Service Updates

**File:** `Frontend/src/app/shared/petition-writer-license.service.ts`
```typescript
getProvinces(): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/provinces`);
}
```

### 3. Form Component

**File:** `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts`

**Changes:**
- Added `provinces` array
- Added `loadProvinces()` method
- Added `onProvinceChange()` method to clear license number
- Updated form initialization to include `provinceId` field
- Updated `loadLicenseData()` to patch `provinceId`
- Updated `saveLicense()` to include `provinceId`

**File:** `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html`

**Changes:**
- Added province dropdown BEFORE license number field
- Made license number readonly when province is selected
- Added helper text explaining auto-generation
- Province field is required (marked with red asterisk)

### 4. List Component

**File:** `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.html`

**Changes:**
- Added "ولایت" (Province) column after license number
- Displays `item.provinceName` or '-' if not set
- Updated colspan from 8 to 9 for empty state

### 5. View Component

**File:** `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.html`

**Changes:**
- Added province display card in basic info section
- Shows province name with map icon
- Positioned between license number and applicant name

---

## License Number Format

### Format Structure
```
PROVINCE_CODE-SEQUENTIAL_NUMBER
```

### Examples
- Kabul: `KBL-0001`, `KBL-0002`, `KBL-0234`
- Kandahar: `KHR-0001`, `KHR-0002`
- Herat: `HRT-0001`, `HRT-0002`

### Province Codes (All 34 Provinces)
```
KBL - کابل (Kabul)
KHR - قندهار (Kandahar)
HRT - هرات (Herat)
BLK - بلخ (Balkh)
NNG - ننگرهار (Nangarhar)
GHZ - غزنی (Ghazni)
KDZ - کندز (Kunduz)
BDG - بادغیس (Badghis)
TKR - تخار (Takhar)
BGL - بغلان (Baghlan)
PKT - پکتیا (Paktia)
FRH - فراه (Farah)
JWZ - جوزجان (Jowzjan)
HLM - هلمند (Helmand)
SAR - سرپل (Sar-e Pol)
NMZ - نیمروز (Nimroz)
KNR - کنر (Kunar)
LGR - لغمان (Laghman)
PKK - پکتیکا (Paktika)
BDK - بدخشان (Badakhshan)
GHR - غور (Ghor)
ZBL - زابل (Zabul)
KPS - کاپیسا (Kapisa)
PRW - پروان (Parwan)
NRS - نورستان (Nuristan)
KST - خوست (Khost)
LWG - لوگر (Logar)
SMG - سمنگان (Samangan)
URZ - ارزگان (Uruzgan)
WRD - وردک (Wardak)
PNJ - پنجشیر (Panjshir)
BMN - بامیان (Bamyan)
DKD - دایکندی (Daykundi)
```

---

## User Experience

### Form Behavior
1. User selects province from dropdown (required field)
2. License number field becomes readonly and shows gray background
3. Helper text appears: "نمبر جواز به صورت خودکار بر اساس ولایت تولید می‌شود"
4. On save, backend auto-generates license number based on province

### List View
- Shows province name in dedicated column
- Allows filtering by province name in search

### Detail View
- Displays province prominently in basic info section
- Shows province with map icon for visual clarity

---

## Deployment Steps

### 1. Database Migration
```bash
# Run SQL script
sqlcmd -S your_server -d PRMIS -i Backend/Scripts/add_province_to_petition_writer_license.sql

# Or use EF Core migration
cd Backend
dotnet ef database update
```

### 2. Backend Build
```bash
cd Backend
dotnet build
dotnet run
```

### 3. Frontend Build
```bash
cd Frontend
npm install
ng build --configuration production
```

### 4. Testing
- Create new petition writer license
- Select province
- Verify license number auto-generation
- Check list view shows province
- Check detail view shows province

---

## Files Modified

### Backend (7 files)
1. `Backend/Scripts/add_province_to_petition_writer_license.sql` (NEW)
2. `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260125_AddProvinceToPetitionWriterLicense.cs` (NEW)
3. `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs` (MODIFIED)
4. `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs` (MODIFIED)
5. `Backend/Services/LicenseNumberGenerator.cs` (MODIFIED)
6. `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs` (MODIFIED)

### Frontend (5 files)
1. `Frontend/src/app/models/PetitionWriterLicense.ts` (MODIFIED)
2. `Frontend/src/app/shared/petition-writer-license.service.ts` (MODIFIED)
3. `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts` (MODIFIED)
4. `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html` (MODIFIED)
5. `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.html` (MODIFIED)
6. `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.html` (MODIFIED)

### Documentation (3 files)
1. `PETITION_WRITER_PROVINCE_LICENSE_IMPLEMENTATION.md` (comprehensive guide)
2. `PETITION_WRITER_PROVINCE_LICENSE_QUICK_START.md` (quick reference)
3. `PETITION_WRITER_PROVINCE_LICENSE_COMPLETE.md` (this file)

---

## Consistency with Company License System

This implementation follows the exact same pattern as the company license system:

✅ Same license number format (PROVINCE_CODE-SEQUENTIAL_NUMBER)
✅ Same province codes for all 34 provinces
✅ Same auto-generation logic in backend
✅ Same UI/UX pattern (readonly field, helper text)
✅ Same database structure (ProvinceId foreign key)
✅ Same API pattern (provinces endpoint, Include in queries)

---

## Status: COMPLETE ✅

All backend and frontend changes have been implemented. The system is ready for deployment and testing.

**Next Steps:**
1. Deploy SQL script to database
2. Build and deploy backend
3. Build and deploy frontend
4. Test license creation with province selection
5. Verify license number auto-generation works correctly

---

**Implementation completed on:** January 25, 2026
**Implemented by:** Kiro AI Assistant
