using Org.BouncyCastle.Math.EC.Rfc8032;

namespace TodoApp.Services.Discord;

/// <summary>
/// Discord signs every request (Ed25519 over timestamp + body). Anything that fails this check is rejected, which is what
/// guarantees that the Discord user id inside a command is real.
/// </summary>
public class DiscordSignatureVerifier
{
    private static readonly TimeSpan MaxClockSkew = TimeSpan.FromMinutes(5);

    public bool Verify(string publicKeyHex, string signatureHex, string timestamp, byte[] body, DateTimeOffset? now = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(signatureHex) || string.IsNullOrWhiteSpace(timestamp)) return false;
            var publicKey = Convert.FromHexString(publicKeyHex.Trim());
            var signature = Convert.FromHexString(signatureHex.Trim());
            if (publicKey.Length != 32 || signature.Length != 64) return false;

            // reject replays of old requests
            if (!long.TryParse(timestamp, out var seconds)) return false;
            var sent = DateTimeOffset.FromUnixTimeSeconds(seconds);
            if ((now ?? DateTimeOffset.UtcNow) - sent > MaxClockSkew || sent - (now ?? DateTimeOffset.UtcNow) > MaxClockSkew) return false;

            var message = new byte[timestamp.Length + body.Length];
            System.Text.Encoding.UTF8.GetBytes(timestamp, 0, timestamp.Length, message, 0);
            Buffer.BlockCopy(body, 0, message, timestamp.Length, body.Length);

            return Ed25519.Verify(signature, 0, publicKey, 0, message, 0, message.Length);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            return false;
        }
    }
}
