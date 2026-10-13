using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public class AchievementService(AppDbContext db)
{
    public async Task<AchievementContext> BuildContextAsync(Character c, UserClock clock)
    {
        var logs = await db.ActivityLogs.AsNoTracking().Where(l => l.UserId == c.UserId)
            .Select(l => new { l.Category, l.Amount, l.LoggedAt }).ToListAsync();
        var gear = c.Items.Count(i => ItemCatalog.Find(i.ItemId)?.Slot != ItemSlot.Egg);

        return new AchievementContext(
            LogCount: logs.Count,
            Xp: c.Exp,
            Level: PlayerStats.Level(c),
            Streak: Gamification.Streak(logs.Select(l => l.LoggedAt), clock),
            Totals: logs.GroupBy(l => l.Category).ToDictionary(g => g.Key, g => g.Sum(l => l.Amount)),
            PetCount: c.Pets.Count,
            HasLegendaryPet: c.Pets.Any(p => PetCatalog.Get(p.SpeciesId).Rarity == Rarity.Legendary),
            DistinctItems: gear,
            EquippedCount: c.Equipment.Count,
            CoopLevel: c.CoopLevel,
            TasksDone: c.TasksCompleted);
    }

    public static List<AchievementStatus> Evaluate(AchievementContext ctx, Character c) =>
        AchievementCatalog.All.Select(def => new AchievementStatus
        {
            Def = def,
            Current = Math.Min(def.Progress(ctx), def.Target),
            Claimed = c.Achievements.Any(a => a.AchievementId == def.Id),
        }).ToList();

    public static GameResult Claim(Character c, string id, AchievementContext ctx)
    {
        var def = AchievementCatalog.Find(id);
        if (def is null) return GameResult.Fail("Achievement does not exist.");
        if (c.Achievements.Any(a => a.AchievementId == id)) return GameResult.Fail("You already claimed this achievement.");
        if (def.Progress(ctx) < def.Target) return GameResult.Fail("Requirements not met yet.");

        c.Achievements.Add(new CharacterAchievement { UserId = c.UserId, AchievementId = id });
        GrantTitle(c, def.TitleId, $"achievement:{id}");
        c.ActiveTitleId ??= def.TitleId;
        c.Gold += def.RewardGold;
        GameLog.Ledger(c, LedgerKinds.AchievementReward, gold: def.RewardGold, note: def.Name);
        return GameResult.Success($"{def.Icon} Achievement \"{def.Name}\": +{def.RewardGold} 🪙");
    }

    public static void GrantTitle(Character c, string titleId, string source)
    {
        if (c.Titles.All(t => t.TitleId != titleId))
            c.Titles.Add(new CharacterTitle { UserId = c.UserId, TitleId = titleId, Source = source });
    }

    public static GameResult SetTitle(Character c, string? titleId)
    {
        if (titleId is null) { c.ActiveTitleId = null; return GameResult.Success("Displayed title removed"); }
        if (c.Titles.All(t => t.TitleId != titleId)) return GameResult.Fail("You do not own this title.");
        c.ActiveTitleId = titleId;
        var name = GameCatalog.Current.Titles.GetValueOrDefault(titleId)?.Name ?? titleId;
        return GameResult.Success($"Displayed title: {name}");
    }
}
