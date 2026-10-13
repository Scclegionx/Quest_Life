namespace TodoApp.Models;

public class Character
{
    public Guid UserId { get; set; }
    public int Exp { get; set; }
    public int Gold { get; set; }
    public int Hp { get; set; }
    public int Mana { get; set; }
    /// <summary>Last local day (in the user's time zone) mana was refilled; mana refills to full when a new day starts.</summary>
    public DateOnly ManaRefreshedOn { get; set; }
    public int Deaths { get; set; }
    public int TasksCompleted { get; set; }
    public int CoopLevel { get; set; } = 1;
    public int IncubatorLevel { get; set; } = 1;
    public Guid? CompanionPetId { get; set; }
    public string? ActiveTitleId { get; set; }
    public int SkillPoints { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<CharacterItem> Items { get; set; } = [];
    public List<CharacterEquipment> Equipment { get; set; } = [];
    public List<OwnedPet> Pets { get; set; } = [];
    public List<IncubatingEgg> IncubatingEggs { get; set; } = [];
    public List<CharacterTitle> Titles { get; set; } = [];
    public List<CharacterAchievement> Achievements { get; set; } = [];
    public List<CharacterSkill> Skills { get; set; } = [];
    public List<LedgerEntry> Ledger { get; set; } = [];
    public List<Notification> Notifications { get; set; } = [];
}

/// <summary>Everything the character owns (bag + worn gear). Gear has quantity 1, eggs stack.</summary>
public class CharacterItem
{
    public Guid UserId { get; set; }
    public string ItemId { get; set; } = "";
    public int Quantity { get; set; }
}

public class CharacterEquipment
{
    public Guid UserId { get; set; }
    public ItemSlot SlotId { get; set; }
    public string ItemId { get; set; } = "";
}

public class OwnedPet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string SpeciesId { get; set; } = "";
    public string Name { get; set; } = "";
    public int Level { get; set; } = 1;
    public int Xp { get; set; }
    public DateTime AdoptedAt { get; set; } = DateTime.UtcNow;
}

public class IncubatingEgg
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string EggItemId { get; set; } = "";
    public DateTime StartedAt { get; set; }
    public DateTime ReadyAt { get; set; }
}

public class CharacterTitle
{
    public Guid UserId { get; set; }
    public string TitleId { get; set; } = "";
    public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = "";
}

public class CharacterAchievement
{
    public Guid UserId { get; set; }
    public string AchievementId { get; set; } = "";
    public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>A character can know many skills and a skill can be known by many characters.</summary>
public class CharacterSkill
{
    public Guid UserId { get; set; }
    public string SkillId { get; set; } = "";
    public int Level { get; set; } = 1;
    public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Where it came from: "item", "tree", ...</summary>
    public string Source { get; set; } = "";
}

public class LedgerEntry
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public string Kind { get; set; } = "";
    public int GoldDelta { get; set; }
    public int ExpDelta { get; set; }
    public int HpDelta { get; set; }
    public Guid? TaskId { get; set; }
    public string? ItemId { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Notification
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public string Kind { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
    public DateTime? DiscordSentAt { get; set; }
}

public static class LedgerKinds
{
    public const string TaskReward = "task_reward";
    public const string HabitFail = "habit_fail";
    public const string MissPenalty = "miss_penalty";
    public const string Death = "death";
    public const string Purchase = "purchase";
    public const string PetFeed = "pet_feed";
    public const string Upgrade = "upgrade";
    public const string AchievementReward = "achievement_reward";
    public const string RaidReward = "raid_reward";
}

public record GameResult(bool Ok, string Message)
{
    /// <summary>Save the character's changes even though the action itself failed (automatic penalties were applied).</summary>
    public bool Persist { get; init; }

    public static GameResult Success(string message) => new(true, message);
    public static GameResult Fail(string message) => new(false, message);
}
