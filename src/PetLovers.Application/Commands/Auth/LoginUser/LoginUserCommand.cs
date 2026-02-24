using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Commands.Auth.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : ICommand<AuthResponseDto>;
