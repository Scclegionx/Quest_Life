using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public class ProfileViewModel
{
    public string Username { get; set; } = "";
    public DateTime JoinedAt { get; set; }
    public bool IsMe { get; set; }

    public int Level { get; set; }
    public string LevelTitle { get; set; } = "";
    public string? ActiveTitle { get; set; }
    public int Exp { get; set; }
    public int LevelStartXp { get; set; }
    public int NextLevelXp { get; set; }
    public int PercentToNext => NextLevelXp == LevelStartXp ? 0 : (Exp - LevelStartXp) * 100 / (NextLevelXp - LevelStartXp);

    public int Hp { get; set; }
    public int Mana { get; set; }
    public int Gold { get; set; }
    public StatBlock Total { get; set; } = StatBlock.Zero;
    public StatBlock Base { get; set; } = StatBlock.Zero;
    public StatBlock Gear { get; set; } = StatBlock.Zero;

    public int TasksCompleted { get; set; }
    public int Deaths { get; set; }
    public int Streak { get; set; }

    public List<SlotView> Slots { get; set; } = [];
    public OwnedPet? Companion { get; set; }
    public List<OwnedPet> Pets { get; set; } = [];
    public List<TitleEntity> Titles { get; set; } = [];
    public List<AchievementDef> Achievements { get; set; } = [];
    public int AchievementTotal { get; set; }

    public List<CategoryCardViewModel> Cards { get; set; } = [];

    public long RaidDamageThisWeek { get; set; }
    public int? RaidRank { get; set; }
    public long RaidDamageCareer { get; set; }
    public int RaidsContributed { get; set; }

    public ActivityCharts Charts { get; set; } = null!;
}

public record PlayerCard(string Username, int Level, string Title, int Exp, int TasksCompleted, DateTime JoinedAt);

public class PlayersViewModel
{
    public string Query { get; set; } = "";
    public bool IsTopList => string.IsNullOrWhiteSpace(Query);
    public List<PlayerCard> Results { get; set; } = [];
}
