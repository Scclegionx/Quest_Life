using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TodoApp.Services.Discord;

namespace TodoApp.Controllers;

/// <summary>
/// The address Discord calls for every slash command, button press and autocomplete request
/// (set it as "Interactions Endpoint URL" in the Discord developer portal).
/// </summary>
public class DiscordController(
    IOptions<DiscordOptions> options, DiscordSignatureVerifier verifier, DiscordInteractionHandler handler,
    ILogger<DiscordController> logger) : Controller
{
    [HttpPost("/discord/interactions"), AllowAnonymous]
    public async Task<IActionResult> Interactions()
    {
        var o = options.Value;
        if (!o.IsConfigured) return NotFound();

        // the signature covers the raw bytes, so read them exactly as sent
        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer);
        var body = buffer.ToArray();

        if (!verifier.Verify(o.PublicKey, Request.Headers["X-Signature-Ed25519"].ToString(), Request.Headers["X-Signature-Timestamp"].ToString(), body))
        {
            logger.LogWarning("Discord request rejected: invalid signature.");
            return Unauthorized("invalid request signature");
        }

        JsonDocument doc;
        try { doc = JsonDocument.Parse(body); }
        catch (JsonException) { return BadRequest(); }

        using (doc)
        {
            var response = await handler.HandleAsync(doc.RootElement);
            return Content(JsonSerializer.Serialize(response, DiscordJson.Options), "application/json");
        }
    }
}
