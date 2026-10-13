using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.ViewModels;

namespace TodoApp.Controllers;

public class AccountController(AppDbContext db, IPasswordHasher<User> hasher) : Controller
{
    [HttpGet, AllowAnonymous]
    public IActionResult Register() =>
        User.Identity?.IsAuthenticated == true ? RedirectToAction("Index", "Dashboard") : View(new RegisterViewModel());

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!UserClock.IsValidZone(model.TimeZone)) model.TimeZone = UserClock.DefaultZoneId;
        if (!ModelState.IsValid) return View(model);

        var username = model.Username.Trim().ToLowerInvariant();
        var email = model.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Username == username))
            ModelState.AddModelError(nameof(model.Username), "This username is already taken");
        if (await db.Users.AnyAsync(u => u.Email == email))
            ModelState.AddModelError(nameof(model.Email), "This email is already in use");
        if (!ModelState.IsValid) return View(model);

        var user = new User { Username = username, Email = email, TimeZone = model.TimeZone };
        user.PasswordHash = hasher.HashPassword(user, model.Password);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        await SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard");
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var key = model.Login.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == key || u.Email == key);
        var ok = user is not null &&
                 hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) != PasswordVerificationResult.Failed;
        if (!ok)
        {
            ModelState.AddModelError(string.Empty, "Incorrect username or password");
            return View(model);
        }

        user!.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await SignInAsync(user, model.RememberMe);
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Index", "Dashboard");
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet, Authorize]
    public async Task<IActionResult> Settings()
    {
        var zone = await db.Users.AsNoTracking().Where(u => u.Id == User.GetUserId()).Select(u => u.TimeZone).FirstOrDefaultAsync();
        return View(new SettingsViewModel { TimeZone = zone ?? UserClock.DefaultZoneId });
    }

    [HttpPost, Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Settings(SettingsViewModel model)
    {
        if (!UserClock.IsValidZone(model.TimeZone)) ModelState.AddModelError(nameof(model.TimeZone), "Invalid time zone");
        if (!ModelState.IsValid) return View(model);

        var userId = User.GetUserId()!.Value;
        var user = await db.Users.FirstAsync(u => u.Id == userId);
        user.TimeZone = model.TimeZone;
        await db.SaveChangesAsync();

        await SignInAsync(user, isPersistent: true); // refresh the time zone claim
        TempData["Toast"] = "Time zone updated. Your new day now starts according to this time zone.";
        return RedirectToAction(nameof(Settings));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => RedirectToAction("Login");

    private Task SignInAsync(User user, bool isPersistent)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimsExtensions.TimeZoneClaim, user.TimeZone),
        ], CookieAuthenticationDefaults.AuthenticationScheme);

        return HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = isPersistent });
    }
}
