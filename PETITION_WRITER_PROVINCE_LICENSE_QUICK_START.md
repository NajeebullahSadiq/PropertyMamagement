# Quick Start: Province-Based License Numbering for Petition Writer Licenses

## What Was Implemented

Province-based license numbering system for petition writer licenses (ثبت جواز عریضه‌نویسان) with format: `PROVINCE_CODE-SEQUENTIAL_NUMBER`

Examples:
- Kabul: `KBL-0001`, `KBL-0002`
- Kandahar: `KHR-0001`, `KHR-0002`
- Herat: `HRT-0001`

## Backend Changes Complete ✅

### 1. Database
- ✅ SQL script created: `Backend/Scripts/add_province_to_petition_writer_license.sql`
- ✅ Migration created: `Backend/Infrastructure/Migrations/PetitionWriterLicense/20260125_AddProvinceToPetitionWriterLicense.cs`

### 2. Models
- ✅ Added `ProvinceId` to `PetitionWriterLicenseEntity`
- ✅ Added `Province` navigation property
- ✅ Added `ProvinceId` to `PetitionWriterLicenseData`

### 3. Service
- ✅ Added `GenerateNextPetitionWriterLicenseNumber()` method to `LicenseNumberGenerator`
- ✅ Updated interface `ILicenseNumberGenerator`

### 4. Controller
- ✅ Injected `ILicenseNumberGenerator` in constructor
- ✅ Auto-generates license number when `ProvinceId` is provided
- ✅ Added `/api/PetitionWriterLicense/provinces` endpoint
- ✅ Updated GetAll and GetById to include Province data

## Frontend Changes Needed

### Files to Update:

1. **Model** - `Frontend/src/app/models/PetitionWriterLicense.ts`
   ```typescript
   export interface PetitionWriterLicense {
       id?: number;
       licenseNumber: string;
       provinceId?: number;  // ADD THIS
       provinceName?: string;  // ADD THIS
       applicantName: string;
       // ... rest of fields
   }
   ```

2. **Service** - `Frontend/src/app/shared/petition-writer-license.service.ts`
   ```typescript
   getProvinces(): Observable<any> {
       return this.http.get(`${this.apiUrl}/provinces`);
   }
   ```

3. **Form Component TS** - `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts`
   
   Add properties:
   ```typescript
   provinces: any[] = [];
   ```
   
   Load provinces in ngOnInit:
   ```typescript
   this.loadProvinces();
   ```
   
   Add method:
   ```typescript
   loadProvinces(): void {
       this.sellerService.getprovince().subscribe({
           next: (data: any) => {
               this.provinces = data;
           },
           error: (err: any) => console.error('Error loading provinces', err)
       });
   }
   ```
   
   Update form initialization:
   ```typescript
   this.licenseForm = this.fb.group({
       licenseNumber: [''],  // Optional - auto-generated
       provinceId: [null, Validators.required],  // ADD THIS
       applicantName: ['', Validators.required],
       // ... rest
   });
   ```
   
   Add method to handle province change:
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

4. **Form Component HTML** - `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html`
   
   Add before license number field:
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
       <div *ngIf="provinceId?.invalid && provinceId?.touched" class="text-danger">
           ولایت الزامی است
       </div>
   </div>

   <!-- License Number (Auto-generated) -->
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

5. **List Component** - Add province column to display

6. **View Component** - Add province field to display

## Deployment Steps

### 1. Run Database Migration

**Option A: SQL Script**
```sql
-- Execute in SQL Server Management Studio or Azure Data Studio
USE PRMIS;
GO
-- Run: Backend/Scripts/add_province_to_petition_writer_license.sql
```

**Option B: FluentMigrator** (if configured)
```bash
dotnet fm migrate -p sqlserver -c "YourConnectionString" up
```

### 2. Build and Run Backend
```bash
cd Backend
dotnet build
dotnet run
```

### 3. Update Frontend (after making the changes above)
```bash
cd Frontend
npm install
ng serve
```

### 4. Test the Feature

1. Navigate to Petition Writer License form
2. Select a province from dropdown
3. License number field should become readonly
4. Save the form
5. Verify license number is auto-generated (e.g., `KBL-0001`)
6. Create another license in same province
7. Verify sequential numbering (e.g., `KBL-0002`)
8. Create license in different province
9. Verify it starts from 0001 (e.g., `HRT-0001`)

## Testing Checklist

- [ ] Database migration runs successfully
- [ ] Backend compiles without errors
- [ ] `/api/PetitionWriterLicense/provinces` endpoint returns provinces
- [ ] Frontend compiles after changes
- [ ] Province dropdown loads in form
- [ ] License number becomes readonly when province selected
- [ ] License number auto-generates with correct format
- [ ] Sequential numbering works per province
- [ ] List view shows province name
- [ ] View details shows province name
- [ ] Edit preserves province selection

## Province Codes

All 34 Afghanistan provinces:
- Kabul = KBL, Herat = HRT, Kandahar = KHR, Balkh = BLK
- Nangarhar = NGR, Ghazni = GHZ, Helmand = HLM, Badakhshan = BDK
- Takhar = TKR, Kunduz = KDZ, Baghlan = BGL, Bamyan = BMN
- Farah = FRH, Faryab = FRB, Ghor = GHR, Jawzjan = JWZ
- Kapisa = KPS, Khost = KHT, Kunar = KNR, Laghman = LGM
- Logar = LGR, Nimroz = NMZ, Nuristan = NRS, Paktia = PKT
- Paktika = PKK, Panjshir = PNJ, Parwan = PRW, Samangan = SMG
- Sar-e Pol = SRP, Uruzgan = URZ, Wardak = WRD, Zabul = ZBL
- Badghis = BDG, Daykundi = DYK

## Files Modified

### Backend (Complete ✅):
1. ✅ `Backend/Models/PetitionWriterLicense/PetitionWriterLicense.cs`
2. ✅ `Backend/Models/RequestData/PetitionWriterLicense/PetitionWriterLicenseData.cs`
3. ✅ `Backend/Services/LicenseNumberGenerator.cs`
4. ✅ `Backend/Controllers/PetitionWriterLicense/PetitionWriterLicenseController.cs`

### Frontend (To Do):
1. ⏳ `Frontend/src/app/models/PetitionWriterLicense.ts`
2. ⏳ `Frontend/src/app/shared/petition-writer-license.service.ts`
3. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.ts`
4. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-form/petition-writer-license-form.component.html`
5. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.ts`
6. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-list/petition-writer-license-list.component.html`
7. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.ts`
8. ⏳ `Frontend/src/app/petition-writer-license/petition-writer-license-view/petition-writer-license-view.component.html`

## Summary

✅ **Backend is complete and ready to use!**
⏳ **Frontend changes needed** - Follow the code snippets above to update the frontend components.

The system will auto-generate license numbers in the format `PROVINCE_CODE-SEQUENTIAL_NUMBER` when a province is selected, just like the company license system.
