namespace PetLovers.Domain.Interfaces;

/// <summary>
/// Service responsible for secure password hashing and verification.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Generates a secure hash for the given password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies if the provided password matches the stored hash.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
