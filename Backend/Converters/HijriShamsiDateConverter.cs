using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebAPIBackend.Helpers;
using WebAPIBackend.Models;

namespace WebAPIBackend.Converters
{
    /// <summary>
    /// Global JSON converter that automatically converts between Hijri Shamsi (frontend) and Gregorian (database)
    /// - When receiving from frontend: Hijri Shamsi string → Gregorian DateTime
    /// - When sending to frontend: Gregorian DateTime → Hijri Shamsi string
    /// </summary>
    public class HijriShamsiDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return default;
            }

            // Parse Hijri Shamsi date from frontend and convert to Gregorian
            var gregorianDate = DateConversionHelper.ParseDateString(dateString, CalendarType.HijriShamsi);
            return gregorianDate ?? default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value == default || value == DateTime.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            // Convert Gregorian DateTime to Hijri Shamsi string for frontend
            var hijriShamsiString = DateConversionHelper.FormatDate(value, CalendarType.HijriShamsi);
            writer.WriteStringValue(hijriShamsiString);
        }
    }

    /// <summary>
    /// Global JSON converter for nullable DateTime
    /// </summary>
    public class HijriShamsiNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // Parse Hijri Shamsi date from frontend and convert to Gregorian
            return DateConversionHelper.ParseDateString(dateString, CalendarType.HijriShamsi);
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue || value.Value == DateTime.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            // Convert Gregorian DateTime to Hijri Shamsi string for frontend
            var hijriShamsiString = DateConversionHelper.FormatDate(value.Value, CalendarType.HijriShamsi);
            writer.WriteStringValue(hijriShamsiString);
        }
    }

    /// <summary>
    /// Global JSON converter for DateOnly
    /// </summary>
    public class HijriShamsiDateOnlyConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return default;
            }

            // Parse Hijri Shamsi date from frontend and convert to Gregorian DateOnly
            if (DateConversionHelper.TryParseToDateOnly(dateString, CalendarType.HijriShamsi, out var dateOnly))
            {
                return dateOnly;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            if (value == default || value == DateOnly.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            // Check for invalid years (data corruption)
            if (value.Year < 622 || value.Year > 9999)
            {
                writer.WriteNullValue();
                return;
            }

            // Convert Gregorian DateOnly to Hijri Shamsi string for frontend
            var hijriShamsiString = DateConversionHelper.FormatDateOnly(value, CalendarType.HijriShamsi);
            
            // If conversion failed or returned empty, write null
            if (string.IsNullOrWhiteSpace(hijriShamsiString))
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStringValue(hijriShamsiString);
        }
    }

    /// <summary>
    /// Global JSON converter for nullable DateOnly
    /// </summary>
    public class HijriShamsiNullableDateOnlyConverter : JsonConverter<DateOnly?>
    {
        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var dateString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            // Parse Hijri Shamsi date from frontend and convert to Gregorian DateOnly
            if (DateConversionHelper.TryParseToDateOnly(dateString, CalendarType.HijriShamsi, out var dateOnly))
            {
                return dateOnly;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (!value.HasValue || value.Value == DateOnly.MinValue)
            {
                writer.WriteNullValue();
                return;
            }

            // Check for invalid years (data corruption)
            if (value.Value.Year < 622 || value.Value.Year > 9999)
            {
                writer.WriteNullValue();
                return;
            }

            // Convert Gregorian DateOnly to Hijri Shamsi string for frontend
            var hijriShamsiString = DateConversionHelper.FormatDateOnly(value.Value, CalendarType.HijriShamsi);
            
            // If conversion failed or returned empty, write null
            if (string.IsNullOrWhiteSpace(hijriShamsiString))
            {
                writer.WriteNullValue();
                return;
            }
            
            writer.WriteStringValue(hijriShamsiString);
        }
    }
}
