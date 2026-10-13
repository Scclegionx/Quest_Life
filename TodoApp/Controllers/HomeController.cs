using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

public class HomeController(LeaderboardService leaderboards) : GameControllerBase
{
    public async Task<IActionResult> Index(string period = "week")
    {
        period = period == "all" ? "all" : "week";
        return View(new HomeViewModel
        {
            Period = period,
            Boards = await leaderboards.BuildBoardsAsync(LeaderboardService.SinceFor(period), UserIdOrNull),
        });
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
