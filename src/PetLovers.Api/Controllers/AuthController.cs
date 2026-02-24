using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetLovers.Application.Commands.Auth.LoginUser;
using PetLovers.Application.Commands.Auth.RefreshToken;
using PetLovers.Application.Commands.Auth.RegisterUser;
using PetLovers.Application.DTOs.Auth;

namespace PetLovers.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        var command = new RegisterUserCommand(
            request.Email,
            request.Password,
            request.ConfirmPassword,
            request.FirstName,
            request.LastName);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error?.Contains("already exists") == true)
            {
                return Conflict(new { error = result.Error });
            }

            return BadRequest(new { error = result.Error, validationErrors = result.ValidationErrors });
        }

        _logger.LogInformation("User registered successfully: {UserId}", result.Data!.User.Id);
        return CreatedAtAction(nameof(Register), result.Data);
    }

    /// <summary>
    /// Authenticates a user and returns access/refresh tokens.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);
            return Unauthorized(new { error = result.Error });
        }

        _logger.LogInformation("User logged in successfully: {UserId}", result.Data!.User.Id);
        return Ok(result.Data);
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokensDto>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token refresh attempt");

        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed token refresh attempt");
            return Unauthorized(new { error = result.Error });
        }

        _logger.LogInformation("Token refreshed successfully");
        return Ok(result.Data);
    }
}

// Request DTOs for API contracts
public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName);

public sealed record LoginUserRequest(
    string Email,
    string Password);

public sealed record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);
