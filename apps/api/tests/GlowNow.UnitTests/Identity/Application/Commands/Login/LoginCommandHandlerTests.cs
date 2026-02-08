using GlowNow.Identity.Application.Commands.Login;
using GlowNow.Identity.Application.Interfaces;

namespace GlowNow.UnitTests.Identity.Application.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly ICognitoIdentityProvider _cognitoService;
    private readonly IUserRepository _userRepository;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _cognitoService = Substitute.For<ICognitoIdentityProvider>();
        _userRepository = Substitute.For<IUserRepository>();
        _handler = new LoginCommandHandler(_cognitoService, _userRepository);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CredentialsAreValid()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");
        var tokens = new AuthTokens("access", "id", "refresh", 3600);
        var user = User.Create(Email.Create(command.Email).Value, "John", "Doe", null, "id", DateTime.UtcNow);

        _cognitoService.LoginAsync(command.Email, command.Password).Returns(Result.Success(tokens));
        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CognitoFails()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "wrong");
        _cognitoService.LoginAsync(command.Email, command.Password).Returns(Result.Failure<AuthTokens>(IdentityErrors.InvalidCredentials));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFoundInLocalDb()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");
        var tokens = new AuthTokens("access", "id", "refresh", 3600);

        _cognitoService.LoginAsync(command.Email, command.Password).Returns(Result.Success(tokens));
        _userRepository.GetByEmailAsync(Arg.Any<Email>()).Returns((User)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(IdentityErrors.UserNotFound);
    }
}
