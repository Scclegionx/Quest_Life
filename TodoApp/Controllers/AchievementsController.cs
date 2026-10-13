using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

[Authorize]
public class AchievementsController(CharacterService characters, AchievementService achievements) : GameControllerBase
{
    public async Task<IActionResult> Index()
    {
        var c = await characters.GetAsync(UserId);
        var ctx = await achievements.BuildContextAsync(c, Clock);
        var titles = GameCatalog.Current.Titles;

        return View(new AchievementsViewModel
        {
            Player = c,
            Items = AchievementService.Evaluate(ctx, c)
                .OrderBy(a => a.Claimed).ThenByDescending(a => a.Completed).ThenByDescending(a => a.Percent).ToList(),
            OwnedTitles = c.Titles.Where(t => titles.ContainsKey(t.TitleId)).Select(t => titles[t.TitleId]).ToList(),
            ActiveTitleName = c.ActiveTitleId is { } id && titles.TryGetValue(id, out var active) ? active.Name : null,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Claim(string id) =>
        Done(await characters.RunAsync(UserId, async (c, clock) =>
            AchievementService.Claim(c, id, await achievements.BuildContextAsync(c, clock))), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetTitle(string? id) =>
        Done(await characters.MutateAsync(UserId, (c, _) => AchievementService.SetTitle(c, id)), nameof(Index));
}
