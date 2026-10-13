using System.Globalization;

namespace TodoApp.Services;

/// <summary>
/// The game-wide clock: leaderboard weeks and the weekly raid use ONE fixed zone (default UTC+7, see GameTimeZoneOffsetHours),
/// so every player shares the same boundary. Per-player days use <see cref="UserClock"/> instead.
/// </summary>
public static class AppTime
{
    public static TimeSpan Offset { get; set; } = TimeSpan.FromHours(7);

    public static DateTime LocalNow => DateTime.UtcNow + Offset;
    public static DateTime Today => LocalNow.Date;

    /// <summary>Monday 00:00 (game time) of the current week, expressed in UTC.</summary>
    public static DateTime StartOfWeekUtc()
    {
        var today = Today;
        var sinceMonday = ((int)today.DayOfWeek + 6) % 7;
        return DateTime.SpecifyKind(today.AddDays(-sinceMonday) - Offset, DateTimeKind.Utc);
    }
}

/// <summary>A player's own clock: decides where "today" starts for dailies, deadlines, streaks and mana refresh.</summary>
public sealed class UserClock
{
    public const string DefaultZoneId = "Asia/Ho_Chi_Minh";

    public TimeZoneInfo Zone { get; }
    public DateTime NowUtc { get; }

    public UserClock(TimeZoneInfo zone, DateTime? nowUtc = null)
    {
        Zone = zone;
        NowUtc = nowUtc ?? DateTime.UtcNow;
    }

    public static UserClock For(string? zoneId, DateTime? nowUtc = null) => new(Resolve(zoneId), nowUtc);

    public static TimeZoneInfo Resolve(string? zoneId)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(string.IsNullOrWhiteSpace(zoneId) ? DefaultZoneId : zoneId); }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            return TimeZoneInfo.CreateCustomTimeZone("UTC+7", TimeSpan.FromHours(7), "UTC+7", "UTC+7");
        }
    }

    public static bool IsValidZone(string? zoneId)
    {
        if (string.IsNullOrWhiteSpace(zoneId)) return false;
        try { TimeZoneInfo.FindSystemTimeZoneById(zoneId); return true; }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException) { return false; }
    }

    public DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    public DateTime LocalNow => ToLocal(NowUtc);
    public DateOnly Today => DateOnly.FromDateTime(LocalNow);
    public DateOnly ToLocalDate(DateTime utc) => DateOnly.FromDateTime(ToLocal(utc));
}

public static class Format
{
    public static string Num(this decimal value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    /// <summary>"1 time" / "2 times", "1 hour" / "1.5 hours".</summary>
    public static string UnitFor(this Models.CategoryInfo info, decimal amount) =>
        amount == 1 ? info.Unit.TrimEnd('s') : info.Unit;
}
