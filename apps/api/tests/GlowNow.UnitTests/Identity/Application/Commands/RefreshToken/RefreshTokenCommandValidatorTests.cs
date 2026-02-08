using GlowNow.Identity.Application.Commands.RefreshToken;

namespace GlowNow.UnitTests.Identity.Application.Commands.RefreshToken;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenCommandValidatorTests()
    {
        _validator = new RefreshTokenCommandValidator();
    }

    [Fact]
    public void Validate_Should_ReturnSuccess_When_RefreshTokenIsProvided()
    {
        // Arrange
        var command = new RefreshTokenCommand("valid-refresh-token");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_Should_ReturnFailure_When_RefreshTokenIsEmpty()
    {
        // Arrange
        var command = new RefreshTokenCommand("");

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken");
    }
}
