using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public class DashboardViewModel
{
    public string Username { get; set; } = "";
    public string DataSource { get; set; } = "";

    // Character
    public int Level { get; set; }
    public string LevelTitle { get; set; } = "";
    public int Exp { get; set; }
    public int LevelStartXp { get; set; }
    public int NextLevelXp { get; set; }
    public int PercentToNext => NextLevelXp == LevelStartXp ? 0 : (Exp - LevelStartXp) * 100 / (NextLevelXp - LevelStartXp);
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Mana { get; set; }
    public int MaxMana { get; set; }
    public int Gold { get; set; }
    public int Streak { get; set; }
    public int Deaths { get; set; }
    public bool InRaid { get; set; }

    // Leaderboard stats
    public ActivityCharts Charts { get; set; } = null!;
    public List<CategoryCardViewModel> Cards { get; set; } = [];
    public List<BadgeViewModel> Badges { get; set; } = [];
    public IReadOnlyList<ActivityLog> Recent { get; set; } = [];

    // Tasks
    public UserClock Clock { get; set; } = null!;
    public DateOnly Today { get; set; }
    public Dictionary<Guid, (int Positive, int Negative)> HabitCounts { get; set; } = [];
    /// <summary>Dailies already completed in their current period.</summary>
    public HashSet<Guid> DoneDailyIds { get; set; } = [];
    public List<TaskItem> Habits { get; set; } = [];
    public List<TaskItem> Dailies { get; set; } = [];
    public List<TaskItem> Todos { get; set; } = [];
    public List<TaskItem> DoneTodos { get; set; } = [];
}
