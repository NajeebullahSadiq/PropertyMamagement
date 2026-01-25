# Quick Production Deployment Guide

## 🚀 Ready to Deploy

All code changes are complete. You just need to run ONE SQL script in production.

## 📋 Pre-Deployment Checklist

- [ ] Backup production database
- [ ] Verify database connection details
- [ ] Schedule maintenance window (script takes ~30 seconds)
- [ ] Notify users of brief downtime

## 🔧 Deployment Steps

### 1. Backup Database (5 minutes)
```bash
pg_dump -U postgres -d PRMIS > PRMIS_backup_$(date +%Y%m%d_%H%M%S).sql
```

### 2. Run SQL Script (30 seconds)

**Open pgAdmin:**
1. Connect to production database
2. Right-click database → Query Tool
3. Open file: `Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql`
4. Click Execute (F5)
5. Verify results show all 20 columns as 'text'

**Or use psql:**
```bash
psql -U postgres -d PRMIS -f Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql
```

### 3. Deploy Application (5 minutes)
```bash
# Stop application
sudo systemctl stop prmis-backend

# Deploy new code
cd /path/to/Backend
git pull origin main

# Restart application
sudo systemctl start prmis-backend
```

### 4. Verify (2 minutes)
- [ ] Application starts without errors
- [ ] Dashboard loads correctly
- [ ] Create test property transaction
- [ ] View reports

## ✅ Expected Results

After running the script, you should see:
```
table_name              | column_name      | data_type
------------------------|------------------|----------
BuyerDetails            | Price            | text
PropertyDetails         | Price            | text
SellerDetails           | Price            | text
VehiclesBuyerDetails    | Price            | text
VehiclesPropertyDetails | Price            | text
VehiclesSellerDetails   | Price            | text
... (14 more rows)
```

All 20 columns should show `data_type = 'text'`

## 🔄 Rollback Plan (If Needed)

If something goes wrong:
```bash
# Restore from backup
psql -U postgres -d PRMIS < PRMIS_backup_YYYYMMDD_HHMMSS.sql

# Restart application with old code
git checkout <previous-commit>
sudo systemctl restart prmis-backend
```

## 📞 Support

If you encounter issues:
1. Check application logs: `sudo journalctl -u prmis-backend -f`
2. Check database logs: `/var/log/postgresql/`
3. Verify database connection in `appsettings.json`

## 📝 What Changed

**Database**: 20 columns converted from `double precision` to `text`
**Code**: All models, controllers, and DTOs updated to use `string?` instead of `double?`

## ⏱️ Total Time: ~15 minutes

- Backup: 5 min
- SQL Script: 30 sec
- Deploy Code: 5 min
- Verify: 2 min
- Buffer: 2.5 min

---

**Script Location**: `Backend/Scripts/PRODUCTION_convert_numeric_to_text.sql`
**Full Documentation**: `Backend/Scripts/PRODUCTION_DEPLOYMENT_README.md`
**Completion Summary**: `Backend/PRODUCTION_CONVERSION_COMPLETE.md`
