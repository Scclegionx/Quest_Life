using TodoApp.Data;

namespace TodoApp.Models;

public record AchievementContext(
    int LogCount, int Xp, int Level, int Streak,
    IReadOnlyDictionary<ActivityCategory, decimal> Totals,
    int PetCount, bool HasLegendaryPet, int DistinctItems, int EquippedCount, int CoopLevel, int TasksDone);

public record AchievementDef(
    string Id, string Icon, string Name, string Description, int RewardGold, long Target, string TitleId,
    Func<AchievementContext, long> Progress);

public record BadgeDef(string Id, string Icon, string Name, string Description, long Target, Func<AchievementContext, long> Progress);

public record LevelTitleDef(int MinLevel, string Name, string Icon, string? TitleId);

public class AchievementStatus
{
    public AchievementDef Def { get; set; } = null!;
    public long Current { get; set; }
    public bool Completed => Current >= Def.Target;
    public bool Claimed { get; set; }
    public int Percent => (int)Math.Min(100, Current * 100 / Def.Target);
}

/// <summary>Maps the metric column of an achievement to the player statistic it measures.</summary>
public static class AchievementMetrics
{
    private static decimal T(AchievementContext c, ActivityCategory cat) => c.Totals.GetValueOrDefault(cat);

    public static readonly IReadOnlyDictionary<string, Func<AchievementContext, long>> All =
        new Dictionary<string, Func<AchievementContext, long>>
        {
            ["log_count"] = c => c.LogCount,
            ["streak"] = c => c.Streak,
            ["level"] = c => c.Level,
            ["tasks_done"] = c => c.TasksDone,
            ["study_hours"] = c => (long)T(c, ActivityCategory.Study),
            ["gaming_hours"] = c => (long)T(c, ActivityCategory.Gaming),
            ["sport_hours"] = c => (long)T(c, ActivityCategory.Sport),
            ["clean_times"] = c => (long)T(c, ActivityCategory.CleanEating),
            ["habit_times"] = c => (long)T(c, ActivityCategory.BadHabit),
            ["pets_owned"] = c => c.PetCount,
            ["legendary_pets"] = c => c.HasLegendaryPet ? 1 : 0,
            ["distinct_items"] = c => c.DistinctItems,
            ["equipped_count"] = c => c.EquippedCount,
            ["coop_level"] = c => c.CoopLevel,
        };
}

public static class AchievementCatalog
{
    public static IReadOnlyList<AchievementDef> All => GameCatalog.Current.Achievements;
    public static AchievementDef? Find(string id) => GameCatalog.Current.Achievements.FirstOrDefault(a => a.Id == id);
}
