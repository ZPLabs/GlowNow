namespace GlowNow.UnitTests.Business.Domain.ValueObjects;

public class TimeRangeTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_CloseTimeIsAfterOpenTime()
    {
        // Arrange
        var openTime = new TimeOnly(9, 0);
        var closeTime = new TimeOnly(17, 0);

        // Act
        var result = TimeRange.Create(openTime, closeTime);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OpenTime.Should().Be(openTime);
        result.Value.CloseTime.Should().Be(closeTime);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CloseTimeEqualsOpenTime()
    {
        // Arrange
        var time = new TimeOnly(9, 0);

        // Act
        var result = TimeRange.Create(time, time);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.InvalidTimeRange);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CloseTimeIsBeforeOpenTime()
    {
        // Arrange
        var openTime = new TimeOnly(17, 0);
        var closeTime = new TimeOnly(9, 0);

        // Act
        var result = TimeRange.Create(openTime, closeTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.InvalidTimeRange);
    }

    [Fact]
    public void Create_FromStrings_Should_ReturnSuccess_When_ValidFormat()
    {
        // Act
        var result = TimeRange.Create("09:00", "17:00");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OpenTime.Should().Be(new TimeOnly(9, 0));
        result.Value.CloseTime.Should().Be(new TimeOnly(17, 0));
    }

    [Fact]
    public void Create_FromStrings_Should_ReturnFailure_When_InvalidOpenTimeFormat()
    {
        // Act
        var result = TimeRange.Create("invalid", "17:00");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.InvalidTimeRange);
    }

    [Fact]
    public void Create_FromStrings_Should_ReturnFailure_When_InvalidCloseTimeFormat()
    {
        // Act
        var result = TimeRange.Create("09:00", "invalid");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BusinessErrors.InvalidTimeRange);
    }

    [Fact]
    public void Duration_Should_ReturnCorrectTimeSpan()
    {
        // Arrange
        var openTime = new TimeOnly(9, 0);
        var closeTime = new TimeOnly(17, 0);
        var timeRange = TimeRange.Create(openTime, closeTime).Value;

        // Act
        var duration = timeRange.Duration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(8));
    }

    [Fact]
    public void Contains_Should_ReturnTrue_When_TimeIsWithinRange()
    {
        // Arrange
        var timeRange = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        // Act & Assert
        timeRange.Contains(new TimeOnly(12, 0)).Should().BeTrue();
        timeRange.Contains(new TimeOnly(9, 0)).Should().BeTrue();
        timeRange.Contains(new TimeOnly(16, 59)).Should().BeTrue();
    }

    [Fact]
    public void Contains_Should_ReturnFalse_When_TimeIsOutsideRange()
    {
        // Arrange
        var timeRange = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        // Act & Assert
        timeRange.Contains(new TimeOnly(8, 59)).Should().BeFalse();
        timeRange.Contains(new TimeOnly(17, 0)).Should().BeFalse();
        timeRange.Contains(new TimeOnly(18, 0)).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_ReturnFormattedString()
    {
        // Arrange
        var timeRange = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        // Act
        var result = timeRange.ToString();

        // Assert
        result.Should().Be("09:00-17:00");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_When_ValuesAreEqual()
    {
        // Arrange
        var timeRange1 = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;
        var timeRange2 = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;

        // Act & Assert
        timeRange1.Equals(timeRange2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ValuesAreDifferent()
    {
        // Arrange
        var timeRange1 = TimeRange.Create(new TimeOnly(9, 0), new TimeOnly(17, 0)).Value;
        var timeRange2 = TimeRange.Create(new TimeOnly(10, 0), new TimeOnly(18, 0)).Value;

        // Act & Assert
        timeRange1.Equals(timeRange2).Should().BeFalse();
    }
}
