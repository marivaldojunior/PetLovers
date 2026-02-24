using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Commands.Auth.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName) : ICommand<AuthResponseDto>;
