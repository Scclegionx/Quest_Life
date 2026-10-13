using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services;

public record LeaderboardEntry(Guid UserId, string Username, decimal Total);

public class LeaderboardService(AppDbContext db)
{
    public static DateTime? SinceFor(string period) => period == "all" ? null : AppTime.StartOfWeekUtc();

    public async Task<List<Board>> BuildBoardsAsync(DateTime? since, Guid? me, int top = 10)
    {
        var boards = new List<Board>();
        foreach (var info in Categories.All)
        {
            var ordered = await LoadSortedAsync(info.Key, since);
            boards.Add(new Board
            {
                Info = info,
                Rows = ordered.Take(top)
                    .Select((e, i) => new BoardRow { Rank = i + 1, Username = e.Username, Total = e.Total, IsMe = e.UserId == me })
                    .ToList(),
            });
        }
        return boards;
    }

    /// <summary>Rank (1-based) of a user in each category, null if they have no entry; plus total players.</summary>
    public async Task<Dictionary<ActivityCategory, (int? Rank, int Players)>> GetRanksAsync(Guid me, DateTime? since)
    {
        var result = new Dictionary<ActivityCategory, (int?, int)>();
        foreach (var info in Categories.All)
        {
            var ordered = await LoadSortedAsync(info.Key, since);
            var index = ordered.FindIndex(e => e.UserId == me);
            result[info.Key] = (index < 0 ? null : index + 1, ordered.Count);
        }
        return result;
    }

    private async Task<List<LeaderboardEntry>> LoadSortedAsync(ActivityCategory category, DateTime? since)
    {
        var logs = db.ActivityLogs.AsNoTracking().Where(l => l.Category == category);
        if (since is { } s) logs = logs.Where(l => l.LoggedAt >= s);

        var rows = await (from l in logs
                          join u in db.Users on l.UserId equals u.Id
                          group l by new { u.Id, u.Username } into g
                          select new { g.Key.Id, g.Key.Username, Total = g.Sum(x => x.Amount) })
            .ToListAsync();

        return rows.Where(r => r.Total > 0)
            .Select(r => new LeaderboardEntry(r.Id, r.Username, r.Total))
            .OrderByDescending(e => e.Total).ThenBy(e => e.Username)
            .ToList();
    }
}
