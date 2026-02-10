using System.Security.Claims;
using GlowNow.Api.Infrastructure;
using GlowNow.Identity.Application.Commands.Login;
using GlowNow.Identity.Application.Commands.Logout;
using GlowNow.Identity.Application.Commands.RefreshToken;
using GlowNow.Identity.Application.Commands.RegisterBusiness;
using GlowNow.Identity.Application.Queries.GetCurrentUser;
using MediatR;

namespace GlowNow.Api.Endpoints;

/// <summary>
/// API endpoints for authentication and user management.
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", async (RegisterBusinessCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("RegisterBusiness")
        .WithSummary("Register a new business")
        .WithDescription("Creates a new business account with an owner user. The owner will be authenticated via AWS Cognito and a local user record will be created.")
        .Produces<ApiResponse<RegisterBusinessResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .AllowAnonymous();

        group.MapPost("/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("Login")
        .WithSummary("Authenticate user")
        .WithDescription("Authenticates a user with email and password. Returns JWT access token, refresh token, and user details including business memberships.")
        .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh access token")
        .WithDescription("Exchanges a valid refresh token for a new access token and refresh token pair.")
        .Produces<ApiResponse<RefreshTokenResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .AllowAnonymous();

        group.MapPost("/logout", async (ISender sender, ClaimsPrincipal user) =>
        {
            string? cognitoUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(cognitoUserId)) return Results.Unauthorized();

            var result = await sender.Send(new LogoutCommand(cognitoUserId));
            return result.ToApiResponse();
        })
        .WithName("Logout")
        .WithSummary("Sign out user")
        .WithDescription("Performs a global sign-out, invalidating all tokens for the authenticated user across all devices.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        group.MapGet("/me", async (ISender sender, ClaimsPrincipal user) =>
        {
            string? userIdStr = user.FindFirstValue("user_id");
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Results.Unauthorized();

            var result = await sender.Send(new GetCurrentUserQuery(userId));
            return result.ToApiResponse();
        })
        .WithName("GetCurrentUser")
        .WithSummary("Get current user")
        .WithDescription("Returns the authenticated user's profile including email, name, phone number, and all business memberships with roles.")
        .Produces<ApiResponse<CurrentUserResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
