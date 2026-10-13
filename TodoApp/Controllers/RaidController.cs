using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

public class RaidController(RaidService raid, CharacterService characters) : GameControllerBase
{
    public async Task<IActionResult> Index()
    {
        var me = UserIdOrNull;
        return View(new RaidPageViewModel { Status = await raid.GetStatusAsync(me), Authed = me is not null });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Join() => Done(await raid.JoinAsync(UserId), nameof(Index));

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Claim(Guid raidId) =>
        Done(await characters.RunAsync(UserId, (c, _) => raid.ClaimAsync(c, raidId)), nameof(Index));
}
