using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly ICognitoIdentityProvider _cognitoService;

    public RefreshTokenCommandHandler(ICognitoIdentityProvider cognitoService)
    {
        _cognitoService = cognitoService;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var authResult = await _cognitoService.RefreshTokenAsync(command.RefreshToken);

        if (authResult.IsFailure)
        {
            return Result.Failure<RefreshTokenResponse>(authResult.Error);
        }

        return new RefreshTokenResponse(
            authResult.Value.AccessToken,
            authResult.Value.RefreshToken,
            authResult.Value.ExpiresIn);
    }
}
