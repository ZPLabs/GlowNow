using System.Security.Claims;
using GlowNow.Api.Infrastructure;
using GlowNow.Identity.Application.Commands.Login;
using GlowNow.Identity.Application.Commands.Logout;
using GlowNow.Identity.Application.Commands.RefreshToken;
using GlowNow.Identity.Application.Commands.RegisterBusiness;
using GlowNow.Identity.Application.Queries.GetCurrentUser;
using MediatR;

namespace GlowNow.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapPost("/register", async (RegisterBusinessCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        }).AllowAnonymous();

        group.MapPost("/login", async (LoginCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        }).AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.ToApiResponse();
        }).AllowAnonymous();

        group.MapPost("/logout", async (ISender sender, ClaimsPrincipal user) =>
        {
            string? cognitoUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(cognitoUserId)) return Results.Unauthorized();

            var result = await sender.Send(new LogoutCommand(cognitoUserId));
            return result.ToApiResponse();
        }).RequireAuthorization();

        group.MapGet("/me", async (ISender sender, ClaimsPrincipal user) =>
        {
            string? userIdStr = user.FindFirstValue("user_id");
            if (!Guid.TryParse(userIdStr, out Guid userId)) return Results.Unauthorized();

            var result = await sender.Send(new GetCurrentUserQuery(userId));
            return result.ToApiResponse();
        }).RequireAuthorization();
    }
}
