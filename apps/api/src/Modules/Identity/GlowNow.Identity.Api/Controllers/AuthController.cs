using System.Security.Claims;
using GlowNow.Identity.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
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
    /// Creates a new business account with an owner user. The user must already be authenticated via Cognito.
    /// </remarks>
    [HttpPost("register-business")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<RegisterBusinessResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterBusiness([FromBody] RegisterBusinessCommand command)
    {
        var result = await _sender.Send(command);
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
