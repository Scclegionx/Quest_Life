using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>
/// Demo data for development / the mock database: a demo player with sample tasks, bot players with characters
/// (gear, pets, titles), four weeks of history for the charts and some raid damage. Runs only when there are no users yet.
/// All seeded accounts use the password <see cref="DemoPassword"/>.
/// </summary>
public static class DemoDataSeeder
{
    public const string DemoUsername = "demo";
    public const string DemoPassword = "Demo@123";

    private static readonly string[] BotNames =
        ["kiem_si_khoi", "phap_su_lan", "sieu_nhan_hung", "nu_than_mai", "ninja_tuan", "cung_thu_vy", "ky_si_nam", "thanh_dat"];

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher<User> hasher, RaidService raid)
    {
        if (await db.Users.AnyAsync()) return;

        var clock = UserClock.For(UserClock.DefaultZoneId);
        var rng = new Random(2026);
        var now = DateTime.UtcNow;

        var demo = NewUser(hasher, DemoUsername);
        var bots = BotNames.Select(n => NewUser(hasher, n)).ToList();
        db.Users.Add(demo);
        db.Users.AddRange(bots);

        // demo character
        var character = new Character { UserId = demo.Id, Exp = 1000, Gold = 1000, ManaRefreshedOn = clock.Today, TasksCompleted = 9 };
        var stats = PlayerStats.Total(character);
        character.Hp = stats.MaxHp;
        character.Mana = stats.MaxMana;
        var demoTasks = AddDemoTasks(db, demo.Id, clock);
        AddLedgerHistory(character, rng, now, demoTasks.Select(t => t.Id).ToList());
        db.Characters.Add(character);

        // bot characters with gear, pets and titles so their profiles look alive
        var botCharacters = bots.Select(bot => NewBotCharacter(bot, rng, now, clock)).ToList();
        db.Characters.AddRange(botCharacters);


        // four weeks of activity for the leaderboards and charts
        foreach (var player in bots.Prepend(demo))
        {
            var isDemo = player.Id == demo.Id;
            for (var day = 0; day < 28; day++)
                foreach (var info in Categories.All)
                {
                    var forced = isDemo && day < 5 && info.Key == ActivityCategory.Study; // give demo a streak
                    if (!forced && rng.NextDouble() > (isDemo ? 0.45 : 0.55)) continue;

                    db.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = player.Id, Category = info.Key,
                        Amount = info.IsHours ? (decimal)(rng.Next(1, 7) * 0.5) : rng.Next(1, 5),
                        LoggedAt = now.AddDays(-day).AddMinutes(-rng.Next(0, 600)),
                    });
                }
        }
        await db.SaveChangesAsync();

        // a character and its companion pet reference each other, so the companion is set in a second save
        foreach (var bc in botCharacters) bc.CompanionPetId = bc.Pets.FirstOrDefault()?.Id;
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        // this week's raid: the bots have already joined and hit the boss a few times
        var current = await raid.EnsureCurrentRaidAsync();
        var weekStart = AppTime.StartOfWeekUtc();
        var span = Math.Max(1, (int)(now - weekStart).TotalMinutes);
        long total = 0;
        foreach (var bot in bots)
        {
            db.RaidParticipants.Add(new RaidParticipant { RaidId = current.Id, UserId = bot.Id, JoinedAt = weekStart });
            for (var i = rng.Next(2, 7); i > 0; i--)
            {
                var damage = rng.Next(15, 130);
                total += damage;
                db.RaidHits.Add(new RaidHit
                {
                    RaidId = current.Id, UserId = bot.Id, Damage = damage, Source = "Demo",
                    DealtAt = weekStart.AddMinutes(rng.Next(0, span)),
                });
            }
        }
        var raidRow = await db.Raids.FirstAsync(r => r.Id == current.Id);
        raidRow.DamageDealt = total;
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    private static Character NewBotCharacter(User bot, Random rng, DateTime now, UserClock clock)
    {
        var exp = rng.Next(150, 9000);
        var level = Gamification.LevelOf(exp);
        var c = new Character
        {
            UserId = bot.Id, Exp = exp, Gold = rng.Next(50, 3000), ManaRefreshedOn = clock.Today,
            TasksCompleted = rng.Next(5, 120), Deaths = rng.Next(0, 3),
        };

        // gear: better rarities as the level grows
        var best = level >= 9 ? Rarity.Legendary : level >= 6 ? Rarity.Epic : level >= 3 ? Rarity.Rare : Rarity.Common;
        foreach (var slot in SlotInfo.Gear)
        {
            if (rng.NextDouble() > 0.75) continue;
            var pool = ItemCatalog.All.Where(i => i.Slot == slot && i.Rarity <= best).ToList();
            if (pool.Count == 0) continue;
            var item = pool[rng.Next(pool.Count)];
            c.Items.Add(new CharacterItem { UserId = bot.Id, ItemId = item.Id, Quantity = 1 });
            c.Equipment.Add(new CharacterEquipment { UserId = bot.Id, SlotId = slot, ItemId = item.Id });
        }

        // pets
        var petCount = rng.Next(0, Math.Min(4, level / 2 + 1));
        for (var i = 0; i < petCount; i++)
        {
            var species = PetCatalog.Species[rng.Next(PetCatalog.Species.Count)];
            c.Pets.Add(new OwnedPet
            {
                UserId = bot.Id, SpeciesId = species.Id, Name = species.Name, Level = rng.Next(1, 10), Xp = rng.Next(0, 40),
                AdoptedAt = now.AddDays(-rng.Next(1, 30)),
            });
        }
        c.CoopLevel = 1 + petCount / 3;

        // achievements and titles
        var achievements = AchievementCatalog.All.OrderBy(_ => rng.Next()).Take(rng.Next(0, 9)).ToList();
        foreach (var a in achievements)
        {
            c.Achievements.Add(new CharacterAchievement { UserId = bot.Id, AchievementId = a.Id });
            c.Titles.Add(new CharacterTitle { UserId = bot.Id, TitleId = a.TitleId, Source = $"achievement:{a.Id}" });
        }
        c.ActiveTitleId = achievements.Count > 0 ? achievements[rng.Next(achievements.Count)].TitleId : null;

        var stats = PlayerStats.Total(c);
        c.Hp = (int)(stats.MaxHp * (0.4 + rng.NextDouble() * 0.6));
        c.Mana = stats.MaxMana;
        AddLedgerHistory(c, rng, now);
        return c;
    }

    /// <summary>Four weeks of "task completed" rewards (feeds the EXP/gold chart and the heat map).</summary>
    private static void AddLedgerHistory(Character c, Random rng, DateTime now, IReadOnlyList<Guid>? taskIds = null)
    {
        for (var day = 0; day < 28; day++)
            for (var n = rng.Next(0, 5); n > 0; n--)
                c.Ledger.Add(new LedgerEntry
                {
                    UserId = c.UserId, Kind = LedgerKinds.TaskReward,
                    GoldDelta = rng.Next(8, 70), ExpDelta = rng.Next(6, 50), Note = "Demo",
                    TaskId = taskIds is { Count: > 0 } ? taskIds[rng.Next(taskIds.Count)] : null,
                    CreatedAt = now.AddDays(-day).AddMinutes(-rng.Next(0, 900)),
                });
    }

    private static List<TaskItem> AddDemoTasks(AppDbContext db, Guid userId, UserClock clock)
    {
        var today = clock.Today;
        TaskItem Habit(string title, string? desc, int diff, TaskPriority pri) =>
            new() { UserId = userId, Type = TaskType.Habit, Title = title, Description = desc, Difficulty = diff, Priority = pri };
        TaskItem Daily(string title, string? desc, int diff, TaskPriority pri, RepeatType rep) => new()
        {
            UserId = userId, Type = TaskType.Daily, Title = title, Description = desc, Difficulty = diff, Priority = pri,
            Repeat = rep, LastCheckedPeriod = Periods.Previous(rep, Periods.Start(rep, today)),
        };
        TaskItem Todo(string title, string? desc, int diff, TaskPriority pri, int? dueInDays, ActivityCategory? cat = null, decimal? effort = null) => new()
        {
            UserId = userId, Type = TaskType.Todo, Title = title, Description = desc, Difficulty = diff, Priority = pri,
            Deadline = dueInDays is null ? null : today.AddDays(dueInDays.Value), Category = cat, Effort = effort,
        };

        var tasks = new List<TaskItem>
        {
            Habit("Drink enough water", "Every glass counts", 1, TaskPriority.Low),
            Habit("No late-night snacks", null, 2, TaskPriority.Medium),
            Habit("Doomscrolling", "Press − if you slip", 3, TaskPriority.Medium),
            Daily("Read for 30 minutes", null, 2, TaskPriority.Medium, RepeatType.Daily),
            Daily("Work out", "At least 20 minutes", 3, TaskPriority.High, RepeatType.Daily),
            Daily("Clean the house", null, 3, TaskPriority.Low, RepeatType.Weekly),
            Daily("Review monthly spending", null, 4, TaskPriority.Medium, RepeatType.Monthly),
            Todo("Review chapter 3", "Redo the end-of-chapter exercises", 3, TaskPriority.High, 2, ActivityCategory.Study, 2),
            Todo("5 km run", null, 3, TaskPriority.Medium, 1, ActivityCategory.Sport, 1),
            Todo("Skip bubble tea today", null, 2, TaskPriority.Low, 0, ActivityCategory.CleanEating, 1),
            Todo("Finish the weekly report", "Compile the numbers and send them to the team", 4, TaskPriority.High, 3),
            Todo("Plan the Discord integration", null, 2, TaskPriority.Low, null),
        };
        db.Tasks.AddRange(tasks);
        return tasks;
    }

    private static User NewUser(IPasswordHasher<User> hasher, string username)
    {
        var user = new User { Username = username, Email = $"{username}@example.com" };
        user.PasswordHash = hasher.HashPassword(user, DemoPassword);
        return user;
    }
}
