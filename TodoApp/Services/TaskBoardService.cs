using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>A player's tasks, ready to show (used by the dashboard and by Discord).</summary>
public class TaskBoard
{
    public DateOnly Today { get; set; }
    public List<TaskItem> Habits { get; set; } = [];
    public List<TaskItem> Dailies { get; set; } = [];
    public List<TaskItem> Todos { get; set; } = [];
    public List<TaskItem> DoneTodos { get; set; } = [];
    public Dictionary<Guid, (int Positive, int Negative)> HabitCounts { get; set; } = [];
    /// <summary>Dailies already completed in their current period.</summary>
    public HashSet<Guid> DoneDailyIds { get; set; } = [];
}

public class TaskBoardService(AppDbContext db)
{
    public async Task<TaskBoard> LoadAsync(Guid userId, UserClock clock)
    {
        var all = await db.Tasks.AsNoTracking().Where(t => t.UserId == userId).OrderBy(t => t.CreatedAt).ToListAsync();
        var today = clock.Today;

        // habit counters and "done this period" flags come from the history tables
        var habitIds = all.Where(t => t.Type == TaskType.Habit).Select(t => t.Id).ToList();
        var habitCounts = (await db.HabitLogs.AsNoTracking().Where(h => habitIds.Contains(h.TaskId))
                .GroupBy(h => new { h.TaskId, h.Positive }).Select(g => new { g.Key.TaskId, g.Key.Positive, N = g.Count() }).ToListAsync())
            .GroupBy(x => x.TaskId)
            .ToDictionary(g => g.Key, g => (g.Where(x => x.Positive).Sum(x => x.N), g.Where(x => !x.Positive).Sum(x => x.N)));

        var dailies = all.Where(t => t.Type == TaskType.Daily).ToList();
        var periodStarts = dailies.Select(t => Periods.Start(t.Repeat ?? RepeatType.Daily, today)).Distinct().ToList();
        var dailyIds = dailies.Select(t => t.Id).ToList();
        var completions = await db.DailyCompletions.AsNoTracking()
            .Where(x => dailyIds.Contains(x.TaskId) && periodStarts.Contains(x.PeriodStart))
            .Select(x => new { x.TaskId, x.PeriodStart }).ToListAsync();
        var doneDailies = dailies
            .Where(t => completions.Any(x => x.TaskId == t.Id && x.PeriodStart == Periods.Start(t.Repeat ?? RepeatType.Daily, today)))
            .Select(t => t.Id).ToHashSet();

        static int Pri(TaskItem t) => -(int)t.Priority;
        return new TaskBoard
        {
            Today = today,
            HabitCounts = habitCounts,
            DoneDailyIds = doneDailies,
            Habits = all.Where(t => t.Type == TaskType.Habit).OrderBy(Pri).ThenBy(t => t.CreatedAt).ToList(),
            Dailies = dailies.OrderBy(Pri).ThenBy(t => t.CreatedAt).ToList(),
            Todos = all.Where(t => t.Type == TaskType.Todo && !t.IsDone)
                .OrderBy(Pri).ThenBy(t => t.Deadline ?? DateOnly.MaxValue).ToList(),
            DoneTodos = all.Where(t => t.Type == TaskType.Todo && t.IsDone)
                .OrderByDescending(t => t.CompletedAt).Take(5).ToList(),
        };
    }
}
