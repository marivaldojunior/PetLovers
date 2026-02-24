namespace PetLovers.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// Rich Domain Model with encapsulated state and behavior.
/// </summary>
public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    private readonly List<string> _roles = [];
    public IReadOnlyCollection<string> Roles => _roles.AsReadOnly();

    // EF Core constructor
    private User()
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    /// <summary>
    /// Creates a new User with hashed password.
    /// </summary>
    public User(
        string email,
        string passwordHash,
        string firstName,
        string lastName)
    {
        ValidateEmail(email);
        ValidateName(firstName, nameof(firstName));
        ValidateName(lastName, nameof(lastName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        Id = Guid.NewGuid();
        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        _roles.Add("User");
    }

    public string FullName => $"{FirstName} {LastName}";

    public void UpdateRefreshToken(string? refreshToken, DateTime? expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void AddRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        if (!_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            _roles.Add(role);
    }

    public void RemoveRole(string role)
    {
        _roles.RemoveAll(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public void Deactivate()
    {
        IsActive = false;
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public bool HasValidRefreshToken(string refreshToken)
    {
        return IsActive &&
               RefreshToken == refreshToken &&
               RefreshTokenExpiryTime.HasValue &&
               RefreshTokenExpiryTime.Value > DateTime.UtcNow;
    }

    private static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.", nameof(email));

        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Email format is invalid.", nameof(email));

        if (email.Length > 256)
            throw new ArgumentException("Email cannot exceed 256 characters.", nameof(email));
    }

    private static void ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);

        if (name.Length > 100)
            throw new ArgumentException($"{paramName} cannot exceed 100 characters.", paramName);
    }
}
