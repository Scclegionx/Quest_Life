using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>Level curve, rank names and streaks. The numbers and names come from the database (game_settings, level_titles).</summary>
public static class Gamification
{
    /// <summary>EXP needed to reach <paramref name="level"/> (level 1 = 0): base x (level-1)^exponent.</summary>
    public static int XpForLevel(int level) =>
        level <= 1 ? 0 : (int)Math.Round((double)GameSettings.LevelExpBase * Math.Pow(level - 1, GameSettings.LevelExpExponent));

    public static int LevelOf(int exp)
    {
        var baseExp = (double)GameSettings.LevelExpBase;
        var level = baseExp <= 0 ? 1 : (int)Math.Floor(Math.Pow(Math.Max(0, exp) / baseExp, 1 / GameSettings.LevelExpExponent)) + 1;
        while (XpForLevel(level + 1) <= exp) level++;          // fix floating-point edges in both directions
        while (level > 1 && XpForLevel(level) > exp) level--;
        return Math.Max(1, level);
    }

    private static LevelTitleDef? RankFor(int level) =>
        GameCatalog.Current.LevelTitles.LastOrDefault(t => t.MinLevel <= level);

    public static string TitleOf(int level) => RankFor(level)?.Name ?? "Adventurer";
    public static string RankIconOf(int level) => RankFor(level)?.Icon ?? "⚜️";

    /// <summary>Consecutive local days with at least one logged activity, ending today (or yesterday if nothing yet today).</summary>
    public static int Streak(IEnumerable<DateTime> loggedAtUtc, UserClock clock)
    {
        var days = loggedAtUtc.Select(clock.ToLocalDate).ToHashSet();
        var day = clock.Today;
        if (!days.Contains(day)) day = day.AddDays(-1);
        var streak = 0;
        while (days.Contains(day)) { streak++; day = day.AddDays(-1); }
        return streak;
    }
}
