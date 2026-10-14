using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace TodoApp.Services.Discord;

/// <summary>Uploads the slash command definitions to Discord (guild commands appear instantly, global ones can take a while).</summary>
public class DiscordCommandRegistrar(IOptions<DiscordOptions> options, IHttpClientFactory http, ILogger<DiscordCommandRegistrar> logger)
{
    public async Task<bool> RegisterAsync(CancellationToken ct = default)
    {
        var o = options.Value;
        if (string.IsNullOrWhiteSpace(o.ApplicationId) || string.IsNullOrWhiteSpace(o.BotToken))
        {
            logger.LogWarning("Discord: ApplicationId and BotToken are required to register commands. Skipped.");
            return false;
        }

        var commands = DiscordCommands.Build();
        var problems = DiscordCommands.Validate(commands);
        if (problems.Count > 0)
        {
            logger.LogError("Discord: command definitions are invalid: {Problems}", string.Join("; ", problems));
            return false;
        }

        var path = string.IsNullOrWhiteSpace(o.GuildId)
            ? $"{o.ApiBase.TrimEnd('/')}/applications/{o.ApplicationId}/commands"
            : $"{o.ApiBase.TrimEnd('/')}/applications/{o.ApplicationId}/guilds/{o.GuildId}/commands";

        using var request = new HttpRequestMessage(HttpMethod.Put, path)
        {
            Content = new StringContent(JsonSerializer.Serialize(commands), Encoding.UTF8, "application/json"),
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", o.BotToken);

        try
        {
            using var response = await http.CreateClient().SendAsync(request, ct);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Discord: registered {Count} slash commands ({Scope}).", commands.Count,
                    string.IsNullOrWhiteSpace(o.GuildId) ? "global" : $"guild {o.GuildId}");
                return true;
            }
            logger.LogError("Discord: registering commands failed with {Status}: {Body}", (int)response.StatusCode,
                await response.Content.ReadAsStringAsync(ct));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogError(ex, "Discord: could not reach the Discord API to register commands.");
        }
        return false;
    }
}
