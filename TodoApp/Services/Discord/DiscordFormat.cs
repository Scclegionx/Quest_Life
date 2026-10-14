using System.Text;
using TodoApp.Models;

namespace TodoApp.Services.Discord;

/// <summary>Small helpers that make Discord messages look good (colours, progress bars, safe text).</summary>
public static class DiscordFormat
{
    public const int Success = 0x3DDC97;   // green
    public const int Warning = 0xFFD166;   // gold
    public const int Danger = 0xFF4D6D;    // red
    public const int Brand = 0x7C5CFF;     // purple
    public const int Info = 0x00D4FF;      // cyan

    /// <summary>▰▰▰▰▱▱▱▱▱▱ style bar.</summary>
    public static string Bar(int value, int max, int length = 10)
    {
        if (max <= 0) return new string('▱', length);
        var filled = (int)Math.Round(Math.Clamp(value / (double)max, 0, 1) * length);
        return new string('▰', filled) + new string('▱', length - filled);
    }

    /// <summary>Escapes Discord markdown so a task title can't break the formatting of the message.</summary>
    public static string Escape(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        var sb = new StringBuilder(text.Length + 8);
        foreach (var ch in text)
        {
            if (ch is '\\' or '*' or '_' or '~' or '`' or '|' or '>' or '[' or ']' or '#') sb.Append('\\');
            sb.Append(ch is '\n' or '\r' ? ' ' : ch);
        }
        return sb.ToString();
    }

    public static string Truncate(string? text, int max)
    {
        if (string.IsNullOrEmpty(text) || text.Length <= max) return text ?? "";
        return text[..Math.Max(0, max - 1)] + "…";
    }

    public static string Stars(int difficulty) => TaskLabels.Stars(difficulty);

    public static DcEmbed Card(string title, string? description, int color) =>
        new() { Title = Truncate(title, 256), Description = Truncate(description, 4000), Color = color, Footer = new DcFooter { Text = "QuestLife" } };
}
