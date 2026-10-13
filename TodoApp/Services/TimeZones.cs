using TodoApp.Data;

namespace TodoApp.Services;

/// <summary>Time zones offered in the UI (from the time_zones table; any valid IANA id is accepted by the server).</summary>
public static class TimeZones
{
    public static IReadOnlyList<(string Id, string Label)> Common => GameCatalog.Current.TimeZones;
}
