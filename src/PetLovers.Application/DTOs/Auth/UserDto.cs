namespace PetLovers.Application.DTOs.Auth;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Email"></param>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="FullName"></param>
/// <param name="Roles"></param>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    IReadOnlyCollection<string> Roles);
