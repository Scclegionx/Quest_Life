using Microsoft.AspNetCore.Mvc;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.Controllers;

public abstract class GameControllerBase : Controller
{
    protected Guid UserId => User.GetUserId()!.Value;
    protected Guid? UserIdOrNull => User.GetUserId();
    protected UserClock Clock => User.GetClock();

    /// <summary>Flash the result as a toast and redirect.</summary>
    protected IActionResult Done(GameResult result, string action, object? routeValues = null)
    {
        TempData[result.Ok ? "Toast" : "ToastError"] = result.Message;
        return RedirectToAction(action, routeValues);
    }

    protected IActionResult Done(GameResult result, string action, string controller)
    {
        TempData[result.Ok ? "Toast" : "ToastError"] = result.Message;
        return RedirectToAction(action, controller);
    }
}
