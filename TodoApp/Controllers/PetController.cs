using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

[Authorize]
public class PetController(CharacterService characters) : GameControllerBase
{
    public async Task<IActionResult> Index()
    {
        var c = await characters.GetAsync(UserId);
        return View(new PetPageViewModel
        {
            Player = c,
            NowUtc = DateTime.UtcNow,
            Eggs = c.Items.Where(i => PetCatalog.Eggs.ContainsKey(i.ItemId))
                .Select(i => new BagEntry(ItemCatalog.Find(i.ItemId)!, i.Quantity)).ToList(),
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Incubate(string eggId) =>
        Done(await characters.MutateAsync(UserId, (c, clock) => PetLogic.Incubate(c, eggId, clock.NowUtc)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Hatch(Guid id) =>
        Done(await characters.MutateAsync(UserId, (c, clock) => PetLogic.Hatch(c, id, clock.NowUtc, Random.Shared)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Feed(Guid id) =>
        Done(await characters.MutateAsync(UserId, (c, _) => PetLogic.Feed(c, id)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Companion(Guid id) =>
        Done(await characters.MutateAsync(UserId, (c, _) => PetLogic.SetCompanion(c, id)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpgradeCoop() =>
        Done(await characters.MutateAsync(UserId, (c, _) => PetLogic.UpgradeCoop(c)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpgradeIncubator() =>
        Done(await characters.MutateAsync(UserId, (c, _) => PetLogic.UpgradeIncubator(c)), nameof(Index));
}
