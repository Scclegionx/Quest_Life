namespace TodoApp.Services.Discord;

/// <summary>
/// Settings from the "Discord" configuration section. Keep real values (token!) out of the repository:
/// use `dotnet user-secrets`, environment variables (Discord__BotToken) or AWS Secrets Manager.
/// </summary>
public class DiscordOptions
{
    public bool Enabled { get; set; }
    /// <summary>Application (client) id from the Discord developer portal.</summary>
    public string ApplicationId { get; set; } = "";
    /// <summary>Hex public key from the developer portal; used to verify that requests really come from Discord.</summary>
    public string PublicKey { get; set; } = "";
    /// <summary>Bot token. Only needed to register the slash commands (and, later, to post messages).</summary>
    public string BotToken { get; set; } = "";
    /// <summary>The Discord server (guild) to register commands in. Guild commands update instantly; leave empty for global commands.</summary>
    public string GuildId { get; set; } = "";
    /// <summary>If set, commands only work in this channel.</summary>
    public string AllowedChannelId { get; set; } = "";
    /// <summary>Public address of the website, used for links inside Discord messages (e.g. https://questlife.example.com).</summary>
    public string WebBaseUrl { get; set; } = "";
    /// <summary>Register/update the slash commands at startup (needs ApplicationId and BotToken).</summary>
    public bool RegisterCommandsOnStartup { get; set; }
    public string ApiBase { get; set; } = "https://discord.com/api/v10";

    public bool IsConfigured => Enabled && !string.IsNullOrWhiteSpace(PublicKey);
}
