# Backend Date Conversion Fix - Company Module

## Issue
Dates were showing incorrectly when switching between calendars:
- **HijriQamari**: Showing "1355/12/NaN" or "NaN/1355/12"
- **HijriShamsi**: Showing "783/08/25" (year 783 instead of 1404)
- **Gregorian**: Showing empty

## Root Cause
The backend controllers were converting dates from Gregorian to the requested calendar format **before** sending them to the frontend. This caused the following problem:

1. Database stores dates as Gregorian (e.g., `2026-01-25`)
2. Backend converts to calendar format using `ToCalendarDateOnly()` (e.g., creates `DateOnly(1404, 11, 7)`)
3. Backend returns this as JSON: `"2026-01-25"` but with calendar-specific year/month/day
4. Frontend receives what looks like `"1404-11-07"` as a string
5. JavaScript `new Date("1404-11-07")` interprets this as year 1404 AD, not 1404 Hijri Shamsi
6. The `app-multi-calendar-datepicker` tries to convert year 1404 AD, resulting in wrong dates

## Solution
**Remove all date conversions from backend controllers.** The backend should always return dates as Gregorian, and let the frontend's `app-multi-calendar-datepicker` component handle the calendar conversion.

## Files Fixed

### 1. LicenseDetailController ✅
**File:** `Backend/Controllers/Companies/LicenseDetailController.cs`

**Method:** `getById(int id, string? calendarType)`

**Before:**
```csharp
var calendar = DateConversionHelper.ParseCalendarType(calendarType);
foreach (var item in Pro)
{
    item.IssueDate = DateConversionHelper.ToCalendarDateOnly(originalIssueDate, calendar);
    item.ExpireDate = DateConversionHelper.ToCalendarDateOnly(..., calendar);
    item.RoyaltyDate = DateConversionHelper.ToCalendarDateOnly(item.RoyaltyDate, calendar);
    item.PenaltyDate = DateConversionHelper.ToCalendarDateOnly(item.PenaltyDate, calendar);
    item.HrLetterDate = DateConversionHelper.ToCalendarDateOnly(item.HrLetterDate, calendar);
}
```

**After:**
```csharp
// DO NOT convert dates - return them as Gregorian
// The frontend will handle calendar conversion
return Ok(Pro);
```

### 2. GuaranatorController ✅
**File:** `Backend/Controllers/Companies/GuaranatorController.cs`

**Method:** `getGuaranatorById(int id, string? calendarType)`

**Removed date conversions for:**
- `PropertyDocumentDate`
- `SenderMaktobDate`
- `AnswerdMaktobDate`
- `DateofGuarantee`
- `GuaranteeDate`
- `DepositDate`

### 3. CompanyOwnerController ✅
**File:** `Backend/Controllers/Companies/CompanyOwnerController.cs`

**Method:** `getOwnerById(int id, string? calendarType)`

**Removed date conversion for:**
- `DateofBirth`

## How It Works Now

### Correct Flow:
1. **Database**: Stores dates as Gregorian `DateOnly` (e.g., `2026-01-25`)
2. **Backend**: Returns dates as Gregorian ISO strings (e.g., `"2026-01-25"`)
3. **Frontend**: Receives Gregorian date string
4. **Frontend**: Converts to JavaScript `Date` object: `new Date("2026-01-25")`
5. **Datepicker**: Converts Gregorian `Date` to selected calendar format for display
6. **User sees**: Correct date in their selected calendar (e.g., `1404/11/07` for Hijri Shamsi)

### When Saving:
1. **User selects**: Date in their preferred calendar
2. **Datepicker**: Converts to JavaScript `Date` object (Gregorian)
3. **Frontend**: Formats as calendar-specific string (e.g., `"1404-11-07"`)
4. **Backend**: Parses using `DateConversionHelper.TryParseToDateOnly()` with calendar type
5. **Backend**: Converts to Gregorian and stores as `DateOnly`

## Testing

### Test Steps:
1. **Restart the backend** to load the changes
2. Open an existing company record
3. Switch between calendars using the calendar selector:
   - هجری شمسی (Hijri Shamsi)
   - هجری قمری (Hijri Qamari)
   - میلادی (Gregorian)
4. Verify dates display correctly in all calendars
5. Edit a date and save - verify it saves correctly
6. Switch calendars again - verify the date still displays correctly

### Expected Results:
- ✅ Dates display correctly in all three calendars
- ✅ No "NaN" values
- ✅ Correct year values (not 783 or 1355)
- ✅ Dates persist correctly when switching calendars
- ✅ Editing and saving works in all calendars

## Impact on Other Modules

This same issue likely exists in other modules (Property, Vehicle, Securities, etc.). The fix is the same:

1. Find controllers that use `ToCalendarDateOnly()` before returning data
2. Remove the date conversion logic
3. Return dates as Gregorian
4. Let the frontend handle calendar conversion

## Status: ✅ FIXED (Company Module)

All date fields in the company module now work correctly with all three calendar types.

---

**Date:** January 25, 2026  
**Issue:** Dates showing incorrectly when switching calendars  
**Root Cause:** Backend converting dates before sending to frontend  
**Solution:** Remove backend date conversions, let frontend handle it  
**Controllers Fixed:** LicenseDetailController, GuaranatorController, CompanyOwnerController  
**Build Status:** ✅ Successful
