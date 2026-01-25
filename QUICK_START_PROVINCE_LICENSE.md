# Quick Start: Province-Based License Numbering

## 🚀 Quick Implementation Guide

### Step 1: Deploy Database Changes (5 minutes)

```bash
cd Backend
psql -U postgres -d your_database_name -f Scripts/deploy_province_license_numbering.sql
```

**What this does:**
- Adds `ProvinceId` column to `LicenseDetails` table
- Creates indexes for performance
- Links licenses to provinces
- Populates existing licenses with province data (if possible)

### Step 2: Restart Backend (2 minutes)

```bash
cd Backend
dotnet build
dotnet run
```

**What's new:**
- `LicenseNumberGenerator` service auto-generates license numbers
- Format: `PROVINCE_CODE-XXXX` (e.g., `KBL-0001`)
- Controller automatically uses the service

### Step 3: Update Frontend (10 minutes)

#### A. Update License Detail Model

**File:** `Frontend/src/app/models/LicenseDetail.ts`

```typescript
export interface LicenseDetail {
  id?: number;
  licenseNumber?: string;
  provinceId?: number;  // ADD THIS LINE
  issueDate?: string;
  expireDate?: string;
  // ... other fields
}
```

#### B. Update License Details Component

**File:** `Frontend/src/app/realestate/licensedetails/licensedetails.component.ts`

```typescript
export class LicensedetailsComponent implements OnInit {
  provinces: any[] = [];  // ADD THIS
  
  ngOnInit() {
    this.loadProvinces();  // ADD THIS
    // ... existing code
  }
  
  // ADD THIS METHOD
  loadProvinces() {
    this.http.get<any[]>('http://your-api/api/Location/provinces')
      .subscribe(data => {
        this.provinces = data;
      });
  }
  
  // UPDATE YOUR FORM TO INCLUDE provinceId
  createForm() {
    this.licenseForm = this.fb.group({
      provinceId: ['', Validators.required],  // ADD THIS
      licenseNumber: [''],  // Make this read-only
      issueDate: ['', Validators.required],
      // ... other fields
    });
  }
}
```

#### C. Update License Details Template

**File:** `Frontend/src/app/realestate/licensedetails/licensedetails.component.html`

```html
<!-- ADD PROVINCE SELECTOR -->
<div class="form-group">
  <label>ولایت / Province</label>
  <select formControlName="provinceId" class="form-control">
    <option value="">انتخاب کنید / Select</option>
    <option *ngFor="let province of provinces" [value]="province.id">
      {{ province.dari }} - {{ province.name }}
    </option>
  </select>
</div>

<!-- MAKE LICENSE NUMBER READ-ONLY -->
<div class="form-group">
  <label>شماره جواز / License Number</label>
  <input 
    type="text" 
    formControlName="licenseNumber" 
    class="form-control" 
    readonly
    placeholder="خودکار تولید می‌شود / Auto-generated"
  />
</div>
```

### Step 4: Test (5 minutes)

1. **Open the license form**
2. **Select a province** (e.g., Kabul)
3. **Fill other required fields**
4. **Submit the form**
5. **Check the generated license number** (should be `KBL-0001`)

## 📋 Province Codes Reference

Quick reference for all 34 provinces:

```
KBL - Kabul       | HRT - Herat      | KHR - Kandahar
BLK - Balkh       | NGR - Nangarhar  | GHZ - Ghazni
HLM - Helmand     | BDK - Badakhshan | TKR - Takhar
KDZ - Kunduz      | BGL - Baghlan    | BMN - Bamyan
FRH - Farah       | FRB - Faryab     | GHR - Ghor
JWZ - Jawzjan     | KPS - Kapisa     | KHT - Khost
KNR - Kunar       | LGM - Laghman    | LGR - Logar
NMZ - Nimroz      | NRS - Nuristan   | PKT - Paktia
PKK - Paktika     | PNJ - Panjshir   | PRW - Parwan
SMG - Samangan    | SRP - Sar-e Pol  | URZ - Uruzgan
WRD - Wardak      | ZBL - Zabul      | BDG - Badghis
DYK - Daykundi
```

## 🔧 API Usage Examples

### Create License (Auto-Generate Number)

```bash
curl -X POST http://localhost:5000/api/LicenseDetail \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "companyId": 123,
    "provinceId": 1,
    "issueDate": "1403/10/05",
    "areaId": 2,
    "officeAddress": "کابل، ناحیه ۱۰",
    "licenseType": "رهنمای معاملات",
    "licenseCategory": "جدید",
    "calendarType": "hijriShamsi"
  }'
```

**Response:**
```json
{
  "id": 456,
  "licenseNumber": "KBL-0235"
}
```

### Query Licenses by Province

```sql
-- Get all Kabul licenses
SELECT * FROM org."LicenseDetails" 
WHERE "ProvinceId" = 1 
ORDER BY "LicenseNumber";

-- Get license count by province
SELECT 
    l."Name" as "Province",
    COUNT(*) as "LicenseCount"
FROM org."LicenseDetails" ld
JOIN look."Location" l ON ld."ProvinceId" = l."ID"
GROUP BY l."Name"
ORDER BY "LicenseCount" DESC;
```

## 🐛 Common Issues & Solutions

### Issue 1: "Province not found" error

**Cause:** ProvinceId doesn't exist in Location table

**Solution:**
```sql
-- Check if province exists
SELECT * FROM look."Location" WHERE "TypeId" = 2;

-- If missing, run the seeder
-- Or manually insert the province
```

### Issue 2: License number not auto-generating

**Cause:** ProvinceId not provided in request

**Solution:** Ensure frontend sends `provinceId` in the request body

### Issue 3: Duplicate license numbers

**Cause:** Race condition or missing indexes

**Solution:**
```sql
-- Verify indexes exist
SELECT * FROM pg_indexes 
WHERE tablename = 'LicenseDetails';

-- If missing, run the deployment script again
```

## 📊 Monitoring & Analytics

### Check License Distribution

```sql
-- License count by province
SELECT 
    l."Dari" as "ولایت",
    l."Name" as "Province",
    COUNT(ld."Id") as "تعداد جواز",
    MIN(ld."LicenseNumber") as "First License",
    MAX(ld."LicenseNumber") as "Last License"
FROM look."Location" l
LEFT JOIN org."LicenseDetails" ld ON l."ID" = ld."ProvinceId"
WHERE l."TypeId" = 2
GROUP BY l."ID", l."Dari", l."Name"
ORDER BY COUNT(ld."Id") DESC;
```

### Find Next Available Number

```sql
-- For Kabul (ProvinceId = 1)
SELECT 
    COALESCE(MAX(CAST(SPLIT_PART("LicenseNumber", '-', 2) AS INTEGER)), 0) + 1 
    as "NextNumber"
FROM org."LicenseDetails"
WHERE "LicenseNumber" LIKE 'KBL-%';
```

## 🎯 Best Practices

1. **Always select province first** - This ensures proper license number generation
2. **Don't manually edit license numbers** - Let the system auto-generate them
3. **Use province codes consistently** - Don't create custom codes
4. **Monitor for gaps** - Check for missing numbers in sequence
5. **Backup before migration** - Always backup database before running scripts

## 📚 Additional Resources

- Full Documentation: `PROVINCE_BASED_LICENSE_NUMBERING.md`
- Dari/Pashto Guide: `PROVINCE_LICENSE_SYSTEM_DARI.md`
- Deployment Script: `Backend/Scripts/deploy_province_license_numbering.sql`
- Service Code: `Backend/Services/LicenseNumberGenerator.cs`
- Migration: `Backend/Infrastructure/Migrations/Company/20260125_AddProvinceToLicenseDetails.cs`

## ✅ Checklist

- [ ] Database migration deployed
- [ ] Backend restarted
- [ ] Frontend model updated
- [ ] Frontend component updated
- [ ] Frontend template updated
- [ ] Tested with sample data
- [ ] Verified license numbers generate correctly
- [ ] Checked all 34 provinces work
- [ ] Documented for team
- [ ] Trained users

## 🆘 Need Help?

1. Check the full documentation
2. Review the deployment script output
3. Test with curl/Postman first
4. Check backend logs for errors
5. Verify database changes applied
6. Contact system administrator

---

**Quick Start Version:** 1.0  
**Last Updated:** January 25, 2026  
**Estimated Setup Time:** 20-30 minutes
