using TodoApp.Data;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Services;

/// <summary>Evaluates the badges defined in the badges table against a player's statistics.</summary>
public static class BadgeService
{
    public static List<BadgeViewModel> Evaluate(AchievementContext ctx) =>
        GameCatalog.Current.Badges
            .Select(b => new BadgeViewModel(b.Icon, b.Name, b.Description, b.Progress(ctx) >= b.Target))
            .ToList();
}
