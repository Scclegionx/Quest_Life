using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public record RewardResult(int Gold, int Exp, int Damage);

/// <summary>
/// Reward / penalty rules. The numbers per difficulty live in the difficulty_levels table.
/// Reward = base(difficulty) x effort (hours or times) x streak bonus, e.g. Study, difficulty 3, 3 hours = 20 x 3 = 60 gold.
/// </summary>
public static class Rewards
{
    public static int[] BaseGold => GameCatalog.Current.BaseGold;
    public static int[] BaseExp => GameCatalog.Current.BaseExp;
    public static int[] BaseDamage => GameCatalog.Current.BaseDamage;
    public static int[] BaseHpLoss => GameCatalog.Current.BaseHpLoss;

    /// <summary>Bonus per streak day (default +5%) up to a cap of days (default 10); both are game settings.</summary>
    public static double StreakMultiplier(int streak) =>
        1 + GameSettings.StreakStep * Math.Clamp(streak, 0, GameSettings.StreakCapDays);

    public static RewardResult Compute(int difficulty, decimal effort, double streakMultiplier, StatBlock stats)
    {
        var d = Math.Clamp(difficulty, 1, 5);
        var e = (double)effort;
        var luck = 1 + stats.Luk / GameSettings.LuckGoldDivisor;                    // LUK: bonus gold
        var power = 1 + (stats.Atk + stats.Int / 2.0) / GameSettings.PowerDamageDivisor; // ATK + INT/2: boss damage

        return new RewardResult(
            Gold: Round(BaseGold[d] * e * streakMultiplier * luck),
            Exp: Round(BaseExp[d] * e * streakMultiplier),
            Damage: Round(BaseDamage[d] * e * power));
    }

    /// <summary>HP lost for a miss; DEF shaves 1% per point, up to 60%.</summary>
    public static int HpLoss(int difficulty, StatBlock stats)
    {
        var d = Math.Clamp(difficulty, 1, 5);
        return Math.Max(1, Round(BaseHpLoss[d] * (1 - Math.Min(GameSettings.DefReductionCap, stats.Def / GameSettings.DefReductionDivisor))));
    }

    private static int Round(double v) => Math.Max(1, (int)Math.Round(v, MidpointRounding.AwayFromZero));
}

/// <summary>Day / week / month / year buckets used by daily tasks.</summary>
public static class Periods
{
    public static DateOnly Start(RepeatType r, DateOnly d) => r switch
    {
        RepeatType.Daily => d,
        RepeatType.Weekly => d.AddDays(-(((int)d.DayOfWeek + 6) % 7)),
        RepeatType.Monthly => new DateOnly(d.Year, d.Month, 1),
        _ => new DateOnly(d.Year, 1, 1),
    };

    public static DateOnly Next(RepeatType r, DateOnly periodStart) => r switch
    {
        RepeatType.Daily => periodStart.AddDays(1),
        RepeatType.Weekly => periodStart.AddDays(7),
        RepeatType.Monthly => periodStart.AddMonths(1),
        _ => periodStart.AddYears(1),
    };

    public static DateOnly Previous(RepeatType r, DateOnly periodStart) => r switch
    {
        RepeatType.Daily => periodStart.AddDays(-1),
        RepeatType.Weekly => periodStart.AddDays(-7),
        RepeatType.Monthly => periodStart.AddMonths(-1),
        _ => periodStart.AddYears(-1),
    };
}
