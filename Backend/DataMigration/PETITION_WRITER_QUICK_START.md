# Petition Writer Migration - Quick Start

## 🚀 Quick Commands

### Windows
```bash
cd Backend\DataMigration
run-petition-writer-migration.bat
```

### Linux/Mac
```bash
cd Backend/DataMigration
dotnet run petitionwriter
```

## 📋 Pre-Flight Checklist

- [ ] PostgreSQL is running
- [ ] Database `PRMIS` exists
- [ ] Table `org.PetitionWriterLicense` exists
- [ ] Files exist:
  - [ ] `petition_1403_records.json` (138 records)
  - [ ] `petition_1404_records.json` (133 records)
- [ ] Connection string is configured

## 🔧 Quick Connection String Setup

```bash
# Windows PowerShell
$env:MIGRATION_CONNECTION_STRING="Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=YourPassword"

# Linux/Mac
export MIGRATION_CONNECTION_STRING="Host=localhost;Port=5432;Database=PRMIS;Username=prmis_user;Password=YourPassword"
```

## 📊 Expected Results

```
Total records: 271 (138 from 1403 + 133 from 1404)
Licenses created: ~265-271
Skipped: ~0-6 (duplicates or missing data)
Errors: 0 (ideally)
```

## ✅ Quick Verification

```sql
-- Count records
SELECT COUNT(*) FROM org."PetitionWriterLicense";

-- Should return approximately 271 records
```

## 🆘 Quick Troubleshooting

| Problem | Solution |
|---------|----------|
| File not found | Copy JSON files to `Backend/DataMigration` |
| Connection failed | Check PostgreSQL is running |
| Duplicate error | Normal - migration skips existing records |
| Permission denied | Check database user permissions |

## 📞 Need Help?

See full guide: `PETITION_WRITER_MIGRATION_GUIDE.md`
