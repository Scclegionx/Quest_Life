using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public class ShopViewModel
{
    public IReadOnlyList<ShopCategory> Categories { get; set; } = [];
    public ShopCategory Current { get; set; } = null!;
    public string? Sub { get; set; }
    public List<string> SubTypes { get; set; } = [];
    public List<Item> Items { get; set; } = [];
    public bool Authed { get; set; }
    public Character? Player { get; set; }
}

public record SlotView(ItemSlot Slot, Item? Item);
public record BagEntry(Item Item, int Qty);

public class CharacterViewModel
{
    public string Username { get; set; } = "";
    public int Level { get; set; }
    public string Title { get; set; } = "";
    public Character Player { get; set; } = null!;
    public StatBlock Base { get; set; } = StatBlock.Zero;
    public StatBlock Gear { get; set; } = StatBlock.Zero;
    public StatBlock Total { get; set; } = StatBlock.Zero;
    public int LevelStartXp { get; set; }
    public int NextLevelXp { get; set; }
    public List<SlotView> Slots { get; set; } = [];
    public List<BagEntry> Bag { get; set; } = [];
    public OwnedPet? Companion { get; set; }
}

public class PetPageViewModel
{
    public Character Player { get; set; } = null!;
    public DateTime NowUtc { get; set; }
    public List<BagEntry> Eggs { get; set; } = [];
}

public class AchievementsViewModel
{
    public Character Player { get; set; } = null!;
    public List<AchievementStatus> Items { get; set; } = [];
    public List<TitleEntity> OwnedTitles { get; set; } = [];
    public string? ActiveTitleName { get; set; }
}

public class RaidPageViewModel
{
    public RaidStatus Status { get; set; } = null!;
    public bool Authed { get; set; }
}
