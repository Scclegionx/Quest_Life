using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;
using static TodoApp.Services.Discord.DiscordFormat;

namespace TodoApp.Services.Discord;

/// <summary>
/// Turns a verified Discord interaction (slash command, button press or autocomplete request) into calls to the same
/// services the website uses, and builds the colourful reply.
/// </summary>
public class DiscordInteractionHandler(
    AppDbContext db, DiscordLinkService links, TaskService tasks, TaskBoardService boards, CharacterService characters,
    IOptions<DiscordOptions> options, ILogger<DiscordInteractionHandler> logger)
{
    private static readonly CultureInfo Ci = CultureInfo.InvariantCulture;

    private sealed record Ctx(JsonElement Root, string DiscordUserId, string DiscordName, string? ChannelId, JsonElement Data);

    private sealed record Args(string Command, string? Sub, Dictionary<string, JsonElement> Values, string? Focused)
    {
        public string? Str(string name) => Values.TryGetValue(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        public int? Int(string name) => Values.TryGetValue(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i) ? i : null;
        public decimal? Num(string name) => Values.TryGetValue(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out var d) ? d : null;
    }

    // =====================================================================================================
    // entry point
    // =====================================================================================================

    public async Task<DcResponse> HandleAsync(JsonElement root)
    {
        var type = root.GetProperty("type").GetInt32();
        if (type == 1) return new DcResponse { Type = DcCallback.Pong };

        try
        {
            var ctx = ReadContext(root);
            return type switch
            {
                2 => await HandleCommandAsync(ctx),
                3 => await HandleComponentAsync(ctx),
                4 => await HandleAutocompleteAsync(ctx),
                _ => Reply(Card("Not supported", "QuestLife doesn't handle that kind of interaction yet.", Warning)),
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Discord interaction failed");
            return type == 4
                ? new DcResponse { Type = DcCallback.Autocomplete, Data = new DcData { Choices = [] } }
                : Reply(Card("Something went wrong", "QuestLife hit an unexpected error. Please try again in a moment.", Danger));
        }
    }

    private static Ctx ReadContext(JsonElement root)
    {
        var user = root.TryGetProperty("member", out var member) && member.TryGetProperty("user", out var mu) ? mu : root.GetProperty("user");
        var id = user.GetProperty("id").GetString()!;
        var name = user.TryGetProperty("global_name", out var g) && g.ValueKind == JsonValueKind.String ? g.GetString()!
            : user.TryGetProperty("username", out var u) && u.ValueKind == JsonValueKind.String ? u.GetString()! : id;
        var channel = root.TryGetProperty("channel_id", out var ch) && ch.ValueKind == JsonValueKind.String ? ch.GetString() : null;
        var data = root.TryGetProperty("data", out var d) ? d : default;
        return new Ctx(root, id, name, channel, data);
    }

    private static Args ReadArgs(JsonElement data)
    {
        var command = data.GetProperty("name").GetString()!;
        string? sub = null;
        string? focused = null;
        var values = new Dictionary<string, JsonElement>();

        if (data.TryGetProperty("options", out var options) && options.ValueKind == JsonValueKind.Array)
        {
            var list = options.EnumerateArray().ToList();
            if (list.Count > 0 && list[0].GetProperty("type").GetInt32() == 1)       // sub-command: its options are nested
            {
                sub = list[0].GetProperty("name").GetString();
                list = list[0].TryGetProperty("options", out var nested) ? nested.EnumerateArray().ToList() : [];
            }
            foreach (var o in list)
            {
                var name = o.GetProperty("name").GetString()!;
                if (o.TryGetProperty("value", out var v)) values[name] = v.Clone();
                if (o.TryGetProperty("focused", out var f) && f.ValueKind == JsonValueKind.True) focused = name;
            }
        }
        return new Args(command, sub, values, focused);
    }

    // =====================================================================================================
    // responses
    // =====================================================================================================

    private static DcResponse Reply(DcEmbed embed, List<DcRow>? rows = null, string? content = null) =>
        Reply([embed], rows, content);

    private static DcResponse Reply(List<DcEmbed> embeds, List<DcRow>? rows = null, string? content = null) => new()
    {
        Type = DcCallback.ChannelMessage,
        Data = new DcData { Content = content, Embeds = embeds, Components = rows, Flags = DcFlags.Ephemeral },
    };

    private static DcResponse Update(List<DcEmbed> embeds, List<DcRow> rows, string? content) => new()
    {
        Type = DcCallback.UpdateMessage,
        Data = new DcData { Content = content ?? "", Embeds = embeds, Components = rows },
    };

    private DcResponse NotLinked() => Reply(Card("Link your account first",
        "QuestLife doesn't know who you are yet.\n\n" +
        "**1.** Open QuestLife → **Settings** → **Link Discord** and copy the code.\n" +
        "**2.** Come back here and run `/link code:YOUR-CODE`.\n\n" +
        "It takes about ten seconds and you only do it once.", Warning));

    // =====================================================================================================
    // slash commands
    // =====================================================================================================

    private async Task<DcResponse> HandleCommandAsync(Ctx ctx)
    {
        var args = ReadArgs(ctx.Data);

        var allowed = options.Value.AllowedChannelId;
        if (!string.IsNullOrWhiteSpace(allowed) && ctx.ChannelId != allowed)
            return Reply(Card("Wrong channel", $"Please use QuestLife commands in <#{allowed}>.", Warning));

        switch (args.Command)
        {
            case "help": return Help();
            case "link": return await LinkAsync(ctx, args);
            case "unlink": return await UnlinkAsync(ctx);
        }

        var userId = await links.FindUserIdAsync(ctx.DiscordUserId);
        if (userId is null) return NotLinked();
        var user = await GetUserAsync(userId.Value);

        return args.Command switch
        {
            "me" => await MeAsync(user),
            "quests" => await QuestsAsync(user, content: null),
            "done" => await DoneAsync(user, args),
            "todo" => await CreateTaskAsync(user, args, TaskType.Todo),
            "daily" => await CreateTaskAsync(user, args, TaskType.Daily),
            "habit" => args.Sub switch
            {
                "add" => await CreateTaskAsync(user, args, TaskType.Habit),
                "plus" => await HabitAsync(user, args, positive: true),
                "minus" => await HabitAsync(user, args, positive: false),
                _ => Reply(Card("Unknown habit command", "Use `/habit add`, `/habit plus` or `/habit minus`.", Warning)),
            },
            _ => Reply(Card("Unknown command", "Try `/help` to see what QuestLife can do.", Warning)),
        };
    }

    private sealed record PlayerInfo(Guid Id, string Username, UserClock Clock);

    private async Task<PlayerInfo> GetUserAsync(Guid userId)
    {
        var u = await db.Users.AsNoTracking().Where(x => x.Id == userId).Select(x => new { x.Username, x.TimeZone }).FirstAsync();
        return new PlayerInfo(userId, u.Username, UserClock.For(u.TimeZone));
    }

    // ---------- help / link / unlink ----------

    private static DcResponse Help()
    {
        var embed = Card("⚔️ QuestLife in Discord",
            "Manage your quests without leaving Discord. Replies are private (only you see them).", Brand);
        embed.Fields =
        [
            new() { Name = "🔗 Account", Value = "`/link` connect your account\n`/unlink` disconnect it", Inline = true },
            new() { Name = "🧙 Character", Value = "`/me` your level, HP, stats and streak", Inline = true },
            new() { Name = "📋 Quests", Value = "`/quests` all your tasks with ✅ buttons\n`/done` complete a task by name", Inline = false },
            new() { Name = "➕ Create", Value = "`/todo` a to-do (deadline, category, effort)\n`/daily add` a recurring task\n`/habit add` a habit", Inline = false },
            new() { Name = "🔁 Habits", Value = "`/habit plus` I did well\n`/habit minus` I slipped", Inline = false },
        ];
        return Reply(embed);
    }

    private async Task<DcResponse> LinkAsync(Ctx ctx, Args args)
    {
        var outcome = await links.LinkAsync(args.Str("code") ?? "", ctx.DiscordUserId, ctx.DiscordName);
        if (!outcome.Ok) return Reply(Card("Couldn't link", outcome.Message, Danger));

        var embed = Card("✅ Linked!", $"Your Discord is now connected to the QuestLife account **{Escape(outcome.Username)}**.", Success);
        embed.Fields =
        [
            new() { Name = "Try it", Value = "`/quests` see your tasks\n`/me` see your character\n`/help` all commands", Inline = false },
        ];
        return Reply(embed);
    }

    private async Task<DcResponse> UnlinkAsync(Ctx ctx)
    {
        var removed = await links.UnlinkByDiscordIdAsync(ctx.DiscordUserId);
        return Reply(removed
            ? Card("Unlinked", "Your Discord is no longer connected to a QuestLife account. Link again any time with `/link`.", Warning)
            : Card("Nothing to unlink", "This Discord account isn't linked to QuestLife.", Info));
    }

    // ---------- /me ----------

    private async Task<DcResponse> MeAsync(PlayerInfo p)
    {
        var notice = await TakeNoticeAsync(p.Id);          // brings HP up to date (misses since the last visit)
        var c = await characters.GetAsync(p.Id);
        var stats = PlayerStats.Total(c);
        var level = PlayerStats.Level(c);
        var start = Gamification.XpForLevel(level);
        var next = Gamification.XpForLevel(level + 1);
        var logDates = await db.ActivityLogs.AsNoTracking().Where(l => l.UserId == p.Id).Select(l => l.LoggedAt).ToListAsync();
        var streak = Gamification.Streak(logDates, p.Clock);

        var rank = c.ActiveTitleId is { } tid && GameCatalog.Current.Titles.TryGetValue(tid, out var t)
            ? $"{t.Icon} {t.Name}" : $"{Gamification.RankIconOf(level)} {Gamification.TitleOf(level)}";
        var pet = c.Pets.FirstOrDefault(x => x.Id == c.CompanionPetId);

        var embed = Card($"{Escape(p.Username)} — Level {level}", $"**{rank}**", c.Hp * 3 < stats.MaxHp ? Danger : Info);
        embed.Author = new DcAuthor { Name = "Your character" };
        if (!string.IsNullOrWhiteSpace(options.Value.WebBaseUrl))
            embed.Url = $"{options.Value.WebBaseUrl.TrimEnd('/')}/u/{Uri.EscapeDataString(p.Username)}";
        embed.Fields =
        [
            new() { Name = "❤️ HP", Value = $"`{Bar(c.Hp, stats.MaxHp)}` {c.Hp}/{stats.MaxHp}", Inline = false },
            new() { Name = "💧 Mana", Value = $"`{Bar(c.Mana, stats.MaxMana)}` {c.Mana}/{stats.MaxMana}", Inline = false },
            new() { Name = "⭐ EXP", Value = $"`{Bar(c.Exp - start, next - start)}` {c.Exp:N0}/{next:N0}", Inline = false },
            new() { Name = "Stats", Value = $"⚔️ ATK **{stats.Atk}** · 🔮 INT **{stats.Int}** · 🛡️ DEF **{stats.Def}** · 🍀 LUK **{stats.Luk}**", Inline = false },
            new() { Name = "🪙 Gold", Value = c.Gold.ToString("N0", Ci), Inline = true },
            new() { Name = "🔥 Streak", Value = $"{streak} day{(streak == 1 ? "" : "s")}", Inline = true },
            new() { Name = "✅ Tasks done", Value = c.TasksCompleted.ToString("N0", Ci), Inline = true },
        ];
        if (pet is not null)
        {
            var sp = PetCatalog.Get(pet.SpeciesId);
            embed.Fields.Add(new DcField { Name = "🐾 Companion", Value = $"{sp.Emoji} {Escape(pet.Name)} (Lv {pet.Level})", Inline = true });
        }
        if (notice is not null) embed.Fields.Insert(0, new DcField { Name = "⚠️ While you were away", Value = Truncate(notice, 1000), Inline = false });
        embed.Timestamp = DateTime.UtcNow.ToString("o");

        return Reply(embed, [new DcRow { Components = [QuestsButton()] }]);
    }

    // ---------- /quests ----------

    private async Task<DcResponse> QuestsAsync(PlayerInfo p, string? content)
    {
        var (embeds, rows) = await BuildQuestsAsync(p, includeNotice: true);
        return Reply(embeds, rows, content);
    }

    private async Task<(List<DcEmbed>, List<DcRow>)> BuildQuestsAsync(PlayerInfo p, bool includeNotice)
    {
        var notice = includeNotice ? await TakeNoticeAsync(p.Id) : null;
        var board = await boards.LoadAsync(p.Id, p.Clock);
        var today = board.Today;

        var pendingDailies = board.Dailies.Where(d => !board.DoneDailyIds.Contains(d.Id)).ToList();
        var embed = Card($"📋 {Escape(p.Username)}'s quests", null, Brand);

        if (board.Habits.Count + board.Dailies.Count + board.Todos.Count == 0)
        {
            embed.Description = "You have no tasks yet.\n\nCreate one with `/todo`, `/daily add` or `/habit add`.";
        }
        else
        {
            if (board.Habits.Count > 0)
                embed.Fields = [.. embed.Fields ?? [], Section("🔁 Habits", board.Habits.Select(h =>
                {
                    var counts = board.HabitCounts.GetValueOrDefault(h.Id);
                    return $"• **{Escape(Truncate(h.Title, 60))}** {Stars(h.Difficulty)} · +{counts.Positive} −{counts.Negative}";
                }).ToList())];

            if (board.Dailies.Count > 0)
                embed.Fields = [.. embed.Fields ?? [], Section("📅 Dailies", board.Dailies.Select(d =>
                {
                    var done = board.DoneDailyIds.Contains(d.Id);
                    var repeat = d.Repeat is { } r && r != RepeatType.Daily ? $" ({TaskLabels.Repeat(r)})" : "";
                    var streak = d.Streak > 0 ? $" 🔥{d.Streak}" : "";
                    return done ? $"✅ ~~{Escape(Truncate(d.Title, 60))}~~{repeat}" : $"⬜ **{Escape(Truncate(d.Title, 60))}**{repeat}{streak}";
                }).ToList())];

            if (board.Todos.Count > 0)
                embed.Fields = [.. embed.Fields ?? [], Section("📌 To-Dos", board.Todos.Select(t =>
                {
                    var bits = new List<string>();
                    if (t.Deadline is { } due)
                        bits.Add(due < today ? $"⚠️ overdue ({due.ToString("MMM d", Ci)})" : due == today ? "⏰ today" : $"⏰ {due.ToString("MMM d", Ci)}");
                    if (t.Category is { } cat && t.Effort is { } eff)
                    {
                        var info = Categories.Get(cat);
                        bits.Add($"{info.Emoji} {eff.Num()} {info.UnitFor(eff)}");
                    }
                    return $"⬜ **{Escape(Truncate(t.Title, 60))}** {Stars(t.Difficulty)}" + (bits.Count > 0 ? " · " + string.Join(" · ", bits) : "");
                }).ToList())];

            embed.Footer = new DcFooter { Text = $"QuestLife · {pendingDailies.Count} daily left · {board.Todos.Count} to-do open · press a button to complete" };
        }
        if (notice is not null)
            embed.Fields = [new DcField { Name = "⚠️ While you were away", Value = Truncate(notice, 1000), Inline = false }, .. embed.Fields ?? []];

        // buttons: up to 10 things to complete (dailies first, then to-dos) and 5 habits to press +
        var rows = new List<DcRow>();
        var completable = pendingDailies.Concat(board.Todos).Take(10).ToList();
        foreach (var chunk in completable.Chunk(5))
            rows.Add(new DcRow { Components = chunk.Select(t => new DcButton { Style = 3, Label = Truncate("✅ " + t.Title, 80), CustomId = $"done:{t.Id}" }).ToList() });
        var habits = board.Habits.Take(5).ToList();
        if (habits.Count > 0)
            rows.Add(new DcRow { Components = habits.Select(h => new DcButton { Style = 2, Label = Truncate("＋ " + h.Title, 80), CustomId = $"habit:{h.Id}" }).ToList() });
        rows.Add(new DcRow { Components = [QuestsButton("🔄 Refresh")] });

        return ([embed], rows);
    }

    private static DcField Section(string name, List<string> lines)
    {
        const int maxLines = 8;
        var shown = lines.Take(maxLines).ToList();
        if (lines.Count > maxLines) shown.Add($"…and {lines.Count - maxLines} more (see the website)");
        return new DcField { Name = name, Value = Truncate(string.Join("\n", shown), 1024), Inline = false };
    }

    private static DcButton QuestsButton(string label = "📋 My quests") => new() { Style = 1, Label = label, CustomId = "quests" };

    // ---------- /done, habits ----------

    private async Task<DcResponse> DoneAsync(PlayerInfo p, Args args)
    {
        var (taskId, error) = await ResolveTaskAsync(p, args.Str("task"), TaskType.Todo, TaskType.Daily);
        if (taskId is null) return Reply(Card("Couldn't find that task", error!, Warning), [new DcRow { Components = [QuestsButton()] }]);

        var result = await tasks.CompleteAsync(p.Id, taskId.Value);
        return Reply(ResultCard("✅ Task completed", "⚠️ Couldn't complete it", result), [new DcRow { Components = [QuestsButton()] }]);
    }

    private async Task<DcResponse> HabitAsync(PlayerInfo p, Args args, bool positive)
    {
        var (taskId, error) = await ResolveTaskAsync(p, args.Str("task"), TaskType.Habit);
        if (taskId is null) return Reply(Card("Couldn't find that habit", error!, Warning));

        var result = await tasks.HabitAsync(p.Id, taskId.Value, positive);
        // a successful "−" is a penalty, so it is shown in red
        var embed = positive || !result.Ok
            ? ResultCard("💪 Nice work!", "⚠️ Couldn't update the habit", result)
            : Card("😔 Slipped", result.Message, Danger);
        return Reply(embed, [new DcRow { Components = [QuestsButton()] }]);
    }

    private static DcEmbed ResultCard(string okTitle, string failTitle, GameResult r) =>
        r.Ok ? Card(okTitle, r.Message, Success) : Card(failTitle, r.Message, Danger);

    /// <summary>The "task" option is normally a task id picked from autocomplete; if the user typed free text, match it by title.</summary>
    private async Task<(Guid? Id, string? Error)> ResolveTaskAsync(PlayerInfo p, string? value, params TaskType[] types)
    {
        if (string.IsNullOrWhiteSpace(value)) return (null, "Tell me which task you mean.");

        var board = await boards.LoadAsync(p.Id, p.Clock);
        var open = new List<TaskItem>();
        if (types.Contains(TaskType.Habit)) open.AddRange(board.Habits);
        if (types.Contains(TaskType.Daily)) open.AddRange(board.Dailies.Where(d => !board.DoneDailyIds.Contains(d.Id)));
        if (types.Contains(TaskType.Todo)) open.AddRange(board.Todos);

        if (Guid.TryParse(value, out var id))
            return open.Any(t => t.Id == id) ? (id, null) : (null, "That task isn't open any more (it may already be completed). Use `/quests` to see what's left.");

        var matches = open.Where(t => t.Title.Contains(value.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
        if (matches.Count == 1) return (matches[0].Id, null);
        if (matches.Count == 0) return (null, $"No open task matches “{Escape(Truncate(value, 50))}”. Use `/quests` to see your tasks.");
        return (null, "More than one task matches. Start typing and pick one from the list:\n" +
            string.Join("\n", matches.Take(5).Select(t => $"• {Escape(Truncate(t.Title, 60))}")));
    }

    // ---------- creating tasks ----------

    private async Task<DcResponse> CreateTaskAsync(PlayerInfo p, Args args, TaskType type)
    {
        var title = args.Str("title")?.Trim() ?? "";
        if (title.Length == 0 || title.Length > 200)
            return Reply(Card("Check the title", "The title must be 1 to 200 characters.", Warning));

        var form = new TaskFormViewModel
        {
            Type = type,
            Title = title,
            Description = args.Str("description"),
            Difficulty = Math.Clamp(args.Int("difficulty") ?? 3, 1, 5),
            Priority = Enum.IsDefined(typeof(TaskPriority), args.Int("priority") ?? 2) ? (TaskPriority)(args.Int("priority") ?? 2) : TaskPriority.Medium,
        };

        if (type == TaskType.Daily)
            form.Repeat = Enum.TryParse<RepeatType>(args.Str("repeat"), out var rep) ? rep : RepeatType.Daily;

        if (type == TaskType.Todo)
        {
            if (Enum.TryParse<ActivityCategory>(args.Str("category"), out var cat) && Enum.IsDefined(cat)) form.Category = cat;
            form.Effort = args.Num("effort");
            if (form.Category is null && form.Effort is not null)
                return Reply(Card("Effort needs a category", "Pick a category too, or leave the effort out.", Warning));

            if (args.Str("deadline") is { Length: > 0 } text)
            {
                var deadline = ParseDeadline(text, p.Clock.Today);
                if (deadline is null)
                    return Reply(Card("I couldn't read that deadline", "Use `today`, `tomorrow`, `+3d` (three days from now) or a date like `2026-10-25`.", Warning));
                if (deadline < p.Clock.Today)
                    return Reply(Card("That deadline is in the past", "A deadline that has already passed would cost you HP straight away.", Warning));
                form.Deadline = deadline;
            }
        }

        var errors = TaskFormValidator.Validate(form);
        if (errors.Count > 0)
            return Reply(Card("Check your input", string.Join("\n", errors.Select(e => "• " + e.Message)), Warning));

        await tasks.CreateAsync(p.Id, form, p.Clock);

        var kind = type switch { TaskType.Habit => "🔁 Habit", TaskType.Daily => "📅 Daily", _ => "📌 To-do" };
        var embed = Card($"{kind} created", $"**{Escape(title)}**", Success);
        var fields = new List<DcField>
        {
            new() { Name = "Difficulty", Value = $"{Stars(form.Difficulty)} {TaskLabels.Difficulty(form.Difficulty)}", Inline = true },
            new() { Name = "Priority", Value = TaskLabels.Priority(form.Priority), Inline = true },
        };
        if (form.Repeat is { } r) fields.Add(new DcField { Name = "Repeats", Value = TaskLabels.Repeat(r), Inline = true });
        if (form.Deadline is { } dl) fields.Add(new DcField { Name = "Deadline", Value = dl.ToString("ddd, MMM d", Ci), Inline = true });
        if (form.Category is { } ct && form.Effort is { } ef)
        {
            var info = Categories.Get(ct);
            fields.Add(new DcField { Name = "Category", Value = $"{info.Emoji} {info.Name} · {ef.Num()} {info.UnitFor(ef)}", Inline = true });
        }
        var effort = form.Effort ?? 1;
        fields.Add(new DcField
        {
            Name = "Base reward",
            Value = $"🪙 {Math.Round(Rewards.BaseGold[form.Difficulty] * effort)} · ⭐ {Math.Round(Rewards.BaseExp[form.Difficulty] * effort)} EXP" +
                    (type == TaskType.Habit ? " per +" : ""),
            Inline = false,
        });
        embed.Fields = fields;
        return Reply(embed, [new DcRow { Components = [QuestsButton()] }]);
    }

    private static DateOnly? ParseDeadline(string text, DateOnly today)
    {
        text = text.Trim().ToLowerInvariant();
        switch (text)
        {
            case "today": return today;
            case "tomorrow" or "tmrw": return today.AddDays(1);
        }
        var m = Regex.Match(text, @"^\+(\d{1,3})d?$");
        if (m.Success) return today.AddDays(int.Parse(m.Groups[1].Value, Ci));
        return DateOnly.TryParseExact(text, "yyyy-MM-dd", Ci, DateTimeStyles.None, out var d) ? d : null;
    }

    // =====================================================================================================
    // buttons
    // =====================================================================================================

    private async Task<DcResponse> HandleComponentAsync(Ctx ctx)
    {
        var customId = ctx.Data.GetProperty("custom_id").GetString() ?? "";

        var allowed = options.Value.AllowedChannelId;
        if (!string.IsNullOrWhiteSpace(allowed) && ctx.ChannelId != allowed)
            return Reply(Card("Wrong channel", $"Please use QuestLife in <#{allowed}>.", Warning));

        var userId = await links.FindUserIdAsync(ctx.DiscordUserId);
        if (userId is null) return NotLinked();
        var p = await GetUserAsync(userId.Value);

        string? content = null;
        if (customId.StartsWith("done:") && Guid.TryParse(customId[5..], out var doneId))
        {
            var r = await tasks.CompleteAsync(p.Id, doneId);
            content = (r.Ok ? "✅ " : "⚠️ ") + r.Message;
        }
        else if (customId.StartsWith("habit:") && Guid.TryParse(customId[6..], out var habitId))
        {
            var r = await tasks.HabitAsync(p.Id, habitId, positive: true);
            content = (r.Ok ? "💪 " : "⚠️ ") + r.Message;
        }
        // "quests" (and anything else) simply refreshes the list

        var (embeds, rows) = await BuildQuestsAsync(p, includeNotice: content is null);
        return Update(embeds, rows, content is null ? null : Truncate(content, 1900));
    }

    // =====================================================================================================
    // autocomplete
    // =====================================================================================================

    private async Task<DcResponse> HandleAutocompleteAsync(Ctx ctx)
    {
        var empty = new DcResponse { Type = DcCallback.Autocomplete, Data = new DcData { Choices = [] } };
        var args = ReadArgs(ctx.Data);
        if (args.Focused != "task") return empty;

        var userId = await links.FindUserIdAsync(ctx.DiscordUserId);
        if (userId is null) return empty;
        var p = await GetUserAsync(userId.Value);
        var board = await boards.LoadAsync(p.Id, p.Clock);
        var typed = (args.Str("task") ?? "").Trim();

        List<(TaskItem Task, string Label)> candidates;
        if (args.Command == "habit")
            candidates = board.Habits.Select(h => (h, "🔁 " + h.Title)).ToList();
        else
            candidates = board.Dailies.Where(d => !board.DoneDailyIds.Contains(d.Id)).Select(d => (d, $"📅 {d.Title} ({TaskLabels.Repeat(d.Repeat ?? RepeatType.Daily)})"))
                .Concat(board.Todos.Select(t => (t, "📌 " + t.Title + (t.Deadline is { } due ? $" · ⏰ {due.ToString("MMM d", Ci)}" : ""))))
                .ToList();

        var choices = candidates
            .Where(c => typed.Length == 0 || c.Task.Title.Contains(typed, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(c => new DcChoice { Name = Truncate(c.Label, 100), Value = c.Task.Id.ToString() })
            .ToList();
        return new DcResponse { Type = DcCallback.Autocomplete, Data = new DcData { Choices = choices } };
    }

    // =====================================================================================================
    // misc
    // =====================================================================================================

    /// <summary>Penalties applied while the player was away (daily periods and deadlines), as one line of text.</summary>
    private async Task<string?> TakeNoticeAsync(Guid userId)
    {
        var r = await tasks.TakeNoticesAsync(userId);
        return r.Ok && !string.IsNullOrWhiteSpace(r.Message) ? r.Message : null;
    }
}
