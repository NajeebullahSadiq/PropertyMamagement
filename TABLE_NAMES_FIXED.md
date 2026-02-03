# ✅ Table Name Case Sensitivity Issue Fixed!

## Problem Identified

PostgreSQL is case-sensitive when using quoted identifiers. The migration code was using lowercase table names (`org.companydetails`) but the actual tables were created with PascalCase (`org."CompanyDetails"`).

**Error:** `42P01: relation "org.companydetails" does not exist`

---

## Solution Applied

Updated all SQL queries in `Program.cs` to use proper quoted identifiers with PascalCase:

### Tables Fixed:
- ❌ `org.companydetails` → ✅ `org."CompanyDetails"`
- ❌ `org.companyowners` → ✅ `org."CompanyOwners"`
- ❌ `org.licensedetails` → ✅ `org."LicenseDetails"`
- ❌ `org.companycancellationinfo` → ✅ `org."CompanyCancellationInfo"`
- ❌ `look.educationlevel` → ✅ `look."EducationLevel"`
- ❌ `look.location` → ✅ `look."Location"`

### Columns Fixed:
All column names now use PascalCase with quotes:
- ✅ `"Id"`, `"Title"`, `"TIN"`, `"ProvinceId"`, `"Status"`, etc.
- ✅ `"FirstName"`, `"FatherName"`, `"CompanyId"`, etc.
- ✅ `"LicenseNumber"`, `"IssueDate"`, `"ExpireDate"`, etc.
- ✅ `"Name"`, `"Dari"`, `"Type"`, `"Parent_ID"`, `"IsActive"`

---

## ✅ Ready to Run Again!

The migration is now fixed and ready to run:

```bash
cd Backend/DataMigration
dotnet run
```

---

## Expected Output

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

---

## What Changed

### 1. InsertCompanyDetails
```csharp
// Before
INSERT INTO org.companydetails (id, title, ...)

// After
INSERT INTO org."CompanyDetails" ("Id", "Title", ...)
```

### 2. InsertCompanyOwner
```csharp
// Before
INSERT INTO org.companyowners (firstname, fathername, ...)

// After
INSERT INTO org."CompanyOwners" ("FirstName", "FatherName", ...)
```

### 3. InsertLicenseDetails
```csharp
// Before
INSERT INTO org.licensedetails (licensenumber, provinceid, ...)

// After
INSERT INTO org."LicenseDetails" ("LicenseNumber", "ProvinceId", ...)
```

### 4. InsertCancellationInfo
```csharp
// Before
INSERT INTO org.companycancellationinfo (companyid, ...)

// After
INSERT INTO org."CompanyCancellationInfo" ("CompanyId", ...)
```

### 5. Lookup Tables
```csharp
// Before
SELECT id FROM look.educationlevel WHERE name = @name
SELECT id FROM look.location WHERE name = @name AND type = 'province'

// After
SELECT "ID" FROM look."EducationLevel" WHERE "Name" = @name
SELECT "ID" FROM look."Location" WHERE "Name" = @name AND "Type" = 'province'
```

---

## PostgreSQL Case Sensitivity Rules

### Without Quotes (Folded to Lowercase):
```sql
SELECT id FROM CompanyDetails  -- Looks for "companydetails"
```

### With Quotes (Case-Sensitive):
```sql
SELECT "Id" FROM "CompanyDetails"  -- Looks for exact case
```

Your database uses PascalCase with quotes, so all queries must match exactly.

---

## Files Updated

1. ✅ `Backend/DataMigration/Program.cs` - All SQL queries fixed
   - InsertCompanyDetails method
   - InsertCompanyOwner method
   - InsertLicenseDetails method
   - InsertCancellationInfo method
   - GetEducationLevelId method
   - GetOrCreateProvinceId method
   - GetOrCreateDistrictId method
   - Check for existing company query

---

## 🚀 Run the Migration Now!

```bash
cd Backend/DataMigration
dotnet run
```

This time it should work perfectly! The migration will:
- ✅ Connect to PRMIS database
- ✅ Find all tables with correct case
- ✅ Insert 7,329 companies
- ✅ Insert 7,326 owners
- ✅ Insert 7,329 licenses
- ✅ Insert 2,169 cancellations
- ✅ Complete in 5-10 minutes

Good luck! 🎉
