using System.IO;
using System.Text.Json;

namespace Taskly.Desktop.Common.Composition.Extensions;

public static class JsonExtension
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true
    };

    public static void ToJson<T>(this T obj, string path) where T : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object to serialize cannot be null.");
        }

        var json = JsonSerializer.Serialize(obj, DefaultOptions);

        File.WriteAllText(path, json);
    }

    public static T FromJson<T>(this string json) => JsonSerializer.Deserialize<T>(json) ??
                                                     throw new InvalidOperationException("Deserialization failed.");
}
