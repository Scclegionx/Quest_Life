using Microsoft.AspNetCore.Mvc;
using TodoApp.Services;

namespace TodoApp.Controllers;

/// <summary>Public, read-only player profiles (no email, tasks or notes are shown).</summary>
public class ProfileController(ProfileService profiles) : GameControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(string username)
    {
        var profile = await profiles.GetAsync(username, UserIdOrNull);
        return profile is null ? NotFound() : View(profile);
    }

    /// <summary>"My profile" shortcut for the signed-in player.</summary>
    [HttpGet]
    public IActionResult Me() =>
        User.Identity?.IsAuthenticated == true
            ? RedirectToAction(nameof(Index), new { username = User.Identity!.Name })
            : RedirectToAction("Login", "Account");
}
