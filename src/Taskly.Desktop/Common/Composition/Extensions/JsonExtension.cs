using System.Text.Json;

namespace Taskly.Desktop.Common.Composition.Extensions;

public static class JsonExtension
{
    public static void ToJson<T>(this T obj, string path)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object to serialize cannot be null.");
        }

#pragma warning disable CA1869
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
#pragma warning restore CA1869

        File.WriteAllText(path, json);
    }

    public static T FromJson<T>(this string json) => JsonSerializer.Deserialize<T>(json) ??
                                                     throw new InvalidOperationException("Deserialization failed.");
}