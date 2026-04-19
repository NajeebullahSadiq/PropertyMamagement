# How to Backup Database Using pgAdmin 4

## Step-by-Step Guide

### Step 1: Open pgAdmin 4
1. Launch **pgAdmin 4** from your Start menu or desktop
2. Enter your master password if prompted

### Step 2: Connect to Your Server
1. In the left sidebar (Browser panel), expand **Servers**
2. Click on your PostgreSQL server (e.g., "PostgreSQL 14" or "localhost")
3. Enter your password if prompted (usually: `Khan@223344`)

### Step 3: Select Your Database
1. Expand **Databases** in the tree
2. Find and **right-click** on **PRMIS** database
3. Select **Backup...** from the context menu

```
Servers
  └─ PostgreSQL 14
      └─ Databases
          └─ PRMIS  ← Right-click here
              ├─ Schemas
              ├─ Extensions
              └─ ...
```

### Step 4: Configure Backup Settings

#### General Tab:
1. **Filename**: Click the folder icon (📁) to choose location
   - Navigate to a safe location (e.g., `C:\Backups\` or Desktop)
   - Enter filename: `PRMIS_backup_before_date_conversion_2026-04-20.backup`
   - Click **Select**

2. **Format**: Select **Custom** (recommended)
   - Custom format is compressed and allows selective restore
   - Alternative: **Plain** (SQL text file, larger but readable)

3. **Compression**: Leave at default (usually 6-9)

4. **Encoding**: Leave as **UTF8**

5. **Role name**: Leave empty (uses current user)

#### Data/Objects Tab (Optional - Review):
- **Sections**:
  - ✅ Pre-data (schema definitions)
  - ✅ Data (actual data)
  - ✅ Post-data (indexes, triggers)

- **Type of objects**:
  - ✅ Only data (uncheck if you only want schema)
  - ✅ Only schema (uncheck if you only want data)
  - Keep both checked for full backup

#### Options Tab (Optional - Advanced):
- **Do not save**:
  - ☐ Owner
  - ☐ Privilege
  - ☐ Tablespace
  - ☐ Unlogged table data

- **Queries**:
  - ☐ Use Column Inserts
  - ☐ Use Insert Commands
  - ☐ Include CREATE DATABASE statement
  - ☐ Include DROP DATABASE statement

- **Disable**:
  - ☐ Trigger (leave unchecked)
  - ☐ $ quoting (leave unchecked)

**For safety, use default settings!**

### Step 5: Start Backup
1. Click **Backup** button at the bottom
2. A progress dialog will appear
3. Wait for the backup to complete (may take 1-10 minutes depending on database size)

### Step 6: Verify Backup
1. Check the **Processes** tab at the bottom of pgAdmin
2. Look for "Backup completed successfully" message
3. Navigate to your backup location and verify the file exists
4. Check file size (should be several MB to GB depending on your data)

## Quick Backup Checklist

```
☐ 1. Open pgAdmin 4
☐ 2. Connect to PostgreSQL server
☐ 3. Right-click PRMIS database → Backup...
☐ 4. Choose filename: PRMIS_backup_before_date_conversion_2026-04-20.backup
☐ 5. Format: Custom
☐ 6. Click Backup button
☐ 7. Wait for completion
☐ 8. Verify file exists and has reasonable size
```

## Backup File Naming Convention

Use descriptive names with dates:
```
✅ GOOD:
   PRMIS_backup_before_date_conversion_2026-04-20.backup
   PRMIS_backup_2026-04-20_10-30-AM.backup
   PRMIS_full_backup_20260420.backup

❌ BAD:
   backup.backup
   db.backup
   test.backup
```

## Backup Location Recommendations

### Best Locations:
1. **External Drive**: `E:\Database_Backups\`
2. **Network Drive**: `\\server\backups\PRMIS\`
3. **Cloud Sync Folder**: `C:\Users\YourName\OneDrive\Database_Backups\`
4. **Dedicated Backup Folder**: `C:\Database_Backups\`

### Avoid:
- ❌ Desktop (easy to delete accidentally)
- ❌ Downloads folder (gets cleaned up)
- ❌ Temp folders
- ❌ Same drive as database (if drive fails, backup is lost)

## How to Restore from Backup (If Needed)

### Method 1: Using pgAdmin 4

1. Right-click on **PRMIS** database
2. Select **Restore...**
3. **Filename**: Browse to your backup file
4. **Format**: Custom (should auto-detect)
5. **Options Tab**:
   - ✅ Clean before restore (drops existing objects)
   - ✅ Do not save - Owner
6. Click **Restore**
7. Wait for completion

### Method 2: Using Command Line

```bash
# Restore from custom format backup
pg_restore -U postgres -d PRMIS -c -v "C:\Backups\PRMIS_backup_before_date_conversion_2026-04-20.backup"

# Restore from plain SQL backup
psql -U postgres -d PRMIS < "C:\Backups\PRMIS_backup_2026-04-20.sql"
```

## Troubleshooting

### Problem: "Permission denied" error
**Solution**: 
- Run pgAdmin as Administrator
- Or choose a location where you have write permissions (e.g., Documents folder)

### Problem: Backup takes too long
**Solution**:
- This is normal for large databases (>1GB)
- Don't close pgAdmin while backup is running
- Check the Processes tab for progress

### Problem: "Could not find pg_dump" error
**Solution**:
- Ensure PostgreSQL bin folder is in system PATH
- Or specify full path in pgAdmin settings:
  - File → Preferences → Paths → Binary paths → PostgreSQL Binary Path
  - Set to: `C:\Program Files\PostgreSQL\14\bin` (adjust version number)

### Problem: Backup file is 0 KB or very small
**Solution**:
- Database might be empty
- Or backup failed - check error messages in Processes tab
- Try again with different format (Plain instead of Custom)

## Alternative: Quick Command Line Backup

If pgAdmin is not working, use command line:

```bash
# Open Command Prompt or PowerShell
cd "C:\Program Files\PostgreSQL\14\bin"

# Create backup
pg_dump -U postgres -F c -b -v -f "C:\Backups\PRMIS_backup_%date:~-4,4%%date:~-10,2%%date:~-7,2%.backup" PRMIS

# Enter password when prompted: Khan@223344
```

## Backup Best Practices

1. **Before Major Changes**: Always backup before:
   - Running migration scripts
   - Updating database schema
   - Converting data formats
   - Upgrading PostgreSQL version

2. **Regular Backups**: Schedule automatic backups:
   - Daily: Keep last 7 days
   - Weekly: Keep last 4 weeks
   - Monthly: Keep last 12 months

3. **Test Restores**: Periodically test that backups can be restored

4. **Multiple Copies**: Keep backups in multiple locations:
   - Local drive
   - External drive
   - Cloud storage

5. **Document Backups**: Keep a log of when backups were taken and why

## Backup Size Reference

Typical PRMIS database sizes:
- **Small** (< 1,000 records): 10-50 MB
- **Medium** (1,000-10,000 records): 50-500 MB
- **Large** (> 10,000 records): 500 MB - 5 GB

Your backup file should be roughly:
- **Custom format**: 20-40% of database size (compressed)
- **Plain format**: 80-120% of database size (uncompressed SQL)

## After Taking Backup

✅ **Verify the backup file**:
```bash
# Check file size
dir "C:\Backups\PRMIS_backup_before_date_conversion_2026-04-20.backup"

# Should show file size in MB/GB
```

✅ **Test the backup** (optional but recommended):
1. Create a test database: `CREATE DATABASE PRMIS_TEST;`
2. Restore backup to test database
3. Verify data is intact
4. Drop test database: `DROP DATABASE PRMIS_TEST;`

✅ **Document the backup**:
- Date and time taken
- Database size
- Reason for backup
- Location of backup file

## Now You're Ready!

Once you have a verified backup, you can safely proceed with the date conversion:

1. ✅ Backup complete
2. ✅ Backup file verified
3. ✅ Backup location documented
4. ➡️ **Next step**: Run `Backend/DataMigration/run-convert-dates.bat`

---

**Need Help?** If you encounter any issues with the backup process, stop and ask for assistance before proceeding with the date conversion.
