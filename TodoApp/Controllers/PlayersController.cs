using Microsoft.AspNetCore.Mvc;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

public class PlayersController(ProfileService profiles) : GameControllerBase
{
    public async Task<IActionResult> Index(string? q) =>
        View(new PlayersViewModel { Query = q?.Trim() ?? "", Results = await profiles.SearchAsync(q) });
}
