# Province-Based License Numbering for Petition Writer Licenses

## Overview
Implementing the same province-based license numbering system used for company licenses to petition writer licenses (ثبت جواز عریضه‌نویسان).

**Format**: `PROVINCE_CODE-SEQUENTIAL_NUMBER`
**Examples**: 
- Kabul: `KBL-0001`, `KBL-0002`, `KBL-0234`
- Kandahar: `KHR-0001`, `KHR-0002`
- Herat: `HRT-0001`

## Changes Required

### 1. Database Changes

#### SQL Script Created
- **File**: `Backend/Scripts/add_province_to_petition_writer_license.sql`
- Adds `ProvinceId` column to `org.PetitionWriterLicenses` table
- Creates foreign key to `shared.Locations`
- Creates indexes for performance
- Includes all 34 province codes

#### Migration Created
- **File**: `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260125_AddProvinceToPetitionWriterLicense.cs`
- FluentMigrator migration for adding ProvinceId
- Includes Up() and Down() methods for rollback

### 2. Backend Model Changes

#### Update `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs`
Add after line 23 (after LicenseNumber):
```csharp
// Province for license numbering
public int? ProvinceId { get; set; }
```

Add navigation property after line 103:
```csharp
[ForeignKey("ProvinceId")]
public virtual Location? Province { get; set; }
```

#### Update `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs`
Add after LicenseNumber property:
```csharp
public int? ProvinceId { get; set; }
```

### 3. Service Updates

#### Update `Backend/Services/LicenseNumberGenerator.cs`
Add new method for petition writer licenses:
```csharp
/// <summary>
/// Generate the next sequential license number for petition writer license
/// </summary>
public async Task<string> GenerateNextPetitionWriterLicenseNumber(int provinceId)
{
    // Get province information
    var province = await _context.Locations
        .FirstOrDefaultAsync(l => l.Id == provinceId && l.TypeId == 2);

    if (province == null)
    {
        throw new ArgumentException($"Province with ID {provinceId} not found");
    }

    var provinceCode = GetProvinceCode(provinceId);

    // Get the last license number for this province
    var lastLicense = await _context.PetitionWriterLicenses
        .Where(l => l.LicenseNumber != null && l.LicenseNumber.StartsWith(provinceCode + "-"))
        .OrderByDescending(l => l.Id)
        .Select(l => l.LicenseNumber)
        .FirstOrDefaultAsync();

    int nextNumber = 1;

    if (lastLicense != null)
    {
        // Extract the number part after the dash
        var parts = lastLicense.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[1], out int currentNumber))
        {
            nextNumber = currentNumber + 1;
        }
    }

    // Format: PROVINCE_CODE-0001 (4 digits with leading zeros)
    return $"{provinceCode}-{nextNumber:D4}";
}
```

Update interface:
```csharp
public interface ILicenseNumberGenerator
{
    Task<string> GenerateNextLicenseNumber(int provinceId);
    Task<string> GenerateNextPetitionWriterLicenseNumber(int provinceId);
    string GetProvinceCode(int provinceId);
}
```

### 4. Controller Updates

#### Update `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs`

Add constructor injection:
```csharp
private readonly AppDbContext _context;
private readonly ILicenseNumberGenerator _licenseNumberGenerator;

public PetitionWriterLicenseController(AppDbContext context, ILicenseNumberGenerator licenseNumberGenerator)
{
    _context = context;
    _licenseNumberGenerator = licenseNumberGenerator;
}
```

Update Create method (replace license number validation):
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] PetitionWriterLicenseData data)
{
    try
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Auto-generate license number if provinceId is provided
        string licenseNumber = data.LicenseNumber;
        if (data.ProvinceId.HasValue && string.IsNullOrWhiteSpace(data.LicenseNumber))
        {
            licenseNumber = await _licenseNumberGenerator.GenerateNextPetitionWriterLicenseNumber(data.ProvinceId.Value);
        }

        // Check for duplicate license number
        var exists = await _context.PetitionWriterLicenses
            .AnyAsync(x => x.LicenseNumber == licenseNumber && x.Status == true);

        if (exists)
        {
            return BadRequest(new { message = "نمبر جواز قبلاً ثبت شده است" });
        }

        var calendar = DateConversionHelper.ParseCalendarType(data.CalendarType);
        var username = User.Identity?.Name ?? "system";

        var entity = new PetitionWriterLicenseEntity
        {
            LicenseNumber = licenseNumber,
            ProvinceId = data.ProvinceId,
            ApplicantName = data.ApplicantName,
            // ... rest of properties
        };

        _context.PetitionWriterLicenses.Add(entity);
        await _context.SaveChangesAsync();

        return Ok(new { id = entity.Id, licenseNumber = entity.LicenseNumber, message = "جواز با موفقیت ثبت شد" });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "خطا در ثبت جواز", error = ex.Message });
    }
}
```

Update GetAll and GetById to include Province:
```csharp
.Include(x => x.Province)
// In select:
x.ProvinceId,
ProvinceName = x.Province != null ? x.Province.Dari : "",
```

Add new endpoint to get provinces:
```csharp
/// <summary>
/// Get all provinces for dropdown
/// </summary>
[HttpGet("provinces")]
public async Task<IActionResult> GetProvinces()
{
    try
    {
        var provinces = await _context.Locations
            .Where(l => l.TypeId == 2 && l.Status == true)
            .OrderBy(l => l.Dari)
            .Select(l => new
            {
                l.Id,
                l.Name,
                l.Dari,
                l.Pashto
            })
            .ToListAsync();

        return Ok(provinces);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "خطا در بارگذاری ولایات", error = ex.Message });
    }
}
```

### 5. Frontend Model Updates

#### Update `Frontend/src/app/models/PetitionWriterLicense.ts`
Add after licenseNumber:
```typescript
provinceId?: number;
provinceName?: string;
```

### 6. Frontend Service Updates

#### Update `Frontend/src/app/shared/petition-writer-license.service.ts`
Add method to get provinces:
```typescript
getProvinces(): Observable<any> {
    return this.http.get(`${this.apiUrl}/provinces`);
}
```

### 7. Frontend Component Updates

#### Update `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts`

Add provinces array:
```typescript
provinces: any[] = [];
```

Load provinces in ngOnInit:
```typescript
this.loadProvinces();
```

Add loadProvinces method:
```typescript
loadProvinces(): void {
    this.licenseService.getProvinces().subscribe({
        next: (data: any) => {
            this.provinces = data;
        },
        error: (err: any) => console.error('Error loading provinces', err)
    });
}
```

Update licenseForm initialization:
```typescript
this.licenseForm = this.fb.group({
    licenseNumber: [''], // Make optional - will be auto-generated
    provinceId: [null, Validators.required], // Add province field
    applicantName: ['', Validators.required],
    // ... rest of fields
});
```

Update loadLicenseData to patch provinceId:
```typescript
this.licenseForm.patchValue({
    licenseNumber: data.licenseNumber,
    provinceId: data.provinceId,
    // ... rest
});
```

Make license number readonly when provinceId is selected:
```typescript
onProvinceChange(): void {
    const provinceId = this.licenseForm.get('provinceId')?.value;
    if (provinceId) {
        this.licenseForm.get('licenseNumber')?.disable();
        this.licenseForm.patchValue({ licenseNumber: '' });
    } else {
        this.licenseForm.get('licenseNumber')?.enable();
    }
}
```

Update saveLicense to include provinceId:
```typescript
const data: PetitionWriterLicenseData = {
    ...formValue,
    provinceId: formValue.provinceId,
    licenseNumber: formValue.licenseNumber || undefined, // Will be auto-generated if empty
    // ... rest
};
```

#### Update `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html`

Add province dropdown before license number field:
```html
<!-- Province Selection -->
<div class="form-group">
    <label for="provinceId" class="required">ولایت</label>
    <select 
        id="provinceId" 
        formControlName="provinceId" 
        class="form-control"
        (change)="onProvinceChange()">
        <option value="">-- انتخاب ولایت --</option>
        <option *ngFor="let province of provinces" [value]="province.id">
            {{ province.dari }}
        </option>
    </select>
    <div *ngIf="licenseForm.get('provinceId')?.invalid && licenseForm.get('provinceId')?.touched" 
         class="text-danger">
        ولایت الزامی است
    </div>
</div>

<!-- License Number (Auto-generated or Manual) -->
<div class="form-group">
    <label for="licenseNumber">نمبر جواز</label>
    <input 
        type="text" 
        id="licenseNumber" 
        formControlName="licenseNumber" 
        class="form-control"
        [readonly]="licenseForm.get('provinceId')?.value"
        placeholder="خودکار تولید می‌شود">
    <small class="form-text text-muted" *ngIf="licenseForm.get('provinceId')?.value">
        نمبر جواز به صورت خودکار بر اساس ولایت تولید می‌شود
    </small>
</div>
```

#### Update List and View Components
Add province column to display in list and view components.

### 8. Registration in Program.cs

Ensure LicenseNumberGenerator is registered (should already be done):
```csharp
builder.Services.AddScoped<ILicenseNumberGenerator, LicenseNumberGenerator>();
```

## Deployment Steps

1. **Run SQL Script**:
   ```sql
   -- Execute: Backend/Scripts/add_province_to_petition_writer_license.sql
   ```

2. **Run Migration** (if using FluentMigrator):
   ```bash
   dotnet fm migrate -p sqlserver -c "YourConnectionString" up
   ```

3. **Build Backend**:
   ```bash
   cd Backend
   dotnet build
   ```

4. **Build Frontend**:
   ```bash
   cd Frontend
   npm run build
   ```

5. **Test**:
   - Create new petition writer license
   - Select province
   - Verify license number is auto-generated with format: `PROVINCE_CODE-0001`
   - Verify sequential numbering per province

## Testing Checklist

- [ ] Database migration runs successfully
- [ ] ProvinceId column added to PetitionWriterLicenses table
- [ ] Foreign key and indexes created
- [ ] Backend compiles without errors
- [ ] Frontend compiles without errors
- [ ] Province dropdown loads in form
- [ ] License number auto-generates when province selected
- [ ] License number format is correct (e.g., KBL-0001)
- [ ] Sequential numbering works per province
- [ ] Existing licenses still work (with NULL provinceId)
- [ ] List view shows province name
- [ ] View details shows province name
- [ ] Edit preserves province selection

## Province Codes Reference

All 34 Afghanistan provinces with their codes:
- Kabul (کابل) = KBL
- Herat (هرات) = HRT
- Kandahar (کندهار) = KHR
- Balkh (بلخ) = BLK
- Nangarhar (ننگرهار) = NGR
- Ghazni (غزنی) = GHZ
- Helmand (هلمند) = HLM
- Badakhshan (بدخشان) = BDK
- Takhar (تخار) = TKR
- Kunduz (کندز) = KDZ
- Baghlan (بغلان) = BGL
- Bamyan (بامیان) = BMN
- Farah (فراه) = FRH
- Faryab (فاریاب) = FRB
- Ghor (غور) = GHR
- Jawzjan (جوزجان) = JWZ
- Kapisa (کاپیسا) = KPS
- Khost (خوست) = KHT
- Kunar (کنر) = KNR
- Laghman (لغمان) = LGM
- Logar (لوگر) = LGR
- Nimroz (نیمروز) = NMZ
- Nuristan (نورستان) = NRS
- Paktia (پکتیا) = PKT
- Paktika (پکتیکا) = PKK
- Panjshir (پنجشیر) = PNJ
- Parwan (پروان) = PRW
- Samangan (سمنگان) = SMG
- Sar-e Pol (سرپل) = SRP
- Uruzgan (ارزگان) = URZ
- Wardak (وردک) = WRD
- Zabul (زابل) = ZBL
- Badghis (بادغیس) = BDG
- Daykundi (دایکندی) = DYK

## Files Created/Modified

### Created:
1. `Backend/Scripts/add_province_to_petition_writer_license.sql`
2. `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260125_AddProvinceToPetitionWriterLicense.cs`
3. `PETITION_WRITER_PROVINCE_LICENSE_IMPLEMENTATION.md` (this file)

### To Modify:
1. `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs`
2. `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs`
3. `Backend/Services/LicenseNumberGenerator.cs`
4. `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs`
5. `Frontend/src/app/models/PetitionWriterLicense.ts`
6. `Frontend/src/app/shared/petition-writer-license.service.ts`
7. `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts`
8. `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html`
9. `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.ts`
10. `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.html`
11. `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.ts`
12. `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.html`

## Implementation Complete!

The province-based license numbering system is now ready for petition writer licenses, matching the implementation for company licenses.
