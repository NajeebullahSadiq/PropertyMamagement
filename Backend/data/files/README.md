# Data Migration Guide - Access to PostgreSQL

This guide will help you migrate 7,329 records from your old Access database to the new PostgreSQL structure.

## Overview

**What This Migration Does:**
- Migrates 7,329 company/license records
- Creates CompanyDetails, CompanyOwners, LicenseDetails, and CompanyCancellationInfo records
- Automatically creates lookup data (provinces, districts, education levels) as needed
- Handles Jalali (Solar Hijri) dates
- Maintains referential integrity with transactions
- Provides detailed migration statistics and error reporting

## Prerequisites

1. **.NET 8.0 SDK or later** installed
   - Download from: https://dotnet.microsoft.com/download

2. **PostgreSQL Database** with the new schema created
   - All tables from the database structure document must exist
   - Schemas: `org`, `log`, `look` must be created

3. **Files Required:**
   - `mainform_records.json` (already generated)
   - Migration project files (Program.cs, Models.cs, DataMigration.csproj)

## Project Structure

```
DataMigration/
├── DataMigration.csproj      # Project configuration
├── Program.cs                # Main migration logic
├── Models.cs                 # Data models
├── mainform_records.json     # Your data file (copy here)
└── README.md                 # This file
```

## Step-by-Step Migration Process

### Step 1: Prepare the Database

Before running the migration, ensure your PostgreSQL database has all the required tables created. Run these SQL commands:

```sql
-- Create schemas if they don't exist
CREATE SCHEMA IF NOT EXISTS org;
CREATE SCHEMA IF NOT EXISTS log;
CREATE SCHEMA IF NOT EXISTS look;

-- Verify tables exist
SELECT table_schema, table_name 
FROM information_schema.tables 
WHERE table_schema IN ('org', 'log', 'look')
ORDER BY table_schema, table_name;
```

You should see all tables mentioned in the database structure document.

### Step 2: Configure Database Connection

Edit `Program.cs` and update the connection string (line 13):

```csharp
private static string connectionString = "Host=localhost;Port=5432;Database=your_database_name;Username=your_username;Password=your_password";
```

Replace:
- `your_database_name` - Your PostgreSQL database name
- `your_username` - Your PostgreSQL username
- `your_password` - Your PostgreSQL password
- `localhost` - Your database server (if remote)
- `5432` - PostgreSQL port (if different)

### Step 3: Copy Data File

Copy `mainform_records.json` to the `DataMigration` folder:

```bash
cp mainform_records.json DataMigration/
```

### Step 4: Build the Project

```bash
cd DataMigration
dotnet restore
dotnet build
```

### Step 5: Run the Migration

```bash
dotnet run
```

The migration will:
1. Load all 7,329 records from JSON
2. Process each record in a transaction
3. Create lookup data automatically (provinces, districts, education levels)
4. Show progress every 100 records
5. Display final statistics

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
Owners created: 7329
Licenses created: 7329
Cancellations created: 1234
Records skipped: 0
Errors encountered: 0

================================================================================
```

## Field Mapping Reference

### CompanyDetails
| Old Field | New Field | Type |
|-----------|-----------|------|
| RID | Id | Integer |
| RealEstateName | Title | String |
| TIN | TIN | String |
| PerProvince | ProvinceId | FK → Location |
| Halat | Status | Boolean |

### CompanyOwners
| Old Field | New Field | Type |
|-----------|-----------|------|
| FName | FirstName | String |
| FathName | FatherName | String |
| GFName | GrandFatherName | String |
| Education | EducationLevelId | FK → EducationLevel |
| DOB | DateofBirth | String/Date |
| TazkeraNo | ElectronicNationalIdNumber | String |
| ContactNo | PhoneNumber | String |
| PerProvince | OwnerProvinceId | FK → Location |
| PerWoloswaly | OwnerDistrictId | FK → Location |
| TempProvince | PermanentProvinceId | FK → Location |
| TempWoloswaly | PermanentDistrictId | FK → Location |
| ExactAddress | OwnerVillage, PermanentVillage | String |

### LicenseDetails
| Old Field | New Field | Type |
|-----------|-----------|------|
| LicenseNo | LicenseNumber | String |
| SYear/SMonth/SDay | IssueDate | Date |
| EYear/EMonth/EDay | ExpireDate | Date |
| LicenseType | LicenseType, LicenseCategory | String |
| CreditRightAmount | RoyaltyAmount | Decimal |
| CreditRightYear/Month/Day | RoyaltyDate | Date |
| LateFine | PenaltyAmount | Decimal |
| HRNo | HrLetter | String |

### CompanyCancellationInfo
| Old Field | New Field | Type |
|-----------|-----------|------|
| LicnsCancelNo | LicenseCancellationLetterNumber | String |
| CancelYear/Month/Day | LicenseCancellationLetterDate | Date |
| Remarks | Remarks | String |

## Troubleshooting

### Issue: "File 'mainform_records.json' not found"
**Solution:** Make sure mainform_records.json is in the DataMigration folder.

### Issue: "Cannot connect to database"
**Solution:** 
- Verify PostgreSQL is running: `pg_isready`
- Check connection string credentials
- Ensure database exists: `psql -l`
- Check firewall settings if remote

### Issue: "Table does not exist"
**Solution:** 
- Run your database schema creation scripts first
- Verify all tables exist using the SQL query in Step 1

### Issue: "Duplicate key violation"
**Solution:** 
- The migration checks for existing records by ID
- If you want to re-migrate, either:
  - Delete existing data: `TRUNCATE org.companydetails CASCADE;`
  - Or the script will skip existing records

### Issue: "Foreign key constraint violation"
**Solution:** 
- This usually means a lookup table is missing data
- The migration automatically creates provinces, districts, and education levels
- Check that all lookup tables exist

### Issue: Date format errors
**Solution:** 
- The script handles Jalali dates as strings in YYYY-MM-DD format
- If you need Gregorian conversion, you'll need to add a date conversion library

## Data Validation

After migration, verify the data:

```sql
-- Check record counts
SELECT 'CompanyDetails' as table_name, COUNT(*) as count FROM org.companydetails
UNION ALL
SELECT 'CompanyOwners', COUNT(*) FROM org.companyowners
UNION ALL
SELECT 'LicenseDetails', COUNT(*) FROM org.licensedetails
UNION ALL
SELECT 'CompanyCancellationInfo', COUNT(*) FROM org.companycancellationinfo;

-- Check sample data
SELECT cd.id, cd.title, co.firstname, co.fathername, ld.licensenumber
FROM org.companydetails cd
LEFT JOIN org.companyowners co ON co.companyid = cd.id
LEFT JOIN org.licensedetails ld ON ld.companyid = cd.id
LIMIT 10;

-- Check for missing relationships
SELECT cd.id, cd.title
FROM org.companydetails cd
LEFT JOIN org.companyowners co ON co.companyid = cd.id
WHERE co.id IS NULL;

-- Check province distribution
SELECT l.name as province, COUNT(cd.id) as company_count
FROM org.companydetails cd
LEFT JOIN look.location l ON l.id = cd.provinceid
GROUP BY l.name
ORDER BY company_count DESC;
```

## Advanced Configuration

### Batch Size
To process records in larger/smaller batches, modify line 33 in Program.cs:
```csharp
int batchSize = 50;  // Change this value
```

### Parallel Processing
For faster migration (use with caution):
```csharp
// Replace the for loop with:
await Parallel.ForEachAsync(records, 
    new ParallelOptions { MaxDegreeOfParallelism = 4 },
    async (record, ct) => await MigrateRecord(record));
```

### Dry Run Mode
To test without committing:
```csharp
// In MigrateRecord method, replace:
await transaction.CommitAsync();
// With:
await transaction.RollbackAsync(); // Dry run - no changes
```

## Important Notes

1. **Jalali Dates:** The migration stores dates as-is in Jalali format (YYYY-MM-DD). If you need Gregorian conversion, add a Jalali-to-Gregorian converter library.

2. **Data Integrity:** All operations are wrapped in transactions. If any part fails, the entire record is rolled back.

3. **Lookup Tables:** The migration automatically creates:
   - Provinces and districts from location names
   - Education levels from education names
   
4. **Status Field:** Records with "فسخ" (cancelled) in the Halat field are marked as inactive (Status = false).

5. **Missing Data:** NULL values in the old database are preserved as NULL in the new database.

6. **Re-running:** The migration checks for existing companies by ID and skips them. You can safely re-run the migration.

## Performance Tips

- Expected time: ~5-10 minutes for 7,329 records (depends on hardware)
- Uses batched transactions for better performance
- Creates indexes after migration for faster queries
- Consider adding indexes after migration:

```sql
CREATE INDEX idx_company_province ON org.companydetails(provinceid);
CREATE INDEX idx_owner_company ON org.companyowners(companyid);
CREATE INDEX idx_license_company ON org.licensedetails(companyid);
CREATE INDEX idx_license_number ON org.licensedetails(licensenumber);
```

## Support

If you encounter issues:
1. Check the error messages in the console output
2. Review the migration statistics
3. Check PostgreSQL logs: `tail -f /var/log/postgresql/postgresql-*.log`
4. Verify data with the validation queries above

## Next Steps

After successful migration:
1. Verify data integrity with validation queries
2. Create database indexes for performance
3. Set up your .NET backend API to connect to this database
4. Test React.js frontend with the migrated data
5. Create database backups

## License & Credits

Migration Tool created for Real Estate License Management System
Uses: .NET 8.0, Npgsql, System.Text.Json
