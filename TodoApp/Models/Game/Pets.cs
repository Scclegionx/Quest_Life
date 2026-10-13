using TodoApp.Data;

namespace TodoApp.Models;

public record PetSpecies(string Id, string Name, string Emoji, Rarity Rarity);

public static class PetCatalog
{
    public static IReadOnlyList<PetSpecies> Species => GameCatalog.Current.Species;
    public static PetSpecies Get(string id) => GameCatalog.Current.SpeciesById[id];

    /// <summary>Hatch time and base rarity per egg item.</summary>
    public static IReadOnlyDictionary<string, (Rarity Rarity, int Minutes)> Eggs => GameCatalog.Current.Eggs;
}
