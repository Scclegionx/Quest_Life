using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services.Discord;

public record LinkOutcome(bool Ok, string Message, string? Username = null);

/// <summary>Creates the one-time codes shown on the website and turns a /link command into a Discord-account link.</summary>
public class DiscordLinkService(AppDbContext db)
{
    public static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(10);

    // no 0/O/1/I, so a code is easy to read and copy
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;

    // after 5 wrong codes in 10 minutes a Discord user must wait (stops code guessing)
    private const int MaxFailures = 5;
    private static readonly TimeSpan FailureWindow = TimeSpan.FromMinutes(10);
    private static readonly ConcurrentDictionary<string, (int Failures, DateTime Since)> Failures = new();

    public static string Normalize(string? input) =>
        new((input ?? "").Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());

    public static string Format(string code) => code.Length == CodeLength ? $"{code[..4]}-{code[4..]}" : code;

    // ---------- website side ----------

    public Task<DiscordLink?> GetLinkAsync(Guid userId) =>
        db.DiscordLinks.AsNoTracking().FirstOrDefaultAsync(l => l.UserId == userId);

    public Task<DiscordLinkCode?> GetActiveCodeAsync(Guid userId) =>
        db.DiscordLinkCodes.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.ExpiresAt > DateTime.UtcNow);

    /// <summary>Creates a fresh code for the user (any older code is discarded).</summary>
    public async Task<DiscordLinkCode> CreateCodeAsync(Guid userId)
    {
        await db.DiscordLinkCodes.Where(c => c.UserId == userId || c.ExpiresAt < DateTime.UtcNow).ExecuteDeleteAsyncSafe(db);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            var code = new DiscordLinkCode
            {
                Code = new string(Enumerable.Range(0, CodeLength).Select(_ => Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)]).ToArray()),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow + CodeLifetime,
            };
            db.DiscordLinkCodes.Add(code);
            try
            {
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                return code;
            }
            catch (DbUpdateException) { db.Entry(code).State = EntityState.Detached; } // extremely unlikely collision: try again
        }
        throw new InvalidOperationException("Could not create a link code.");
    }

    public async Task<bool> UnlinkByUserIdAsync(Guid userId)
    {
        var link = await db.DiscordLinks.FirstOrDefaultAsync(l => l.UserId == userId);
        if (link is null) return false;
        db.DiscordLinks.Remove(link);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return true;
    }

    // ---------- Discord side ----------

    public Task<Guid?> FindUserIdAsync(string discordUserId) =>
        db.DiscordLinks.AsNoTracking().Where(l => l.DiscordUserId == discordUserId).Select(l => (Guid?)l.UserId).FirstOrDefaultAsync();

    public async Task<bool> UnlinkByDiscordIdAsync(string discordUserId)
    {
        var link = await db.DiscordLinks.FirstOrDefaultAsync(l => l.DiscordUserId == discordUserId);
        if (link is null) return false;
        db.DiscordLinks.Remove(link);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return true;
    }

    public async Task<LinkOutcome> LinkAsync(string rawCode, string discordUserId, string? discordUsername)
    {
        if (IsBlocked(discordUserId))
            return new LinkOutcome(false, "Too many wrong codes. Please wait a few minutes and try again.");

        var code = Normalize(rawCode);
        var entry = code.Length == 0 ? null : await db.DiscordLinkCodes.FirstOrDefaultAsync(c => c.Code == code);
        if (entry is null || entry.ExpiresAt <= DateTime.UtcNow)
        {
            RecordFailure(discordUserId);
            return new LinkOutcome(false, "That code is not valid or has expired. Generate a new one in QuestLife → Settings → Link Discord.");
        }

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == entry.UserId);
        if (user is null) return new LinkOutcome(false, "That account no longer exists.");

        var existingForDiscord = await db.DiscordLinks.AsNoTracking().FirstOrDefaultAsync(l => l.DiscordUserId == discordUserId);
        if (existingForDiscord is not null && existingForDiscord.UserId != user.Id)
            return new LinkOutcome(false, "This Discord account is already linked to another QuestLife account. Use /unlink first.");

        var existingForUser = await db.DiscordLinks.FirstOrDefaultAsync(l => l.UserId == user.Id);
        if (existingForUser is not null && existingForUser.DiscordUserId != discordUserId)
            return new LinkOutcome(false, "Your QuestLife account is already linked to a different Discord account. Unlink it in Settings first.");

        if (existingForUser is null)
            db.DiscordLinks.Add(new DiscordLink { UserId = user.Id, DiscordUserId = discordUserId, DiscordUsername = discordUsername });
        else
            existingForUser.DiscordUsername = discordUsername;

        db.DiscordLinkCodes.Remove(entry);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        Failures.TryRemove(discordUserId, out _);
        return new LinkOutcome(true, "Linked!", user.Username);
    }

    private static bool IsBlocked(string discordUserId) =>
        Failures.TryGetValue(discordUserId, out var f) && DateTime.UtcNow - f.Since < FailureWindow && f.Failures >= MaxFailures;

    private static void RecordFailure(string discordUserId) =>
        Failures.AddOrUpdate(discordUserId, _ => (1, DateTime.UtcNow),
            (_, f) => DateTime.UtcNow - f.Since >= FailureWindow ? (1, DateTime.UtcNow) : (f.Failures + 1, f.Since));
}

internal static class QueryExtensions
{
    /// <summary>ExecuteDelete is not available on the in-memory provider, so fall back to loading and removing.</summary>
    public static async Task ExecuteDeleteAsyncSafe<T>(this IQueryable<T> query, AppDbContext db) where T : class
    {
        if (db.Database.IsRelational()) { await query.ExecuteDeleteAsync(); return; }
        db.RemoveRange(await query.ToListAsync());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }
}
