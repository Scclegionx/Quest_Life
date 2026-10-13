using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

[Authorize]
public class CharacterController(CharacterService characters) : GameControllerBase
{
    public async Task<IActionResult> Index()
    {
        var c = await characters.GetAsync(UserId);
        var level = PlayerStats.Level(c);
        var titleName = c.ActiveTitleId is { } tid && GameCatalog.Current.Titles.TryGetValue(tid, out var t) ? t.Name : null;

        return View(new CharacterViewModel
        {
            Username = User.Identity!.Name!,
            Level = level,
            Title = titleName ?? Gamification.TitleOf(level),
            Player = c,
            Base = PlayerStats.Base(level),
            Gear = PlayerStats.Gear(c),
            Total = PlayerStats.Total(c),
            LevelStartXp = Gamification.XpForLevel(level),
            NextLevelXp = Gamification.XpForLevel(level + 1),
            Slots = SlotInfo.Gear
                .Select(s => new SlotView(s, c.Equipment.FirstOrDefault(e => e.SlotId == s) is { } e ? ItemCatalog.Find(e.ItemId) : null))
                .ToList(),
            // the bag shows what is owned but not worn
            Bag = c.Items
                .Where(i => ItemCatalog.Find(i.ItemId) is not null && !ShopLogic.IsEquipped(c, i.ItemId))
                .Select(i => new BagEntry(ItemCatalog.Find(i.ItemId)!, i.Quantity))
                .OrderBy(b => b.Item.Slot).ThenBy(b => b.Item.Rarity).ToList(),
            Companion = c.Pets.FirstOrDefault(p => p.Id == c.CompanionPetId),
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Equip(string itemId) =>
        Done(await characters.MutateAsync(UserId, (c, _) => ShopLogic.Equip(c, itemId)), nameof(Index));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unequip(string slot) =>
        Done(await characters.MutateAsync(UserId, (c, _) => ShopLogic.Unequip(c, slot)), nameof(Index));
}
