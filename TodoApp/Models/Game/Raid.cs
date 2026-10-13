using TodoApp.Data;

namespace TodoApp.Models;

public record BossRewardDef(RewardKind Kind, int Gold, string? ItemId, int Quantity, string? PetSpeciesId, string? TitleId);

public record BossDef(int Id, string Name, string Emoji, string Title, long MaxHp, string Lore, IReadOnlyList<BossRewardDef> Rewards);

public static class BossCatalog
{
    public static IReadOnlyList<BossDef> All => GameCatalog.Current.Bosses;
}
