using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Commands.Auth.LoginUser;

public sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<CommandResult<AuthResponseDto>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        // Generic error message to prevent user enumeration attacks
        const string invalidCredentialsMessage = "Invalid email or password.";

        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return CommandResult<AuthResponseDto>.Failure(invalidCredentialsMessage);
        }

        if (!user.IsActive)
        {
            return CommandResult<AuthResponseDto>.Failure("This account has been deactivated.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return CommandResult<AuthResponseDto>.Failure(invalidCredentialsMessage);
        }

        user.RecordLogin();

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = _tokenService.GetRefreshTokenExpiryTime();
        user.UpdateRefreshToken(refreshToken, refreshTokenExpiry);

        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
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
