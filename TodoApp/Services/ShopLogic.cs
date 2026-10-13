using TodoApp.Models;

namespace TodoApp.Services;

public static class ShopLogic
{
    public static bool Owns(Character c, string itemId) => c.Items.Any(i => i.ItemId == itemId && i.Quantity > 0);

    public static bool IsEquipped(Character c, string itemId) => c.Equipment.Any(e => e.ItemId == itemId);

    public static GameResult Buy(Character c, string itemId)
    {
        var item = ItemCatalog.Find(itemId);
        if (item is null) return GameResult.Fail("Item does not exist.");

        var isEgg = item.Slot == ItemSlot.Egg;
        if (!isEgg && Owns(c, item.Id)) return GameResult.Fail($"You already own {item.Name}.");
        if (c.Gold < item.Price) return GameResult.Fail($"Not enough gold: you need {item.Price} 🪙 but have {c.Gold} 🪙.");

        c.Gold -= item.Price;
        var row = c.Items.FirstOrDefault(i => i.ItemId == item.Id);
        if (row is null) c.Items.Add(new CharacterItem { UserId = c.UserId, ItemId = item.Id, Quantity = 1 });
        else row.Quantity++;

        GameLog.Ledger(c, LedgerKinds.Purchase, gold: -item.Price, itemId: item.Id, note: item.Name);
        return GameResult.Success($"Bought {item.Emoji} {item.Name} (-{item.Price} 🪙)");
    }

    public static GameResult Equip(Character c, string itemId)
    {
        var item = ItemCatalog.Find(itemId);
        if (item is null || item.Slot == ItemSlot.Egg) return GameResult.Fail("This item cannot be equipped.");
        if (!Owns(c, item.Id)) return GameResult.Fail("You do not own this item.");

        var slot = c.Equipment.FirstOrDefault(e => e.SlotId == item.Slot);
        if (slot is null) c.Equipment.Add(new CharacterEquipment { UserId = c.UserId, SlotId = item.Slot, ItemId = item.Id });
        else slot.ItemId = item.Id; // the previously worn item stays in the bag (it is still owned)

        PlayerStats.ClampVitals(c);
        return GameResult.Success($"Equipped {item.Emoji} {item.Name}");
    }

    public static GameResult Unequip(Character c, string slotName)
    {
        if (!Enum.TryParse<ItemSlot>(slotName, out var slot)) return GameResult.Fail("Invalid equipment slot.");
        var worn = c.Equipment.FirstOrDefault(e => e.SlotId == slot);
        if (worn is null) return GameResult.Fail("This slot is empty.");

        c.Equipment.Remove(worn);
        PlayerStats.ClampVitals(c);
        return GameResult.Success($"Moved {ItemCatalog.Find(worn.ItemId)?.Name} to your bag");
    }
}
