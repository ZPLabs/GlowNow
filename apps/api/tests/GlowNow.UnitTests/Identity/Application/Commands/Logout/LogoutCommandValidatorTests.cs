using GlowNow.Identity.Application.Commands.Logout;

namespace GlowNow.UnitTests.Identity.Application.Commands.Logout;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator;

    public LogoutCommandValidatorTests()
    {
        _validator = new LogoutCommandValidator();
    }

    [Fact]
    public void Validate_Should_ReturnSuccess_When_CognitoUserIdIsProvided()
    {
        // Arrange
        var command = new LogoutCommand("cognito-user-id");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_CognitoUserIdIsEmpty()
    {
        // Arrange
        var command = new LogoutCommand("");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CognitoUserId");
    }
}
