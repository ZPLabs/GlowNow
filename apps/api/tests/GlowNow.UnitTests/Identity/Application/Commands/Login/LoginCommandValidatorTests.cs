using GlowNow.Identity.Application.Commands.Login;

namespace GlowNow.UnitTests.Identity.Application.Commands.Login;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_Should_ReturnSuccess_When_CommandIsValid()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "Password123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_EmailIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("", "Password123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_PasswordIsEmpty()
    {
        // Arrange
        var command = new LoginCommand("test@example.com", "");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_EmailFormatIsInvalid()
    {
        // Arrange
        var command = new LoginCommand("not-an-email", "Password123!");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
