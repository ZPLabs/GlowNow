using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Identity.Application.Interfaces;

public interface ICognitoIdentityProvider
{
    Task<Result<string>> RegisterUserAsync(string email, string password, IDictionary<string, string> attributes);
    Task<Result<AuthTokens>> LoginAsync(string email, string password);
    Task<Result<AuthTokens>> RefreshTokenAsync(string refreshToken);
    Task<Result> GlobalSignOutAsync(string cognitoUserId);
    Task DeleteUserAsync(string cognitoUserId);
}
