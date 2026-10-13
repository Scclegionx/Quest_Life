using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

[Authorize]
public class DashboardController(
    AppDbContext db, LeaderboardService leaderboards, TaskService tasks, CharacterService characters, ActivityStatsService activityStats,
    AchievementService achievements,
    RaidService raid, DataSourceMode mode) : GameControllerBase
{
    public async Task<IActionResult> Index()
    {
        var userId = UserId;
        var clock = Clock;
        await raid.EnsureCurrentRaidAsync();

        // Penalties are normally applied by the background sweeper; this catches up anything it hasn't reached yet
        // and shows what happened while the player was away.
        var notices = await tasks.TakeNoticesAsync(userId);
        if (notices.Ok)
            TempData["ToastError"] = TempData.Peek("ToastError") is string existing ? $"{existing} {notices.Message}" : notices.Message;

        var c = await characters.GetAsync(userId);
        var stats = PlayerStats.Total(c);
        var level = PlayerStats.Level(c);

        var logs = await db.ActivityLogs.AsNoTracking().Where(l => l.UserId == userId).OrderByDescending(l => l.LoggedAt).ToListAsync();
        var weekStart = AppTime.StartOfWeekUtc();
        var ranks = await leaderboards.GetRanksAsync(userId, weekStart);
        var streak = Gamification.Streak(logs.Select(l => l.LoggedAt), clock);
        var totals = logs.GroupBy(l => l.Category).ToDictionary(g => g.Key, g => g.Sum(l => l.Amount));

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
        var titleName = c.ActiveTitleId is { } tid && GameCatalog.Current.Titles.TryGetValue(tid, out var title) ? title.Name : null;

        return View(new DashboardViewModel
        {
            Username = User.Identity!.Name!,
            DataSource = mode == DataSourceMode.Mock ? "In-memory mock (not saved)" : "PostgreSQL",
            Level = level,
            LevelTitle = titleName ?? Gamification.TitleOf(level),
            Exp = c.Exp,
            LevelStartXp = Gamification.XpForLevel(level),
            NextLevelXp = Gamification.XpForLevel(level + 1),
            Hp = c.Hp, MaxHp = stats.MaxHp, Mana = c.Mana, MaxMana = stats.MaxMana,
            Gold = c.Gold,
            Streak = streak,
            Deaths = c.Deaths,
            InRaid = await db.RaidParticipants.AnyAsync(p => p.UserId == userId && db.Raids.Any(r => r.Id == p.RaidId && r.Status == RaidState.Active)),

            Charts = await activityStats.BuildAsync(userId, clock),
            Cards = Categories.All.Select(info => new CategoryCardViewModel
            {
                Info = info,
                AllTotal = totals.GetValueOrDefault(info.Key),
                WeekTotal = logs.Where(l => l.Category == info.Key && l.LoggedAt >= weekStart).Sum(l => l.Amount),
                WeekRank = ranks[info.Key].Rank,
                WeekPlayers = ranks[info.Key].Players,
            }).ToList(),
            Badges = BadgeService.Evaluate(await achievements.BuildContextAsync(c, clock)),
            Recent = logs.Take(6).ToList(),

            Clock = clock,
            Today = today,
            HabitCounts = habitCounts,
            DoneDailyIds = doneDailies,
            Habits = all.Where(t => t.Type == TaskType.Habit).OrderBy(Pri).ThenBy(t => t.CreatedAt).ToList(),
            Dailies = dailies.OrderBy(Pri).ThenBy(t => t.CreatedAt).ToList(),
            Todos = all.Where(t => t.Type == TaskType.Todo && !t.IsDone)
                .OrderBy(Pri).ThenBy(t => t.Deadline ?? DateOnly.MaxValue).ToList(),
            DoneTodos = all.Where(t => t.Type == TaskType.Todo && t.IsDone)
                .OrderByDescending(t => t.CompletedAt).Take(5).ToList(),
        });
    }
}
