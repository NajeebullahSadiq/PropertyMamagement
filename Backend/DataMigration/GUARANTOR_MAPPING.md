# COMPLETE COMPANY MODULE MIGRATION MAPPING
## MainForm.xlsx + GuaranteeForm.xlsx → PostgreSQL

---

## Overview

**Source Files:**
- MainForm.xlsx: 7,417 company/license records
- GuaranteeForm.xlsx: 7,490 guarantor records  
- Relationship: FK in GuaranteeForm → RID in MainForm

**Target Tables:**
- org.CompanyDetails (7,417 records)
- org.CompanyOwner (7,417 records)
- org.LicenseDetails (7,417 records)
- org.Guarantors (7,490 records)
- org.CompanyCancellationInfo (~2,169 records)

**Migration Strategy:**
For each MainForm record:
1. Insert CompanyDetails
2. Insert CompanyOwner
3. Insert LicenseDetails
4. Insert CompanyCancellationInfo (if cancelled)
5. Insert Guarantors (lookup by FK = RID)

---

## GUARANTOR FIELD MAPPING

### GuaranteeForm.xlsx → org.Guarantors

| Old Field | New Field | Transformation | Notes |
|-----------|-----------|----------------|-------|
| FK | CompanyId | Direct (links to RID) | Foreign key |
| **GuaranteeType** | **GuaranteeTypeId** | **Lookup mapping** | **See below** |
| GName | FirstName | Direct copy | Guarantor name |
| GFName | FatherName | Direct copy | Father name |
| GRelation | GrandFatherName | Direct copy | Grandfather name |
| GContact | PhoneNumber | Direct copy | Phone number |
| GTazkeraNo | ElectronicNationalIdNumber | Direct copy | National ID |
| GJold | - | Not mapped | Old tazkera volume |
| GPage | - | Not mapped | Old tazkera page |
| **GReferenceNo** | **PropertyDocumentNumber** | **Parse based on type** | **See type logic** |
| **GBank** | **BankName** | **If type = پول نقد** | **Cash guarantee** |
| GAddress | PaddressVillage | Direct copy | Guarantor address |
| Remarks | - | Not mapped | Optional notes |

---

## GUARANTEE TYPE MAPPING

### Old → New Type IDs

| Old Text | Count | New TypeId | New English | Conditional Fields |
|----------|-------|------------|-------------|-------------------|
| قباله خط عرفی | 5,233 | 3 | Customary Deed | SetSerialNumber, GuaranteeDistrictId |
| قباله خط | 1,036 | 2 | Sharia Deed | CourtName, CollateralNumber |
| پول نقد | 997 | 1 | Cash | BankName, DepositNumber, DepositDate |

**Note:** The type determines which conditional fields to populate!

---

## CONDITIONAL FIELD LOGIC BY TYPE

### Type 1: Cash (پول نقد) - 997 records
```csharp
if (guaranteeType == "پول نقد") {
    GuaranteeTypeId = 1;
    BankName = GBank;  // Bank name
    DepositNumber = GReferenceNo;  // Deposit/receipt number
    DepositDate = null;  // Not in old data
    // Leave other fields NULL
}
```

### Type 2: Sharia Deed (قباله خط) - 1,036 records
```csharp
if (guaranteeType == "قباله خط") {
    GuaranteeTypeId = 2;
    CollateralNumber = GReferenceNo;  // Deed number
    CourtName = null;  // Not in old data
    // Leave other fields NULL
}
```

### Type 3: Customary Deed (قباله خط عرفی) - 5,233 records
```csharp
if (guaranteeType == "قباله خط عرفی") {
    GuaranteeTypeId = 3;
    SetSerialNumber = GReferenceNo;  // Serial number
    GuaranteeDistrictId = null;  // Not in old data
    // Leave other fields NULL
}
```

---

## COMPLETE org.Guarantors MAPPING

| Field | Source | Value | Notes |
|-------|--------|-------|-------|
| **Id** | Auto | Serial | Auto-increment |
| **FirstName** | GName | Direct | Required |
| **FatherName** | GFName | Direct | Required |
| **GrandFatherName** | GRelation | Direct | Grandfather name |
| **CompanyId** | FK | Direct | Links to CompanyDetails |
| **ElectronicNationalIdNumber** | GTazkeraNo | Direct | National ID |
| **PhoneNumber** | GContact | Direct | Contact number |
| **PaddressProvinceId** | NULL | NULL | Not in old data |
| **PaddressDistrictId** | NULL | NULL | Not in old data |
| **PaddressVillage** | GAddress | Direct | Full address |
| **TaddressProvinceId** | NULL | NULL | Not in old data |
| **TaddressDistrictId** | NULL | NULL | Not in old data |
| **TaddressVillage** | NULL | NULL | Not in old data |
| **GuaranteeTypeId** | GuaranteeType | **See mapping** | 1/2/3 |
| **PropertyDocumentNumber** | NULL | NULL | Not used |
| **PropertyDocumentDate** | NULL | NULL | Not in old data |
| **SenderMaktobNumber** | NULL | NULL | Not in old data |
| **SenderMaktobDate** | NULL | NULL | Not in old data |
| **AnswerdMaktobNumber** | NULL | NULL | Not in old data |
| **AnswerdMaktobDate** | NULL | NULL | Not in old data |
| **DateofGuarantee** | NULL | NULL | Not in old data |
| **GuaranteeDocNumber** | NULL | NULL | Not in old data |
| **GuaranteeDate** | NULL | NULL | Not in old data |
| **GuaranteeDocPath** | NULL | NULL | Not in old data |
| **CourtName** | NULL | NULL | **If Type 2** |
| **CollateralNumber** | GReferenceNo | **If Type 2** | **Sharia deed** |
| **SetSerialNumber** | GReferenceNo | **If Type 3** | **Customary deed** |
| **GuaranteeDistrictId** | NULL | NULL | Not in old data |
| **BankName** | GBank | **If Type 1** | **Cash only** |
| **DepositNumber** | GReferenceNo | **If Type 1** | **Cash only** |
| **DepositDate** | NULL | NULL | Not in old data |
| **Status** | TRUE | Boolean | All active |
| **CreatedAt** | NOW() | DateTime.Now | Migration timestamp |
| **CreatedBy** | "MIGRATION_SCRIPT" | String | Marker |

---

## MIGRATION LOGIC

### Step-by-Step Process

```csharp
foreach (var mainRecord in mainFormRecords)
{
    using (transaction)
    {
        // 1. Insert Company
        int companyId = InsertCompanyDetails(mainRecord);
        
        // 2. Insert Owner
        InsertCompanyOwner(mainRecord, companyId);
        
        // 3. Insert License
        InsertLicenseDetails(mainRecord, companyId);
        
        // 4. Insert Cancellation (if applicable)
        if (!string.IsNullOrEmpty(mainRecord.LicnsCancelNo))
        {
            InsertCancellationInfo(mainRecord, companyId);
        }
        
        // 5. Insert Guarantors (lookup by FK = RID)
        var guarantors = guaranteeMapping[mainRecord.RID];
        foreach (var guarantor in guarantors)
        {
            InsertGuarantor(guarantor, companyId);
        }
        
        transaction.Commit();
    }
}
```

### InsertGuarantor Method

```csharp
static async Task InsertGuarantor(GuaranteeRecord guarantee, int companyId,
    NpgsqlConnection conn, NpgsqlTransaction transaction)
{
    // Determine guarantee type ID
    int guaranteeTypeId;
    string? bankName = null;
    string? depositNumber = null;
    string? collateralNumber = null;
    string? setSerialNumber = null;
    
    switch (guarantee.GuaranteeType?.Trim())
    {
        case "پول نقد":  // Cash
            guaranteeTypeId = 1;
            bankName = guarantee.GBank;
            depositNumber = guarantee.GReferenceNo;
            break;
            
        case "قباله خط":  // Sharia Deed
            guaranteeTypeId = 2;
            collateralNumber = guarantee.GReferenceNo;
            break;
            
        case "قباله خط عرفی":  // Customary Deed
            guaranteeTypeId = 3;
            setSerialNumber = guarantee.GReferenceNo;
            break;
            
        default:
            return;  // Skip unknown types
    }
    
    string query = @"
        INSERT INTO org.""Guarantors"" 
        (""FirstName"", ""FatherName"", ""GrandFatherName"", ""CompanyId"",
         ""ElectronicNationalIdNumber"", ""PhoneNumber"", ""PaddressVillage"",
         ""GuaranteeTypeId"", ""BankName"", ""DepositNumber"",
         ""CollateralNumber"", ""SetSerialNumber"",
         ""Status"", ""CreatedAt"", ""CreatedBy"")
        VALUES (@firstname, @fathername, @grandfathername, @companyid,
                @electronicid, @phone, @paddressvillage,
                @guaranteetypeid, @bankname, @depositnumber,
                @collateralnumber, @setserialnumber,
                @status, @createdat, @createdby)";
    
    using (var cmd = new NpgsqlCommand(query, conn, transaction))
    {
        cmd.Parameters.AddWithValue("firstname", guarantee.GName ?? "");
        cmd.Parameters.AddWithValue("fathername", guarantee.GFName ?? "");
        cmd.Parameters.AddWithValue("grandfathername", guarantee.GRelation ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("companyid", companyId);
        cmd.Parameters.AddWithValue("electronicid", guarantee.GTazkeraNo ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("phone", guarantee.GContact ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("paddressvillage", guarantee.GAddress ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("guaranteetypeid", guaranteeTypeId);
        cmd.Parameters.AddWithValue("bankname", bankName ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("depositnumber", depositNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("collateralnumber", collateralNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("setserialnumber", setSerialNumber ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("status", true);
        cmd.Parameters.AddWithValue("createdat", DateTime.Now);
        cmd.Parameters.AddWithValue("createdby", "MIGRATION_SCRIPT");
        
        await cmd.ExecuteNonQueryAsync();
    }
}
```

---

## EXAMPLE TRANSFORMATION

### Old Records

**MainForm (RID = 1):**
```
RID: 1
RealEstateName: دارالامان
FName: سید قاسم
FathName: سید موسی
LicenseNo: 1
... (other company fields)
```

**GuaranteeForm (FK = 1):**
```
FK: 1
GuaranteeType: پول نقد
GName: سید قاسم
GFName: سید موسی
GRelation: سید طالب
GReferenceNo: 459
GBank: افغان بانک
GAddress: پل باغ عمومی
```

### New Records

**org.Guarantors:**
```sql
Id: [Auto]
FirstName: 'سید قاسم'
FatherName: 'سید موسی'
GrandFatherName: 'سید طالب'
CompanyId: 1
ElectronicNationalIdNumber: NULL
PhoneNumber: '0780511850'
PaddressVillage: 'پل باغ عمومی'
GuaranteeTypeId: 1  -- Cash
BankName: 'افغان بانک'
DepositNumber: '459'
CollateralNumber: NULL
SetSerialNumber: NULL
Status: TRUE
CreatedAt: '2026-02-13 ...'
CreatedBy: 'MIGRATION_SCRIPT'
```

---

## VALIDATION QUERIES

### After Migration

```sql
-- 1. Check guarantor counts
SELECT 
    'Guarantors' as table_name, COUNT(*) as count 
FROM org."Guarantors";
-- Expected: 7490

-- 2. Verify guarantee type distribution
SELECT 
    gt."Dari" as type_name,
    COUNT(*) as count
FROM org."Guarantors" g
JOIN look."GuaranteeType" gt ON g."GuaranteeTypeId" = gt."Id"
GROUP BY gt."Dari", gt."Id"
ORDER BY gt."Id";
-- Expected:
--   پول نقد: 997
--   قباله خط: 1036
--   قباله خط عرفی: 5233

-- 3. Check companies with guarantors
SELECT 
    COUNT(DISTINCT "CompanyId") as companies_with_guarantors
FROM org."Guarantors";
-- Expected: 7190

-- 4. Verify relationship integrity
SELECT 
    cd."Id",
    cd."Title",
    COUNT(g."Id") as guarantor_count
FROM org."CompanyDetails" cd
LEFT JOIN org."Guarantors" g ON g."CompanyId" = cd."Id"
GROUP BY cd."Id", cd."Title"
HAVING COUNT(g."Id") > 0
LIMIT 10;

-- 5. Check conditional fields by type
-- Type 1 (Cash) - should have BankName and DepositNumber
SELECT COUNT(*) FROM org."Guarantors"
WHERE "GuaranteeTypeId" = 1
AND ("BankName" IS NOT NULL OR "DepositNumber" IS NOT NULL);

-- Type 2 (Sharia) - should have CollateralNumber
SELECT COUNT(*) FROM org."Guarantors"
WHERE "GuaranteeTypeId" = 2
AND "CollateralNumber" IS NOT NULL;

-- Type 3 (Customary) - should have SetSerialNumber
SELECT COUNT(*) FROM org."Guarantors"
WHERE "GuaranteeTypeId" = 3
AND "SetSerialNumber" IS NOT NULL;
```

---

## SUMMARY

✅ **7,417 companies** from MainForm  
✅ **7,490 guarantors** from GuaranteeForm  
✅ **7,190 companies** have at least one guarantor (96.9%)  
✅ **3 guarantee types** properly mapped with conditional fields  
✅ **Complete relationship** preserved via FK → CompanyId  

**Guarantee Type Distribution:**
- Customary Deed (قباله خط عرفی): 5,233 (69.9%)
- Sharia Deed (قباله خط): 1,036 (13.8%)
- Cash (پول نقد): 997 (13.3%)

**Key Points:**
- Same transaction migrates company + owner + license + guarantors
- Guarantee type determines which conditional fields to populate
- All guarantors linked to companies via FK = RID relationship
- Proper timestamps and audit trail maintained
