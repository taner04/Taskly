using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Taskly.WebApi.Common.Composition.Serialization;

/// <summary>
///     Custom JSON converter for DateTime that accepts multiple date/time formats.
/// </summary>
/// <remarks>
///     Supports these formats:
///     - ISO 8601: "2026-03-27T14:30:00Z", "2026-03-27T14:30:00+00:00"
///     - Date only: "2026-03-27", "27.03.2026", "03/27/2026", "27/03/2026"
///     - DateTime: "2026-03-27 14:30:00"
/// </remarks>
internal class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateTimeString = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateTimeString))
            {
                throw new JsonException("DateTime value cannot be null or empty");
            }

            // Formats WITH timezone info (Z or zzz)
            var tzFormats = new[]
            {
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fffzzz"
            };

            foreach (var format in tzFormats)
            {
                if (DateTime.TryParseExact(
                        dateTimeString,
                        format,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal,
                        out var parsedDate))
                {
                    return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }
            }

            // Formats WITHOUT timezone info - treat as UTC directly
            var noTzFormats = new[]
            {
                "yyyy-MM-ddTHH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-dd",
                "dd.MM.yyyy",
                "yyyy.MM.dd",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss.fff",
                "dd.MM.yyyy HH:mm:ss",
                "d/M/yyyy",
                "dd/MM/yyyy",
                "M/d/yyyy",
                "MM/dd/yyyy"
            };

            foreach (var format in noTzFormats)
            {
                if (DateTime.TryParseExact(
                        dateTimeString,
                        format,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, // Don't assume or adjust - take value as-is
                        out var parsedDate))
                {
                    // Treat as UTC directly without any conversion
                    return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }
            }

            // Fallback: standard parsing as UTC
            if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,
                    out var date))
            {
                // If it has no kind or is local, convert to UTC
                return date.Kind switch
                {
                    DateTimeKind.Local => date.ToUniversalTime(),
                    DateTimeKind.Unspecified => DateTime.SpecifyKind(date, DateTimeKind.Utc),
                    _ => date
                };
            }

            throw new JsonException(
                $"Unable to parse '{dateTimeString}' as a valid DateTime. Supported formats: ISO 8601, yyyy-MM-dd, dd.MM.yyyy, HH:mm:ss");
        }

        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("DateTime must be a string or number");
        }

        if (reader.TryGetInt64(out var longValue))
        {
            // Unix timestamps are always UTC
            return DateTime.SpecifyKind(DateTime.UnixEpoch.AddMilliseconds(longValue), DateTimeKind.Utc);
        }

        throw new JsonException("DateTime must be a string or number");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("o")); // ISO 8601 format for output
    }
}