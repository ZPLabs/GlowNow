using GlowNow.Identity.Application.Commands.RefreshToken;
using GlowNow.Identity.Application.Interfaces;

namespace GlowNow.UnitTests.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _cognitoService = Substitute.For<ICognitoIdentityProvider>();
        _handler = new RefreshTokenCommandHandler(_cognitoService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RefreshTokenIsValid()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid-refresh-token");
        var tokens = new AuthTokens("new-access", "new-id", "new-refresh", 3600);
        _cognitoService.RefreshTokenAsync("valid-refresh-token").Returns(Result.Success(tokens));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-access");
        result.Value.RefreshToken.Should().Be("new-refresh");
        result.Value.ExpiresIn.Should().Be(3600);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RefreshTokenIsInvalid()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalid-token");
        _cognitoService.RefreshTokenAsync("invalid-token")
            .Returns(Result.Failure<AuthTokens>(IdentityErrors.InvalidRefreshToken));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidRefreshToken);
    }
}
