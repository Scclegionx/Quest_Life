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
    AchievementService achievements, TaskBoardService boards,
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

        var board = await boards.LoadAsync(userId, clock);
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
            Today = board.Today,
            HabitCounts = board.HabitCounts,
            DoneDailyIds = board.DoneDailyIds,
            Habits = board.Habits,
            Dailies = board.Dailies,
            Todos = board.Todos,
            DoneTodos = board.DoneTodos,
        });
    }
}
