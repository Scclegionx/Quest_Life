using TodoApp.Data;

namespace TodoApp.Models;

public enum Rarity { Common, Rare, Epic, Legendary }
public enum ItemSlot { Weapon, Helmet, Armor, Boots, Ring, Amulet, Egg }

/// <summary>Read model of a shop item (loaded from the items table). Stats are explicit bonuses.</summary>
public record Item(
    string Id, string Name, ItemSlot Slot, string SubType, Rarity Rarity,
    int Price, StatBlock Stats, string Emoji, string Description);

public record ShopCategory(string Key, string Name, string Emoji, ItemSlot[] Slots);

public static class RarityInfo
{
    public static string Label(this Rarity r) => GameCatalog.Current.RarityLabels.TryGetValue(r, out var l) ? l : r.ToString();
    public static string Css(this Rarity r) => "rar-" + r.ToString().ToLowerInvariant();
}

public static class SlotInfo
{
    public static string Label(this ItemSlot s) => GameCatalog.Current.Slots.TryGetValue(s, out var x) ? x.Name : s.ToString();
    public static string Emoji(this ItemSlot s) => GameCatalog.Current.Slots.TryGetValue(s, out var x) ? x.Emoji : "❔";
    /// <summary>Equippable slots (everything except eggs).</summary>
    public static readonly ItemSlot[] Gear = [ItemSlot.Weapon, ItemSlot.Helmet, ItemSlot.Armor, ItemSlot.Boots, ItemSlot.Ring, ItemSlot.Amulet];
}

public static class ItemCatalog
{
    public static IReadOnlyList<ShopCategory> Categories => GameCatalog.Current.ShopCategories;
    public static IReadOnlyList<Item> All => GameCatalog.Current.Items;
    public static Item? Find(string? id) => id is not null && GameCatalog.Current.ItemById.TryGetValue(id, out var item) ? item : null;
}
