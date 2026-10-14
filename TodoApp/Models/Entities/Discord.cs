namespace TodoApp.Models;

/// <summary>Connects a QuestLife account to one Discord account (one-to-one), so slash commands know who is talking.</summary>
public class DiscordLink
{
    public Guid UserId { get; set; }
    /// <summary>The Discord snowflake id. Discord signs every interaction, so this identity cannot be faked.</summary>
    public string DiscordUserId { get; set; } = "";
    public string? DiscordUsername { get; set; }
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>A short-lived, single-use code created on the website and entered with /link in Discord.</summary>
public class DiscordLinkCode
{
    public string Code { get; set; } = "";
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}
