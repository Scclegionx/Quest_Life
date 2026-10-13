using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public static class PetLogic
{
    // all of these numbers are game settings (see the game_settings table)
    public static int MaxCoopLevel => GameSettings.MaxCoopLevel;
    public static int MaxIncubatorLevel => GameSettings.MaxIncubatorLevel;
    public static int MaxPetLevel => GameSettings.MaxPetLevel;
    public static int FeedCost => GameSettings.PetFeedCost;
    public static int FeedXp => GameSettings.PetFeedXp;

    public static int CoopCapacity(Character c) => GameSettings.CoopCapacityBase + GameSettings.CoopCapacityPerLevel * c.CoopLevel;
    public static int IncubatorSlots(Character c) => c.IncubatorLevel;
    public static int CoopUpgradeCost(Character c) => GameSettings.CoopUpgradeCostPerLevel * c.CoopLevel;
    public static int IncubatorUpgradeCost(Character c) => GameSettings.IncubatorUpgradeCostPerLevel * c.IncubatorLevel;
    public static int XpToNext(OwnedPet p) => GameSettings.PetXpPerLevel * p.Level;

    public static GameResult Incubate(Character c, string eggItemId, DateTime nowUtc)
    {
        if (!PetCatalog.Eggs.TryGetValue(eggItemId, out var egg)) return GameResult.Fail("That is not an egg.");
        var row = c.Items.FirstOrDefault(i => i.ItemId == eggItemId);
        if (row is null || row.Quantity <= 0) return GameResult.Fail("You do not have this egg.");
        if (c.IncubatingEggs.Count >= IncubatorSlots(c))
            return GameResult.Fail("The incubator is full. Wait for an egg to hatch or upgrade the incubator.");

        if (row.Quantity <= 1) c.Items.Remove(row); else row.Quantity--;
        c.IncubatingEggs.Add(new IncubatingEgg
        {
            UserId = c.UserId, EggItemId = eggItemId, StartedAt = nowUtc, ReadyAt = nowUtc.AddMinutes(egg.Minutes),
        });
        return GameResult.Success($"Placed {ItemCatalog.Find(eggItemId)?.Name} in the incubator. It hatches in {FormatMinutes(egg.Minutes)}.");
    }

    public static GameResult Hatch(Character c, Guid eggId, DateTime nowUtc, Random rng)
    {
        var egg = c.IncubatingEggs.FirstOrDefault(e => e.Id == eggId);
        if (egg is null) return GameResult.Fail("Egg not found.");
        if (egg.ReadyAt > nowUtc) return GameResult.Fail("The egg is not ready to hatch yet.");
        if (c.Pets.Count >= CoopCapacity(c)) return GameResult.Fail("The coop is full! Upgrade the coop to welcome more pets.");

        var tier = PetCatalog.Eggs[egg.EggItemId].Rarity;
        if (tier != Rarity.Legendary && rng.NextDouble() < GameSettings.HatchUpgradeChance) tier++;
        var pool = PetCatalog.Species.Where(s => s.Rarity == tier).ToList();
        if (pool.Count == 0) pool = PetCatalog.Species.ToList();
        var species = pool[rng.Next(pool.Count)];

        c.IncubatingEggs.Remove(egg);
        c.Pets.Add(new OwnedPet { UserId = c.UserId, SpeciesId = species.Id, Name = species.Name, AdoptedAt = nowUtc });
        return GameResult.Success($"🎉 The egg hatched! You got {species.Emoji} {species.Name} ({species.Rarity.Label()})");
    }

    public static GameResult Feed(Character c, Guid petId)
    {
        var pet = c.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet is null) return GameResult.Fail("Pet not found.");
        if (pet.Level >= MaxPetLevel) return GameResult.Fail($"{pet.Name} has reached the maximum level.");
        if (c.Gold < FeedCost) return GameResult.Fail($"You need {FeedCost} 🪙 to buy food.");

        c.Gold -= FeedCost;
        GameLog.Ledger(c, LedgerKinds.PetFeed, gold: -FeedCost, note: pet.Name);
        pet.Xp += FeedXp;
        var leveled = false;
        while (pet.Level < MaxPetLevel && pet.Xp >= XpToNext(pet))
        {
            pet.Xp -= XpToNext(pet);
            pet.Level++;
            leveled = true;
        }
        if (pet.Level >= MaxPetLevel) pet.Xp = 0;
        return GameResult.Success(leveled ? $"🎊 {pet.Name} reached level {pet.Level}!" : $"{pet.Name} enjoyed the meal (+{FeedXp} XP)");
    }

    public static GameResult SetCompanion(Character c, Guid petId)
    {
        var pet = c.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet is null) return GameResult.Fail("Pet not found.");
        c.CompanionPetId = pet.Id;
        return GameResult.Success($"{pet.Name} is now your companion");
    }

    public static GameResult UpgradeCoop(Character c)
    {
        if (c.CoopLevel >= MaxCoopLevel) return GameResult.Fail("The coop is already at the maximum level.");
        var cost = CoopUpgradeCost(c);
        if (c.Gold < cost) return GameResult.Fail($"You need {cost} 🪙 to upgrade the coop.");
        c.Gold -= cost;
        c.CoopLevel++;
        GameLog.Ledger(c, LedgerKinds.Upgrade, gold: -cost, note: $"Pet coop level {c.CoopLevel}");
        return GameResult.Success($"🏠 Coop upgraded to level {c.CoopLevel}, capacity {CoopCapacity(c)} pets");
    }

    public static GameResult UpgradeIncubator(Character c)
    {
        if (c.IncubatorLevel >= MaxIncubatorLevel) return GameResult.Fail("The incubator is already at the maximum level.");
        var cost = IncubatorUpgradeCost(c);
        if (c.Gold < cost) return GameResult.Fail($"You need {cost} 🪙 to upgrade the incubator.");
        c.Gold -= cost;
        c.IncubatorLevel++;
        GameLog.Ledger(c, LedgerKinds.Upgrade, gold: -cost, note: $"Incubator level {c.IncubatorLevel}");
        return GameResult.Success($"🔥 Incubator upgraded to level {c.IncubatorLevel}, it can now hold {IncubatorSlots(c)} eggs at once");
    }

    private static string FormatMinutes(int m) => m >= 60 ? $"{m / 60} h" : $"{m} min";
}
