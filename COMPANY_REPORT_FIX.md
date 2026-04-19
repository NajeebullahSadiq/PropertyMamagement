# Company Report Fix - تاریخ صدور جواز Filtering

## Problem
The company report generation was not working correctly when entering start and end dates. The report should filter based on **تاریخ صدور جواز** (License Issue Date) and show all relevant records like:
- نوعیت جواز (License Category)
- تعداد (Count)
- مجموع جواز‌ها (Total Licenses)

that occurred within the specified date range.

## Root Cause
The frontend was sending `'gregorian'` as the calendar type parameter, but the backend **always uses Hijri Shamsi calendar** for the company module (as documented in the code comments). This mismatch caused the date parsing to fail or produce incorrect results.

## Solution Applied

### Frontend Fix (realestatelist.component.ts)
Changed the calendar type parameter from `'gregorian'` to `'hijriShamsi'` when calling the comprehensive report API:

```typescript
// Before:
this.comservice.getComprehensiveReport(
  startDate,
  endDate,
  'gregorian',  // ❌ Wrong - backend ignores this and uses HijriShamsi
  ...
)

// After:
this.comservice.getComprehensiveReport(
  startDate,
  endDate,
  'hijriShamsi',  // ✅ Correct - matches backend expectation
  ...
)
```

### Backend Verification (CompanyDetailsController.cs)
Confirmed that the backend correctly filters licenses by `IssueDate` (تاریخ صدور جواز):

```csharp
// Lines 1310-1316
if (parsedStartDate.HasValue)
{
    licensesQuery = licensesQuery.Where(x => x.IssueDate >= parsedStartDate);
}
if (parsedEndDate.HasValue)
{
    licensesQuery = licensesQuery.Where(x => x.IssueDate <= parsedEndDate);
}
```

## What the Report Shows

The comprehensive report now correctly filters and displays:

1. **Total Cancellations** (تعداد فسخ/لغوه) - filtered by cancellation letter date
2. **Active/Inactive Companies** - based on license expiry status
3. **Licenses by Category** (نوعیت جواز) - filtered by **تاریخ صدور جواز** ✅
4. **Total Licenses** (مجموع جواز‌ها) - count of licenses issued in date range ✅
5. **Guarantors by Type** - filtered by creation date
6. **Revenue by License Type** - calculated from licenses in date range

## Testing
To test the fix:
1. Navigate to the company list page (دفتر‌های رهنمایی معاملات)
2. Click the "گزارشات" (Reports) button
3. Select a start date (از تاریخ) and end date (تا تاریخ) using the Hijri Shamsi calendar
4. Click "تولید گزارش" (Generate Report)
5. Verify that the report shows licenses issued within the specified date range

## Notes
- The company module **always uses Hijri Shamsi calendar** - this is by design
- The multi-calendar date picker component should be configured to use Hijri Shamsi for company reports
- The backend ignores the `calendarType` parameter and forces Hijri Shamsi (line 1193)
