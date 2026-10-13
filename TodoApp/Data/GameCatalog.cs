using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

/// <summary>Immutable in-memory copy of the reference tables, built from the database.</summary>
public sealed class CatalogSnapshot
{
    public IReadOnlyList<CategoryInfo> Categories { get; init; } = [];
    public IReadOnlyDictionary<ActivityCategory, CategoryInfo> CategoryByKey { get; init; } = new Dictionary<ActivityCategory, CategoryInfo>();

    /// <summary>Indexed by difficulty level (index 0 unused).</summary>
    public int[] BaseGold { get; init; } = new int[6];
    public int[] BaseExp { get; init; } = new int[6];
    public int[] BaseDamage { get; init; } = new int[6];
    public int[] BaseHpLoss { get; init; } = new int[6];

    public IReadOnlyList<ShopCategory> ShopCategories { get; init; } = [];
    public IReadOnlyList<Item> Items { get; init; } = [];
    public IReadOnlyDictionary<string, Item> ItemById { get; init; } = new Dictionary<string, Item>();
    public IReadOnlyDictionary<string, (Rarity Rarity, int Minutes)> Eggs { get; init; } = new Dictionary<string, (Rarity, int)>();
    public IReadOnlyList<PetSpecies> Species { get; init; } = [];
    public IReadOnlyDictionary<string, PetSpecies> SpeciesById { get; init; } = new Dictionary<string, PetSpecies>();
    public IReadOnlyDictionary<string, TitleEntity> Titles { get; init; } = new Dictionary<string, TitleEntity>();
    public IReadOnlyList<AchievementDef> Achievements { get; init; } = [];
    public IReadOnlyList<BadgeDef> Badges { get; init; } = [];
    /// <summary>Sorted by MinLevel ascending.</summary>
    public IReadOnlyList<LevelTitleDef> LevelTitles { get; init; } = [];
    public IReadOnlyDictionary<Rarity, string> RarityLabels { get; init; } = new Dictionary<Rarity, string>();
    public IReadOnlyDictionary<ItemSlot, (string Name, string Emoji)> Slots { get; init; } = new Dictionary<ItemSlot, (string, string)>();
    public IReadOnlyList<(string Id, string Label)> TimeZones { get; init; } = [];
    public IReadOnlyDictionary<string, decimal> Settings { get; init; } = new Dictionary<string, decimal>();
    public IReadOnlyList<BossDef> Bosses { get; init; } = [];
}

/// <summary>
/// The reference data lives in the database (seeded by the first migration); the app reads it through this snapshot
/// so views and rules don't need a query for every lookup. Call <see cref="ReloadAsync"/> to pick up edits.
/// </summary>
public static class GameCatalog
{
    private static CatalogSnapshot _current = new();

    public static CatalogSnapshot Current => _current;

    public static async Task ReloadAsync(AppDbContext db, CancellationToken ct = default)
    {
        var categories = await db.Categories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync(ct);
        var difficulties = await db.DifficultyLevels.AsNoTracking().ToListAsync(ct);
        var shopCategories = await db.ShopCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync(ct);
        var slots = await db.EquipmentSlots.AsNoTracking().ToListAsync(ct);
        var subtypes = (await db.ItemSubtypes.AsNoTracking().ToListAsync(ct)).ToDictionary(s => s.Id);
        var items = await db.Items.AsNoTracking().Where(i => i.IsActive).ToListAsync(ct);
        var eggs = await db.EggTypes.AsNoTracking().ToListAsync(ct);
        var species = await db.PetSpecies.AsNoTracking().ToListAsync(ct);
        var titles = await db.Titles.AsNoTracking().ToListAsync(ct);
        var achievements = await db.Achievements.AsNoTracking().ToListAsync(ct);
        var bosses = await db.Bosses.AsNoTracking().OrderBy(b => b.SortOrder).ToListAsync(ct);
        var bossRewards = await db.BossRewards.AsNoTracking().ToListAsync(ct);
        var badges = await db.Badges.AsNoTracking().OrderBy(b => b.SortOrder).ToListAsync(ct);
        var levelTitles = await db.LevelTitles.AsNoTracking().OrderBy(t => t.MinLevel).ToListAsync(ct);
        var rarities = await db.Rarities.AsNoTracking().ToListAsync(ct);
        var zones = await db.TimeZones.AsNoTracking().OrderBy(z => z.SortOrder).ToListAsync(ct);
        var settings = await db.GameSettings.AsNoTracking().ToListAsync(ct);

        int[] Column(Func<DifficultyLevelEntity, int> pick)
        {
            var a = new int[6];
            foreach (var d in difficulties.Where(d => d.Level is >= 1 and <= 5)) a[d.Level] = pick(d);
            return a;
        }

        var categoryInfos = categories.Select(c => new CategoryInfo(
            c.Id, c.Slug, c.Name, c.BoardName, c.Emoji, c.Unit, c.IsHours, c.MaxPerEntry, c.Tagline)).ToList();

        var itemModels = items.Select(i => new Item(
            i.Id, i.Name, i.SlotId, subtypes[i.SubtypeId].Name, i.Rarity, i.Price,
            new StatBlock(i.MaxHp, i.MaxMana, i.Atk, i.Def, i.Int, i.Luk), i.Emoji, i.Description)).ToList();

        var speciesModels = species.Select(s => new PetSpecies(s.Id, s.Name, s.Emoji, s.Rarity)).ToList();

        var achievementDefs = achievements
            .Where(a => AchievementMetrics.All.ContainsKey(a.Metric))
            .Select(a => new AchievementDef(a.Id, a.Icon, a.Name, a.Description, a.RewardGold, Math.Max(1, a.Target), a.TitleId,
                AchievementMetrics.All[a.Metric]))
            .ToList();

        var bossDefs = bosses.Select(b => new BossDef(b.Id, b.Name, b.Emoji, b.Title, b.MaxHp, b.Lore,
            bossRewards.Where(r => r.BossId == b.Id)
                .Select(r => new BossRewardDef(r.Kind, r.Gold, r.ItemId, r.Quantity, r.PetSpeciesId, r.TitleId)).ToList())).ToList();

        _current = new CatalogSnapshot
        {
            Categories = categoryInfos,
            CategoryByKey = categoryInfos.ToDictionary(c => c.Key),
            BaseGold = Column(d => d.BaseGold),
            BaseExp = Column(d => d.BaseExp),
            BaseDamage = Column(d => d.BaseDamage),
            BaseHpLoss = Column(d => d.BaseHpLoss),
            ShopCategories = shopCategories.Select(sc => new ShopCategory(
                sc.Key, sc.Name, sc.Emoji, slots.Where(s => s.ShopCategoryKey == sc.Key).Select(s => s.Id).ToArray())).ToList(),
            Items = itemModels,
            ItemById = itemModels.ToDictionary(i => i.Id),
            Eggs = eggs.ToDictionary(e => e.ItemId, e => (e.BaseRarity, e.HatchMinutes)),
            Species = speciesModels,
            SpeciesById = speciesModels.ToDictionary(s => s.Id),
            Titles = titles.ToDictionary(t => t.Id),
            Achievements = achievementDefs,
            Bosses = bossDefs,
            Badges = badges.Where(b => AchievementMetrics.All.ContainsKey(b.Metric))
                .Select(b => new BadgeDef(b.Id, b.Icon, b.Name, b.Description, Math.Max(1, b.Target), AchievementMetrics.All[b.Metric])).ToList(),
            LevelTitles = levelTitles.Select(t => new LevelTitleDef(t.MinLevel, t.Name, t.Icon, t.TitleId)).ToList(),
            RarityLabels = rarities.ToDictionary(r => r.Id, r => r.Label),
            Slots = slots.ToDictionary(s => s.Id, s => (s.Name, s.Emoji)),
            TimeZones = zones.Select(z => (z.Id, z.Label)).ToList(),
            Settings = settings.ToDictionary(x => x.Key, x => x.Value),
        };
    }
}
