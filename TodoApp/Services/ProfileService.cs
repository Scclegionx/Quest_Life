using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services;

/// <summary>Builds the read-only public profile of a player (character, stats, rankings, activity charts).</summary>
public class ProfileService(
    AppDbContext db, CharacterService characters, LeaderboardService leaderboards,
    ActivityStatsService stats, RaidService raid)
{
    public async Task<ProfileViewModel?> GetAsync(string username, Guid? viewerId)
    {
        var key = username.Trim().ToLowerInvariant();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == key);
        if (user is null) return null;

        var clock = UserClock.For(user.TimeZone);
        var c = await characters.GetAsync(user.Id);
        var level = PlayerStats.Level(c);
        var catalog = GameCatalog.Current;

        var logs = await db.ActivityLogs.AsNoTracking().Where(l => l.UserId == user.Id)
            .Select(l => new { l.Category, l.Amount, l.LoggedAt }).ToListAsync();
        var weekStart = AppTime.StartOfWeekUtc();
        var weekRanks = await leaderboards.GetRanksAsync(user.Id, weekStart);
        var allRanks = await leaderboards.GetRanksAsync(user.Id, null);
        var raidStatus = await raid.GetStatusAsync(user.Id);

        var careerHits = await db.RaidHits.AsNoTracking().Where(h => h.UserId == user.Id)
            .GroupBy(h => h.RaidId).Select(g => new { Damage = g.Sum(h => (long)h.Damage) }).ToListAsync();

        string? activeTitle = c.ActiveTitleId is { } tid && catalog.Titles.TryGetValue(tid, out var t) ? t.Name : null;
        var claimed = c.Achievements.Select(a => a.AchievementId).ToHashSet();

        return new ProfileViewModel
        {
            Username = user.Username,
            JoinedAt = user.CreatedAt,
            IsMe = viewerId == user.Id,
            Level = level,
            LevelTitle = Gamification.TitleOf(level),
            ActiveTitle = activeTitle,
            Exp = c.Exp,
            LevelStartXp = Gamification.XpForLevel(level),
            NextLevelXp = Gamification.XpForLevel(level + 1),
            Hp = c.Hp,
            Mana = c.Mana,
            Gold = c.Gold,
            Total = PlayerStats.Total(c),
            Base = PlayerStats.Base(level),
            Gear = PlayerStats.Gear(c),
            TasksCompleted = c.TasksCompleted,
            Deaths = c.Deaths,
            Streak = Gamification.Streak(logs.Select(l => l.LoggedAt), clock),

            Slots = SlotInfo.Gear
                .Select(s => new SlotView(s, c.Equipment.FirstOrDefault(e => e.SlotId == s) is { } e ? ItemCatalog.Find(e.ItemId) : null))
                .ToList(),
            Companion = c.Pets.FirstOrDefault(p => p.Id == c.CompanionPetId),
            Pets = c.Pets.OrderByDescending(p => p.Level).ThenBy(p => p.AdoptedAt).ToList(),
            Titles = c.Titles.Where(x => catalog.Titles.ContainsKey(x.TitleId)).Select(x => catalog.Titles[x.TitleId]).ToList(),
            Achievements = catalog.Achievements.Where(a => claimed.Contains(a.Id)).ToList(),
            AchievementTotal = catalog.Achievements.Count,

            Cards = Categories.All.Select(info => new CategoryCardViewModel
            {
                Info = info,
                AllTotal = logs.Where(l => l.Category == info.Key).Sum(l => l.Amount),
                WeekTotal = logs.Where(l => l.Category == info.Key && l.LoggedAt >= weekStart).Sum(l => l.Amount),
                WeekRank = weekRanks[info.Key].Rank,
                WeekPlayers = weekRanks[info.Key].Players,
                AllRank = allRanks[info.Key].Rank,
                AllPlayers = allRanks[info.Key].Players,
            }).ToList(),

            RaidDamageThisWeek = raidStatus.MyDamage,
            RaidRank = raidStatus.MyRank,
            RaidDamageCareer = careerHits.Sum(h => h.Damage),
            RaidsContributed = careerHits.Count,

            Charts = await stats.BuildAsync(user.Id, clock),
        };
    }

    public async Task<List<PlayerCard>> SearchAsync(string? query, int take = 48)
    {
        var key = (query ?? "").Trim().ToLowerInvariant();
        var users = db.Users.AsNoTracking().AsQueryable();
        if (key.Length > 0) users = users.Where(u => u.Username.Contains(key));

        var rows = await (from u in users
                          join ch in db.Characters on u.Id equals ch.UserId into joined
                          from c in joined.DefaultIfEmpty()
                          select new { u.Username, u.CreatedAt, Exp = c == null ? 0 : c.Exp, Tasks = c == null ? 0 : c.TasksCompleted, c.ActiveTitleId })
            .ToListAsync();

        return rows
            .OrderByDescending(r => r.Username == key).ThenByDescending(r => r.Username.StartsWith(key))
            .ThenByDescending(r => r.Exp).ThenBy(r => r.Username)
            .Take(take)
            .Select(r =>
            {
                var level = Gamification.LevelOf(r.Exp);
                var title = r.ActiveTitleId is { } tid && GameCatalog.Current.Titles.TryGetValue(tid, out var t) ? t.Name : Gamification.TitleOf(level);
                return new PlayerCard(r.Username, level, title, r.Exp, r.Tasks, r.CreatedAt);
            }).ToList();
    }
}
