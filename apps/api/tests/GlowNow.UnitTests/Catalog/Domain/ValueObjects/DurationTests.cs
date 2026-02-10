namespace GlowNow.UnitTests.Catalog.Domain.ValueObjects;

public class DurationTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_DurationIsValid()
    {
        // Arrange
        var minutes = 60;

        // Act
        var result = Duration.Create(minutes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Minutes.Should().Be(60);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_DurationIsMinimum()
    {
        // Act
        var result = Duration.Create(Duration.MinMinutes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Minutes.Should().Be(5);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_DurationIsMaximum()
    {
        // Act
        var result = Duration.Create(Duration.MaxMinutes);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Minutes.Should().Be(480);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DurationIsBelowMinimum()
    {
        // Act
        var result = Duration.Create(Duration.MinMinutes - 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Catalog.InvalidDuration");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DurationIsAboveMaximum()
    {
        // Act
        var result = Duration.Create(Duration.MaxMinutes + 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Catalog.InvalidDuration");
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DurationIsZero()
    {
        // Act
        var result = Duration.Create(0);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DurationIsNegative()
    {
        // Act
        var result = Duration.Create(-10);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void ToTimeSpan_Should_ReturnCorrectTimeSpan()
    {
        // Arrange
        var duration = Duration.Create(90).Value;

        // Act
        var timeSpan = duration.ToTimeSpan();

        // Assert
        timeSpan.Should().Be(TimeSpan.FromMinutes(90));
    }

    [Fact]
    public void ImplicitConversion_Should_ReturnMinutes()
    {
        // Arrange
        var duration = Duration.Create(60).Value;

        // Act
        int minutes = duration;

        // Assert
        minutes.Should().Be(60);
    }

    [Fact]
    public void ToString_Should_ReturnFormattedString()
    {
        // Arrange
        var duration = Duration.Create(45).Value;

        // Act
        var result = duration.ToString();

        // Assert
        result.Should().Be("45 min");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_When_ValuesAreEqual()
    {
        // Arrange
        var duration1 = Duration.Create(60).Value;
        var duration2 = Duration.Create(60).Value;

        // Act & Assert
        duration1.Equals(duration2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ValuesAreDifferent()
    {
        // Arrange
        var duration1 = Duration.Create(60).Value;
        var duration2 = Duration.Create(90).Value;

        // Act & Assert
        duration1.Equals(duration2).Should().BeFalse();
    }
}
