using System.Text.Json;
using System.Text.Json.Serialization;

namespace TodoApp.Services.Discord;

/// <summary>JSON shapes Discord expects in an interaction response (snake_case names, nulls omitted).</summary>
public static class DiscordJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };
}

/// <summary>Interaction callback types.</summary>
public static class DcCallback
{
    public const int Pong = 1;
    public const int ChannelMessage = 4;
    public const int UpdateMessage = 7;
    public const int Autocomplete = 8;
}

public static class DcFlags
{
    /// <summary>Only the person who used the command sees the message.</summary>
    public const int Ephemeral = 64;
}

public class DcResponse
{
    [JsonPropertyName("type")] public int Type { get; set; }
    [JsonPropertyName("data")] public DcData? Data { get; set; }
}

public class DcData
{
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("embeds")] public List<DcEmbed>? Embeds { get; set; }
    [JsonPropertyName("components")] public List<DcRow>? Components { get; set; }
    [JsonPropertyName("flags")] public int? Flags { get; set; }
    /// <summary>Always empty: task titles typed by users must never ping @everyone or roles.</summary>
    [JsonPropertyName("allowed_mentions")] public DcAllowedMentions AllowedMentions { get; set; } = new();
    [JsonPropertyName("choices")] public List<DcChoice>? Choices { get; set; }
}

public class DcAllowedMentions
{
    [JsonPropertyName("parse")] public string[] Parse { get; set; } = [];
}

public class DcEmbed
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    /// <summary>Colour of the bar on the left, as an RGB integer.</summary>
    [JsonPropertyName("color")] public int? Color { get; set; }
    [JsonPropertyName("fields")] public List<DcField>? Fields { get; set; }
    [JsonPropertyName("footer")] public DcFooter? Footer { get; set; }
    [JsonPropertyName("author")] public DcAuthor? Author { get; set; }
    [JsonPropertyName("timestamp")] public string? Timestamp { get; set; }
}

public class DcField
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("value")] public string Value { get; set; } = "";
    [JsonPropertyName("inline")] public bool Inline { get; set; }
}

public class DcFooter { [JsonPropertyName("text")] public string Text { get; set; } = ""; }
public class DcAuthor { [JsonPropertyName("name")] public string Name { get; set; } = ""; }

public class DcRow
{
    [JsonPropertyName("type")] public int Type { get; set; } = 1;
    [JsonPropertyName("components")] public List<DcButton> Components { get; set; } = [];
}

public class DcButton
{
    [JsonPropertyName("type")] public int Type { get; set; } = 2;
    /// <summary>1 primary (blurple), 2 secondary (grey), 3 success (green), 4 danger (red).</summary>
    [JsonPropertyName("style")] public int Style { get; set; } = 2;
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("custom_id")] public string CustomId { get; set; } = "";
    [JsonPropertyName("disabled")] public bool? Disabled { get; set; }
}

public class DcChoice
{
    [JsonPropertyName("name")] public string Name { get; set; } = "";
    [JsonPropertyName("value")] public object Value { get; set; } = "";
}
