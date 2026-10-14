using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;
using TodoApp.Services.Discord;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Login cookies and anti-forgery tokens are protected with keys. In Docker the default key location is lost whenever the
// container is recreated (everybody would be logged out after each deploy), so production keeps them on a mounted volume.
var dataProtectionPath = builder.Configuration["DataProtection:KeysPath"];
if (!string.IsNullOrWhiteSpace(dataProtectionPath))
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath))
        .SetApplicationName("QuestLife");
AppTime.Offset = TimeSpan.FromHours(builder.Configuration.GetValue("GameTimeZoneOffsetHours", 7.0));

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing connection string 'ConnectionStrings:Default'.");

using var bootstrapLoggers = LoggerFactory.Create(b => b.AddSimpleConsole());
var dataMode = await DataSourceResolver.ResolveAsync(
    builder.Configuration, builder.Environment, connectionString, bootstrapLoggers.CreateLogger("DataSource"));
builder.Services.AddSingleton(typeof(DataSourceMode), dataMode);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dataMode == DataSourceMode.Database)
        options.UseNpgsql(connectionString);
    else
        options.UseInMemoryDatabase("questlife-mock")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TaskBoardService>();
builder.Services.AddScoped<RaidService>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<ActivityStatsService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AchievementService>();
builder.Services.AddHostedService<PenaltySweeper>();

// Discord slash-command integration (see docs/discord-setup.md)
builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection("Discord"));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DiscordSignatureVerifier>();
builder.Services.AddSingleton<DiscordCommandRegistrar>();
builder.Services.AddScoped<DiscordLinkService>();
builder.Services.AddScoped<DiscordInteractionHandler>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/AccessDenied";
        o.ExpireTimeSpan = TimeSpan.FromDays(14);
        o.SlidingExpiration = true;
        o.Cookie.HttpOnly = true;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (dataMode == DataSourceMode.Database) await db.Database.MigrateAsync();   // creates/updates the tables from the migrations
    else await db.Database.EnsureCreatedAsync();

    await GameCatalog.ReloadAsync(db);

    var discordOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<DiscordOptions>>().Value;
    if (discordOptions.IsConfigured && discordOptions.RegisterCommandsOnStartup)
    {
        using var registerTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        await app.Services.GetRequiredService<DiscordCommandRegistrar>().RegisterAsync(registerTimeout.Token);
    }

    if (dataMode == DataSourceMode.Mock || builder.Configuration.GetValue<bool>("DemoData:Enabled"))
        await DemoDataSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>(),
            scope.ServiceProvider.GetRequiredService<RaidService>());
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en-US").AddSupportedCultures("en-US").AddSupportedUICultures("en-US"));
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "profile",
    pattern: "u/{username}",
    defaults: new { controller = "Profile", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
