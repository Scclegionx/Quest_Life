using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

public class ShopController(CharacterService characters) : GameControllerBase
{
    public async Task<IActionResult> Index(string category = "weapon", string? sub = null)
    {
        var current = ItemCatalog.Categories.FirstOrDefault(c => c.Key == category) ?? ItemCatalog.Categories[0];
        var inCategory = ItemCatalog.All.Where(i => current.Slots.Contains(i.Slot)).ToList();
        var subTypes = inCategory.Select(i => i.SubType).Distinct().ToList();
        if (sub is not null && !subTypes.Contains(sub)) sub = null;

        return View(new ShopViewModel
        {
            Categories = ItemCatalog.Categories,
            Current = current,
            Sub = sub,
            SubTypes = subTypes,
            Items = inCategory.Where(i => sub is null || i.SubType == sub).OrderBy(i => i.Rarity).ThenBy(i => i.Price).ToList(),
            Authed = User.Identity?.IsAuthenticated == true,
            Player = UserIdOrNull is { } id ? await characters.GetAsync(id) : null,
        });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Buy(string itemId, string category = "weapon", string? sub = null)
    {
        var result = await characters.MutateAsync(UserId, (c, _) => ShopLogic.Buy(c, itemId));
        return Done(result, nameof(Index), new { category, sub });
    }
}
