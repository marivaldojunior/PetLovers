namespace PetLovers.Application.DTOs.Auth;

public sealed record AuthResponseDto(
    UserDto User,
    AuthTokensDto Tokens);
