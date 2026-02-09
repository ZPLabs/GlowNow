using GlowNow.Identity.Application.Commands.Logout;
using GlowNow.Identity.Application.Interfaces;

namespace GlowNow.UnitTests.Identity.Application.Commands.Logout;

public class LogoutCommandHandlerTests
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _cognitoService = Substitute.For<ICognitoIdentityProvider>();
        _handler = new LogoutCommandHandler(_cognitoService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_SignOutSucceeds()
    {
        // Arrange
        var command = new LogoutCommand("cognito-user-id");
        _cognitoService.GlobalSignOutAsync("cognito-user-id").Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _cognitoService.Received(1).GlobalSignOutAsync("cognito-user-id");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CognitoSignOutFails()
    {
        // Arrange
        var command = new LogoutCommand("cognito-user-id");
        _cognitoService.GlobalSignOutAsync("cognito-user-id").Returns(Result.Failure(IdentityErrors.CognitoError));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.CognitoError);
    }
}
