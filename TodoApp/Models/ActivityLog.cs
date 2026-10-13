using TodoApp.Data;

namespace TodoApp.Models;

public enum ActivityCategory { Study, Gaming, Sport, CleanEating, BadHabit }

/// <summary>One logged session that feeds the leaderboards: hours for Study/Gaming/Sport, times for CleanEating/BadHabit.</summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public ActivityCategory Category { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public Guid? SourceTaskId { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Read model of a leaderboard category (loaded from the categories table).</summary>
public record CategoryInfo(
    ActivityCategory Key, string Slug, string Name, string BoardName, string Emoji,
    string Unit, bool IsHours, decimal MaxPerEntry, string Tagline);

public static class Categories
{
    public static IReadOnlyList<CategoryInfo> All => GameCatalog.Current.Categories;
    public static CategoryInfo Get(ActivityCategory c) => GameCatalog.Current.CategoryByKey[c];
}
