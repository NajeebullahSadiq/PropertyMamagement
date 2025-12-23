using System;
using System.Globalization;
using Persia;

namespace WebAPIBackend.Helpers
{
    public static class DateConversionHelper
    {
        public static DateTime ToGregorian(int year, int month, int day, CalendarType calendarType)
        {
            try
            {
                switch (calendarType)
                {
                    case CalendarType.Gregorian:
                        return new DateTime(year, month, day);

                    case CalendarType.HijriShamsi:
                        var persianCalendar = new PersianCalendar();
                        return persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);

                    case CalendarType.HijriQamari:
                        var hijriCalendar = new HijriCalendar();
                        return hijriCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);

                    default:
                        throw new ArgumentException("Unknown calendar type", nameof(calendarType));
                }
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
                switch (targetCalendar)
                {
                    case CalendarType.Gregorian:
                        return (gregorianDate.Year, gregorianDate.Month, gregorianDate.Day);

                    case CalendarType.HijriShamsi:
                        var persianCalendar = new PersianCalendar();
                        return (
                            persianCalendar.GetYear(gregorianDate),
                            persianCalendar.GetMonth(gregorianDate),
                            persianCalendar.GetDayOfMonth(gregorianDate)
                        );

                    case CalendarType.HijriQamari:
                        var hijriCalendar = new HijriCalendar();
                        return (
                            hijriCalendar.GetYear(gregorianDate),
                            hijriCalendar.GetMonth(gregorianDate),
                            hijriCalendar.GetDayOfMonth(gregorianDate)
                        );

                    default:
                        throw new ArgumentException("Unknown calendar type", nameof(targetCalendar));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert Gregorian date {gregorianDate} to {targetCalendar}", ex);
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
    }
}
