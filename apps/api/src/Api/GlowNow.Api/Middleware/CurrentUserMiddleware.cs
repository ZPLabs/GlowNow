using System.Security.Claims;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using UserEntity = GlowNow.Identity.Domain.Entities.User;

namespace GlowNow.Api.Middleware;

public sealed class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository, ICurrentUserProvider currentUserProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string? cognitoUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(cognitoUserId))
            {
                UserEntity? user = await userRepository.GetByCognitoUserIdAsync(cognitoUserId);

                if (user is not null)
                {
                    currentUserProvider.Set(user.Id, cognitoUserId);

                    // Add local user id to claims for easier access in endpoints if needed
                    var identity = (ClaimsIdentity)context.User.Identity;
                    identity.AddClaim(new Claim("user_id", user.Id.ToString()));
                }
            }
        }

        await _next(context);
    }
}
