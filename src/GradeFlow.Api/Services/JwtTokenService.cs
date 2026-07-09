using System.Security.Cryptography;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using GradeFlow.Application.DTOs.Auth;
using GradeFlow.Application.Services;
using GradeFlow.Domain.Entities;

namespace GradeFlow.Api.Services;

public sealed class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public AuthResponse Create(User user)
    {
        var expiresAt = DateTime.UtcNow.AddHours(8);
        var token = JwtToken.Write(
            new Dictionary<string, object?>
            {
                ["iss"] = configuration["Jwt:Issuer"],
                ["aud"] = configuration["Jwt:Audience"],
                ["sub"] = user.Id.ToString(),
                [ClaimTypes.NameIdentifier] = user.Id.ToString(),
                [ClaimTypes.Name] = user.Name,
                [ClaimTypes.Email] = user.Email,
                [ClaimTypes.Role] = user.Role.ToString(),
                ["exp"] = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
            },
            configuration["Jwt:Key"]!);

        return new AuthResponse(
            token,
            expiresAt,
            new UserResponse(user.Id, user.Name, user.Email, user.Role));
    }
}

public static class JwtToken
{
    public static string Write(Dictionary<string, object?> payload, string key)
    {
        var header = Base64Url(JsonSerializer.SerializeToUtf8Bytes(new { alg = "HS256", typ = "JWT" }));
        var body = Base64Url(JsonSerializer.SerializeToUtf8Bytes(payload));
        var unsigned = $"{header}.{body}";
        return $"{unsigned}.{Sign(unsigned, key)}";
    }

    public static Dictionary<string, JsonElement>? ReadValid(string token, string key)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
            return null;

        var unsigned = $"{parts[0]}.{parts[1]}";
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(Sign(unsigned, key)),
                Encoding.UTF8.GetBytes(parts[2])))
            return null;

        return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Base64UrlDecode(parts[1]));
    }

    private static string Sign(string value, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        return Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private static string Base64Url(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }
}
