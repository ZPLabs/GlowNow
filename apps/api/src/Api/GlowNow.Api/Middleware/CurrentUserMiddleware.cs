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

    public async Task InvokeAsync(
        HttpContext context, 
        IUserRepository userRepository, 
        ICurrentUserProvider currentUserProvider,
        IIdentityUnitOfWork identityUnitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<CurrentUserMiddleware> logger)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string? cognitoUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(cognitoUserId))
            {
                UserEntity? user = await userRepository.GetByCognitoUserIdAsync(cognitoUserId);

                if (user is null)
                {
                    // Lazy creation of local user from JWT claims
                    string? email = context.User.FindFirstValue(ClaimTypes.Email) ?? context.User.FindFirstValue("email");
                    string? firstName = context.User.FindFirstValue(ClaimTypes.GivenName) ?? context.User.FindFirstValue("given_name");
                    string? lastName = context.User.FindFirstValue(ClaimTypes.Surname) ?? context.User.FindFirstValue("family_name");

                    // Google via Cognito federation may only include `name` (full name) rather than
                    // split given_name/family_name if those attributes aren't enabled on the User Pool.
                    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    {
                        string? fullName = context.User.FindFirstValue("name");
                        if (!string.IsNullOrEmpty(fullName))
                        {
                            string[] parts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                            firstName ??= parts[0];
                            lastName ??= parts.Length > 1 ? parts[1] : parts[0];
                        }
                    }

                    // Last resort: derive from email so we never block a valid authenticated user
                    if (string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(email))
                        firstName = email.Split('@')[0];
                    if (string.IsNullOrEmpty(lastName))
                        lastName = "-";

                    if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName))
                    {
                        try
                        {
                            var emailResult = GlowNow.SharedKernel.Domain.ValueObjects.Email.Create(email);
                            if (emailResult.IsSuccess)
                            {
                                user = UserEntity.Create(
                                    emailResult.Value,
                                    firstName,
                                    lastName,
                                    null,
                                    cognitoUserId,
                                    dateTimeProvider.UtcNow);

                                userRepository.Add(user);
                                await identityUnitOfWork.SaveChangesAsync();
                                logger.LogInformation("Lazy-created local user for Cognito ID {CognitoId}", cognitoUserId);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to lazy-create local user for Cognito ID {CognitoId}", cognitoUserId);
                        }
                    }
                }

                if (user is not null)
                {
                    currentUserProvider.Set(user.Id, cognitoUserId);

                    // Add local user id to claims for easier access in endpoints if needed
                    var identity = (ClaimsIdentity)context.User.Identity;
                    if (!identity.HasClaim(c => c.Type == "user_id"))
                    {
                        identity.AddClaim(new Claim("user_id", user.Id.ToString()));
                    }
                }
            }
        }

        await _next(context);
    }
}
