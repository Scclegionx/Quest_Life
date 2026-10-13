using TodoApp.Data;

namespace TodoApp.Models;

public record StatBlock(int MaxHp, int MaxMana, int Atk, int Def, int Int, int Luk)
{
    public static readonly StatBlock Zero = new(0, 0, 0, 0, 0, 0);

    public static StatBlock operator +(StatBlock a, StatBlock b) =>
        new(a.MaxHp + b.MaxHp, a.MaxMana + b.MaxMana, a.Atk + b.Atk, a.Def + b.Def, a.Int + b.Int, a.Luk + b.Luk);

    public bool IsZero => this == Zero;
}

public static class ItemStats
{
    public static string Describe(Item i)
    {
        var s = i.Stats;
        var parts = new List<string>();
        void Add(int v, string label) { if (v > 0) parts.Add($"{label} +{v}"); }
        Add(s.Atk, "⚔️ ATK"); Add(s.Def, "🛡️ DEF"); Add(s.Int, "🔮 INT"); Add(s.Luk, "🍀 LUK");
        Add(s.MaxHp, "❤️ HP"); Add(s.MaxMana, "💧 MP");
        return string.Join(" · ", parts);
    }
}

public static class PlayerStats
{
    public static int Level(Character c) => Services.Gamification.LevelOf(c.Exp);

    public static StatBlock Base(int level) => new(
        MaxHp: (int)(GameSettings.HpBase + GameSettings.HpPerLevel * level),
        MaxMana: (int)(GameSettings.ManaBase + GameSettings.ManaPerLevel * level),
        Atk: (int)(GameSettings.AtkBase + GameSettings.AtkPerLevel * level),
        Def: GameSettings.DefBase + level / GameSettings.DefLevelsPerPoint,
        Int: (int)(GameSettings.IntBase + GameSettings.IntPerLevel * level),
        Luk: GameSettings.LukBase + level / GameSettings.LukLevelsPerPoint);

    public static StatBlock Gear(Character c)
    {
        var total = StatBlock.Zero;
        foreach (var e in c.Equipment)
            if (ItemCatalog.Find(e.ItemId) is { } item) total += item.Stats;
        return total;
    }

    public static StatBlock Total(Character c) => Base(Level(c)) + Gear(c);

    /// <summary>Keeps current HP/MP within the (possibly changed) maximums.</summary>
    public static void ClampVitals(Character c)
    {
        var s = Total(c);
        c.Hp = Math.Clamp(c.Hp, 0, s.MaxHp);
        c.Mana = Math.Clamp(c.Mana, 0, s.MaxMana);
    }
}
