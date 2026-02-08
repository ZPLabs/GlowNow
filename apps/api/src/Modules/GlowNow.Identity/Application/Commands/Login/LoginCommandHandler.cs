using GlowNow.Identity.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(ICognitoIdentityProvider cognitoService, IUserRepository userRepository)
    {
        _cognitoService = cognitoService;
        _userRepository = userRepository;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var authResult = await _cognitoService.LoginAsync(command.Email, command.Password);

        if (authResult.IsFailure)
        {
            return Result.Failure<LoginResponse>(authResult.Error);
        }

        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure) return Result.Failure<LoginResponse>(emailResult.Error);

        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginResponse>(IdentityErrors.UserNotFound);
        }

        var response = new LoginResponse(
            authResult.Value.AccessToken,
            authResult.Value.RefreshToken,
            authResult.Value.ExpiresIn,
            new UserResponse(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Memberships.Select(m => new MembershipResponse(m.BusinessId, m.Role))));

        return response;
    }
}
