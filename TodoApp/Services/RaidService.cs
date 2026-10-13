using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public record RaidAttacker(int Rank, string Username, long Damage, bool IsMe);
public record ClaimableRaid(Guid RaidId, BossDef Boss);

public class RaidStatus
{
    public Guid RaidId { get; set; }
    public BossDef Boss { get; set; } = null!;
    public int WeekNumber { get; set; }
    public DateTime EndsUtc { get; set; }
    public long MaxHp { get; set; }
    public long TotalDamage { get; set; }
    public long Hp => Math.Max(0, MaxHp - TotalDamage);
    public bool Defeated { get; set; }
    public int HpPercent => MaxHp <= 0 ? 0 : (int)Math.Round(Hp * 100.0 / MaxHp);
    public List<RaidAttacker> Top { get; set; } = [];
    public int AttackerCount { get; set; }
    public long MyDamage { get; set; }
    public int? MyRank { get; set; }
    public bool Joined { get; set; }
    public List<ClaimableRaid> Claimable { get; set; } = [];
}

/// <summary>
/// Weekly raid boss shared by everyone (week boundary = game time, see AppTime). A raid row is created lazily for each week;
/// players who joined deal damage whenever they complete a task, and everyone who contributed can claim the boss rewards
/// once it is defeated.
/// </summary>
public class RaidService(AppDbContext db)
{
    // Monday 2026-01-05 00:00 game time (UTC+7) = week 0.
    private static readonly DateTime Epoch = new(2026, 1, 4, 17, 0, 0, DateTimeKind.Utc);

    private static int WeekIndex() => (int)Math.Round((AppTime.StartOfWeekUtc() - Epoch).TotalDays / 7);

    public static int CurrentWeekNumber() => WeekIndex() + 1;

    /// <summary>Creates this week's raid if it doesn't exist yet. Call with a clean change tracker (it saves).</summary>
    public async Task<Raid> EnsureCurrentRaidAsync()
    {
        var start = AppTime.StartOfWeekUtc();
        var raid = await db.Raids.AsNoTracking().FirstOrDefaultAsync(r => r.StartsAt == start);
        if (raid is not null) return raid;

        var bosses = BossCatalog.All;
        if (bosses.Count == 0) throw new InvalidOperationException("No bosses defined.");
        var boss = bosses[((WeekIndex() % bosses.Count) + bosses.Count) % bosses.Count];

        var created = new Raid { BossId = boss.Id, StartsAt = start, EndsAt = start.AddDays(7), MaxHp = boss.MaxHp };
        db.Raids.Add(created);
        try
        {
            await db.SaveChangesAsync();
            db.Entry(created).State = EntityState.Detached;
            return created;
        }
        catch (DbUpdateException) // another request created it first
        {
            db.Entry(created).State = EntityState.Detached;
            return await db.Raids.AsNoTracking().FirstAsync(r => r.StartsAt == start);
        }
    }

    public Task<Guid?> FindCurrentRaidIdAsync()
    {
        var start = AppTime.StartOfWeekUtc();
        return db.Raids.AsNoTracking().Where(r => r.StartsAt == start).Select(r => (Guid?)r.Id).FirstOrDefaultAsync();
    }

    public async Task<RaidStatus> GetStatusAsync(Guid? me, int top = 10)
    {
        var raid = await EnsureCurrentRaidAsync();
        var boss = BossCatalog.All.First(b => b.Id == raid.BossId);

        var rows = await (from h in db.RaidHits.AsNoTracking().Where(h => h.RaidId == raid.Id)
                          join u in db.Users on h.UserId equals u.Id
                          group h by new { u.Id, u.Username } into g
                          select new { g.Key.Id, g.Key.Username, Total = g.Sum(x => (long)x.Damage) }).ToListAsync();
        var ordered = rows.Where(r => r.Total > 0).OrderByDescending(r => r.Total).ThenBy(r => r.Username).ToList();
        var myIndex = me is null ? -1 : ordered.FindIndex(r => r.Id == me);

        var status = new RaidStatus
        {
            RaidId = raid.Id,
            Boss = boss,
            WeekNumber = CurrentWeekNumber(),
            EndsUtc = raid.EndsAt,
            MaxHp = raid.MaxHp,
            TotalDamage = raid.DamageDealt,
            Defeated = raid.Status == RaidState.Defeated,
            AttackerCount = ordered.Count,
            Top = ordered.Take(top).Select((r, i) => new RaidAttacker(i + 1, r.Username, r.Total, r.Id == me)).ToList(),
            MyDamage = myIndex < 0 ? 0 : ordered[myIndex].Total,
            MyRank = myIndex < 0 ? null : myIndex + 1,
        };

        if (me is { } userId)
        {
            status.Joined = await db.RaidParticipants.AnyAsync(p => p.RaidId == raid.Id && p.UserId == userId);
            status.Claimable = await GetClaimableAsync(userId);
        }
        return status;
    }

    /// <summary>Defeated raids (this week or earlier) the player contributed to but hasn't claimed yet.</summary>
    public async Task<List<ClaimableRaid>> GetClaimableAsync(Guid userId)
    {
        var raids = await db.Raids.AsNoTracking()
            .Where(r => r.Status == RaidState.Defeated
                        && db.RaidHits.Any(h => h.RaidId == r.Id && h.UserId == userId && h.Damage > 0)
                        && !db.RaidRewardClaims.Any(c => c.RaidId == r.Id && c.UserId == userId))
            .OrderByDescending(r => r.StartsAt).ToListAsync();
        return raids.Select(r => new ClaimableRaid(r.Id, BossCatalog.All.First(b => b.Id == r.BossId))).ToList();
    }

    public async Task<GameResult> JoinAsync(Guid userId)
    {
        var raid = await EnsureCurrentRaidAsync();
        if (raid.Status != RaidState.Active) return GameResult.Fail("This week's boss is already defeated. Wait for the next one.");
        if (!await db.RaidParticipants.AnyAsync(p => p.RaidId == raid.Id && p.UserId == userId))
        {
            db.RaidParticipants.Add(new RaidParticipant { RaidId = raid.Id, UserId = userId });
            try { await db.SaveChangesAsync(); }
            catch (DbUpdateException) { /* joined concurrently */ }
        }
        var boss = BossCatalog.All.First(b => b.Id == raid.BossId);
        return GameResult.Success($"You joined the raid against {boss.Name}! Every completed task now deals damage.");
    }

    /// <summary>Deals damage inside the caller's transaction. Returns the damage actually dealt (0 if not joined / already defeated).</summary>
    public async Task<int> DealDamageAsync(Guid userId, int damage, TaskItem task)
    {
        var raidId = await FindCurrentRaidIdAsync();
        if (raidId is null) return 0;
        if (!await db.RaidParticipants.AnyAsync(p => p.RaidId == raidId && p.UserId == userId)) return 0;

        // Lock the raid row so simultaneous hits can't push damage past the boss's HP.
        var raid = db.Database.IsRelational()
            ? (await db.Raids.FromSqlInterpolated($"SELECT * FROM raids WHERE id = {raidId} FOR UPDATE").ToListAsync()).FirstOrDefault()
            : await db.Raids.FirstOrDefaultAsync(r => r.Id == raidId);
        if (raid is null || raid.Status != RaidState.Active) return 0;

        var dealt = (int)Math.Min(damage, raid.MaxHp - raid.DamageDealt);
        if (dealt <= 0) return 0;

        raid.DamageDealt += dealt;
        if (raid.DamageDealt >= raid.MaxHp) { raid.Status = RaidState.Defeated; raid.DefeatedAt = DateTime.UtcNow; }
        db.RaidHits.Add(new RaidHit { RaidId = raid.Id, UserId = userId, Damage = dealt, TaskId = task.Id, Source = task.Title });
        return dealt;
    }

    /// <summary>Grants every reward of the defeated boss (gold, items, pets, titles) to a contributor, once.</summary>
    public async Task<GameResult> ClaimAsync(Character c, Guid raidId)
    {
        var raid = await db.Raids.AsNoTracking().FirstOrDefaultAsync(r => r.Id == raidId);
        if (raid is null || raid.Status != RaidState.Defeated) return GameResult.Fail("This boss has not been defeated yet.");
        if (!await db.RaidHits.AnyAsync(h => h.RaidId == raidId && h.UserId == c.UserId && h.Damage > 0))
            return GameResult.Fail("You have not dealt any damage in this raid.");
        if (await db.RaidRewardClaims.AnyAsync(x => x.RaidId == raidId && x.UserId == c.UserId))
            return GameResult.Fail("You already claimed the rewards for this raid.");

        var boss = BossCatalog.All.First(b => b.Id == raid.BossId);
        var parts = new List<string>();
        foreach (var r in boss.Rewards)
        {
            switch (r.Kind)
            {
                case RewardKind.Gold:
                    c.Gold += r.Gold;
                    GameLog.Ledger(c, LedgerKinds.RaidReward, gold: r.Gold, note: boss.Name);
                    parts.Add($"+{r.Gold} 🪙");
                    break;

                case RewardKind.Item when ItemCatalog.Find(r.ItemId) is { } item:
                    var owned = c.Items.FirstOrDefault(i => i.ItemId == item.Id);
                    if (item.Slot != ItemSlot.Egg && owned is not null)
                    {
                        c.Gold += item.Price; // gear is unique: compensate with its price
                        GameLog.Ledger(c, LedgerKinds.RaidReward, gold: item.Price, itemId: item.Id, note: $"{boss.Name} (already owned {item.Name})");
                        parts.Add($"{item.Emoji} {item.Name} (already owned, converted to +{item.Price} 🪙)");
                    }
                    else
                    {
                        if (owned is null) c.Items.Add(new CharacterItem { UserId = c.UserId, ItemId = item.Id, Quantity = r.Quantity });
                        else owned.Quantity += r.Quantity;
                        GameLog.Ledger(c, LedgerKinds.RaidReward, itemId: item.Id, note: boss.Name);
                        parts.Add($"{item.Emoji} {item.Name} x{r.Quantity}");
                    }
                    break;

                case RewardKind.Pet when r.PetSpeciesId is not null && GameCatalog.Current.SpeciesById.TryGetValue(r.PetSpeciesId, out var species):
                    c.Pets.Add(new OwnedPet { UserId = c.UserId, SpeciesId = species.Id, Name = species.Name });
                    parts.Add($"{species.Emoji} {species.Name}");
                    break;

                case RewardKind.Title when r.TitleId is not null && GameCatalog.Current.Titles.TryGetValue(r.TitleId, out var title):
                    AchievementService.GrantTitle(c, title.Id, $"raid:{boss.Id}");
                    parts.Add($"{title.Icon} title \"{title.Name}\"");
                    break;
            }
        }

        db.RaidRewardClaims.Add(new RaidRewardClaim { RaidId = raidId, UserId = c.UserId });
        return GameResult.Success($"🎁 Rewards for defeating {boss.Name}: {string.Join(", ", parts)}");
    }
}
