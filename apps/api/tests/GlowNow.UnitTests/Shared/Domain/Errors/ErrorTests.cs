namespace GlowNow.UnitTests.Shared.Domain.Errors;

public class ErrorTests
{
    [Fact]
    public void Errors_ShouldBeEqual_WhenCodesAndMessagesMatch()
    {
        // Arrange
        var error1 = new Error("Code", "Message");
        var error2 = new Error("Code", "Message");

        // Act & Assert
        error1.Should().Be(error2);
    }

    [Fact]
    public void ValidationError_FromErrors_ShouldContainAllErrors()
    {
        // Arrange
        var errors = new[]
        {
            new Error("Error1", "Message1"),
            new Error("Error2", "Message2")
        };

        // Act
        var validationError = ValidationError.FromErrors(errors);

        // Assert
        validationError.Errors.Should().HaveCount(2);
        validationError.Errors.Should().BeEquivalentTo(errors);
    }
}
