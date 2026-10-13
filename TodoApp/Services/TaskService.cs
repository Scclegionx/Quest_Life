using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services;

public record RolloverOutcome(bool Changed, bool Died, string Message);

/// <summary>
/// Task rules: rewards for completing tasks, automatic HP penalties for misses, death, and raid damage.
/// Every state-changing call runs inside <see cref="CharacterService.RunAsync"/> (one transaction, character row locked).
/// </summary>
public class TaskService(AppDbContext db, CharacterService characters, RaidService raid)
{
    // ---------- CRUD ----------

    public Task<TaskItem?> GetAsync(Guid userId, Guid id) =>
        db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

    public async Task CreateAsync(Guid userId, TaskFormViewModel f, UserClock clock)
    {
        var t = new TaskItem { UserId = userId, Type = f.Type };
        Apply(t, f, clock);
        db.Tasks.Add(t);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    public async Task<bool> UpdateAsync(Guid userId, TaskFormViewModel f, UserClock clock)
    {
        var t = f.Id is { } id ? await db.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id) : null;
        if (t is null) return false;
        Apply(t, f, clock);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        return true;
    }

    /// <summary>Soft delete: the task disappears but rewards and logs that reference it stay valid.</summary>
    public async Task DeleteAsync(Guid userId, Guid id)
    {
        var t = await db.Tasks.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
        if (t is null) return;
        t.ArchivedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }

    private static void Apply(TaskItem t, TaskFormViewModel f, UserClock clock)
    {
        t.Title = f.Title.Trim();
        t.Description = string.IsNullOrWhiteSpace(f.Description) ? null : f.Description.Trim();
        t.Difficulty = Math.Clamp(f.Difficulty, 1, 5);
        t.Priority = f.Priority;

        if (t.Type == TaskType.Todo)
        {
            t.Deadline = f.Deadline;
            t.Category = f.Category;
            t.Effort = f.Category is null ? null : f.Effort;
            t.OverduePenalizedAt = null; // a moved deadline can be missed again
        }

        if (t.Type == TaskType.Daily && f.Repeat is { } r && t.Repeat != r)
        {
            t.Repeat = r;
            t.Streak = 0;
            // Nothing before today counts as missed.
            t.LastCheckedPeriod = Periods.Previous(r, Periods.Start(r, clock.Today));
        }
    }

    // ---------- Misses (daily periods that ended, todos past their deadline) ----------

    /// <summary>Applies pending penalties for one player (used by the background sweeper).</summary>
    public Task<GameResult> RolloverAsync(Guid userId) =>
        characters.RunAsync(userId, async (c, clock) =>
        {
            await RolloverCoreAsync(c, clock);
            return GameResult.Fail("") with { Persist = db.ChangeTracker.HasChanges() };
        });

    /// <summary>Catches up on misses, then returns (and marks read) the notifications waiting for the player. Ok = there were some.</summary>
    public Task<GameResult> TakeNoticesAsync(Guid userId) =>
        characters.RunAsync(userId, async (c, clock) =>
        {
            await RolloverCoreAsync(c, clock);
            // saved ones plus the ones the catch-up above just created (not in the database yet)
            var unread = await db.Notifications.Where(n => n.UserId == c.UserId && n.ReadAt == null).OrderBy(n => n.Id).ToListAsync();
            unread.AddRange(c.Notifications.Where(n => n.Id == 0 && n.ReadAt == null));
            foreach (var n in unread) n.ReadAt = clock.NowUtc;
            return new GameResult(unread.Count > 0, string.Join(" ", unread.Select(n => n.Message))) with
            {
                Persist = db.ChangeTracker.HasChanges(),
            };
        });

    private async Task<RolloverOutcome> RolloverCoreAsync(Character c, UserClock clock)
    {
        var today = clock.Today;
        var stats = PlayerStats.Total(c);
        int misses = 0, hpLost = 0;

        var candidates = await db.Tasks
            .Where(t => t.UserId == c.UserId &&
                        (t.Type == TaskType.Daily || (t.Type == TaskType.Todo && t.CompletedAt == null && t.OverduePenalizedAt == null)))
            .ToListAsync();

        foreach (var t in candidates)
        {
            if (t.Type == TaskType.Daily && t.Repeat is { } rep)
            {
                var current = Periods.Start(rep, today);
                var checkedUpTo = t.LastCheckedPeriod ?? Periods.Previous(rep, Periods.Start(rep, clock.ToLocalDate(t.CreatedAt)));
                var next = Periods.Next(rep, checkedUpTo);
                if (next >= current) continue;

                var done = (await db.DailyCompletions
                    .Where(x => x.TaskId == t.Id && x.PeriodStart >= next && x.PeriodStart < current)
                    .Select(x => x.PeriodStart).ToListAsync()).ToHashSet();

                for (var p = next; p < current; p = Periods.Next(rep, p))
                {
                    if (!done.Contains(p)) { misses++; hpLost += Rewards.HpLoss(t.Difficulty, stats); t.Streak = 0; }
                    t.LastCheckedPeriod = p;
                }
            }
            else if (t.Type == TaskType.Todo && t.Deadline is { } due && due < today)
            {
                misses++;
                hpLost += Rewards.HpLoss(t.Difficulty, stats);
                t.OverduePenalizedAt = clock.NowUtc;
            }
        }

        if (misses == 0) return new RolloverOutcome(false, false, "");

        c.Hp -= hpLost;
        GameLog.Ledger(c, LedgerKinds.MissPenalty, hp: -hpLost, note: $"{misses} task(s) missed");

        if (c.Hp <= 0)
        {
            var death = Die(c);
            GameLog.Notify(c, "death", death);
            return new RolloverOutcome(true, true, death);
        }

        var message = $"You missed {misses} task(s): -{hpLost} ❤️ HP.";
        GameLog.Notify(c, "penalty", message);
        return new RolloverOutcome(true, false, message);
    }

    /// <summary>Death: back to level 1, losing gold, gear, bag, pets and buildings. Achievements, titles, skills and raid history stay.</summary>
    private static string Die(Character c)
    {
        GameLog.Ledger(c, LedgerKinds.Death, gold: -c.Gold, exp: -c.Exp, note: $"Fell at level {PlayerStats.Level(c)}");
        c.Deaths++;
        c.Exp = 0;
        c.Gold = 0;
        c.CompanionPetId = null;
        c.Equipment.Clear();
        c.Items.Clear();
        c.IncubatingEggs.Clear();
        c.Pets.Clear();
        c.CoopLevel = 1;
        c.IncubatorLevel = 1;
        var stats = PlayerStats.Total(c);
        c.Hp = stats.MaxHp;
        c.Mana = stats.MaxMana;
        return "💀 Out of HP! Your character fell, returned to level 1 and lost all gold, gear, bag items and pets. Get back up and try again!";
    }

    // ---------- Completing tasks ----------

    public async Task<GameResult> CompleteAsync(Guid userId, Guid taskId)
    {
        await raid.EnsureCurrentRaidAsync(); // before the transaction: it may insert this week's raid
        return await characters.RunAsync(userId, async (c, clock) =>
        {
            var roll = await RolloverCoreAsync(c, clock);
            if (roll.Died) return DiedWhileAway();

            var t = await db.Tasks.FirstOrDefaultAsync(x => x.Id == taskId && x.UserId == userId);
            var result = t switch
            {
                null => GameResult.Fail("Task not found."),
                { Type: TaskType.Todo } => await CompleteTodoAsync(c, clock, t),
                { Type: TaskType.Daily } => await CompleteDailyAsync(c, clock, t),
                _ => GameResult.Fail("Habits use the + / − buttons."),
            };
            return Combine(roll, result);
        });
    }

    public async Task<GameResult> HabitAsync(Guid userId, Guid taskId, bool positive)
    {
        await raid.EnsureCurrentRaidAsync();
        return await characters.RunAsync(userId, async (c, clock) =>
        {
            var roll = await RolloverCoreAsync(c, clock);
            if (roll.Died) return DiedWhileAway();

            var t = await db.Tasks.FirstOrDefaultAsync(x => x.Id == taskId && x.UserId == userId);
            if (t is null || t.Type != TaskType.Habit) return Combine(roll, GameResult.Fail("Habit not found."));

            db.HabitLogs.Add(new HabitLog { TaskId = t.Id, Positive = positive });

            GameResult result;
            if (positive)
            {
                result = await ApplyRewardAsync(c, t, effort: 1, streakMultiplier: 1);
            }
            else
            {
                var loss = Rewards.HpLoss(t.Difficulty, PlayerStats.Total(c));
                c.Hp -= loss;
                GameLog.Ledger(c, LedgerKinds.HabitFail, hp: -loss, taskId: t.Id, note: t.Title);
                result = c.Hp <= 0
                    ? GameResult.Success(Die(c))
                    : GameResult.Success($"Habit \"{t.Title}\" failed: -{loss} ❤️ HP");
            }
            return Combine(roll, result);
        });
    }

    private async Task<GameResult> CompleteTodoAsync(Character c, UserClock clock, TaskItem t)
    {
        if (t.IsDone) return GameResult.Fail("This task is already completed.");

        decimal effort = 1;
        double streakMultiplier = 1;
        if (t.Category is { } category)
        {
            effort = t.Effort ?? 1;
            // Feeds the leaderboards; the streak bonus uses the streak as of before today's new entry.
            var days = await db.ActivityLogs.AsNoTracking().Where(l => l.UserId == c.UserId).Select(l => l.LoggedAt).ToListAsync();
            streakMultiplier = Rewards.StreakMultiplier(Gamification.Streak(days, clock));
            db.ActivityLogs.Add(new ActivityLog
            {
                UserId = c.UserId, Category = category, Amount = effort, Note = t.Title, SourceTaskId = t.Id, LoggedAt = clock.NowUtc,
            });
        }

        t.CompletedAt = clock.NowUtc;
        return await ApplyRewardAsync(c, t, effort, streakMultiplier);
    }

    private async Task<GameResult> CompleteDailyAsync(Character c, UserClock clock, TaskItem t)
    {
        var rep = t.Repeat ?? RepeatType.Daily;
        var period = Periods.Start(rep, clock.Today);
        var previous = Periods.Previous(rep, period);

        var recent = await db.DailyCompletions.Where(x => x.TaskId == t.Id && (x.PeriodStart == period || x.PeriodStart == previous))
            .Select(x => x.PeriodStart).ToListAsync();
        if (recent.Contains(period)) return GameResult.Fail("You already completed this task for the current period.");

        var streakBefore = recent.Contains(previous) ? t.Streak : 0;
        t.Streak = streakBefore + 1;
        t.BestStreak = Math.Max(t.BestStreak, t.Streak);
        db.DailyCompletions.Add(new DailyCompletion { TaskId = t.Id, PeriodStart = period, CompletedAt = clock.NowUtc });
        return await ApplyRewardAsync(c, t, effort: 1, streakMultiplier: Rewards.StreakMultiplier(streakBefore));
    }

    private async Task<GameResult> ApplyRewardAsync(Character c, TaskItem t, decimal effort, double streakMultiplier)
    {
        var reward = Rewards.Compute(t.Difficulty, effort, streakMultiplier, PlayerStats.Total(c));
        var levelBefore = PlayerStats.Level(c);
        var hpBefore = c.Hp;

        c.Gold += reward.Gold;
        c.Exp += reward.Exp;
        c.TasksCompleted++;

        var after = PlayerStats.Total(c);
        var levelAfter = PlayerStats.Level(c);
        if (levelAfter > levelBefore) { c.Hp = after.MaxHp; c.Mana = after.MaxMana; }
        else c.Hp = Math.Min(after.MaxHp, c.Hp + t.Difficulty * GameSettings.HealPerDifficulty); // small heal per completed task

        GameLog.Ledger(c, LedgerKinds.TaskReward, gold: reward.Gold, exp: reward.Exp, hp: c.Hp - hpBefore, taskId: t.Id, note: t.Title);

        var message = $"{t.Title}: +{reward.Gold} 🪙 +{reward.Exp} EXP";
        var dealt = await raid.DealDamageAsync(c.UserId, reward.Damage, t);
        if (dealt > 0) message += $" · ⚔️ {dealt} damage to the boss";
        if (levelAfter > levelBefore)
        {
            message += $" · 🎊 Level up! You are now level {levelAfter}!";
            // level milestones can grant a (selectable) title, see the level_titles table
            foreach (var lt in GameCatalog.Current.LevelTitles.Where(x => x.TitleId != null && x.MinLevel > levelBefore && x.MinLevel <= levelAfter))
            {
                AchievementService.GrantTitle(c, lt.TitleId!, $"level:{lt.MinLevel}");
                message += $" · 🏷️ New title: {GameCatalog.Current.Titles.GetValueOrDefault(lt.TitleId!)?.Name ?? lt.Name}";
            }
        }
        return GameResult.Success(message);
    }

    private static GameResult DiedWhileAway() =>
        GameResult.Fail("💀 Your character fell before you could do this.") with { Persist = true };

    /// <summary>Penalties changed HP, so the character must be saved even when the action itself failed.</summary>
    private static GameResult Combine(RolloverOutcome roll, GameResult result) =>
        roll.Changed && !result.Ok ? result with { Persist = true } : result;
}
