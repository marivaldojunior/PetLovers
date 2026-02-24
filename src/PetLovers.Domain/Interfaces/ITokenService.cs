using PetLovers.Domain.Entities;

namespace PetLovers.Domain.Interfaces;

/// <summary>
/// Service responsible for JWT and Refresh Token generation and validation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates an access token (JWT) for the specified user.
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a cryptographically secure refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the configured refresh token expiry time.
    /// </summary>
    DateTime GetRefreshTokenExpiryTime();

    /// <summary>
    /// Extracts the user ID from an expired access token.
    /// Used during refresh token flow.
    /// </summary>
    Guid? GetUserIdFromExpiredToken(string accessToken);
}
