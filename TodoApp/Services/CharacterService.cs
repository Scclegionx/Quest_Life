using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>
/// Loads and changes a player's character. Every change runs inside <see cref="RunAsync"/>: one transaction, with the
/// character row locked (SELECT ... FOR UPDATE on PostgreSQL) so two requests - even on two web instances - can't spend
/// the same gold or complete the same task twice. A per-user in-process lock also covers the in-memory mock database.
/// </summary>
public class CharacterService(AppDbContext db)
{
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> Locks = new();

    private IQueryable<Character> Aggregate() => db.Characters
        .Include(c => c.Items).Include(c => c.Equipment).Include(c => c.Pets).Include(c => c.IncubatingEggs)
        .Include(c => c.Titles).Include(c => c.Achievements).Include(c => c.Skills)
        .AsSplitQuery();

    /// <summary>Read-only copy of the whole character (created on first use).</summary>
    public async Task<Character> GetAsync(Guid userId)
    {
        var existing = await Aggregate().AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId);
        if (existing is not null) return existing;

        await RunAsync(userId, (_, _) => Task.FromResult(GameResult.Success("")));
        return await Aggregate().AsNoTracking().FirstAsync(c => c.UserId == userId);
    }

    public async Task<int> GetGoldAsync(Guid userId)
    {
        var gold = await db.Characters.AsNoTracking().Where(c => c.UserId == userId).Select(c => (int?)c.Gold).FirstOrDefaultAsync();
        return gold ?? (await GetAsync(userId)).Gold;
    }

    public Task<GameResult> MutateAsync(Guid userId, Func<Character, UserClock, GameResult> action) =>
        RunAsync(userId, (c, clock) => Task.FromResult(action(c, clock)));

    /// <summary>Runs <paramref name="action"/> on the tracked character; saves if it succeeded (or asked to Persist).</summary>
    public async Task<GameResult> RunAsync(Guid userId, Func<Character, UserClock, Task<GameResult>> action)
    {
        var gate = Locks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        IDbContextTransaction? tx = null;
        var committed = false;
        try
        {
            db.ChangeTracker.Clear();
            if (db.Database.IsRelational())
            {
                tx = await db.Database.BeginTransactionAsync();
                await db.Database.ExecuteSqlInterpolatedAsync($"SELECT 1 FROM characters WHERE user_id = {userId} FOR UPDATE");
            }

            var zone = await db.Users.AsNoTracking().Where(u => u.Id == userId).Select(u => u.TimeZone).FirstOrDefaultAsync();
            if (zone is null) return GameResult.Fail("Account does not exist.");
            var clock = UserClock.For(zone);

            var character = await Aggregate().FirstOrDefaultAsync(c => c.UserId == userId);
            if (character is null)
            {
                character = NewCharacter(userId, clock);
                db.Characters.Add(character);
            }
            RefreshMana(character, clock);

            var result = await action(character, clock);
            if (result.Ok || result.Persist)
            {
                character.UpdatedAt = clock.NowUtc;
                await db.SaveChangesAsync();
                if (tx is not null) { await tx.CommitAsync(); committed = true; }
            }
            return result;
        }
        finally
        {
            if (tx is not null)
            {
                if (!committed) await tx.RollbackAsync();
                await tx.DisposeAsync();
            }
            db.ChangeTracker.Clear();
            gate.Release();
        }
    }

    private static Character NewCharacter(Guid userId, UserClock clock)
    {
        var stats = PlayerStats.Base(1);
        return new Character
        {
            UserId = userId, Gold = GameSettings.StartingGold, Hp = stats.MaxHp, Mana = stats.MaxMana, ManaRefreshedOn = clock.Today,
        };
    }

    /// <summary>Mana refills to full when a new local day starts.</summary>
    private static void RefreshMana(Character c, UserClock clock)
    {
        if (c.ManaRefreshedOn >= clock.Today) return;
        c.Mana = PlayerStats.Total(c).MaxMana;
        c.ManaRefreshedOn = clock.Today;
    }
}

public static class GameLog
{
    public static void Ledger(Character c, string kind, int gold = 0, int exp = 0, int hp = 0,
        Guid? taskId = null, string? itemId = null, string? note = null) =>
        c.Ledger.Add(new LedgerEntry
        {
            UserId = c.UserId, Kind = kind, GoldDelta = gold, ExpDelta = exp, HpDelta = hp,
            TaskId = taskId, ItemId = itemId, Note = note,
        });

    public static void Notify(Character c, string kind, string message) =>
        c.Notifications.Add(new Notification { UserId = c.UserId, Kind = kind, Message = message });
}
