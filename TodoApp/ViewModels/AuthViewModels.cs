using System.ComponentModel.DataAnnotations;

namespace TodoApp.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter a username")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3 to 50 characters")]
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Letters, numbers and underscores only")]
    [Display(Name = "Username")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(256)]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Please enter a password")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = "";

    [Display(Name = "Time zone")]
    public string TimeZone { get; set; } = TodoApp.Services.UserClock.DefaultZoneId;

    [Required(ErrorMessage = "Please confirm your password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";
}

public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your username or email")]
    [Display(Name = "Username or email")]
    public string Login { get; set; } = "";

    [Required(ErrorMessage = "Please enter a password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

public class SettingsViewModel
{
    [Required]
    [Display(Name = "Time zone")]
    public string TimeZone { get; set; } = TodoApp.Services.UserClock.DefaultZoneId;
}
