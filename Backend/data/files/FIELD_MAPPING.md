# CORRECT FIELD MAPPING - Old Database to New Database

## Understanding the Data Structure

### OLD DATABASE (Access - MainForm table)
**Structure:** Single flat table with 50 columns  
**Records:** 7,329 records  
**Relationship:** 1 record = 1 company + 1 owner + 1 license  

### NEW DATABASE (PostgreSQL - Normalized)
**Structure:** Multiple related tables  
**Schemas:** org, log, look  
**Relationship:** Proper foreign keys and normalization  

---

## CRITICAL UNDERSTANDING

### 1. Company vs Owner
```
❌ WRONG: RealEstateName = Owner's name
✅ CORRECT: RealEstateName = Company/Office name

Example from data:
- RealEstateName: "دارالامان" (Company name)
- FName: "سید قاسم" (Owner's first name)
- FathName: "سید موسی" (Owner's father name)
- GFName: "سید طالب" (Owner's grandfather name)
```

### 2. Province Fields - THREE DIFFERENT MEANINGS

```
1. Owner's Permanent Address (PerProvince/PerWoloswaly)
   - Where the OWNER is originally FROM
   - Owner's home province/district
   - Maps to: CompanyOwners.OwnerProvinceId, OwnerDistrictId

2. Owner's Current Address (TempProvince/TempWoloswaly)
   - Where the OWNER currently LIVES
   - Current residence
   - Maps to: CompanyOwners.PermanentProvinceId, PermanentDistrictId

3. License Registration Province
   - Where the LICENSE was ISSUED
   - ✅ ALWAYS KABUL for all 7,329 records
   - Maps to: CompanyDetails.ProvinceId, LicenseDetails.ProvinceId
```

### 3. Status Fields
```
Halat = Overall status (جدید = new, فسخ = cancelled)
LicnsCancelNo = Cancellation letter number (if cancelled)

✅ CORRECT Logic:
- If LicnsCancelNo is not empty OR Halat contains "فسخ" → Status = FALSE (Inactive)
- Otherwise → Status = TRUE (Active)
```

---

## COMPLETE FIELD MAPPING

### 📋 Table 1: org.companydetails (7,329 records)

| New Field | Old Field | Type | Notes |
|-----------|-----------|------|-------|
| Id | RID | Integer | Direct copy (Primary Key) |
| Title | RealEstateName | String | Company/Office name (e.g., "دارالامان") |
| TIN | TIN | String | Tax Identification Number |
| **ProvinceId** | **"کابل" (hardcoded)** | **FK** | **ALL companies registered in Kabul** |
| DocPath | NULL | String | Not in old data |
| Status | Halat + LicnsCancelNo | Boolean | FALSE if contains "فسخ" or cancelled |
| CreatedAt | NOW() | Timestamp | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String | Migration marker |

**Key Points:**
- ✅ ProvinceId is ALWAYS Kabul for ALL 7,329 records
- ❌ Do NOT use PerProvince for CompanyDetails.ProvinceId
- Title is the COMPANY name, not owner name

---

### 👤 Table 2: org.companyowners (7,326 records)

| New Field | Old Field | Type | Notes |
|-----------|-----------|------|-------|
| Id | Auto-generated | Serial | Auto-increment |
| FirstName | FName | String | Owner's first name |
| FatherName | FathName | String | Owner's father name |
| GrandFatherName | GFName | String | Owner's grandfather name |
| EducationLevelId | Education | FK | Lookup to educationlevel table |
| DateofBirth | DOB | String/Date | Birth date (may need parsing) |
| ElectronicNationalIdNumber | TazkeraNo | String | National ID number |
| PhoneNumber | ContactNo | String | Phone number |
| WhatsAppNumber | NULL | String | Not in old data |
| CompanyId | RID | FK | References companydetails.id |
| PothoPath | NULL | String | Not in old data |
| **OWNER'S HOME ADDRESS** | | | |
| OwnerProvinceId | PerProvince | FK | Where owner is FROM |
| OwnerDistrictId | PerWoloswaly | FK | Owner's home district |
| OwnerVillage | ExactAddress | String | Detailed home address |
| **OWNER'S CURRENT ADDRESS** | | | |
| PermanentProvinceId | TempProvince | FK | Where owner currently LIVES |
| PermanentDistrictId | TempWoloswaly | FK | Current residence district |
| PermanentVillage | ExactAddress | String | Current address (same as above) |
| Status | Halat + LicnsCancelNo | Boolean | Active/Inactive |
| CreatedAt | NOW() | Timestamp | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String | Migration marker |

**Key Points:**
- ✅ OwnerProvinceId = PerProvince (owner's HOME province, NOT license province)
- ✅ PermanentProvinceId = TempProvince (owner's CURRENT province)
- ❌ These are NOT the same as the company registration province (Kabul)
- In the old data, ExactAddress is used for both home and current village

---

### 📜 Table 3: org.licensedetails (7,329 records)

| New Field | Old Field | Type | Notes |
|-----------|-----------|------|-------|
| Id | Auto-generated | Serial | Auto-increment |
| LicenseNumber | LicenseNo | String | Convert number to string |
| **ProvinceId** | **"کابل" (hardcoded)** | **FK** | **ALL licenses issued in Kabul** |
| IssueDate | SYear + SMonth + SDay | Date | Combine into YYYY-MM-DD |
| ExpireDate | EYear + EMonth + EDay | Date | Combine into YYYY-MM-DD |
| AreaId | DistLocation | FK | May need lookup |
| OfficeAddress | ExactAddress | String | Office location |
| CompanyId | RID | FK | References companydetails.id |
| DocPath | NULL | String | Not in old data |
| LicenseType | LicenseType | String | "جدید" or "تجدید" |
| LicenseCategory | LicenseType | String | Same as LicenseType |
| RenewalRound | NULL | Integer | Not in old data |
| **FINANCIAL INFORMATION** | | | |
| RoyaltyAmount | CreditRightAmount | Decimal | License fee amount |
| RoyaltyDate | CreditRightYear + Month + Day | Date | Combine into date |
| TariffNumber | CreditRightNo | String | Tariff reference (e.g., "1/4089") |
| PenaltyAmount | LateFine | Decimal | Late payment penalty |
| PenaltyDate | NULL | Date | Not in old data |
| **HR INFORMATION** | | | |
| HrLetter | HRNo | String | HR letter number |
| HrLetterDate | Combo211 + HRMonth + HRDay | Date | HR letter date |
| IsComplete | TRUE | Boolean | All migrated records complete |
| Status | Halat + LicnsCancelNo | Boolean | Active/Inactive |
| CreatedAt | NOW() | Timestamp | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String | Migration marker |

**Key Points:**
- ✅ ProvinceId is ALWAYS Kabul (license issuing province)
- ✅ OfficeAddress = ExactAddress (office location, not owner's home)
- Dates are combined from separate year/month/day fields
- TariffNumber stores the CreditRightNo value

---

### ❌ Table 4: org.companycancellationinfo (~2,169 records)

| New Field | Old Field | Type | Notes |
|-----------|-----------|------|-------|
| Id | Auto-generated | Serial | Auto-increment |
| CompanyId | RID | FK | References companydetails.id |
| LicenseCancellationLetterNumber | LicnsCancelNo | String | Cancellation letter number |
| RevenueCancellationLetterNumber | NULL | String | Not in old data |
| LicenseCancellationLetterDate | CancelYear + Month + Day | Date | Combine into date |
| Remarks | Remarks | String | Cancellation notes |
| Status | TRUE | Boolean | Active cancellation record |
| CreatedAt | NOW() | Timestamp | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String | Migration marker |

**Key Points:**
- Only created if LicnsCancelNo is not empty
- ~29.6% of records have cancellation info

---

## LOOKUP TABLES

### 📍 look.location (Provinces & Districts)

**Provinces (35 unique):**
- Auto-created from PerProvince and TempProvince fields
- Type = 'province'
- Example: کابل, پروان, وردک, etc.

**Districts (741 unique):**
- Auto-created from PerWoloswaly and TempWoloswaly fields
- Type = 'district'
- Parent_id = province id
- Example: شاروالی, پغمان, دهسبز, etc.

**IMPORTANT:** These are OWNER addresses, not company/license locations!

### 🎓 look.educationlevel (3 unique)

Auto-created from Education field:
- بکلوریا (Bachelor) - 7,308 records
- لیسانس (License) - 13 records
- چهارده پاس (14th Grade) - 8 records

---

## DATE FIELD COMBINATIONS

### License Issue Date
```sql
IssueDate = CONCAT(SYear, '-', LPAD(SMonth, 2, '0'), '-', LPAD(SDay, 2, '0'))
Example: 1394-09-09
```

### License Expire Date
```sql
ExpireDate = CONCAT(EYear, '-', LPAD(EMonth, 2, '0'), '-', LPAD(EDay, 2, '0'))
Example: 1397-09-09
```

### Royalty Payment Date
```sql
RoyaltyDate = CONCAT(CreditRightYear, '-', LPAD(CreditRightMonth, 2, '0'), '-', LPAD(CreditRightDay, 2, '0'))
Example: 1394-09-09
```

### HR Letter Date
```sql
HrLetterDate = CONCAT(Combo211, '-', LPAD(HRMonth, 2, '0'), '-', LPAD(HRDay, 2, '0'))
Example: If available
```

### Cancellation Date
```sql
LicenseCancellationLetterDate = CONCAT(CancelYear, '-', LPAD(CancelMonth, 2, '0'), '-', LPAD(CancelDay, 2, '0'))
Example: If cancelled
```

**Note:** All dates are in Jalali (Solar Hijri) format

---

## FIELDS NOT MIGRATED

These old fields are NOT used in the new structure:

| Old Field | Reason |
|-----------|--------|
| UserDisplay | Not needed |
| Page | Old tazkera page (replaced by ElectronicNationalIdNumber) |
| Jold | Old tazkera volume (replaced by ElectronicNationalIdNumber) |
| EmailID | Not populated in old data |
| Combo211 | Used only for HR date year |
| RecordBook | Historical fee (not in new structure) |
| FK | Old foreign key (not needed) |
| SearchFor | UI field (not data) |
| SearchResults | UI field (not data) |
| Text34 | Unknown purpose |
| CancelAmount | Always NULL in old data |

---

## VALIDATION RULES

### After Migration, Verify:

```sql
-- 1. All companies should be in Kabul
SELECT DISTINCT l.name 
FROM org.companydetails cd
JOIN look.location l ON l.id = cd.provinceid;
-- Should return ONLY: کابل

-- 2. All licenses should be in Kabul
SELECT DISTINCT l.name 
FROM org.licensedetails ld
JOIN look.location l ON l.id = ld.provinceid;
-- Should return ONLY: کابل

-- 3. Owners should be from various provinces
SELECT l.name as province, COUNT(*) as count
FROM org.companyowners co
JOIN look.location l ON l.id = co.ownerprovinceid
GROUP BY l.name
ORDER BY count DESC;
-- Should show distribution across many provinces

-- 4. Check one-to-one relationships
SELECT 
    (SELECT COUNT(*) FROM org.companydetails) as companies,
    (SELECT COUNT(*) FROM org.companyowners) as owners,
    (SELECT COUNT(*) FROM org.licensedetails) as licenses;
-- Should be approximately equal (7329, 7326, 7329)
```

---

## EXAMPLE TRANSFORMATION

### Old Record (RID = 1):
```
RID: 1
RealEstateName: دارالامان
FName: سید قاسم
FathName: سید موسی
GFName: سید طالب
Education: چهارده پاس
PerProvince: کابل
PerWoloswaly: شاروالی
TempProvince: کابل
TempWoloswaly: شاروالی
ExactAddress: منطقه علی مردان
LicenseNo: 1
LicenseType: جدید
SYear: 1394, SMonth: 9, SDay: 9
EYear: 1397, EMonth: 9, EDay: 9
CreditRightAmount: 15000
LicnsCancelNo: فسخ شده
```

### New Records:

**org.companydetails:**
```
Id: 1
Title: دارالامان
TIN: NULL
ProvinceId: [Kabul ID] ← Hardcoded Kabul
Status: FALSE ← Because cancelled
```

**org.companyowners:**
```
Id: [Auto]
FirstName: سید قاسم
FatherName: سید موسی
GrandFatherName: سید طالب
EducationLevelId: [14th Grade ID]
CompanyId: 1
OwnerProvinceId: [Kabul ID] ← From PerProvince
OwnerDistrictId: [Municipality ID] ← From PerWoloswaly
OwnerVillage: منطقه علی مردان
PermanentProvinceId: [Kabul ID] ← From TempProvince
PermanentDistrictId: [Municipality ID] ← From TempWoloswaly
PermanentVillage: منطقه علی مردان
Status: FALSE
```

**org.licensedetails:**
```
Id: [Auto]
LicenseNumber: "1"
ProvinceId: [Kabul ID] ← Hardcoded Kabul
IssueDate: 1394-09-09
ExpireDate: 1397-09-09
OfficeAddress: منطقه علی مردان
CompanyId: 1
LicenseType: جدید
LicenseCategory: جدید
RoyaltyAmount: 15000.00
Status: FALSE
```

**org.companycancellationinfo:**
```
Id: [Auto]
CompanyId: 1
LicenseCancellationLetterNumber: فسخ شده
Remarks: [From Remarks field]
Status: TRUE
```

---

## SUMMARY

✅ **CompanyDetails.ProvinceId = Kabul** (ALL 7,329 records)  
✅ **LicenseDetails.ProvinceId = Kabul** (ALL 7,329 records)  
✅ **CompanyOwners.OwnerProvinceId = PerProvince** (Owner's home - varies)  
✅ **CompanyOwners.PermanentProvinceId = TempProvince** (Owner's current - varies)  

This correctly reflects:
- Companies are registered in Kabul
- Licenses are issued in Kabul  
- Owners are from various provinces across Afghanistan
