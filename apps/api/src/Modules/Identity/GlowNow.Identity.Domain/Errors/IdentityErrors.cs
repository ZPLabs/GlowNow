using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Identity.Domain.Errors;

public static class IdentityErrors
{
    public static readonly Error EmailAlreadyInUse = Error.Conflict(
        "Identity.EmailAlreadyInUse",
        "The provided email is already in use.");

    public static readonly Error InvalidCredentials = Error.Problem(
        "Identity.InvalidCredentials",
        "The provided credentials are invalid.");

    public static readonly Error UserNotFound = Error.NotFound(
        "Identity.UserNotFound",
        "The user with the specified identifier was not found.");

    public static readonly Error CognitoError = Error.Problem(
        "Identity.CognitoError",
        "An error occurred with the identity provider.");

    public static readonly Error InvalidRefreshToken = Error.Problem(
        "Identity.InvalidRefreshToken",
        "The provided refresh token is invalid or expired.");
}
