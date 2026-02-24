using PetLovers.Application.DTOs.Auth;
using PetLovers.Application.Interfaces;
using PetLovers.Domain.Interfaces;

namespace PetLovers.Application.Commands.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<CommandResult<AuthTokensDto>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        const string invalidTokenMessage = "Invalid or expired refresh token.";

        // Extract user ID from the expired access token
        var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
        if (!userId.HasValue)
        {
            return CommandResult<AuthTokensDto>.Failure(invalidTokenMessage);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return CommandResult<AuthTokensDto>.Failure(invalidTokenMessage);
        }

        // Validate the stored refresh token matches and is not expired
        if (!user.HasValidRefreshToken(request.RefreshToken))
        {
            return CommandResult<AuthTokensDto>.Failure(invalidTokenMessage);
        }

        // Rotate refresh token for enhanced security
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenExpiry = _tokenService.GetRefreshTokenExpiryTime();

        user.UpdateRefreshToken(newRefreshToken, newRefreshTokenExpiry);
        await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CommandResult<AuthTokensDto>.Success(
            new AuthTokensDto(newAccessToken, newRefreshToken, newRefreshTokenExpiry));
    }
}
