using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Entities;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Commands.Auth.RegisterUser;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<CommandResult<AuthResponseDto>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailExists = await _unitOfWork.Users.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            return CommandResult<AuthResponseDto>.Failure("An account with this email already exists.");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User(
            request.Email,
            passwordHash,
            request.FirstName,
            request.LastName);

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = _tokenService.GetRefreshTokenExpiryTime();
        user.UpdateRefreshToken(refreshToken, refreshTokenExpiry);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        var response = new AuthResponseDto(
            new UserDto(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.FullName,
                user.Roles.ToList()),
            new AuthTokensDto(
                accessToken,
                refreshToken,
                refreshTokenExpiry));

        return CommandResult<AuthResponseDto>.Success(response);
    }
}
