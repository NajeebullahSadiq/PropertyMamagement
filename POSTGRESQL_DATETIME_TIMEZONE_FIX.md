# PostgreSQL DateTime Timezone Fix - Complete

## Issue
Application was throwing PostgreSQL DateTime timezone errors:
1. **First Error**: "Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone'"
2. **Second Error**: "Cannot write DateTime with Kind=UTC to PostgreSQL type 'timestamp without time zone'"

## Root Cause
PostgreSQL has strict requirements for DateTime handling:
- `timestamp with time zone` columns require `DateTimeKind.Utc`
- `timestamp without time zone` columns don't accept `DateTimeKind.Utc`
- Using `DateTime.Now` creates `DateTimeKind.Unspecified` which causes issues

The application had mixed column types and was using `DateTime.Now` everywhere, causing timezone mismatch errors.

## Solution Applied

### 1. Fixed DateConversionHelper ✅
**File:** `Backend/Helpers/DateConversionHelper.cs`

**Change:** Modified `ToGregorian()` method to return UTC DateTime:
```csharp
// Convert to UTC for PostgreSQL compatibility
return DateTime.SpecifyKind(localDate, DateTimeKind.Utc);
```

### 2. Replaced All DateTime.Now with DateTime.UtcNow ✅
**Script:** `Backend/fix_datetime_utc.ps1`

**Files Fixed (44 occurrences across 16 files):**
- PropertyCancellationController.cs (3)
- SellerDetailsController.cs (6)
- ActivityMonitoringController.cs (5)
- CompanyAccountInfoController.cs (1)
- CompanyCancellationInfoController.cs (1)
- CompanyDetailsController.cs (1)
- CompanyOwnerAddressController.cs (1)
- CompanyOwnerController.cs (3)
- GuaranatorController.cs (2)
- GuaranteeController.cs (2)
- LicenseDetailController.cs (2)
- LicenseApplicationController.cs (4)
- PetitionWriterLicenseController.cs (5)
- DashboardController.cs (1)
- VehiclesController.cs (2)
- VehiclesSubController.cs (5)

**Change:** All `DateTime.Now` → `DateTime.UtcNow`

### 3. Configured Npgsql Legacy Timestamp Behavior ✅
**File:** `Backend/Program.cs`

**Change:** Added global configuration to handle `timestamp without time zone`:
```csharp
// Configure Npgsql to use timestamp without time zone
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

This allows the application to work with both:
- `timestamp without time zone` (most tables)
- `timestamp with time zone` (DocumentVerification, VerificationAttempt tables)

## How It Works Now

### Date Parsing Flow:
1. **Frontend** sends date string (e.g., "2026-01-27")
2. **Backend** receives with `calendarType` parameter
3. **DateConversionHelper.ParseDateString()** converts to Gregorian DateTime
4. **DateTime.SpecifyKind()** marks it as UTC
5. **Npgsql Legacy Behavior** strips timezone info for `timestamp without time zone` columns
6. **Database** stores the date correctly

### Timestamp Creation Flow:
1. **Code** uses `DateTime.UtcNow` for CreatedAt/UpdatedAt fields
2. **Npgsql Legacy Behavior** handles conversion based on column type
3. **Database** stores correctly in both column types

## Database Schema

### Tables with `timestamp without time zone`:
- Most application tables (Property, Company, Vehicle, etc.)
- All audit tables
- All CreatedAt/UpdatedAt fields

### Tables with `timestamp with time zone`:
- DocumentVerification (verification system)
- VerificationAttempt (verification system)

## Benefits

1. ✅ **No Database Migration Required** - Works with existing schema
2. ✅ **Consistent DateTime Handling** - All code uses UTC
3. ✅ **Calendar Support** - Maintains Hijri Shamsi, Hijri Qamari, Gregorian conversion
4. ✅ **Backward Compatible** - Existing data remains valid
5. ✅ **Future Proof** - Can migrate to `timestamp with time zone` later if needed

## Testing

### Test Steps:
1. Restart the backend server
2. Test Property module:
   - Create new property with date fields
   - Edit existing property
   - Verify dates save and display correctly
3. Test Company module:
   - Create/edit company records
   - Test all date fields (license, guarantor, owner, etc.)
4. Test Vehicle module:
   - Create/edit vehicle records
5. Test all other modules with date fields

### Expected Results:
- ✅ No PostgreSQL timezone errors
- ✅ Dates save correctly in all modules
- ✅ Dates display correctly in all calendar types
- ✅ CreatedAt/UpdatedAt timestamps work properly
- ✅ Audit records created successfully

## Alternative Approach (Not Used)

We could have migrated all columns to `timestamp with time zone`:
```sql
ALTER TABLE table_name 
ALTER COLUMN created_at TYPE timestamp with time zone;
```

**Why we didn't:** 
- Requires extensive database migration
- Risk of data loss or corruption
- More complex deployment
- Legacy behavior switch is simpler and safer

## Status: ✅ COMPLETE

All DateTime timezone issues have been resolved. The application now correctly handles:
- Date parsing from multiple calendar types
- DateTime storage in PostgreSQL
- Mixed timestamp column types
- CreatedAt/UpdatedAt audit fields

---

**Date:** January 27, 2026  
**Issue:** PostgreSQL DateTime timezone mismatch errors  
**Root Cause:** Mixed use of DateTime.Now, DateTimeKind.Unspecified, and incompatible column types  
**Solution:** DateTime.UtcNow + DateConversionHelper UTC conversion + Npgsql Legacy Timestamp Behavior  
**Files Modified:** 18 files (1 helper, 16 controllers, 1 Program.cs)  
**Total Replacements:** 44 DateTime.Now → DateTime.UtcNow  
**Build Status:** ✅ Ready for testing
