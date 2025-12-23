# Multi-Calendar Date Support Implementation Summary

## Overview
This document summarizes the implementation of multi-calendar date support across the Property Management System, enabling users to work with three calendar systems:
1. **Hijri Shamsi (هجری شمسی)** - Afghan/Persian Calendar (Default)
2. **Hijri Qamari (هجری قمری)** - Islamic Calendar
3. **Gregorian (میلادی)** - Western Calendar

---

## ✅ Completed Tasks

### 1. **Frontend Infrastructure**

#### **Installed Packages**
- `moment-hijri`: For Hijri Qamari (Islamic) calendar conversions
- `moment-jalaali`: For Hijri Shamsi (Persian/Afghan) calendar conversions

#### **Created Core Services**

**a) CalendarService** (`Frontend/src/app/shared/calendar.service.ts`)
- Manages global calendar selection state
- Persists user's calendar preference in localStorage
- Provides observable for calendar changes across the application
- Default calendar: Hijri Shamsi (هجری شمسی)

**b) CalendarConversionService** (`Frontend/src/app/shared/calendar-conversion.service.ts`)
- Converts dates between all three calendar systems
- Validates dates for each calendar type
- Formats dates for display and input
- Provides month names in appropriate languages

#### **Created Components**

**a) MultiCalendarDatepickerComponent** (`Frontend/src/app/shared/multi-calendar-datepicker/`)
- Reusable date input component
- Automatically switches display based on selected calendar
- Implements ControlValueAccessor for seamless Angular Forms integration
- Features:
  - Auto-converts dates when calendar type changes
  - Validates input based on calendar type
  - Shows calendar type indicator
  - Works with both reactive and template-driven forms

**b) CalendarSelectorComponent** (`Frontend/src/app/shared/calendar-selector/`)
- Global calendar type selector
- Added to main navigation toolbar
- Allows users to switch calendars application-wide
- Persists selection across sessions

#### **Updated Components**
- **Shared Module**: Exports calendar components for use across the application
- **Master Layout**: Integrated calendar selector in the toolbar
- **Buyer Detail Forms** (Estate & Vehicle): Replaced date inputs with multi-calendar picker
- **Property Details Form**: Replaced issuance date and transaction date inputs

---

### 2. **Backend Infrastructure**

#### **Installed Packages**
- `Persia.Net` (v4.3.0): Comprehensive .NET library for Persian, Gregorian, and Hijri calendar conversions

#### **Created Helper Classes**

**a) CalendarType Enum** (`Backend/Helpers/CalendarType.cs`)
```csharp
public enum CalendarType
{
    Gregorian,
    HijriShamsi,
    HijriQamari
}
```

**b) DateConversionHelper** (`Backend/Helpers/DateConversionHelper.cs`)
- `ToGregorian()`: Converts from any calendar to Gregorian
- `FromGregorian()`: Converts from Gregorian to any calendar
- `FormatDate()`: Formats dates for display
- `ParseDateString()`: Parses date strings
- `IsValidDate()`: Validates dates
- `GetMonthName()`: Returns localized month names

---

## 🔄 Data Flow Architecture

### **Storage Strategy**
- **Database**: All dates stored as Gregorian DateTime (ISO standard)
- **Frontend Display**: Dates converted to user-selected calendar
- **User Input**: Dates accepted in any calendar, converted to Gregorian before sending to backend

### **Conversion Flow**
```
User Input (Any Calendar) 
  → Frontend Conversion Service 
  → Gregorian Date 
  → Backend (stores as DateTime) 
  → Database

Database 
  → Backend (retrieves DateTime) 
  → Frontend Conversion Service 
  → Display in Selected Calendar
```

---

## 📝 Pending Tasks

### **High Priority**

1. **Update Additional Forms**
   - Search for remaining `type="date"` inputs in other modules
   - Company Owner: DateofBirth field
   - Company Details: PetitionDate field
   - License forms and other date fields
   - Report date filters

2. **Test All Scenarios**
   - Date entry in each calendar type
   - Date validation for invalid dates
   - Calendar switching with existing data
   - Form submission and data persistence
   - Date comparison/validation (e.g., end date > start date)

3. **Update Report Filters**
   - Search/filter components using date ranges
   - Report generation with date parameters

### **Medium Priority**

4. **Database Schema Review**
   - Current implementation doesn't require schema changes (stores Gregorian)
   - Consider adding calendar type metadata column if needed for audit purposes

5. **API Endpoint Updates**
   - Review if any API endpoints need explicit calendar type handling
   - Most should work automatically with current implementation

6. **Error Handling**
   - Add user-friendly error messages for invalid dates
   - Handle edge cases (leap years, month boundaries, etc.)

### **Low Priority**

7. **UI Enhancements**
   - Add calendar-specific date picker widgets (optional)
   - Improve mobile responsiveness
   - Add tooltips explaining calendar types

8. **Documentation**
   - User guide for calendar functionality
   - API documentation updates

---

## 🎯 Usage Guide

### **For Users**

1. **Select Calendar Type**
   - Use the calendar selector in the navigation bar
   - Choose between هجری شمسی (default), هجری قمری, or میلادی
   - Selection persists across sessions

2. **Enter Dates**
   - Date fields automatically adapt to selected calendar
   - Format:
     - **Gregorian**: YYYY-MM-DD (date picker)
     - **Hijri Shamsi**: YYYY/MM/DD (text input)
     - **Hijri Qamari**: YYYY/MM/DD (text input)

3. **View Data**
   - All displayed dates automatically convert to selected calendar
   - Switch calendar anytime to view dates in different format

### **For Developers**

#### **Using the Multi-Calendar Date Picker**

**In HTML Template:**
```html
<app-multi-calendar-datepicker
  formControlName="rentStartDate"
  label="تاریخ شروع"
  [required]="true">
</app-multi-calendar-datepicker>
```

**In Component:**
```typescript
this.form = this.fb.group({
  rentStartDate: ['', Validators.required]
});
```

#### **Manual Date Conversion**

**Frontend:**
```typescript
// Inject the service
constructor(private conversionService: CalendarConversionService) {}

// Convert to Gregorian
const gregorianDate = this.conversionService.toGregorian({
  year: 1403,
  month: 10,
  day: 1,
  calendarType: CalendarType.HIJRI_SHAMSI
});

// Convert from Gregorian
const shamsiDate = this.conversionService.fromGregorian(
  new Date(), 
  CalendarType.HIJRI_SHAMSI
);

// Format for display
const formatted = this.conversionService.formatDate(
  new Date(),
  CalendarType.HIJRI_SHAMSI
); // Returns: "1403/10/01"
```

**Backend:**
```csharp
using WebAPIBackend.Helpers;

// Convert to Gregorian
var gregorianDate = DateConversionHelper.ToGregorian(1403, 10, 1, CalendarType.HijriShamsi);

// Convert from Gregorian
var (year, month, day) = DateConversionHelper.FromGregorian(
    DateTime.Now, 
    CalendarType.HijriShamsi
);

// Format date
string formatted = DateConversionHelper.FormatDate(DateTime.Now, CalendarType.HijriShamsi);
```

---

## 🧪 Testing Checklist

- [ ] Test date entry in Hijri Shamsi calendar
- [ ] Test date entry in Hijri Qamari calendar
- [ ] Test date entry in Gregorian calendar
- [ ] Test switching calendars with existing dates
- [ ] Test form validation with invalid dates
- [ ] Test date range validation (start < end)
- [ ] Test data persistence to database
- [ ] Test data retrieval and display
- [ ] Test calendar selector persistence
- [ ] Test across all modules (Property, Vehicle, Company)
- [ ] Test report generation with dates
- [ ] Test date filtering in search
- [ ] Test mobile responsiveness
- [ ] Test with different browsers

---

## 🐛 Known Issues / Considerations

1. **Leap Year Handling**: Automatically handled by moment libraries and .NET Calendar classes
2. **Month Boundaries**: Each calendar has different month lengths - validation prevents invalid dates
3. **Year Range**: Consider adding min/max year validation for each calendar
4. **Performance**: Date conversion is fast, but consider caching for large datasets
5. **Timezone**: All dates stored as UTC in database (verify server timezone settings)

---

## 📚 Additional Resources

- **Moment Jalaali**: https://github.com/jalaali/moment-jalaali
- **Moment Hijri**: https://github.com/xsoh/moment-hijri
- **Persia.Net**: https://github.com/shahabfar/persia.net
- **Persian Calendar Info**: https://en.wikipedia.org/wiki/Solar_Hijri_calendar
- **Islamic Calendar Info**: https://en.wikipedia.org/wiki/Islamic_calendar

---

## 🚀 Next Steps

1. Run the application and test the calendar functionality
2. Search for remaining date fields using:
   ```bash
   grep -r "type=\"date\"" Frontend/src/
   grep -r "DateTime" Backend/Models/
   ```
3. Update any remaining date inputs
4. Test thoroughly across all three calendars
5. Update reports and search filters
6. Deploy and train users

---

## 📞 Support

For issues or questions about the multi-calendar implementation:
1. Check this documentation
2. Review the code comments in service files
3. Test with the implementation examples above
4. Verify calendar conversion accuracy with online converters

---

**Implementation Date**: December 2025  
**Status**: Core functionality implemented, testing and refinement needed  
**Next Review**: After initial testing phase
