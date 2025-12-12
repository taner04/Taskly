using Desktop.Abstractions;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Desktop.Extensions;

public static class JsonExtension
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true
    };

    public static void ToJson<T>(this T obj, string path) where T : IJsonExtension
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object to serialize cannot be null.");
        }

        var json = JsonSerializer.Serialize(obj, DefaultOptions);

        File.WriteAllText(path, json);
    }

    public static T FromJson<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Deserialization failed.");
    }
}
