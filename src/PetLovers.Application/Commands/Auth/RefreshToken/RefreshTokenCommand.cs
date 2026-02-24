using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Commands.Auth.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken) : ICommand<AuthTokensDto>;
