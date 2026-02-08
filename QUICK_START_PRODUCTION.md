# Quick Start - Deploy Securities Migration to Production

## Easiest Method (Using WinSCP + PuTTY)

### Step 1: Download Tools (if you don't have them)

- **WinSCP:** https://winscp.net/eng/download.php
- **PuTTY:** https://www.putty.org/

### Step 2: Upload Files Using WinSCP

1. **Open WinSCP**

2. **Create New Connection:**
   - File Protocol: `SFTP`
   - Host name: `185.125.231.135`
   - Port: `22`
   - User name: `root`
   - Password: (your server password)
   - Click "Login"

3. **Navigate to Migration Directory:**
   - In the right panel (server), navigate to: `/var/www/prmis/`
   - Right-click → New → Directory
   - Name it: `migration`
   - Double-click to enter the directory

4. **Upload Files:**
   - In the left panel (local), navigate to: `C:\Users\Najib\OneDrive\Desktop\PropertyMamagement\Backend\DataMigration`
   - Select these files:
     - ✅ `Program.cs`
     - ✅ `SecuritiesMigration.cs`
     - ✅ `Models.cs`
     - ✅ `SecuritiesModels.cs`
     - ✅ `DataMigration.csproj`
     - ✅ `securities_records_clean_fixed.json`
   - Drag and drop them to the right panel (server)
   - Wait for upload to complete

### Step 3: Connect via PuTTY

1. **Open PuTTY**

2. **Connect to Server:**
   - Host Name: `185.125.231.135`
   - Port: `22`
   - Click "Open"
   - Login as: `root`
   - Enter password

### Step 4: Update Connection String

In PuTTY terminal, run:

```bash
cd /var/www/prmis/migration
nano SecuritiesMigration.cs
```

Find line 13 (around line 13-14) and change:
```csharp
# FROM:
private static string connectionString = "Host=127.0.0.1;Port=5432;Database=PRMIS;Username=postgres;Password=Khan@223344";

# TO:
private static string connectionString = "Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=SecurePassword@2024";
```

Save and exit:
- Press `Ctrl + X`
- Press `Y` to confirm
- Press `Enter`

### Step 5: Build Migration Tool

```bash
dotnet build
```

Wait for build to complete. You should see:
```
Build succeeded in X.Xs
```

### Step 6: Run Migration

```bash
dotnet run securities
```

You'll see progress:
```
Loading data from securities_records_clean.json...
Loaded 7022 records

Starting securities migration process...

Processed 100/7022 securities records...
Processed 200/7022 securities records...
...
```

Wait for completion (~2-3 minutes).

### Step 7: Verify Success

You should see:
```
================================================================================
SECURITIES MIGRATION COMPLETED
================================================================================
Total records processed: 7022
Distributions created: 6989
Distribution items created: 12416
Records skipped: 33
Errors encountered: 0
```

### Step 8: Verify in Database

```bash
psql -h localhost -U prmis_user -d PRMIS
```

Enter password: `SecurePassword@2024`

Run verification:
```sql
SELECT COUNT(*) FROM org."SecuritiesDistribution";
SELECT COUNT(*) FROM org."SecuritiesDistributionItem";
```

Expected results:
- First query: ~6,989
- Second query: ~12,416

Type `\q` to exit psql.

### Step 9: Cleanup (Optional)

```bash
cd /var/www/prmis
rm -rf migration
```

### Step 10: Restart Backend

```bash
systemctl restart prmis-backend
```

## Done! 🎉

Your securities migration is complete. You can now:
1. Open the application
2. Navigate to Securities module
3. View the migrated distributions

---

## Alternative: PowerShell Script Method

If you have SSH/SCP configured in PowerShell:

```powershell
cd C:\Users\Najib\OneDrive\Desktop\PropertyMamagement
.\deploy-securities-migration.ps1
```

Follow the prompts.

---

## Troubleshooting

### Issue: "dotnet: command not found"

Install .NET on the server:
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0
```

### Issue: "Connection refused"

Check if PostgreSQL is running:
```bash
systemctl status postgresql
systemctl start postgresql  # if not running
```

### Issue: "Permission denied"

Grant permissions:
```bash
psql -h localhost -U postgres -d PRMIS
```
```sql
GRANT ALL ON SCHEMA org TO prmis_user;
GRANT ALL ON ALL TABLES IN SCHEMA org TO prmis_user;
GRANT ALL ON ALL SEQUENCES IN SCHEMA org TO prmis_user;
\q
```

---

## Need Help?

Check these files for more details:
- `SECURITIES_MIGRATION_PRODUCTION_GUIDE.md` - Full deployment guide
- `SECURITIES_MIGRATION_COMPLETE.md` - Complete summary
- `verify-securities-migration.sql` - Verification queries
