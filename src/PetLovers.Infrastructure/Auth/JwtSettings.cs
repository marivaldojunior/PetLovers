namespace PetLovers.Infrastructure.Auth;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
}
