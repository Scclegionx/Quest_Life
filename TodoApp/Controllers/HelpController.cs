using Microsoft.AspNetCore.Mvc;

namespace TodoApp.Controllers;

/// <summary>The player guide. Public: anyone can read it without logging in.</summary>
public class HelpController : GameControllerBase
{
    public IActionResult Index() => View();
}
