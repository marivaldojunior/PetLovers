namespace PetLovers.Application.DTOs.Auth;

public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    IReadOnlyCollection<string> Roles);
