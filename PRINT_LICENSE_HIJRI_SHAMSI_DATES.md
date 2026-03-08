# Print License - Always Use Hijri Shamsi Dates

## Overview
Modified the company license print functionality to ALWAYS display dates in Hijri Shamsi (Solar Hijri/Jalali) calendar format, regardless of the calendar type stored in the database.

## Problem
Previously, the print license was using the `DateType` field from the license record to determine which calendar format to display dates in. This meant that if a license was created with Gregorian or Hijri Qamari dates, the print would show dates in that format.

## Solution
Updated the backend to always format dates in Hijri Shamsi calendar for the print license, ensuring consistency across all printed licenses.

## Changes Made

### Backend Changes

#### File: `Backend/Controllers/Companies/LicenseDetailController.cs`

**Modified the `GetLicensePrintData` endpoint:**

```csharp
// BEFORE:
var calendar = DateConversionHelper.ParseCalendarType(calendarType);

// Use the stored DateType if available, otherwise fall back to the query parameter
var storedDateType = licenseDetail?.DateType;
var dateCalendar = !string.IsNullOrEmpty(storedDateType) 
    ? DateConversionHelper.ParseCalendarType(storedDateType) 
    : calendar;

// Format dates for the requested calendar
string issueDateFormatted = data.IssueDate.HasValue 
    ? DateConversionHelper.FormatDateOnly(data.IssueDate, dateCalendar) 
    : "";
```

```csharp
// AFTER:
var calendar = DateConversionHelper.ParseCalendarType(calendarType);

// ALWAYS use Hijri Shamsi (Solar Hijri) calendar for print license dates
var printCalendar = CalendarType.HijriShamsi;

// Format dates for print - always in Hijri Shamsi
string issueDateFormatted = data.IssueDate.HasValue 
    ? DateConversionHelper.FormatDateOnly(data.IssueDate, printCalendar) 
    : "";
```

**All affected date fields:**
- `issueDateFormatted` - License issue date
- `expireDateFormatted` - License expiry date
- `dateOfBirthFormatted` - Owner's date of birth
- `hrLetterDateFormatted` - HR letter date
- `royaltyDateFormatted` - Royalty date
- `penaltyDateFormatted` - Penalty date

## Impact

### Positive Changes:
1. **Consistency**: All printed licenses now show dates in the same format (Hijri Shamsi)
2. **Compliance**: Meets the requirement for official documents to use the Solar Hijri calendar
3. **User Experience**: Users don't need to worry about which calendar was used during data entry

### No Breaking Changes:
- The database still stores dates in their original format
- The `DateType` field is preserved for other purposes
- Other parts of the application continue to work as before
- Only the print output is affected

## Date Format
All dates in the print license are now displayed in the format: `YYYY/MM/DD` using the Hijri Shamsi (Solar Hijri/Jalali) calendar.

Example:
- Gregorian: 2024-03-08
- Hijri Shamsi: 1402/12/18

## Testing Checklist

- [ ] Print a license that was created with Gregorian dates
- [ ] Print a license that was created with Hijri Shamsi dates
- [ ] Print a license that was created with Hijri Qamari dates
- [ ] Verify all dates display in Hijri Shamsi format
- [ ] Verify date format is consistent across both Pashto and Dari versions
- [ ] Verify the printed dates match the expected Hijri Shamsi conversion

## Notes

- The `calendar` variable is still used for other date fields that may need different calendar types
- The `printCalendar` variable is specifically set to `CalendarType.HijriShamsi` for all print-related date formatting
- This change only affects the `GetLicensePrintData` endpoint used for printing licenses
- Other endpoints and functionality remain unchanged
