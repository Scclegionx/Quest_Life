namespace TodoApp.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    /// <summary>IANA time zone id. Decides when this user's "day" starts (dailies, deadlines, streaks, mana refresh).</summary>
    public string TimeZone { get; set; } = "Asia/Ho_Chi_Minh";
}
