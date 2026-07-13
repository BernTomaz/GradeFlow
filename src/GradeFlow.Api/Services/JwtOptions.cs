using System.ComponentModel.DataAnnotations;

namespace GradeFlow.Api.Services;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";
    public const int MinimumKeyLength = 32;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    [MinLength(MinimumKeyLength)]
    public string Key { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int ExpirationMinutes { get; init; } = 480;
}
