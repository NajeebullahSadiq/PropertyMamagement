# Province-Based License Numbering System

## Overview
This document describes the implementation of a province-specific license numbering system for Afghanistan's 34 provinces.

## License Number Format
**Format:** `PROVINCE_CODE-SEQUENTIAL_NUMBER`

**Examples:**
- Kabul: `KBL-0001`, `KBL-0002`, `KBL-2234`
- Kandahar: `KHR-0001`, `KHR-0002`
- Herat: `HRT-0001`, `HRT-0234`

## Province Codes

| Province | Code | Province | Code | Province | Code |
|----------|------|----------|------|----------|------|
| Kabul | KBL | Herat | HRT | Kandahar | KHR |
| Balkh | BLK | Nangarhar | NGR | Ghazni | GHZ |
| Helmand | HLM | Badakhshan | BDK | Takhar | TKR |
| Kunduz | KDZ | Baghlan | BGL | Bamyan | BMN |
| Farah | FRH | Faryab | FRB | Ghor | GHR |
| Jawzjan | JWZ | Kapisa | KPS | Khost | KHT |
| Kunar | KNR | Laghman | LGM | Logar | LGR |
| Nimroz | NMZ | Nuristan | NRS | Paktia | PKT |
| Paktika | PKK | Panjshir | PNJ | Parwan | PRW |
| Samangan | SMG | Sar-e Pol | SRP | Uruzgan | URZ |
| Wardak | WRD | Zabul | ZBL | Badghis | BDG |
| Daykundi | DYK |

## Implementation Details

### Backend Components

#### 1. License Number Generator Service
**File:** `Backend/Services/LicenseNumberGenerator.cs`

**Features:**
- Auto-generates sequential license numbers per province
- Format: `PROVINCE_CODE-XXXX` (4 digits with leading zeros)
- Thread-safe sequential numbering
- Province code mapping for all 34 provinces

**Usage:**
```csharp
// Inject the service
private readonly ILicenseNumberGenerator _licenseNumberGenerator;

// Generate license number
string licenseNumber = await _licenseNumberGenerator.GenerateNextLicenseNumber(provinceId);
// Result: "KBL-0001"
```

#### 2. Database Migration
**File:** `Backend/Infrastructure/Migrations/Company/20260125_AddProvinceToLicenseDetails.cs`

**Changes:**
- Adds `ProvinceId` column to `LicenseDetails` table
- Creates foreign key to `Location` table
- Adds indexes for performance:
  - `IX_LicenseDetails_LicenseNumber`
  - `IX_LicenseDetails_ProvinceId`
  - `IX_LicenseDetails_ProvinceId_LicenseNumber` (composite)

#### 3. Model Updates
**Files:**
- `Backend/Models/Company/LicenseDetail.cs` - Added `ProvinceId` and navigation property
- `Backend/Models/RequestData/Company/LicenseDetailData.cs` - Added `ProvinceId` field

#### 4. Controller Updates
**File:** `Backend/Controllers/Companies/LicenseDetailController.cs`

**Features:**
- Auto-generates license number when `ProvinceId` is provided
- Manual license number entry still supported
- Validates province existence
- Handles both POST (create) and PUT (update) operations

### Frontend Components

#### Required Updates

1. **License Details Form** (`Frontend/src/app/realestate/licensedetails/licensedetails.component.html`)
   - Add province dropdown selector
   - Make license number field read-only (auto-generated)
   - Display generated license number

2. **License Details Component** (`Frontend/src/app/realestate/licensedetails/licensedetails.component.ts`)
   - Add `provinceId` field to form
   - Load provinces from API
   - Handle auto-generated license number

3. **License Detail Model** (`Frontend/src/app/models/LicenseDetail.ts`)
   - Add `provinceId?: number` field

## How It Works

### Creating a New License

1. **User selects province** from dropdown
2. **System auto-generates** license number: `PROVINCE_CODE-XXXX`
3. **Sequential numbering** starts from 0001 for each province
4. **License number is saved** with the license record

### Example Flow

```
User Action: Select "Kabul" province
System: Checks last license number for Kabul
System: Finds "KBL-0234"
System: Generates "KBL-0235"
System: Saves license with number "KBL-0235"
```

### Numbering Logic

```csharp
// Get last license for province
var lastLicense = await _context.LicenseDetails
    .Where(l => l.LicenseNumber.StartsWith("KBL-"))
    .OrderByDescending(l => l.Id)
    .FirstOrDefaultAsync();

// Extract number and increment
int nextNumber = ExtractNumber(lastLicense) + 1;

// Format: KBL-0235
string newLicenseNumber = $"KBL-{nextNumber:D4}";
```

## Database Schema

### LicenseDetails Table (Updated)

```sql
CREATE TABLE org."LicenseDetails" (
    "Id" SERIAL PRIMARY KEY,
    "LicenseNumber" TEXT,
    "ProvinceId" INTEGER,  -- NEW FIELD
    "IssueDate" DATE,
    "ExpireDate" DATE,
    "AreaId" INTEGER,
    "OfficeAddress" TEXT,
    "CompanyId" INTEGER,
    -- ... other fields
    
    CONSTRAINT "FK_LicenseDetails_Location_ProvinceId" 
        FOREIGN KEY ("ProvinceId") 
        REFERENCES look."Location"("ID")
);

-- Indexes
CREATE INDEX "IX_LicenseDetails_ProvinceId" ON org."LicenseDetails"("ProvinceId");
CREATE INDEX "IX_LicenseDetails_LicenseNumber" ON org."LicenseDetails"("LicenseNumber");
CREATE INDEX "IX_LicenseDetails_ProvinceId_LicenseNumber" 
    ON org."LicenseDetails"("ProvinceId", "LicenseNumber");
```

## Deployment Steps

### 1. Backend Deployment

```bash
# Navigate to Backend directory
cd Backend

# Run migration
dotnet ef migrations add AddProvinceToLicenseDetails --context AppDbContext

# Update database
dotnet ef database update

# Build and restart
dotnet build
dotnet run
```

### 2. Frontend Deployment

```bash
# Navigate to Frontend directory
cd Frontend

# Update components (see Frontend Updates section)
# Build
ng build --configuration production

# Deploy
```

## API Endpoints

### Create License with Auto-Generated Number

**POST** `/api/LicenseDetail`

```json
{
  "companyId": 123,
  "provinceId": 1,  // Kabul
  "issueDate": "1403/10/05",
  "areaId": 2,
  "officeAddress": "کابل، ناحیه ۱۰",
  "licenseType": "رهنمای معاملات",
  "licenseCategory": "جدید",
  "calendarType": "hijriShamsi"
}
```

**Response:**
```json
{
  "id": 456,
  "licenseNumber": "KBL-0235"
}
```

### Update License

**PUT** `/api/LicenseDetail/{id}`

```json
{
  "id": 456,
  "provinceId": 1,
  "licenseNumber": "KBL-0235",  // Can be updated manually if needed
  // ... other fields
}
```

## Benefits

1. **Unique Identification** - Each province has its own number sequence
2. **Easy Tracking** - Quickly identify which province issued the license
3. **Scalability** - Supports unlimited licenses per province
4. **Standardization** - Follows international best practices
5. **Database Efficiency** - Indexed for fast queries
6. **User-Friendly** - Clear, readable format

## Migration from Old System

If you have existing licenses without province codes:

```sql
-- Example: Update existing licenses to add province
UPDATE org."LicenseDetails" ld
SET "ProvinceId" = co."PermanentProvinceId"
FROM org."CompanyDetails" cd
JOIN org."CompanyOwner" co ON cd."Id" = co."CompanyId"
WHERE ld."CompanyId" = cd."Id"
  AND ld."ProvinceId" IS NULL;

-- Regenerate license numbers with province codes
-- (Run this carefully in a transaction)
```

## Testing

### Test Cases

1. **Create license for Kabul** → Should generate `KBL-0001`
2. **Create second license for Kabul** → Should generate `KBL-0002`
3. **Create license for Kandahar** → Should generate `KHR-0001`
4. **Create license without province** → Should allow manual entry
5. **Update license province** → Should regenerate number if empty

### Test Script

```bash
# Test license creation
curl -X POST http://localhost:5000/api/LicenseDetail \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "companyId": 1,
    "provinceId": 1,
    "issueDate": "2025/01/25",
    "calendarType": "gregorian"
  }'
```

## Troubleshooting

### Issue: License numbers not generating

**Solution:** Check that:
1. ProvinceId is provided in the request
2. Province exists in Location table with TypeId = 2
3. LicenseNumberGenerator service is registered in Program.cs

### Issue: Duplicate license numbers

**Solution:** 
1. Check database indexes are created
2. Ensure sequential generation logic is working
3. Review transaction isolation level

### Issue: Province code not found

**Solution:**
1. Verify province name matches exactly in ProvinceCodeMap
2. Check Location table has correct province names
3. Fallback uses first 3 letters of province name

## Future Enhancements

1. **Batch License Generation** - Generate multiple licenses at once
2. **License Number Reservation** - Reserve numbers before final submission
3. **Custom Province Codes** - Allow admin to customize province codes
4. **License Number History** - Track all license number changes
5. **Analytics Dashboard** - Show license distribution by province

## Support

For questions or issues:
- Check this documentation first
- Review migration files
- Test with sample data
- Contact system administrator

---

**Last Updated:** January 25, 2026
**Version:** 1.0
**Author:** System Development Team
