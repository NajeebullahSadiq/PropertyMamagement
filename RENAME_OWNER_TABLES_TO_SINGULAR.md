# Rename CompanyOwner Tables to Singular

## Problem
The database has **both** singular and plural table names:
- `org."CompanyOwner"` (singular) - **EMPTY** - created by old migration script
- `org."CompanyOwners"` (plural) - **HAS DATA** (7,326 records) - created by data migration

The EF configuration was looking at the empty singular table, causing the API to return empty arrays.

## Business Logic
Since each company has exactly **one owner**, the singular naming convention is more appropriate:
- `CompanyOwner` (not CompanyOwners)
- `CompanyOwnerAddress` (not CompanyOwnerAddresses)
- `CompanyOwnerAddressHistory` (not CompanyOwnerAddressHistories)

## Solution

### Step 1: Run Database Rename Script
Execute the SQL script to:
1. Drop the old empty singular tables
2. Rename the plural tables (with data) to singular

**Run this script:**
```bash
# In your PostgreSQL client or pgAdmin
Backend/Scripts/rename_owner_tables_to_singular.sql
```

**What it does:**
```sql
-- Drop old empty tables
DROP TABLE IF EXISTS org."CompanyOwnerAddressHistory" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwnerAddress" CASCADE;
DROP TABLE IF EXISTS org."CompanyOwner" CASCADE;

-- Rename plural tables to singular
ALTER TABLE org."CompanyOwners" RENAME TO "CompanyOwner";
ALTER TABLE org."CompanyOwnerAddresses" RENAME TO "CompanyOwnerAddress";
ALTER TABLE org."CompanyOwnerAddressHistories" RENAME TO "CompanyOwnerAddressHistory";
```

### Step 2: Backend Configuration
The EF configuration has already been updated to use singular names:
```csharp
entity.ToTable("CompanyOwner", "org");
entity.ToTable("CompanyOwnerAddress", "org");
entity.ToTable("CompanyOwnerAddressHistory", "org");
```

### Step 3: Restart Backend
After running the SQL script:
```bash
cd Backend
dotnet run
```

### Step 4: Test the API
```
GET http://localhost:5143/api/CompanyOwner/2
```

**Expected Result:**
```json
[
  {
    "id": 5,
    "firstName": "بهرام الله",
    "fatherName": "معاز الله",
    "companyId": 2,
    ...
  }
]
```

## Verification Queries

After running the rename script, verify:

```sql
-- Should return 7326 records
SELECT COUNT(*) FROM org."CompanyOwner";

-- Should fail (table doesn't exist anymore)
SELECT COUNT(*) FROM org."CompanyOwners";
```

## Status
✅ Backend code updated to use singular names
✅ SQL rename script created
⏳ **PENDING**: Run the SQL script
⏳ **PENDING**: Restart backend and test

## Files Modified
- `Backend/Configuration/AppDbContext.cs` - Reverted to singular table names
- `Backend/Scripts/rename_owner_tables_to_singular.sql` - SQL script to rename tables
