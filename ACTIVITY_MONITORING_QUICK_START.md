# Activity Monitoring Module - Quick Start Guide

## 🚀 Quick Deployment (5 Minutes)

### Step 1: Deploy Database Migration

Choose your platform:

#### Windows:
```cmd
cd Backend\Scripts\Modules
set PGPASSWORD=your_password
deploy_activity_monitoring.bat
```

#### Linux/Mac:
```bash
cd Backend/Scripts/Modules
export PGPASSWORD=your_password
chmod +x deploy_activity_monitoring.sh
./deploy_activity_monitoring.sh
```

#### Or use psql directly:
```bash
psql -h localhost -U postgres -d prmis_db -f Backend/Scripts/Modules/11_ActivityMonitoring_Initial.sql
```

### Step 2: Verify Database

```sql
-- Check tables were created
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'org' 
AND table_name LIKE 'ActivityMonitoring%';

-- Should return 4 tables:
-- ActivityMonitoringRecords
-- ActivityMonitoringComplaints
-- ActivityMonitoringRealEstateViolations
-- ActivityMonitoringPetitionWriterViolations
```

### Step 3: Build & Run Application

```bash
# Backend (if needed)
cd Backend
dotnet build
dotnet run

# Frontend
cd Frontend
npm install  # if first time
npm start
```

### Step 4: Access the Module

Navigate to: `http://localhost:4200/activity-monitoring/list`

Or use the menu: **نظارت بر فعالیت‌ها** → **جدول نظارت بر فعالیت‌ها**

## 📋 Module Features

### Main Form (Unified)
- **Tab 1**: Main Information
  - Section 1: Financial Clearance (Tax Compliance)
  - Section 2: Annual Activity Report
  - Section 6: Inspection & Supervision Summary
- **Tab 2**: Complaints Registration
- **Tab 3**: Real Estate Office Violations
- **Tab 4**: Petition Writer Violations

### List View
- Pagination & search
- View, Edit, Delete actions
- Role-based access control

### View Page
- Read-only display
- Print functionality
- All sections visible

## 🔐 Permissions

| Role | Create | Edit | View | Delete |
|------|--------|------|------|--------|
| Admin | ✅ | ✅ | ✅ | ✅ |
| CompanyRegistrar | ✅ | ✅ | ✅ | ❌ |
| Authority | ❌ | ❌ | ✅ | ❌ |
| LicenseReviewer | ❌ | ❌ | ✅ | ❌ |

## 🧪 Testing Checklist

- [ ] Create new record (main form)
- [ ] Add complaints
- [ ] Add real estate violations
- [ ] Add petition writer violations
- [ ] Edit existing record
- [ ] View record details
- [ ] Print record
- [ ] Search in list
- [ ] Pagination works
- [ ] Delete record (Admin only)
- [ ] Test with different user roles

## 📊 Database Tables

```
org.ActivityMonitoringRecords (main)
├── org.ActivityMonitoringComplaints (1:N)
├── org.ActivityMonitoringRealEstateViolations (1:N)
└── org.ActivityMonitoringPetitionWriterViolations (1:N)
```

## 🔧 Troubleshooting

### Tables not created?
```sql
-- Check if migration ran
SELECT * FROM public."__EFMigrationsHistory" 
WHERE "MigrationId" LIKE '%ActivityMonitoring%';
```

### Frontend not showing module?
1. Check `app-routing.module.ts` has the route
2. Check `masterlayout.component.html` has menu items
3. Clear browser cache
4. Restart Angular dev server

### API errors?
1. Check `AppDbContext.cs` has DbSets
2. Rebuild backend: `dotnet build`
3. Check API is running: `http://localhost:5000/api/ActivityMonitoring`

## 📝 Sample Data

```sql
-- Insert sample record
INSERT INTO org."ActivityMonitoringRecords" (
    "LicenseHolderName",
    "TaxClearanceDate",
    "InspectionDate",
    "Status",
    "CreatedAt",
    "CreatedBy"
) VALUES (
    'محمد احمدی',
    '2026-01-15',
    '2026-01-20',
    TRUE,
    NOW(),
    'admin'
);
```

## 🔄 Rollback (if needed)

⚠️ **WARNING**: This deletes all data!

```bash
psql -h localhost -U postgres -d prmis_db -f Backend/Scripts/Modules/rollback_activity_monitoring.sql
```

## 📚 Documentation

- Full Implementation: `ACTIVITY_MONITORING_IMPLEMENTATION_SUMMARY.md`
- Migration Guide: `Backend/Scripts/Modules/ACTIVITY_MONITORING_MIGRATION_README.md`
- API Endpoints: Check `ActivityMonitoringController.cs`

## 🎯 API Endpoints

### Main Record
- `GET /api/ActivityMonitoring` - List all
- `GET /api/ActivityMonitoring/{id}` - Get by ID
- `POST /api/ActivityMonitoring` - Create
- `PUT /api/ActivityMonitoring/{id}` - Update
- `DELETE /api/ActivityMonitoring/{id}` - Delete

### Complaints
- `GET /api/ActivityMonitoring/{recordId}/complaints`
- `POST /api/ActivityMonitoring/{recordId}/complaints`
- `PUT /api/ActivityMonitoring/{recordId}/complaints/{id}`
- `DELETE /api/ActivityMonitoring/{recordId}/complaints/{id}`

### Real Estate Violations
- `GET /api/ActivityMonitoring/{recordId}/realestate-violations`
- `POST /api/ActivityMonitoring/{recordId}/realestate-violations`
- `PUT /api/ActivityMonitoring/{recordId}/realestate-violations/{id}`
- `DELETE /api/ActivityMonitoring/{recordId}/realestate-violations/{id}`

### Petition Writer Violations
- `GET /api/ActivityMonitoring/{recordId}/petitionwriter-violations`
- `POST /api/ActivityMonitoring/{recordId}/petitionwriter-violations`
- `PUT /api/ActivityMonitoring/{recordId}/petitionwriter-violations/{id}`
- `DELETE /api/ActivityMonitoring/{recordId}/petitionwriter-violations/{id}`

## ✅ Success Indicators

You'll know it's working when:
1. ✅ 4 tables exist in `org` schema
2. ✅ Menu items appear in navigation
3. ✅ List page loads without errors
4. ✅ Can create and save a record
5. ✅ Can add child entities (complaints, violations)
6. ✅ View page displays all data
7. ✅ Print functionality works

## 🆘 Need Help?

1. Check console for errors (F12 in browser)
2. Check backend logs
3. Verify database connection
4. Review migration README
5. Check all files were created correctly

---

**Module**: Activity Monitoring  
**Status**: ✅ Ready for Production  
**Version**: 1.0.0  
**Date**: January 21, 2026
