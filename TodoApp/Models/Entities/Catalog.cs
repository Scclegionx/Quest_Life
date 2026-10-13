namespace TodoApp.Models;

// Reference data (seeded by migrations, editable in the database). The app reads it through GameCatalog.

public class CategoryEntity
{
    public ActivityCategory Id { get; set; }
    public string Slug { get; set; } = "";
    public string Name { get; set; } = "";
    public string BoardName { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Unit { get; set; } = "";
    public bool IsHours { get; set; }
    public decimal MaxPerEntry { get; set; }
    public string Tagline { get; set; } = "";
    public int SortOrder { get; set; }
}

public class DifficultyLevelEntity
{
    public int Level { get; set; }
    public string Name { get; set; } = "";
    public int BaseGold { get; set; }
    public int BaseExp { get; set; }
    public int BaseDamage { get; set; }
    public int BaseHpLoss { get; set; }
}

public class ShopCategoryEntity
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "";
    public int SortOrder { get; set; }
}

public class EquipmentSlotEntity
{
    public ItemSlot Id { get; set; }
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string ShopCategoryKey { get; set; } = "";
    public bool IsEquippable { get; set; }
}

public class ItemSubtypeEntity
{
    public int Id { get; set; }
    public ItemSlot SlotId { get; set; }
    public string Name { get; set; } = "";
}

public class ItemEntity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public ItemSlot SlotId { get; set; }
    public int SubtypeId { get; set; }
    public Rarity Rarity { get; set; }
    public int Price { get; set; }
    public string Emoji { get; set; } = "";
    public string Description { get; set; } = "";
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Int { get; set; }
    public int Luk { get; set; }
    public int MaxHp { get; set; }
    public int MaxMana { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EggTypeEntity
{
    public string ItemId { get; set; } = "";
    public int HatchMinutes { get; set; }
    public Rarity BaseRarity { get; set; }
}

public class PetSpeciesEntity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "";
    public Rarity Rarity { get; set; }
}

public class TitleEntity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
}

public class AchievementEntity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Description { get; set; } = "";
    public int RewardGold { get; set; }
    /// <summary>Which player statistic is measured (see AchievementMetrics).</summary>
    public string Metric { get; set; } = "";
    public long Target { get; set; }
    public string TitleId { get; set; } = "";
}

public class BossEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Title { get; set; } = "";
    public string Lore { get; set; } = "";
    public long MaxHp { get; set; }
    public int SortOrder { get; set; }
}

public enum RewardKind { Gold, Item, Pet, Title }

/// <summary>One reward granted to every contributor when a boss is defeated. Only the columns for its Kind are used.</summary>
public class BossRewardEntity
{
    public int Id { get; set; }
    public int BossId { get; set; }
    public RewardKind Kind { get; set; }
    public int Gold { get; set; }
    public string? ItemId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? PetSpeciesId { get; set; }
    public string? TitleId { get; set; }
}

public class SkillEntity
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int ManaCost { get; set; }
    public int CooldownSeconds { get; set; }
    /// <summary>Free-form effect description for the future skill engine.</summary>
    public string? EffectJson { get; set; }
}

public class ItemSkillEntity
{
    public string ItemId { get; set; } = "";
    public string SkillId { get; set; } = "";
}

public class SkillNodeEntity
{
    public int Id { get; set; }
    public string SkillId { get; set; } = "";
    public int? ParentNodeId { get; set; }
    public int RequiredLevel { get; set; }
    public int PointCost { get; set; }
}

public class RarityEntity
{
    public Rarity Id { get; set; }
    public string Label { get; set; } = "";
    public int SortOrder { get; set; }
}

/// <summary>Rank names shown for a level range: the row with the highest MinLevel not above the player's level applies.</summary>
public class LevelTitleEntity
{
    public int MinLevel { get; set; }
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";
    /// <summary>Optional: a (selectable) title granted automatically when the player reaches MinLevel.</summary>
    public string? TitleId { get; set; }
}

/// <summary>Dashboard badge: unlocked when the player's value for <see cref="Metric"/> reaches <see cref="Target"/>.</summary>
public class BadgeEntity
{
    public string Id { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Metric { get; set; } = "";
    public long Target { get; set; }
    public int SortOrder { get; set; }
}

public class TimeZoneEntity
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    public int SortOrder { get; set; }
}

/// <summary>A tunable game number (level curve, reward multipliers, pet costs, ...).</summary>
public class GameSettingEntity
{
    public string Key { get; set; } = "";
    public decimal Value { get; set; }
    public string Description { get; set; } = "";
    public string GroupName { get; set; } = "";
}
