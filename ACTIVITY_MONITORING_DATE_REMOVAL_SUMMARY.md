# Activity Monitoring - Violation Date Fields Removal

## Summary
Removed `تاریخ ثبت تخلف` (ViolationDate) and `تاریخ مسدودی` (ClosureDate) fields from the Activity Monitoring violations section (۳. تخلفات دفاتر رهنمای معاملات).

## Date: April 28, 2026

## Changes Made

### 1. Frontend Changes

#### A. Form Component HTML (`Frontend/src/app/activity-monitoring/activity-monitoring-form/activity-monitoring-form.component.html`)
- ✅ Removed `<app-multi-calendar-datepicker formControlName="closureDate">` from "منجر به مسدودی" section
- ✅ Removed `<app-multi-calendar-datepicker formControlName="violationDate">` from "عادی" section
- ✅ Now only shows:
  - **منجر به مسدودی**: علت مسدودی (Closure Reason)
  - **عادی**: نوعیت تخلف (Violation Type)

#### B. Form Component TypeScript (`Frontend/src/app/activity-monitoring/activity-monitoring-form/activity-monitoring-form.component.ts`)
- ✅ Removed `violationDate: ['']` from form group initialization
- ✅ Removed `closureDate: ['']` from form group initialization
- ✅ Updated `clearAllConditionalValidators()` - removed date field validators
- ✅ Updated `onViolationStatusChange()` - removed date field validators
- ✅ Updated `saveForm()` - removed date formatting calls
- ✅ Updated `patchForm()` - removed date parsing logic

#### C. TypeScript Model (`Frontend/src/app/models/ActivityMonitoring.ts`)
- ✅ Removed `violationDate?: Date | string;` from `ActivityMonitoringRecord` interface
- ✅ Removed `violationDateFormatted?: string;` from `ActivityMonitoringRecord` interface
- ✅ Removed `closureDate?: Date | string;` from `ActivityMonitoringRecord` interface
- ✅ Removed `closureDateFormatted?: string;` from `ActivityMonitoringRecord` interface
- ✅ Removed `violationDate?: string;` from `ActivityMonitoringData` interface
- ✅ Removed `closureDate?: string;` from `ActivityMonitoringData` interface

#### D. View Component HTML (`Frontend/src/app/activity-monitoring/activity-monitoring-view/activity-monitoring-view.component.html`)
- ✅ Removed display card for `تاریخ تخلف` (Violation Date)
- ✅ Removed display card for `تاریخ توقف` (Closure Date)
- ✅ Kept display for `نوعیت تخلف` (Violation Type) and `دلیل توقف` (Closure Reason)

### 2. Backend Changes

#### A. Entity Model (`Backend/Models/ActivityMonitoring/ActivityMonitoringRecord.cs`)
- ✅ Removed `public DateOnly? ViolationDate { get; set; }` property
- ✅ Removed `public DateOnly? ClosureDate { get; set; }` property
- ✅ Removed `[Column("ViolationDate")]` attribute
- ✅ Removed `[Column("ClosureDate")]` attribute

#### B. DTO (`Backend/Models/RequestData/ActivityMonitoring/ActivityMonitoringData.cs`)
- ✅ Removed `public string? ViolationDate { get; set; }` property
- ✅ Removed `public string? ClosureDate { get; set; }` property

#### C. Controller (`Backend/Controllers/ActivityMonitoring/ActivityMonitoringController.cs`)
- ✅ **POST endpoint**: Removed date parsing for `ViolationDate` and `ClosureDate`
- ✅ **POST endpoint**: Removed assignment of `ViolationDate` and `ClosureDate` to entity
- ✅ **PUT endpoint**: Removed date parsing for `ViolationDate` and `ClosureDate`
- ✅ **PUT endpoint**: Removed assignment of `ViolationDate` and `ClosureDate` to entity
- ✅ **GET (list) endpoint**: Removed `ViolationDate`, `ViolationDateFormatted`, `ClosureDate`, `ClosureDateFormatted` from select projection
- ✅ **GET (by ID) endpoint**: Removed `ViolationDate`, `ViolationDateFormatted`, `ClosureDate`, `ClosureDateFormatted` from select projection

### 3. Database Changes

#### A. Migration Script (`Backend/Scripts/RemoveViolationDateFields.sql`)
- ✅ Created new SQL script to drop columns from database
- ✅ Drops `ViolationDate` column from `org.ActivityMonitoringRecords` table
- ✅ Drops `ClosureDate` column from `org.ActivityMonitoringRecords` table
- ✅ Includes verification query to confirm removal

#### B. Table Creation Scripts
- ✅ Updated `Backend/Scripts/activity_monitoring_single_table.sql` - removed date columns
- ✅ Updated `Backend/Scripts/activity_monitoring_single_table.sql` - removed ViolationDate index
- ✅ Updated `Backend/Scripts/activity_monitoring_single_table_migration.sql` - removed date columns
- ✅ Updated `Backend/Scripts/activity_monitoring_single_table_migration.sql` - removed ViolationDate index

## Deployment Steps

### 1. Database Migration
Run the following SQL script on the production database:
```bash
psql -U your_user -d your_database -f Backend/Scripts/RemoveViolationDateFields.sql
```

### 2. Backend Deployment
- Deploy the updated backend code
- The API will no longer accept or return `violationDate` and `closureDate` fields

### 3. Frontend Deployment
- Deploy the updated frontend code
- The form will no longer display date pickers for violations section

## Testing Checklist

- [ ] Verify form loads correctly for new violations records
- [ ] Verify form loads correctly for existing violations records (edit mode)
- [ ] Test "منجر به مسدودی" status - should only show closure reason field
- [ ] Test "عادی" status - should only show violation type field
- [ ] Verify save functionality works for new records
- [ ] Verify update functionality works for existing records
- [ ] Verify view component displays correctly without date fields
- [ ] Verify list component displays correctly
- [ ] Check database columns are dropped successfully
- [ ] Verify no console errors in browser
- [ ] Verify no API errors in backend logs

## Rollback Plan

If issues arise, the rollback involves:
1. Restore previous frontend and backend code
2. Re-add database columns using:
```sql
ALTER TABLE org."ActivityMonitoringRecords"
ADD COLUMN "ViolationDate" DATE NULL,
ADD COLUMN "ClosureDate" DATE NULL;
```

## Notes

- The `closureReason` field remains for "منجر به مسدودی" status
- The `violationType` field remains for "عادی" status
- The `violationStatus` field remains to determine which fields to show
- All other violation fields (actionsTaken, remarks) remain unchanged
- This change only affects the Activity Monitoring violations section
- Other modules with similar field names (e.g., Property Violations) are NOT affected
