namespace GlowNow.UnitTests.Shared.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+593987654321")]
    [InlineData("0987654321")]
    [InlineData("987654321")]
    public void Create_Should_ReturnSuccess_When_FormatIsValid(string value)
    {
        // Act
        var result = PhoneNumber.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("+593987654321");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_FormatIsInvalid()
    {
        // Act
        var result = PhoneNumber.Create("12345");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PhoneNumber.InvalidFormat");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PhoneNumberIsEmpty()
    {
        // Act
        var result = PhoneNumber.Create("");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PhoneNumber.Empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_PhoneNumberIsNull()
    {
        // Act
        var result = PhoneNumber.Create(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PhoneNumber.Empty");
    }

    [Fact]
    public void Create_Should_NormalizePhoneWithSpacesAndDashes()
    {
        // Act
        var result = PhoneNumber.Create("+593 987-654-321");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("+593987654321");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CountryCodeIsWrong()
    {
        // Act
        var result = PhoneNumber.Create("+1987654321");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PhoneNumber.InvalidFormat");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DigitCountIsWrong()
    {
        // Act
        var result = PhoneNumber.Create("+59398765432"); // Only 8 digits after +593

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("PhoneNumber.InvalidFormat");
    }
}
