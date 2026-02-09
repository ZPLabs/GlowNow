namespace GlowNow.UnitTests.Shared.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_EmailIsValid()
    {
        // Arrange
        var emailValue = "test@example.com";

        // Act
        var result = Email.Create(emailValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(emailValue);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EmailIsEmpty()
    {
        // Act
        var result = Email.Create("");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.Empty");
    }

    [Fact]
    public void Create_Should_NormalizeToLowercase()
    {
        // Act
        var result = Email.Create("TEST@Example.COM");

        // Assert
        result.Value.Value.Should().Be("test@example.com");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_FormatIsInvalid()
    {
        // Act
        var result = Email.Create("invalid-email");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.InvalidFormat");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EmailExceedsMaxLength()
    {
        // Arrange
        var longEmail = new string('a', 251) + "@b.com";

        // Act
        var result = Email.Create(longEmail);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.TooLong");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EmailIsNull()
    {
        // Act
        var result = Email.Create(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.Empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EmailIsWhitespace()
    {
        // Act
        var result = Email.Create("   ");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Email.Empty");
    }
}
