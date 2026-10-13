using System.Security.Claims;

namespace TodoApp.Services;

public static class ClaimsExtensions
{
    public const string TimeZoneClaim = "tz";

    public static Guid? GetUserId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    /// <summary>The signed-in player's own clock (from the time zone stored in the sign-in cookie).</summary>
    public static UserClock GetClock(this ClaimsPrincipal user) => UserClock.For(user.FindFirstValue(TimeZoneClaim));
}
