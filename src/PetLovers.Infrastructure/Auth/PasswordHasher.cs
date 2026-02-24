using System.Security.Cryptography;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Infrastructure.Auth;

/// <summary>
/// Secure password hasher using PBKDF2 with SHA-512.
/// Implements industry-standard key derivation with high iteration count.
/// </summary>
public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int HashSize = 32; // 256 bits
    private const int Iterations = 100_000; // OWASP recommended minimum
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    /// <summary>
    /// Generates a secure hash using PBKDF2-SHA512.
    /// Format: {iterations}.{salt}.{hash} (all base64 encoded)
    /// </summary>
    public string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithm,
            HashSize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Verifies password using constant-time comparison to prevent timing attacks.
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        var parts = passwordHash.Split('.');
        if (parts.Length != 3)
            return false;

        if (!int.TryParse(parts[0], out var iterations))
            return false;

        var salt = Convert.FromBase64String(parts[1]);
        var storedHash = Convert.FromBase64String(parts[2]);

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithm,
            storedHash.Length);

        // Constant-time comparison prevents timing attacks
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
