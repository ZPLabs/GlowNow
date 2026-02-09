namespace GlowNow.UnitTests.Business.Domain.ValueObjects;

public class RucTests
{
    [Theory]
    [InlineData("0102030405")] // 10 digits
    [InlineData("0102030405001")] // 13 digits
    public void Create_Should_ReturnSuccess_When_FormatIsValid(string value)
    {
        // Act
        var result = Ruc.Create(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(value);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ProvinceIsInvalid()
    {
        // Act
        var result = Ruc.Create("9902030405");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.InvalidProvince");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_LengthIsInvalid()
    {
        // Act
        var result = Ruc.Create("010203040");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.InvalidFormat");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_RucIsEmpty()
    {
        // Act
        var result = Ruc.Create("");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.Empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_RucIsNull()
    {
        // Act
        var result = Ruc.Create(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.Empty");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_RucContainsNonNumericCharacters()
    {
        // Act
        var result = Ruc.Create("01020304AB");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.InvalidFormat");
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_ProvinceCodeIs30()
    {
        // Arrange — province code 30 is for foreigners
        var result = Ruc.Create("3002030405");

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_ProvinceCodeIs00()
    {
        // Act
        var result = Ruc.Create("0002030405");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Ruc.InvalidProvince");
    }
}
