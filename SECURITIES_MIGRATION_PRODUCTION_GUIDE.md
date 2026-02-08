# Securities Migration - Production Deployment Guide

## Prerequisites

✅ Development migration completed successfully (7,022 records migrated)
✅ Database schema already deployed to production
✅ Production server access available

## Files to Upload to Production Server

Upload these files to `/var/www/prmis/migration/` on the production server:

```
Backend/DataMigration/
├── Program.cs
├── SecuritiesMigration.cs
├── Models.cs
├── SecuritiesModels.cs
├── DataMigration.csproj
└── securities_records_clean_fixed.json
```

## Step-by-Step Production Deployment

### Step 1: Connect to Production Server

```bash
ssh root@185.125.231.135
```

### Step 2: Create Migration Directory

```bash
mkdir -p /var/www/prmis/migration
cd /var/www/prmis/migration
```

### Step 3: Upload Files

From your local machine (Windows), use SCP or WinSCP to upload:

**Option A: Using WinSCP (Recommended for Windows)**
- Host: 185.125.231.135
- Username: root
- Upload all files from `Backend/DataMigration/` to `/var/www/prmis/migration/`

**Option B: Using PowerShell SCP**
```powershell
# From Backend/DataMigration directory
scp Program.cs SecuritiesMigration.cs Models.cs SecuritiesModels.cs DataMigration.csproj securities_records_clean_fixed.json root@185.125.231.135:/var/www/prmis/migration/
```

### Step 4: Set Production Connection String

On the production server, set the environment variable:

```bash
export MIGRATION_CONNECTION_STRING="Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024"
```

Or edit the SecuritiesMigration.cs file to use production credentials directly:

```bash
nano /var/www/prmis/migration/SecuritiesMigration.cs
```

Change line 13-14 to:
```csharp
private static string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024";
```

### Step 5: Verify Database Connection

Test the database connection:

```bash
psql -h localhost -U prmis_user -d PRMIS -c "SELECT COUNT(*) FROM org.\"SecuritiesDistribution\";"
```

This should return 0 (or existing count if you've run migrations before).

### Step 6: Run the Migration

```bash
cd /var/www/prmis/migration
dotnet run securities
```

### Step 7: Monitor Progress

The migration will show progress every 100 records:
```
Processed 100/7022 securities records...
Processed 200/7022 securities records...
...
```

Expected completion time: ~2-3 minutes

### Step 8: Verify Migration Results

After completion, verify the data:

```bash
psql -h localhost -U prmis_user -d PRMIS
```

Run these verification queries:

```sql
-- Check total distributions created
SELECT COUNT(*) FROM org."SecuritiesDistribution";
-- Expected: ~6,989

-- Check total distribution items
SELECT COUNT(*) FROM org."SecuritiesDistributionItem";
-- Expected: ~12,416

-- Check sample data
SELECT "RegistrationNumber", "LicenseOwnerName", "LicenseNumber" 
FROM org."SecuritiesDistribution" 
LIMIT 10;

-- Check distribution items by type
SELECT 
    sdt."TypeName",
    COUNT(*) as count,
    SUM(sdi."Quantity") as total_quantity
FROM org."SecuritiesDistributionItem" sdi
JOIN org."SecuritiesDocumentType" sdt ON sdi."DocumentTypeId" = sdt."Id"
GROUP BY sdt."TypeName"
ORDER BY sdt."Id";
```

Expected results:
- سته خرید و فروش (Property Sale): ~6,500 items
- سته بیع وفا (Bay Wafa): ~2,000 items
- سته کرایی (Rent): ~1,500 items
- سته موتر (Vehicle): ~1,500 items
- کتاب ثبت (Registration Book): ~900 items

## Troubleshooting

### Issue: Connection Failed

**Error:** `password authentication failed for user "prmis_user"`

**Solution:** 
1. Verify the password in the connection string
2. Check PostgreSQL pg_hba.conf allows local connections
3. Restart PostgreSQL: `systemctl restart postgresql`

### Issue: Permission Denied

**Error:** `permission denied for schema org`

**Solution:**
```sql
GRANT ALL ON SCHEMA org TO prmis_user;
GRANT ALL ON ALL TABLES IN SCHEMA org TO prmis_user;
GRANT ALL ON ALL SEQUENCES IN SCHEMA org TO prmis_user;
```

### Issue: Duplicate Records

**Error:** `Registration number XXX already exists`

**Solution:** This is expected for ~33 records with registration number "فقط کتاب ثبت". These will be skipped automatically.

## Rollback Plan (If Needed)

If you need to rollback the migration:

```sql
-- Delete all securities data
DELETE FROM org."SecuritiesDistributionItem";
DELETE FROM org."SecuritiesDistribution";

-- Reset sequences
ALTER SEQUENCE org."SecuritiesDistribution_Id_seq" RESTART WITH 1;
ALTER SEQUENCE org."SecuritiesDistributionItem_Id_seq" RESTART WITH 1;
```

## Post-Migration Cleanup

After successful migration:

```bash
# Remove migration files (optional)
rm -rf /var/www/prmis/migration

# Restart the backend service
systemctl restart prmis-backend
```

## Success Criteria

✅ 7,022 records processed
✅ ~6,989 distributions created
✅ ~12,416 distribution items created
✅ 0 errors (33 skipped duplicates is normal)
✅ Data visible in the application

## Next Steps

1. Test the securities module in the application
2. Verify reports are generating correctly
3. Check that users can view securities distributions
4. Backup the production database

## Support

If you encounter issues:
1. Check the migration output for error messages
2. Review the database logs: `tail -f /var/log/postgresql/postgresql-*.log`
3. Verify all prerequisite tables exist in the database
