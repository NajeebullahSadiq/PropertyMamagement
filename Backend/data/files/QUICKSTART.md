# QUICK START GUIDE - Data Migration

## Summary
Migrate 7,329 records from your old Access database to the new PostgreSQL structure.

**What will be migrated:**
- ✅ 7,329 Companies
- ✅ 7,326 Owners (99.96%)
- ✅ 7,329 Licenses (100%)
- ✅ 2,169 Cancellations (29.6%)
- ✅ 35 Provinces (auto-created)
- ✅ 741 Districts (auto-created)
- ✅ 3 Education Levels (auto-created)

**Estimated Time:** 5-10 minutes

---

## Prerequisites Checklist

- [ ] .NET 8.0 SDK installed ([Download](https://dotnet.microsoft.com/download))
- [ ] PostgreSQL running and accessible
- [ ] Database created with all schemas (org, log, look)
- [ ] All tables from the database structure document created
- [ ] Files ready: mainform_records.json + migration project files

---

## Step 1: Setup Database (5 minutes)

### Option A: Run the setup SQL script

```bash
psql -h localhost -U your_username -d your_database_name -f setup.sql
```

### Option B: Manual verification

```sql
-- Connect to your database
psql -h localhost -U your_username -d your_database_name

-- Verify schemas exist
\dn

-- Verify tables exist
SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_schema IN ('org', 'log', 'look')
ORDER BY table_schema, table_name;

-- You should see all required tables
```

---

## Step 2: Configure Connection (1 minute)

Edit `DataMigration/Program.cs` line 13:

```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=YOUR_DB_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD";
```

**Replace:**
- `YOUR_DB_NAME` - Your PostgreSQL database name
- `YOUR_USERNAME` - Your PostgreSQL username  
- `YOUR_PASSWORD` - Your PostgreSQL password

---

## Step 3: Prepare Files (1 minute)

```bash
# Create project directory
mkdir DataMigration
cd DataMigration

# Copy all files:
# - Program.cs
# - Models.cs
# - DataMigration.csproj
# - mainform_records.json
# - README.md

# Verify files
ls -la
```

You should see:
```
DataMigration.csproj
Program.cs
Models.cs
mainform_records.json
README.md
```

---

## Step 4: Build and Run (2 minutes)

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run migration
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

## Step 5: Verify Data (2 minutes)

```sql
-- Check record counts
SELECT 'CompanyDetails' as table_name, COUNT(*) FROM org.companydetails
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org.companyowners
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org.licensedetails
UNION ALL
SELECT 'CancellationInfo', COUNT(*) FROM org.companycancellationinfo;

-- Expected results:
-- CompanyDetails: 7329
-- CompanyOwners: 7326
-- LicenseDetails: 7329
-- CancellationInfo: 2169

-- View sample data
SELECT cd.id, cd.title, co.firstname, ld.licensenumber
FROM org.companydetails cd
JOIN org.companyowners co ON co.companyid = cd.id
JOIN org.licensedetails ld ON ld.companyid = cd.id
LIMIT 5;
```

---

## Troubleshooting

### Error: "File not found"
```bash
# Make sure mainform_records.json is in the DataMigration folder
cp mainform_records.json DataMigration/
```

### Error: "Cannot connect to database"
```bash
# Test PostgreSQL connection
psql -h localhost -U your_username -d your_database_name -c "SELECT 1;"

# If fails, check:
# 1. PostgreSQL is running: sudo systemctl status postgresql
# 2. Credentials are correct
# 3. Database exists: psql -l
```

### Error: "Table does not exist"
```sql
-- Verify all tables exist
\dt org.*
\dt look.*
\dt log.*

-- If missing, run your schema creation scripts
```

### Error: "Duplicate key violation"
```sql
-- Clear existing data (CAUTION: This deletes data!)
TRUNCATE TABLE org.companycancellationinfo CASCADE;
TRUNCATE TABLE org.licensedetails CASCADE;
TRUNCATE TABLE org.companyowners CASCADE;
TRUNCATE TABLE org.companydetails CASCADE;

-- Or let the migration skip existing records (default behavior)
```

---

## Data Mapping Quick Reference

| Old Field | New Table | New Field | Notes |
|-----------|-----------|-----------|-------|
| RID | CompanyDetails | Id | |
| RealEstateName | CompanyDetails | Title | Company name (not owner name) |
| **"کابل"** | **CompanyDetails** | **ProvinceId** | **ALL companies in Kabul** |
| FName | CompanyOwners | FirstName | |
| PerProvince | CompanyOwners | OwnerProvinceId | Owner's HOME province |
| TempProvince | CompanyOwners | PermanentProvinceId | Owner's CURRENT province |
| LicenseNo | LicenseDetails | LicenseNumber | |
| **"کابل"** | **LicenseDetails** | **ProvinceId** | **ALL licenses from Kabul** |
| ExactAddress | LicenseDetails | OfficeAddress | Office location |
| LicnsCancelNo | CompanyCancellationInfo | LicenseCancellationLetterNumber | |

**IMPORTANT:** 
- Company/License Province = Kabul (hardcoded for all 7,329 records)
- Owner Address = PerProvince/TempProvince (varies by owner location)

---

## Important Notes

⚠️ **Backup First:**
```bash
pg_dump -h localhost -U your_username -d your_database_name > backup.sql
```

✅ **Safe to Re-run:** The migration checks for existing records and skips them

📅 **Date Format:** Jalali dates stored as YYYY-MM-DD strings

🔒 **Transactions:** Each record is processed in a transaction (all-or-nothing)

🏛️ **ALL in Kabul:** All 7,329 companies and licenses are registered/issued in Kabul province. Owner addresses vary.

---

## Performance

- **Processing Speed:** ~100-150 records/second
- **Total Time:** 5-10 minutes for 7,329 records
- **Memory Usage:** ~200-300 MB

---

## After Migration

1. ✅ Verify data with SQL queries
2. ✅ Create additional indexes if needed
3. ✅ Test with your .NET API
4. ✅ Connect your React frontend
5. ✅ Create database backup

---

## Support

For issues, check:
1. Console error messages
2. PostgreSQL logs: `tail -f /var/log/postgresql/*.log`
3. README.md for detailed troubleshooting
4. Data analysis report for statistics

---

## Success Criteria

✅ All 7,329 companies created  
✅ 7,326+ owners created  
✅ 7,329 licenses created  
✅ 2,169+ cancellations created  
✅ 0 errors  
✅ Sample queries return correct data  

**You're ready to use your new database! 🎉**
