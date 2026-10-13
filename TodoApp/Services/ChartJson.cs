using System.Text.Json;

namespace TodoApp.Services;

/// <summary>Serializes a chart config to a plain string so Razor HTML-encodes it correctly inside a data-* attribute.</summary>
public static class ChartJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public static string Serialize(object value) => JsonSerializer.Serialize(value, Options);
}
