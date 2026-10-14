using System.Text.RegularExpressions;
using TodoApp.Models;

namespace TodoApp.Services.Discord;

/// <summary>Definitions of the slash commands, in the format Discord's "bulk overwrite" API expects.</summary>
public static class DiscordCommands
{
    // Discord option types
    private const int SubCommand = 1, String = 3, Integer = 4, Number = 10;

    private static Dictionary<string, object?> Opt(int type, string name, string description, bool required = false,
        IEnumerable<object>? choices = null, bool autocomplete = false, int? minLength = null, int? maxLength = null,
        double? minValue = null, double? maxValue = null)
    {
        var d = new Dictionary<string, object?> { ["type"] = type, ["name"] = name, ["description"] = description };
        if (required) d["required"] = true;
        if (choices is not null) d["choices"] = choices.ToList();
        if (autocomplete) d["autocomplete"] = true;
        if (minLength is not null) d["min_length"] = minLength;
        if (maxLength is not null) d["max_length"] = maxLength;
        if (minValue is not null) d["min_value"] = minValue;
        if (maxValue is not null) d["max_value"] = maxValue;
        return d;
    }

    private static Dictionary<string, object?> Sub(string name, string description, params Dictionary<string, object?>[] options) =>
        new() { ["type"] = SubCommand, ["name"] = name, ["description"] = description, ["options"] = options.ToList() };

    private static Dictionary<string, object?> Cmd(string name, string description, params Dictionary<string, object?>[] options)
    {
        var d = new Dictionary<string, object?> { ["name"] = name, ["description"] = description, ["type"] = 1 };
        if (options.Length > 0) d["options"] = options.ToList();
        return d;
    }

    private static object Choice(string name, object value) => new Dictionary<string, object> { ["name"] = name, ["value"] = value };

    public static List<Dictionary<string, object?>> Build()
    {
        // choices come from the database so they always match what the website offers
        var difficulty = Enumerable.Range(1, 5).Select(i => Choice($"{TaskLabels.Stars(i)} {TaskLabels.Difficulty(i)}", i)).ToList();
        var priority = Enum.GetValues<TaskPriority>().Select(p => Choice(TaskLabels.Priority(p), (int)p)).ToList();
        var categories = Categories.All.Select(c => Choice($"{c.Emoji} {c.Name} ({c.Unit})", c.Key.ToString())).ToList();
        var repeats = Enum.GetValues<RepeatType>().Select(r => Choice(TaskLabels.Repeat(r), r.ToString())).ToList();

        Dictionary<string, object?> Title() => Opt(String, "title", "What needs doing", required: true, minLength: 1, maxLength: 200);
        Dictionary<string, object?> Diff() => Opt(Integer, "difficulty", "How hard is it? (default: Medium)", choices: difficulty);
        Dictionary<string, object?> Prio() => Opt(Integer, "priority", "How important is it? (default: Medium)", choices: priority);
        Dictionary<string, object?> Desc() => Opt(String, "description", "Optional notes", maxLength: 500);

        return
        [
            Cmd("help", "What QuestLife can do in Discord"),
            Cmd("link", "Link your Discord to your QuestLife account",
                Opt(String, "code", "The code from QuestLife → Settings → Link Discord", required: true, minLength: 4, maxLength: 20)),
            Cmd("unlink", "Unlink your Discord from your QuestLife account"),
            Cmd("me", "Show your character"),
            Cmd("quests", "Your habits, dailies and to-dos, with buttons to complete them"),
            Cmd("done", "Complete a daily or to-do",
                Opt(String, "task", "Start typing to find the task", required: true, autocomplete: true, maxLength: 100)),
            Cmd("todo", "Create a to-do",
                Title(),
                Diff(), Prio(),
                Opt(String, "category", "Counts for the leaderboards (needs effort)", choices: categories),
                Opt(Number, "effort", "Hours or times, depending on the category", minValue: 0.25, maxValue: 20),
                Opt(String, "deadline", "today, tomorrow, +3d (days from now) or YYYY-MM-DD", maxLength: 20),
                Desc()),
            Cmd("daily", "Create a recurring task",
                Sub("add", "Add a daily",
                    Title(),
                    Opt(String, "repeat", "How often it repeats (default: daily)", choices: repeats),
                    Diff(), Prio(), Desc())),
            Cmd("habit", "Create a habit, or press + / − on one",
                Sub("add", "Add a habit", Title(), Diff(), Prio(), Desc()),
                Sub("plus", "I did well! (earns the reward)",
                    Opt(String, "task", "Start typing to find the habit", required: true, autocomplete: true, maxLength: 100)),
                Sub("minus", "I slipped (costs HP)",
                    Opt(String, "task", "Start typing to find the habit", required: true, autocomplete: true, maxLength: 100))),
        ];
    }

    /// <summary>Checks the rules Discord enforces, so a mistake is caught at startup instead of as a 400 from the API.</summary>
    public static List<string> Validate(IEnumerable<Dictionary<string, object?>> commands)
    {
        var problems = new List<string>();
        var nameRule = new Regex(@"^[-_\p{L}\p{N}]{1,32}$");
        var names = new HashSet<string>();

        void CheckOptions(string path, IList<Dictionary<string, object?>>? options)
        {
            if (options is null) return;
            if (options.Count > 25) problems.Add($"{path}: more than 25 options");
            var seenOptional = false;
            foreach (var o in options)
            {
                var name = (string)o["name"]!;
                var type = (int)o["type"]!;
                var label = $"{path}/{name}";
                if (!nameRule.IsMatch(name) || name != name.ToLowerInvariant()) problems.Add($"{label}: bad name");
                var description = (string)o["description"]!;
                if (description.Length is < 1 or > 100) problems.Add($"{label}: description must be 1-100 characters");
                var required = o.TryGetValue("required", out var r) && r is true;
                if (required && seenOptional) problems.Add($"{label}: required option after an optional one");
                if (!required) seenOptional = true;
                if (o.TryGetValue("choices", out var c) && c is IList<object> choices)
                {
                    if (choices.Count > 25) problems.Add($"{label}: more than 25 choices");
                    if (o.ContainsKey("autocomplete")) problems.Add($"{label}: choices and autocomplete are mutually exclusive");
                    foreach (var ch in choices.Cast<Dictionary<string, object>>())
                        if (((string)ch["name"]).Length is < 1 or > 100) problems.Add($"{label}: choice name must be 1-100 characters");
                }
                if (type == SubCommand) CheckOptions(label, o["options"] as IList<Dictionary<string, object?>>);
            }
        }

        foreach (var cmd in commands)
        {
            var name = (string)cmd["name"]!;
            if (!nameRule.IsMatch(name) || name != name.ToLowerInvariant()) problems.Add($"/{name}: bad name");
            if (!names.Add(name)) problems.Add($"/{name}: duplicate command");
            var description = (string)cmd["description"]!;
            if (description.Length is < 1 or > 100) problems.Add($"/{name}: description must be 1-100 characters");
            CheckOptions($"/{name}", cmd.TryGetValue("options", out var o) ? o as IList<Dictionary<string, object?>> : null);
        }
        return problems;
    }
}
