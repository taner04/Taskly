using System.Text.Json;
using System.Text.Json.Serialization;

namespace Taskly.WebApi.Common.Composition.Serialization;

internal sealed class FlexibleEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    throw new JsonException($"{typeof(T).Name} value cannot be null or empty");
                }

                // Try case-insensitive match
                return Enum.TryParse<T>(stringValue, true, out var parsedEnum)
                    ? parsedEnum
                    : throw new JsonException(
                        $"Unable to parse '{stringValue}' as a valid {typeof(T).Name}. Valid values: {string.Join(", ", Enum.GetNames<T>())}");
            }

            case JsonTokenType.Number:
            {
                if (!reader.TryGetInt32(out var intValue))
                {
                    throw new JsonException($"Unable to parse numeric value as {typeof(T).Name}");
                }

                if (Enum.IsDefined(typeof(T), intValue))
                {
                    return (T)Enum.ToObject(typeof(T), intValue);
                }

                throw new JsonException($"Value {intValue} is not a valid {typeof(T).Name}");
            }

            case JsonTokenType.None:
            case JsonTokenType.StartObject:
            case JsonTokenType.EndObject:
            case JsonTokenType.StartArray:
            case JsonTokenType.EndArray:
            case JsonTokenType.PropertyName:
            case JsonTokenType.Comment:
            case JsonTokenType.True:
            case JsonTokenType.False:
            case JsonTokenType.Null:
            default:
                throw new JsonException($"{typeof(T).Name} must be a string or number");
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
