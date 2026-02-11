using System.Security.Claims;
using GlowNow.Identity.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using GlowNow.Identity.Application.Commands.Login;
using GlowNow.Identity.Application.Commands.Logout;
using GlowNow.Identity.Application.Commands.RefreshToken;
using GlowNow.Identity.Application.Commands.RegisterBusiness;
using GlowNow.Identity.Application.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlowNow.Identity.Api.Controllers;

/// <summary>
/// API controller for authentication and user management.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Register a new business.
    /// </summary>
    /// <remarks>
    /// Creates a new business account with an owner user. The owner will be authenticated via AWS Cognito and a local user record will be created.
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RegisterBusinessResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterBusinessCommand command)
    {
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Authenticate user.
    /// </summary>
    /// <remarks>
    /// Authenticates a user with email and password. Returns JWT access token, refresh token, and user details including business memberships.
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Refresh access token.
    /// </summary>
    /// <remarks>
    /// Exchanges a valid refresh token for a new access token and refresh token pair.
    /// </remarks>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _sender.Send(command);
        return result.ToActionResult();
    }

    /// <summary>
    /// Sign out user.
    /// </summary>
    /// <remarks>
    /// Performs a global sign-out, invalidating all tokens for the authenticated user across all devices.
    /// </remarks>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        string? cognitoUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(cognitoUserId)) return Unauthorized();

        var result = await _sender.Send(new LogoutCommand(cognitoUserId));
        return result.ToActionResult();
    }

    /// <summary>
    /// Get current user.
    /// </summary>
    /// <remarks>
    /// Returns the authenticated user's profile including email, name, phone number, and all business memberships with roles.
    /// </remarks>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<CurrentUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        string? userIdStr = User.FindFirstValue("user_id");
        if (!Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

        var result = await _sender.Send(new GetCurrentUserQuery(userId));
        return result.ToActionResult();
    }
}
