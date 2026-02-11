using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Errors;
using GlowNow.SharedKernel.Domain.Errors;
using Microsoft.Extensions.Options;

namespace GlowNow.Identity.Infrastructure.Services;

internal sealed class CognitoIdentityProvider : ICognitoIdentityProvider
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly CognitoSettings _settings;

    public CognitoIdentityProvider(
        IAmazonCognitoIdentityProvider cognitoClient,
        IOptions<CognitoSettings> settings)
    {
        _cognitoClient = cognitoClient;
        _settings = settings.Value;
    }

    public async Task<Result<string>> RegisterUserAsync(
        string email,
        string password,
        IDictionary<string, string> attributes)
    {
        try
        {
            var userAttributes = attributes.Select(a => new AttributeType { Name = a.Key, Value = a.Value }).ToList();

            var createRequest = new AdminCreateUserRequest
            {
                UserPoolId = _settings.UserPoolId,
                Username = email,
                UserAttributes = userAttributes,
                MessageAction = MessageActionType.SUPPRESS // We handle confirmation via password set
            };

            AdminCreateUserResponse response = await _cognitoClient.AdminCreateUserAsync(createRequest);

            var passwordRequest = new AdminSetUserPasswordRequest
            {
                UserPoolId = _settings.UserPoolId,
                Username = email,
                Password = password,
                Permanent = true
            };

            await _cognitoClient.AdminSetUserPasswordAsync(passwordRequest);

            string cognitoUserId = response.User.Attributes.First(a => a.Name == "sub").Value;

            return cognitoUserId;
        }
        catch (UsernameExistsException)
        {
            return Result.Failure<string>(IdentityErrors.EmailAlreadyInUse);
        }
        catch (Exception)
        {
            // Log ex
            return Result.Failure<string>(IdentityErrors.CognitoError);
        }
    }

    public async Task<Result<AuthTokens>> LoginAsync(string email, string password)
    {
        try
        {
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _settings.UserPoolId,
                ClientId = _settings.ClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "USERNAME", email },
                    { "PASSWORD", password }
                }
            };

            AdminInitiateAuthResponse response = await _cognitoClient.AdminInitiateAuthAsync(request);

            return new AuthTokens(
                response.AuthenticationResult.AccessToken,
                response.AuthenticationResult.IdToken,
                response.AuthenticationResult.RefreshToken,
                response.AuthenticationResult.ExpiresIn);
        }
        catch (NotAuthorizedException)
        {
            return Result.Failure<AuthTokens>(IdentityErrors.InvalidCredentials);
        }
        catch (UserNotFoundException)
        {
            return Result.Failure<AuthTokens>(IdentityErrors.InvalidCredentials);
        }
        catch (Exception)
        {
            // Log ex
            return Result.Failure<AuthTokens>(IdentityErrors.CognitoError);
        }
    }

    public async Task<Result<AuthTokens>> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = _settings.UserPoolId,
                ClientId = _settings.ClientId,
                AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                AuthParameters = new Dictionary<string, string>
                {
                    { "REFRESH_TOKEN", refreshToken }
                }
            };

            AdminInitiateAuthResponse response = await _cognitoClient.AdminInitiateAuthAsync(request);

            return new AuthTokens(
                response.AuthenticationResult.AccessToken,
                response.AuthenticationResult.IdToken,
                refreshToken, // Refresh token is not always rotated in Cognito
                response.AuthenticationResult.ExpiresIn);
        }
        catch (NotAuthorizedException)
        {
            return Result.Failure<AuthTokens>(IdentityErrors.InvalidRefreshToken);
        }
        catch (Exception)
        {
            // Log ex
            return Result.Failure<AuthTokens>(IdentityErrors.CognitoError);
        }
    }

    public async Task<Result> GlobalSignOutAsync(string cognitoUserId)
    {
        try
        {
            var request = new AdminUserGlobalSignOutRequest
            {
                UserPoolId = _settings.UserPoolId,
                Username = cognitoUserId
            };

            await _cognitoClient.AdminUserGlobalSignOutAsync(request);

            return Result.Success();
        }
        catch (Exception)
        {
            // Log ex
            return Result.Failure(IdentityErrors.CognitoError);
        }
    }

    public async Task DeleteUserAsync(string cognitoUserId)
    {
        try
        {
            var request = new AdminDeleteUserRequest
            {
                UserPoolId = _settings.UserPoolId,
                Username = cognitoUserId
            };

            await _cognitoClient.AdminDeleteUserAsync(request);
        }
        catch
        {
            // Best effort
        }
    }
}
