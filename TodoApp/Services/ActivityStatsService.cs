using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>Numbers behind the charts on the dashboard and on profiles (all dates are in the player's own time zone).</summary>
public class ActivityCharts
{
    public List<string> Labels { get; set; } = [];
    public Dictionary<ActivityCategory, decimal[]> PerDay { get; set; } = [];
    public int[] ExpPerDay { get; set; } = [];
    public int[] GoldPerDay { get; set; } = [];
    public Dictionary<TaskType, int> CompletedByType { get; set; } = [];

    // heat map: HeatWeeks columns x 7 rows, starting on a Monday
    public DateOnly HeatStart { get; set; }
    public DateOnly Today { get; set; }
    public int HeatWeeks { get; set; }
    public int[] HeatCounts { get; set; } = [];
    public int ActiveDays { get; set; }
    public int TotalCompletions { get; set; }
}

public class ActivityStatsService(AppDbContext db)
{
    public async Task<ActivityCharts> BuildAsync(Guid userId, UserClock clock, int days = 14, int heatWeeks = 26)
    {
        var today = clock.Today;
        var heatStart = Periods.Start(RepeatType.Weekly, today.AddDays(-7 * (heatWeeks - 1)));
        var since = clock.NowUtc.AddDays(-(heatWeeks * 7 + 8)); // over-fetch a little, then bucket by local date

        var logs = await db.ActivityLogs.AsNoTracking()
            .Where(l => l.UserId == userId && l.LoggedAt >= since)
            .Select(l => new { l.Category, l.Amount, l.LoggedAt }).ToListAsync();
        var rewards = await db.LedgerEntries.AsNoTracking()
            .Where(e => e.UserId == userId && e.Kind == LedgerKinds.TaskReward && e.CreatedAt >= since)
            .Select(e => new { e.CreatedAt, e.GoldDelta, e.ExpDelta, e.TaskId }).ToListAsync();

        var dayList = Enumerable.Range(0, days).Select(i => today.AddDays(-(days - 1 - i))).ToList();
        var index = dayList.Select((d, i) => (d, i)).ToDictionary(x => x.d, x => x.i);

        var charts = new ActivityCharts
        {
            Labels = dayList.Select(d => d.ToString("MMM d", CultureInfo.InvariantCulture)).ToList(),
            ExpPerDay = new int[days],
            GoldPerDay = new int[days],
            Today = today,
            HeatStart = heatStart,
            HeatWeeks = heatWeeks,
            HeatCounts = new int[heatWeeks * 7],
        };
        foreach (var info in Categories.All) charts.PerDay[info.Key] = new decimal[days];

        foreach (var l in logs)
            if (index.TryGetValue(clock.ToLocalDate(l.LoggedAt), out var i))
                charts.PerDay[l.Category][i] += l.Amount;

        foreach (var r in rewards)
        {
            var d = clock.ToLocalDate(r.CreatedAt);
            if (index.TryGetValue(d, out var i)) { charts.ExpPerDay[i] += r.ExpDelta; charts.GoldPerDay[i] += r.GoldDelta; }

            var cell = d.DayNumber - heatStart.DayNumber;
            if (cell >= 0 && cell < charts.HeatCounts.Length) charts.HeatCounts[cell]++;
        }
        charts.ActiveDays = charts.HeatCounts.Count(c => c > 0);
        charts.TotalCompletions = charts.HeatCounts.Sum();

        // what kind of tasks were completed in the last 30 days
        var recentIds = rewards.Where(r => r.TaskId != null && clock.ToLocalDate(r.CreatedAt) >= today.AddDays(-29))
            .Select(r => r.TaskId!.Value).ToList();
        if (recentIds.Count > 0)
        {
            var types = await db.Tasks.IgnoreQueryFilters().AsNoTracking()
                .Where(t => recentIds.Contains(t.Id)).Select(t => new { t.Id, t.Type }).ToDictionaryAsync(t => t.Id, t => t.Type);
            foreach (var id in recentIds)
                if (types.TryGetValue(id, out var type))
                    charts.CompletedByType[type] = charts.CompletedByType.GetValueOrDefault(type) + 1;
        }
        return charts;
    }
}
