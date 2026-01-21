# Activity Monitoring Module - Migration Files Summary

## 📁 Files Created

### 1. Main SQL Migration
**File**: `11_ActivityMonitoring_Initial.sql`  
**Purpose**: Creates all 4 tables with indexes, foreign keys, and comments  
**Usage**: Can be run directly with psql or included in full deployment  
**Idempotent**: Yes (uses IF NOT EXISTS)

### 2. Linux/Mac Deployment Script
**File**: `deploy_activity_monitoring.sh`  
**Purpose**: Automated deployment with connection testing and verification  
**Features**:
- Environment variable support
- Connection testing
- Table verification
- Colored output
- Error handling

**Usage**:
```bash
chmod +x deploy_activity_monitoring.sh
./deploy_activity_monitoring.sh
```

### 3. Windows Deployment Script
**File**: `deploy_activity_monitoring.bat`  
**Purpose**: Windows equivalent of the shell script  
**Features**:
- Environment variable support
- Connection testing
- Table verification
- Error handling

**Usage**:
```cmd
deploy_activity_monitoring.bat
```

### 4. Rollback Script
**File**: `rollback_activity_monitoring.sql`  
**Purpose**: Removes all Activity Monitoring tables and data  
**Warning**: ⚠️ Destructive operation - deletes all data!  
**Usage**:
```bash
psql -h localhost -U postgres -d prmis_db -f rollback_activity_monitoring.sql
```

### 5. Migration Documentation
**File**: `ACTIVITY_MONITORING_MIGRATION_README.md`  
**Purpose**: Comprehensive migration guide  
**Contents**:
- Deployment methods
- Schema documentation
- Troubleshooting
- Verification steps
- Rollback procedures

### 6. Quick Start Guide
**File**: `ACTIVITY_MONITORING_QUICK_START.md` (in root)  
**Purpose**: 5-minute deployment guide  
**Contents**:
- Quick deployment steps
- Testing checklist
- API endpoints
- Sample data

## 🗂️ File Locations

```
Backend/
├── Scripts/
│   └── Modules/
│       ├── 11_ActivityMonitoring_Initial.sql          ← Main migration
│       ├── deploy_activity_monitoring.sh              ← Linux/Mac script
│       ├── deploy_activity_monitoring.bat             ← Windows script
│       ├── rollback_activity_monitoring.sql           ← Rollback script
│       ├── ACTIVITY_MONITORING_MIGRATION_README.md    ← Full guide
│       ├── MIGRATION_FILES_SUMMARY.md                 ← This file
│       └── deploy_all_modules.sql                     ← Updated
└── Infrastructure/
    └── Migrations/
        └── ActivityMonitoring/
            └── 20260121_ActivityMonitoring_Initial.cs ← EF migration

Root/
├── ACTIVITY_MONITORING_IMPLEMENTATION_SUMMARY.md      ← Full summary
└── ACTIVITY_MONITORING_QUICK_START.md                 ← Quick guide
```

## 🚀 Deployment Options

### Option 1: Automated Script (Recommended)
```bash
# Linux/Mac
./deploy_activity_monitoring.sh

# Windows
deploy_activity_monitoring.bat
```
**Pros**: Automated, verified, error handling  
**Cons**: Requires psql installed

### Option 2: Direct SQL
```bash
psql -h localhost -U postgres -d prmis_db -f 11_ActivityMonitoring_Initial.sql
```
**Pros**: Simple, direct  
**Cons**: No verification, manual error checking

### Option 3: Full Module Deployment
```bash
psql -h localhost -U postgres -d prmis_db -f deploy_all_modules.sql
```
**Pros**: Deploys all modules in order  
**Cons**: Takes longer, may deploy unnecessary modules

### Option 4: Entity Framework
```bash
cd Backend
dotnet ef database update
```
**Pros**: Integrated with EF Core  
**Cons**: Requires .NET SDK, slower

## 📊 What Gets Created

### Tables (4)
1. `org.ActivityMonitoringRecords` - Main table
2. `org.ActivityMonitoringComplaints` - Child table
3. `org.ActivityMonitoringRealEstateViolations` - Child table
4. `org.ActivityMonitoringPetitionWriterViolations` - Child table

### Indexes (17)
- 5 on main table
- 4 on complaints table
- 4 on real estate violations table
- 4 on petition writer violations table

### Foreign Keys (3)
- Complaints → Records (CASCADE DELETE)
- RealEstateViolations → Records (CASCADE DELETE)
- PetitionWriterViolations → Records (CASCADE DELETE)

### Comments (30+)
- Table descriptions (Persian & English)
- Column descriptions (Persian)
- Field purpose documentation

## ✅ Verification Commands

### Check Tables
```sql
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name LIKE 'ActivityMonitoring%'
ORDER BY table_name;
```

### Check Indexes
```sql
SELECT indexname 
FROM pg_indexes 
WHERE schemaname = 'org' 
AND tablename LIKE 'ActivityMonitoring%'
ORDER BY tablename, indexname;
```

### Check Foreign Keys
```sql
SELECT
    tc.constraint_name,
    tc.table_name,
    kcu.column_name,
    ccu.table_name AS foreign_table_name,
    ccu.column_name AS foreign_column_name
FROM information_schema.table_constraints AS tc
JOIN information_schema.key_column_usage AS kcu
    ON tc.constraint_name = kcu.constraint_name
JOIN information_schema.constraint_column_usage AS ccu
    ON ccu.constraint_name = tc.constraint_name
WHERE tc.constraint_type = 'FOREIGN KEY'
AND tc.table_schema = 'org'
AND tc.table_name LIKE 'ActivityMonitoring%';
```

### Check Migration History
```sql
SELECT * FROM public."__EFMigrationsHistory" 
WHERE "MigrationId" = '20260121000001_ActivityMonitoring_Initial';
```

## 🔄 Rollback Process

1. **Backup first** (always!):
```bash
pg_dump -h localhost -U postgres -d prmis_db -n org -F c -f backup_before_rollback.dump
```

2. **Run rollback**:
```bash
psql -h localhost -U postgres -d prmis_db -f rollback_activity_monitoring.sql
```

3. **Verify**:
```sql
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name LIKE 'ActivityMonitoring%';
-- Should return 0
```

## 🐛 Common Issues

### Issue: "psql: command not found"
**Solution**: Install PostgreSQL client or add to PATH

### Issue: "permission denied"
**Solution**: 
```bash
chmod +x deploy_activity_monitoring.sh
```

### Issue: "schema org does not exist"
**Solution**: Run Shared module migration first:
```bash
psql -h localhost -U postgres -d prmis_db -f 01_Shared_Initial.sql
```

### Issue: "relation already exists"
**Solution**: Tables already exist - this is safe, script is idempotent

## 📈 Performance Notes

- All foreign keys have indexes
- Date fields are indexed for range queries
- Status field indexed for filtering
- Serial numbers indexed for lookups
- Expected to handle 100K+ records efficiently

## 🔒 Security Notes

- Uses parameterized queries (via EF Core)
- Soft delete (Status field) preserves audit trail
- CreatedBy/UpdatedBy tracks user actions
- Foreign keys enforce referential integrity
- CASCADE DELETE prevents orphaned records

## 📝 Maintenance

### Regular Tasks
- Monitor table sizes
- Analyze query performance
- Review indexes usage
- Archive old records (if needed)

### Monitoring Queries
```sql
-- Table sizes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'org'
AND tablename LIKE 'ActivityMonitoring%'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;

-- Row counts
SELECT 
    'ActivityMonitoringRecords' as table_name,
    COUNT(*) as row_count
FROM org."ActivityMonitoringRecords"
UNION ALL
SELECT 
    'ActivityMonitoringComplaints',
    COUNT(*)
FROM org."ActivityMonitoringComplaints"
UNION ALL
SELECT 
    'ActivityMonitoringRealEstateViolations',
    COUNT(*)
FROM org."ActivityMonitoringRealEstateViolations"
UNION ALL
SELECT 
    'ActivityMonitoringPetitionWriterViolations',
    COUNT(*)
FROM org."ActivityMonitoringPetitionWriterViolations";
```

## 🎯 Success Criteria

Migration is successful when:
- ✅ All 4 tables created
- ✅ All 17 indexes created
- ✅ All 3 foreign keys created
- ✅ Migration history entry added
- ✅ No errors in deployment log
- ✅ Verification queries return expected results

## 📞 Support

For issues:
1. Check deployment logs
2. Review error messages
3. Verify prerequisites
4. Check database permissions
5. Review migration README

---

**Module**: Activity Monitoring  
**Migration ID**: 20260121000001  
**Schema**: org  
**Tables**: 4  
**Status**: ✅ Production Ready
