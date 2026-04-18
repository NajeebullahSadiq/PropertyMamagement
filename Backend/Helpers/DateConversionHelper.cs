using System;
using System.Globalization;

namespace WebAPIBackend.Helpers
{
    public static class DateConversionHelper
    {
        /// <summary>
        /// Parse calendar type string to CalendarType enum, defaults to HijriShamsi
        /// </summary>
        public static CalendarType ParseCalendarType(string? calendarTypeStr)
        {
            if (string.IsNullOrWhiteSpace(calendarTypeStr))
                return CalendarType.HijriShamsi;

            return calendarTypeStr.ToLowerInvariant() switch
            {
                "gregorian" => CalendarType.Gregorian,
                "hijrishamsi" => CalendarType.HijriShamsi,
                "hijriqamari" => CalendarType.HijriQamari,
                _ => CalendarType.HijriShamsi
            };
        }

        /// <summary>
        /// Try to parse a date string using the specified calendar type and convert to DateOnly (Gregorian)
        /// </summary>
        public static bool TryParseToDateOnly(string? input, CalendarType calendarType, out DateOnly result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var gregorianDate = ParseDateString(input, calendarType);
            if (gregorianDate.HasValue)
            {
                result = DateOnly.FromDateTime(gregorianDate.Value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to parse a date string using the specified calendar type string and convert to DateOnly (Gregorian)
        /// </summary>
        public static bool TryParseToDateOnly(string? input, string? calendarTypeStr, out DateOnly result)
        {
            var calendarType = ParseCalendarType(calendarTypeStr);
            return TryParseToDateOnly(input, calendarType, out result);
        }

        /// <summary>
        /// Try to parse a date string with automatic calendar detection fallback
        /// First tries the specified calendar, then falls back to Gregorian if that fails
        /// </summary>
        public static bool TryParseToDateOnlyFlexible(string? input, string? calendarTypeStr, out DateOnly result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(input)) return false;

            var calendarType = ParseCalendarType(calendarTypeStr);
            
            // First try with the specified calendar type
            if (TryParseToDateOnly(input, calendarType, out result))
            {
                return true;
            }

            // If that fails and the specified type wasn't Gregorian, try Gregorian as fallback
            if (calendarType != CalendarType.Gregorian)
            {
                if (TryParseToDateOnly(input, CalendarType.Gregorian, out result))
                {
                    return true;
                }
            }

            // If still failing, try all calendar types
            foreach (var fallbackType in new[] { CalendarType.HijriShamsi, CalendarType.HijriQamari })
            {
                if (fallbackType != calendarType && TryParseToDateOnly(input, fallbackType, out result))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Convert DateOnly to formatted string in the specified calendar
        /// </summary>
        public static string FormatDateOnly(DateOnly? date, CalendarType calendarType)
        {
            if (!date.HasValue) return "";
            
            // Check for DateOnly.MinValue (1/1/0001) which is invalid for calendar conversion
            if (date.Value == DateOnly.MinValue || date.Value.Year < 100) return "";
            
            try
            {
                var dateTime = date.Value.ToDateTime(TimeOnly.MinValue);
                var (year, month, day) = FromGregorian(dateTime, calendarType);
                
                // If conversion returned default date (1/1/1), return empty string
                if (year == 1 && month == 1 && day == 1) return "";
                
                // Additional validation to ensure year is reasonable
                if (year < 1 || year > 9999)
                {
                    Console.WriteLine($"Warning: Invalid year {year} for date {date.Value} in calendar {calendarType}");
                    return "";
                }
                
                return $"{year:D4}/{month:D2}/{day:D2}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error formatting date {date.Value} to {calendarType}: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Convert DateOnly to a new DateOnly with calendar-specific year/month/day values (for display purposes)
        /// </summary>
        public static DateOnly? ToCalendarDateOnly(DateOnly? gregorianDate, CalendarType calendarType)
        {
            if (!gregorianDate.HasValue) return null;
            var (year, month, day) = FromGregorian(gregorianDate.Value.ToDateTime(TimeOnly.MinValue), calendarType);
            return new DateOnly(year, month, day);
        }

        public static DateTime ToGregorian(int year, int month, int day, CalendarType calendarType)
        {
            try
            {
                DateTime localDate;
                switch (calendarType)
                {
                    case CalendarType.Gregorian:
                        localDate = new DateTime(year, month, day);
                        break;

                    case CalendarType.HijriShamsi:
                        var persianCalendar = new PersianCalendar();
                        localDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                        break;

                    case CalendarType.HijriQamari:
                        var hijriCalendar = new HijriCalendar();
                        localDate = hijriCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                        break;

                    default:
                        throw new ArgumentException("Unknown calendar type", nameof(calendarType));
                }
                
                // Convert to UTC for PostgreSQL compatibility
                return DateTime.SpecifyKind(localDate, DateTimeKind.Utc);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert date {year}/{month}/{day} from {calendarType} to Gregorian", ex);
            }
        }

        public static (int year, int month, int day) FromGregorian(DateTime gregorianDate, CalendarType targetCalendar)
        {
            try
            {
                // Check for DateOnly.MinValue equivalent (1/1/0001)
                if (gregorianDate.Year == 1 && gregorianDate.Month == 1 && gregorianDate.Day == 1)
                {
                    return (1, 1, 1);
                }

                // Check for invalid dates (before year 622 for Hijri calendars)
                if ((targetCalendar == CalendarType.HijriQamari || targetCalendar == CalendarType.HijriShamsi) 
                    && gregorianDate.Year < 622)
                {
                    // Return a default valid date instead of throwing
                    return (1, 1, 1);
                }

                switch (targetCalendar)
                {
                    case CalendarType.Gregorian:
                        return (gregorianDate.Year, gregorianDate.Month, gregorianDate.Day);

                    case CalendarType.HijriShamsi:
                        var persianCalendar = new PersianCalendar();
                        var shamsiYear = persianCalendar.GetYear(gregorianDate);
                        var shamsiMonth = persianCalendar.GetMonth(gregorianDate);
                        var shamsiDay = persianCalendar.GetDayOfMonth(gregorianDate);
                        
                        // Validate the result
                        if (shamsiYear < 1 || shamsiYear > 9999)
                        {
                            return (1, 1, 1);
                        }
                        
                        return (shamsiYear, shamsiMonth, shamsiDay);

                    case CalendarType.HijriQamari:
                        var hijriCalendar = new HijriCalendar();
                        var hijriYear = hijriCalendar.GetYear(gregorianDate);
                        var hijriMonth = hijriCalendar.GetMonth(gregorianDate);
                        var hijriDay = hijriCalendar.GetDayOfMonth(gregorianDate);
                        
                        // Validate the result
                        if (hijriYear < 1 || hijriYear > 9999)
                        {
                            return (1, 1, 1);
                        }
                        
                        return (hijriYear, hijriMonth, hijriDay);

                    default:
                        throw new ArgumentException("Unknown calendar type", nameof(targetCalendar));
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // Date is out of range for the target calendar, return a default valid date
                return (1, 1, 1);
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - return a default valid date
                Console.WriteLine($"Warning: Failed to convert Gregorian date {gregorianDate} to {targetCalendar}: {ex.Message}");
                return (1, 1, 1);
            }
        }

        public static string FormatDate(DateTime gregorianDate, CalendarType calendarType)
        {
            var (year, month, day) = FromGregorian(gregorianDate, calendarType);
            return $"{year:D4}/{month:D2}/{day:D2}";
        }

        public static DateTime? ParseDateString(string dateString, CalendarType calendarType)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            try
            {
                var parts = dateString.Split('/', '-');
                if (parts.Length != 3)
                    return null;

                if (!int.TryParse(parts[0], out int year) ||
                    !int.TryParse(parts[1], out int month) ||
                    !int.TryParse(parts[2], out int day))
                {
                    return null;
                }

                return ToGregorian(year, month, day, calendarType);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsValidDate(int year, int month, int day, CalendarType calendarType)
        {
            try
            {
                ToGregorian(year, month, day, calendarType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetMonthName(int month, CalendarType calendarType)
        {
            switch (calendarType)
            {
                case CalendarType.Gregorian:
                    var gregorianMonths = new[] { "January", "February", "March", "April", "May", "June",
                                                  "July", "August", "September", "October", "November", "December" };
                    return month >= 1 && month <= 12 ? gregorianMonths[month - 1] : "";

                case CalendarType.HijriShamsi:
                    var shamsiMonths = new[] { "حمل", "ثور", "جوزا", "سرطان", "اسد", "سنبله",
                                               "میزان", "عقرب", "قوس", "جدی", "دلو", "حوت" };
                    return month >= 1 && month <= 12 ? shamsiMonths[month - 1] : "";

                case CalendarType.HijriQamari:
                    var qamariMonths = new[] { "محرم", "صفر", "ربیع الاول", "ربیع الثانی", "جمادی الاول", "جمادی الثانی",
                                               "رجب", "شعبان", "رمضان", "شوال", "ذی القعده", "ذی الحجه" };
                    return month >= 1 && month <= 12 ? qamariMonths[month - 1] : "";

                default:
                    return "";
            }
        }

        /// <summary>
        /// Parse a date string and return DateOnly (Gregorian)
        /// </summary>
        public static DateOnly? ParseDateOnly(string? dateString, CalendarType calendarType)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            var gregorianDate = ParseDateString(dateString, calendarType);
            if (gregorianDate.HasValue)
            {
                return DateOnly.FromDateTime(gregorianDate.Value);
            }
            return null;
        }
    }
}
