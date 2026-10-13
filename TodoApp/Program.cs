using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
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
builder.Services.AddScoped<RaidService>();
builder.Services.AddScoped<LeaderboardService>();
builder.Services.AddScoped<ActivityStatsService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AchievementService>();
builder.Services.AddHostedService<PenaltySweeper>();

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
