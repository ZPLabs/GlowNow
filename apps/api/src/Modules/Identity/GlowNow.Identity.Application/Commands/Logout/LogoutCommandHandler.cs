using GlowNow.Identity.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Identity.Application.Commands.Logout;

internal sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly ICognitoIdentityProvider _cognitoService;

    public LogoutCommandHandler(ICognitoIdentityProvider cognitoService)
    {
        _cognitoService = cognitoService;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        return await _cognitoService.GlobalSignOutAsync(command.CognitoUserId);
    }
}
