using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>
/// Background job (every 10 minutes): reloads the reference data, closes finished raids, and applies the automatic HP
/// penalties (daily period missed, todo past its deadline) for every player without waiting for them to open the app.
/// Results are queued as notifications on each character.
/// </summary>
public class PenaltySweeper(IServiceScopeFactory scopes, ILogger<PenaltySweeper> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken); // let startup (migrations/seeding) finish
            using var timer = new PeriodicTimer(Interval);
            do { await SweepAsync(stoppingToken); }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
        catch (OperationCanceledException) { /* shutting down */ }
    }

    private async Task SweepAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopes.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var tasks = scope.ServiceProvider.GetRequiredService<TaskService>();

            await GameCatalog.ReloadAsync(db, ct);

            var now = DateTime.UtcNow;
            var finished = await db.Raids.Where(r => r.Status == RaidState.Active && r.EndsAt < now).ToListAsync(ct);
            foreach (var r in finished) r.Status = RaidState.Expired;
            if (finished.Count > 0) await db.SaveChangesAsync(ct);
            db.ChangeTracker.Clear();

            var users = await db.Users.AsNoTracking().Select(u => u.Id).ToListAsync(ct);
            foreach (var id in users)
            {
                ct.ThrowIfCancellationRequested();
                try { await tasks.RolloverAsync(id); }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    logger.LogError(ex, "Penalty sweep failed for user {UserId}", id);
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Penalty sweep failed");
        }
    }
}
