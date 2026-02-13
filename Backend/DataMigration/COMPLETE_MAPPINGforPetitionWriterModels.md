# COMPLETE DATA MIGRATION MAPPING
## Old Access Database → New PostgreSQL (Both Modules)

---

## Migration Strategy

The old MainForm database contains **ISSUED LICENSES** (already approved and active). We migrate this data to TWO separate modules in the new system:

### Module 1: Company Module (Primary)
Stores the actual company, owner, and issued license details.

### Module 2: LicenseApplications Module (Historical)
Stores these as "approved applications" for historical records.

---

## MODULE 1: COMPANY MODULE

### Table 1: org.companydetails

| New Field | Old Field | Value/Transformation | Notes |
|-----------|-----------|---------------------|-------|
| Id | RID | Direct copy | Primary key |
| Title | RealEstateName | Direct copy | Company/Office name |
| TIN | TIN | Direct copy | Tax ID |
| **ProvinceId** | **"کابل" (hardcoded)** | **Lookup: "کابل"** | **ALL in Kabul** |
| DocPath | NULL | NULL | Not in old data |
| Status | Halat + LicnsCancelNo | Check cancellation | FALSE if cancelled |
| CreatedAt | NOW() | `DateTime.Now` | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String literal | Migration marker |

**Format:** CreatedAt stored as: `2026-02-03 21:26:09.450259` (PostgreSQL timestamp)

---

### Table 2: org.companyowners

| New Field | Old Field | Value/Transformation | Notes |
|-----------|-----------|---------------------|-------|
| Id | Auto | Serial | Auto-increment |
| FirstName | FName | Direct copy | Owner's first name |
| FatherName | FathName | Direct copy | Owner's father name |
| GrandFatherName | GFName | Direct copy | Owner's grandfather |
| EducationLevelId | Education | Lookup to educationlevel | FK |
| DateofBirth | DOB | Direct copy | Birth date string |
| ElectronicNationalIdNumber | TazkeraNo | Direct copy | National ID |
| PhoneNumber | ContactNo | Direct copy | Phone number |
| WhatsAppNumber | NULL | NULL | Not in old data |
| CompanyId | RID | From CompanyDetails.Id | FK |
| PothoPath | NULL | NULL | Not in old data |
| **OwnerProvinceId** | PerProvince | Lookup | **Owner's HOME** |
| **OwnerDistrictId** | PerWoloswaly | Lookup | **Owner's HOME** |
| OwnerVillage | ExactAddress | Direct copy | Home village |
| **PermanentProvinceId** | TempProvince | Lookup | **Owner's CURRENT** |
| **PermanentDistrictId** | TempWoloswaly | Lookup | **Owner's CURRENT** |
| PermanentVillage | ExactAddress | Direct copy | Current village |
| Status | Halat + LicnsCancelNo | Check cancellation | Active/Inactive |
| CreatedAt | NOW() | `DateTime.Now` | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String literal | Migration marker |

---

### Table 3: org.licensedetails

| New Field | Old Field | Value/Transformation | Notes |
|-----------|-----------|---------------------|-------|
| Id | Auto | Serial | Auto-increment |
| LicenseNumber | LicenseNo | `LicenseNo.ToString()` | Convert to string |
| **ProvinceId** | **"کابل" (hardcoded)** | **Lookup: "کابل"** | **ALL in Kabul** |
| IssueDate | SYear/SMonth/SDay | `YYYY-MM-DD` format | Combine date parts |
| ExpireDate | EYear/EMonth/EDay | `YYYY-MM-DD` format | Combine date parts |
| AreaId | DistLocation | May need lookup | Area reference |
| OfficeAddress | ExactAddress | Direct copy | Office location |
| CompanyId | RID | From CompanyDetails.Id | FK |
| DocPath | NULL | NULL | Not in old data |
| LicenseType | LicenseType | Direct copy | "جدید" or "تجدید" |
| LicenseCategory | LicenseType | Same as LicenseType | Same value |
| RenewalRound | NULL | NULL | Not in old data |
| RoyaltyAmount | CreditRightAmount | Direct copy | License fee |
| RoyaltyDate | CreditRightYear/Month/Day | `YYYY-MM-DD` format | Combine date parts |
| TariffNumber | CreditRightNo | Direct copy | e.g., "1/4089" |
| PenaltyAmount | LateFine | Direct copy | Penalty |
| PenaltyDate | NULL | NULL | Not in old data |
| HrLetter | HRNo | `HRNo.ToString()` | HR reference |
| HrLetterDate | Combo211/HRMonth/HRDay | `YYYY-MM-DD` format | Combine if available |
| IsComplete | TRUE | Boolean literal | All complete |
| Status | Halat + LicnsCancelNo | Check cancellation | Active/Inactive |
| CreatedAt | NOW() | `DateTime.Now` | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String literal | Migration marker |

**Date Format:** All dates stored as Jalali strings: `1394-09-09`

---

### Table 4: org.companycancellationinfo

| New Field | Old Field | Value/Transformation | Notes |
|-----------|-----------|---------------------|-------|
| Id | Auto | Serial | Auto-increment |
| CompanyId | RID | From CompanyDetails.Id | FK |
| LicenseCancellationLetterNumber | LicnsCancelNo | Direct copy | Letter number |
| RevenueCancellationLetterNumber | NULL | NULL | Not in old data |
| LicenseCancellationLetterDate | CancelYear/Month/Day | `YYYY-MM-DD` format | Combine date parts |
| Remarks | Remarks | Direct copy | Cancellation notes |
| Status | TRUE | Boolean literal | Active record |
| CreatedAt | NOW() | `DateTime.Now` | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String literal | Migration marker |

**Only created if:** `LicnsCancelNo` is not empty (~29.6% of records)

---

## MODULE 2: LICENSE APPLICATIONS MODULE

### Table 5: org.licenseapplications

| New Field | Old Field | Value/Transformation | Notes |
|-----------|-----------|---------------------|-------|
| Id | Auto | Serial | Auto-increment (different from RID) |
| RequestDate | SYear/SMonth/SDay | `YYYY-MM-DD` format | Application date = Issue date |
| **RequestSerialNumber** | **LicenseNo** | **`LicenseNo.ToString()`** | **Unique** |
| **ApplicantName** | **FName + FathName + GFName** | **Concatenate** | **Full name** |
| **ProposedGuideName** | **RealEstateName** | **Direct copy** | **Company name** |
| **PermanentProvinceId** | **PerProvince** | **Lookup** | **Applicant's HOME** |
| **PermanentDistrictId** | **PerWoloswaly** | **Lookup** | **Applicant's HOME** |
| PermanentVillage | ExactAddress | Direct copy | Home village |
| **CurrentProvinceId** | **TempProvince** | **Lookup** | **Applicant's CURRENT** |
| **CurrentDistrictId** | **TempWoloswaly** | **Lookup** | **Applicant's CURRENT** |
| CurrentVillage | ExactAddress | Direct copy | Current village |
| Status | TRUE | Boolean literal | All were approved |
| **IsWithdrawn** | **LicnsCancelNo** | **Check if not empty** | **TRUE if cancelled** |
| CreatedAt | NOW() | `DateTime.Now` | Migration timestamp |
| CreatedBy | "MIGRATION_SCRIPT" | String literal | Migration marker |
| UpdatedAt | NULL | NULL | Not needed for migration |
| UpdatedBy | NULL | NULL | Not needed for migration |

**Key Differences from Company Module:**
- `ApplicantName` = Full name (concatenated)
- `ProposedGuideName` = Company name (what they proposed)
- Province/District = Applicant's address (not license issuing location)
- `RequestSerialNumber` = License number (as reference to approved application)
- `IsWithdrawn` = TRUE if later cancelled

---

### Table 6: org.licenseapplicationguarantors

**NOT MIGRATED** - No guarantor data exists in old system.

This table will remain empty after migration. Future applications will use it.

---

### Table 7: org.licenseapplicationwithdrawals

**NOT MIGRATED** - No withdrawal data exists in old system.

This table will remain empty after migration. Future withdrawals will use it.

---

## CRITICAL DIFFERENCES TO UNDERSTAND

### Province Fields - THREE DIFFERENT CONTEXTS

```
1. COMPANY/LICENSE REGISTRATION:
   ├── CompanyDetails.ProvinceId = کابل (ALL 7,329 records)
   └── LicenseDetails.ProvinceId = کابل (ALL 7,329 records)
   
2. OWNER/APPLICANT HOME ADDRESS:
   ├── CompanyOwners.OwnerProvinceId = PerProvince (varies)
   ├── CompanyOwners.OwnerDistrictId = PerWoloswaly (varies)
   ├── LicenseApplications.PermanentProvinceId = PerProvince (varies)
   └── LicenseApplications.PermanentDistrictId = PerWoloswaly (varies)
   
3. OWNER/APPLICANT CURRENT ADDRESS:
   ├── CompanyOwners.PermanentProvinceId = TempProvince (varies)
   ├── CompanyOwners.PermanentDistrictId = TempWoloswaly (varies)
   ├── LicenseApplications.CurrentProvinceId = TempProvince (varies)
   └── LicenseApplications.CurrentDistrictId = TempWoloswaly (varies)
```

---

## DATE HANDLING

### Source Format (Old Database)
```
Separate fields:
- SYear: 1394.0
- SMonth: 9.0
- SDay: 9.0
```

### Target Format (New Database)
```
Combined string in Jalali format:
- Date: "1394-09-09"
```

### C# Code for Conversion
```csharp
string CreateDateString(double? year, double? month, double? day)
{
    if (!year.HasValue || !month.HasValue || !day.HasValue)
        return null;
    
    int y = (int)year.Value;
    int m = (int)month.Value;
    int d = (int)day.Value;
    
    return $"{y:0000}-{m:00}-{d:00}";
}
```

---

## TIMESTAMP FORMAT

### PostgreSQL Timestamp Format
```
2026-02-03 21:26:09.450259
```

### C# DateTime.Now
```csharp
CreatedAt = DateTime.Now
```

This automatically formats to PostgreSQL timestamp format when using Npgsql.

---

## NAME CONCATENATION

### For LicenseApplications.ApplicantName
```csharp
string applicantName = $"{record.FName} {record.FathName}";
if (!string.IsNullOrWhiteSpace(record.GFName))
    applicantName += $" {record.GFName}";

// Example result: "سید قاسم سید موسی سید طالب"
```

---

## STATUS LOGIC

### Active/Inactive Determination
```csharp
bool GetActiveStatus(string halatText, string cancelText)
{
    // Check if cancelled
    if (!string.IsNullOrWhiteSpace(cancelText) && 
        (cancelText.Contains("فسخ") || cancelText != ""))
        return false;
    
    // Check halat field
    if (!string.IsNullOrWhiteSpace(halatText) && halatText.Contains("فسخ"))
        return false;
        
    return true;
}
```

### IsWithdrawn Logic (LicenseApplications)
```csharp
bool isWithdrawn = !string.IsNullOrWhiteSpace(record.LicnsCancelNo);
```

---

## COMPLETE EXAMPLE TRANSFORMATION

### Old Record (RID = 1)
```json
{
  "RID": 1,
  "RealEstateName": "دارالامان",
  "FName": "سید قاسم",
  "FathName": "سید موسی",
  "GFName": "سید طالب",
  "Education": "چهارده پاس",
  "LicenseNo": 1,
  "LicenseType": "جدید",
  "PerProvince": "کابل",
  "PerWoloswaly": "شاروالی",
  "TempProvince": "کابل",
  "TempWoloswaly": "شاروالی",
  "ExactAddress": "منطقه علی مردان",
  "SYear": 1394,
  "SMonth": 9,
  "SDay": 9,
  "EYear": 1397,
  "EMonth": 9,
  "EDay": 9,
  "CreditRightAmount": 15000,
  "LicnsCancelNo": "فسخ شده"
}
```

### New Records Created

#### 1. org.companydetails
```sql
Id: 1
Title: 'دارالامان'
TIN: NULL
ProvinceId: [Kabul ID]
Status: FALSE
CreatedAt: '2026-02-03 21:26:09.450259'
CreatedBy: 'MIGRATION_SCRIPT'
```

#### 2. org.companyowners
```sql
Id: [Auto]
FirstName: 'سید قاسم'
FatherName: 'سید موسی'
GrandFatherName: 'سید طالب'
EducationLevelId: [14th Grade ID]
CompanyId: 1
OwnerProvinceId: [Kabul ID from PerProvince]
OwnerDistrictId: [Municipality ID from PerWoloswaly]
OwnerVillage: 'منطقه علی مردان'
PermanentProvinceId: [Kabul ID from TempProvince]
PermanentDistrictId: [Municipality ID from TempWoloswaly]
PermanentVillage: 'منطقه علی مردان'
Status: FALSE
CreatedAt: '2026-02-03 21:26:09.450259'
CreatedBy: 'MIGRATION_SCRIPT'
```

#### 3. org.licensedetails
```sql
Id: [Auto]
LicenseNumber: '1'
ProvinceId: [Kabul ID - hardcoded]
IssueDate: '1394-09-09'
ExpireDate: '1397-09-09'
OfficeAddress: 'منطقه علی مردان'
CompanyId: 1
LicenseType: 'جدید'
LicenseCategory: 'جدید'
RoyaltyAmount: 15000.00
Status: FALSE
CreatedAt: '2026-02-03 21:26:09.450259'
CreatedBy: 'MIGRATION_SCRIPT'
```

#### 4. org.companycancellationinfo
```sql
Id: [Auto]
CompanyId: 1
LicenseCancellationLetterNumber: 'فسخ شده'
Remarks: [From Remarks field]
Status: TRUE
CreatedAt: '2026-02-03 21:26:09.450259'
CreatedBy: 'MIGRATION_SCRIPT'
```

#### 5. org.licenseapplications
```sql
Id: [Auto - different from CompanyId]
RequestDate: '1394-09-09'
RequestSerialNumber: '1'
ApplicantName: 'سید قاسم سید موسی سید طالب'
ProposedGuideName: 'دارالامان'
PermanentProvinceId: [Kabul ID from PerProvince]
PermanentDistrictId: [Municipality ID from PerWoloswaly]
PermanentVillage: 'منطقه علی مردان'
CurrentProvinceId: [Kabul ID from TempProvince]
CurrentDistrictId: [Municipality ID from TempWoloswaly]
CurrentVillage: 'منطقه علی مردان'
Status: TRUE
IsWithdrawn: TRUE
CreatedAt: '2026-02-03 21:26:09.450259'
CreatedBy: 'MIGRATION_SCRIPT'
```

---

## VALIDATION QUERIES

### After migration, verify data:

```sql
-- 1. Check Company Module counts
SELECT 
    'CompanyDetails' as table_name, COUNT(*) as count 
FROM org.companydetails
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org.companyowners
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org.licensedetails
UNION ALL
SELECT 'CompanyCancellationInfo', COUNT(*) FROM org.companycancellationinfo;
-- Expected: 7329, 7326, 7329, 2169

-- 2. Check LicenseApplications Module counts
SELECT 
    'LicenseApplications' as table_name, COUNT(*) as count 
FROM org.licenseapplications;
-- Expected: 7329

-- 3. Verify ALL companies in Kabul
SELECT DISTINCT l.name 
FROM org.companydetails cd
JOIN look.location l ON l.id = cd.provinceid;
-- Should return ONLY: کابل

-- 4. Verify ALL licenses in Kabul
SELECT DISTINCT l.name 
FROM org.licensedetails ld
JOIN look.location l ON l.id = ld.provinceid;
-- Should return ONLY: کابل

-- 5. Verify applicant addresses vary
SELECT l.name as province, COUNT(*) as count
FROM org.licenseapplications la
JOIN look.location l ON l.id = la.permanentprovinceid
GROUP BY l.name
ORDER BY count DESC;
-- Should show distribution across provinces

-- 6. Check withdrawn applications
SELECT COUNT(*) 
FROM org.licenseapplications 
WHERE iswithdrawn = true;
-- Expected: ~2169 (same as cancellations)

-- 7. Verify timestamp format
SELECT createdat 
FROM org.companydetails 
LIMIT 1;
-- Should show: 2026-02-03 21:26:09.450259
```

---

## SUMMARY

✅ **7,329 records** migrate to **5 tables** across **2 modules**:

**Company Module (Issued Licenses):**
- org.companydetails (7,329)
- org.companyowners (7,326)
- org.licensedetails (7,329)
- org.companycancellationinfo (2,169)

**LicenseApplications Module (Historical Applications):**
- org.licenseapplications (7,329)

**Key Points:**
- Company/License Province = کابل (ALL records)
- Applicant/Owner Province = Varies by person
- Timestamps use PostgreSQL format with microseconds
- Dates use Jalali calendar format (YYYY-MM-DD)
- All records marked as created by "MIGRATION_SCRIPT"
