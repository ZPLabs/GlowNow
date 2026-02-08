namespace GlowNow.UnitTests.Shared.Domain.Errors;

public class ResultTests
{
    [Fact]
    public void Success_ShouldHaveIsSuccessTrue()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldHaveIsFailureTrue()
    {
        // Arrange
        var error = new Error("Test.Error", "Test message");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessGeneric_ShouldContainValue()
    {
        // Arrange
        var value = "test";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ValueAccess_ShouldThrow_WhenFailure()
    {
        // Arrange
        var result = Result.Failure<string>(new Error("Error", "Message"));

        // Act
        Action action = () => { var _ = result.Value; };

        // Assert
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitOperator_ShouldReturnSuccess_WhenValueIsNotNull()
    {
        // Arrange
        string value = "test";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitOperator_ShouldReturnFailure_WhenValueIsNull()
    {
        // Arrange
        string? value = null;

        // Act
        Result<string> result = value;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NullValue);
    }
}
