# 🚀 Company Data Migration - Ready to Run!

## Current Status: ✅ Database Setup Complete

All database tables have been created successfully:
- ✅ 11 company module tables (org schema)
- ✅ 5 audit tables (log schema)  
- ✅ Lookup data populated (provinces, districts, education levels)
- ✅ Migration tool configured

---

## 📝 Final Steps Before Running

### Step 1: Update Database Password (REQUIRED)

Edit the connection string in `Backend/DataMigration/Program.cs` at **line 13**:

**Current:**
```csharp
private static string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=postgres;Password=YOUR_PASSWORD_HERE";
```

**Update to:**
```csharp
private static string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=postgres;Password=your_actual_password";
```

Replace `your_actual_password` with your PostgreSQL password.

---

### Step 2: Run the Migration

```bash
# Navigate to migration directory
cd Backend/DataMigration

# Restore packages (first time only)
dotnet restore DataMigration.csproj

# Build the project
dotnet build DataMigration.csproj

# Run the migration
dotnet run --project DataMigration.csproj
```

**Or simply:**
```bash
cd Backend/DataMigration
dotnet run
```

The project files have been created and are ready to use.

---

## 📊 Expected Results

### Console Output:
```
=================================================================
Data Migration Tool - Access to PostgreSQL
=================================================================

Loading data from mainform_records.json...
Loaded 7329 records

Starting migration process...

Processed 100/7329 records...
Processed 200/7329 records...
...
Processed 7300/7329 records...

================================================================================
MIGRATION COMPLETED
================================================================================
Total records processed: 7329
Companies created: 7329
Owners created: 7326
Licenses created: 7329
Cancellations created: 2169
Records skipped: 0
Errors encountered: 0

================================================================================
```

### Expected Duration:
- **5-10 minutes** for 7,329 records
- Processing speed: ~100-150 records/second

---

## ✅ Verification Queries

After migration completes, verify the data:

```sql
-- Connect to database
psql -h localhost -U postgres -d PRMIS

-- Check record counts
SELECT 
    'CompanyDetails' as table_name, COUNT(*) as count 
FROM org."CompanyDetails"
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org."CompanyOwners"
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org."LicenseDetails"
UNION ALL
SELECT 'CancellationInfo', COUNT(*) FROM org."CompanyCancellationInfo";
```

**Expected Results:**
- CompanyDetails: **7,329**
- CompanyOwners: **7,326**
- LicenseDetails: **7,329**
- CancellationInfo: **2,169**

```sql
-- Verify all companies registered in Kabul
SELECT 
    l."Name" as province,
    COUNT(cd."Id") as company_count
FROM org."CompanyDetails" cd
LEFT JOIN look."Location" l ON l."ID" = cd."ProvinceId"
GROUP BY l."Name";
```

**Expected:** All 7,329 companies should show **کابل (Kabul)**

```sql
-- Verify all licenses issued in Kabul
SELECT 
    l."Name" as province,
    COUNT(ld."Id") as license_count
FROM org."LicenseDetails" ld
LEFT JOIN look."Location" l ON l."ID" = ld."ProvinceId"
GROUP BY l."Name";
```

**Expected:** All 7,329 licenses should show **کابل (Kabul)**

```sql
-- Check owner province distribution (should be varied)
SELECT 
    l."Name" as owner_province,
    COUNT(co."Id") as owner_count
FROM org."CompanyOwners" co
LEFT JOIN look."Location" l ON l."ID" = co."OwnerProvinceId"
GROUP BY l."Name"
ORDER BY owner_count DESC
LIMIT 10;
```

**Expected:** Various provinces (Kabul, Parwan, Wardak, Ghazni, etc.)

```sql
-- View sample migrated data
SELECT 
    cd."Id",
    cd."Title" as company_name,
    co."FirstName" || ' ' || co."FatherName" as owner_name,
    ld."LicenseNumber",
    ld."IssueDate",
    ld."ExpireDate",
    company_prov."Name" as company_province,
    owner_prov."Name" as owner_home_province
FROM org."CompanyDetails" cd
LEFT JOIN org."CompanyOwners" co ON co."CompanyId" = cd."Id"
LEFT JOIN org."LicenseDetails" ld ON ld."CompanyId" = cd."Id"
LEFT JOIN look."Location" company_prov ON company_prov."ID" = cd."ProvinceId"
LEFT JOIN look."Location" owner_prov ON owner_prov."ID" = co."OwnerProvinceId"
LIMIT 10;
```

---

## 🔍 Key Implementation Details

### Province Assignment Logic:
1. **CompanyDetails.ProvinceId** → Always **Kabul** (central registration)
2. **LicenseDetails.ProvinceId** → Always **Kabul** (central licensing)
3. **CompanyOwners.OwnerProvinceId** → Owner's **actual home province** (from PerProvince field)
4. **CompanyOwners.PermanentProvinceId** → Owner's **current residence** (from TempProvince field)

### Status Determination:
- Checks both `LicnsCancelNo` and `Halat` fields
- If either contains "فسخ" (cancelled) → Status = false
- Otherwise → Status = true

### Field Mappings:
- `CreditRightNo` → `TariffNumber` (receipt/tariff reference)
- `ExactAddress` → `OfficeAddress` (in LicenseDetails)
- `ExactAddress` → `OwnerVillage` and `PermanentVillage` (in CompanyOwners)
- `Combo211` + `HRMonth` + `HRDay` → `HrLetterDate`

---

## ⚠️ Troubleshooting

### Error: "Cannot connect to database"
```bash
# Test connection
psql -h localhost -U postgres -d PRMIS -c "SELECT 1;"

# If fails, check PostgreSQL is running
sudo systemctl status postgresql
```

### Error: "Duplicate key violation"
The migration is safe to re-run. It checks for existing companies and skips them.

If you want to start fresh:
```sql
-- Clear existing data (CAUTION!)
TRUNCATE TABLE org."CompanyCancellationInfo" CASCADE;
TRUNCATE TABLE org."LicenseDetails" CASCADE;
TRUNCATE TABLE org."CompanyOwners" CASCADE;
TRUNCATE TABLE org."CompanyDetails" CASCADE;
```

### Migration Interrupted?
Just re-run `dotnet run` - it will skip already migrated companies.

---

## 📋 Success Checklist

After migration:

- [ ] All 7,329 companies created
- [ ] All companies have ProvinceId = Kabul
- [ ] All 7,329 licenses created  
- [ ] All licenses have ProvinceId = Kabul
- [ ] 7,326+ owners created with varied home provinces
- [ ] 2,169+ cancellations created
- [ ] 0 errors encountered
- [ ] Sample queries return correct data
- [ ] Persian/Dari text displays correctly

---

## 📚 Additional Resources

- **Detailed Guide:** `COMPANY_DATA_MIGRATION_GUIDE_UPDATED.md`
- **Database Structure:** `COMPANY_DATABASE_STRUCTURE.md`
- **Migration Code:** `Backend/DataMigration/Program.cs`
- **Data Models:** `Backend/DataMigration/Models.cs`

---

## 🎯 Ready to Go!

**Next Command:**
```bash
cd Backend/DataMigration
dotnet run
```

The migration will take 5-10 minutes. Watch the console for progress updates!

Good luck! 🚀
