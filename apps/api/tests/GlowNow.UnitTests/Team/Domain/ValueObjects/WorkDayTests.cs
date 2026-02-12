using GlowNow.Team.Domain.Errors;
using GlowNow.Team.Domain.ValueObjects;

namespace GlowNow.UnitTests.Team.Domain.ValueObjects;

public class WorkDayTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidTimes()
    {
        // Arrange
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(18, 0);

        // Act
        var result = WorkDay.Create(start, end);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StartTime.Should().Be(start);
        result.Value.EndTime.Should().Be(end);
        result.Value.HasBreak.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidTimesWithBreak()
    {
        // Arrange
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(18, 0);
        var breakStart = new TimeOnly(13, 0);
        var breakEnd = new TimeOnly(14, 0);

        // Act
        var result = WorkDay.Create(start, end, breakStart, breakEnd);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StartTime.Should().Be(start);
        result.Value.EndTime.Should().Be(end);
        result.Value.BreakStart.Should().Be(breakStart);
        result.Value.BreakEnd.Should().Be(breakEnd);
        result.Value.HasBreak.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EndBeforeStart()
    {
        // Act
        var result = WorkDay.Create(new TimeOnly(18, 0), new TimeOnly(9, 0));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.InvalidWorkDayTimes);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_BreakEndBeforeBreakStart()
    {
        // Act
        var result = WorkDay.Create(
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            new TimeOnly(14, 0),
            new TimeOnly(13, 0));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.InvalidBreakTimes);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_BreakOutsideWorkHours()
    {
        // Act
        var result = WorkDay.Create(
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            new TimeOnly(8, 0),
            new TimeOnly(10, 0));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.BreakOutsideWorkHours);
    }

    [Fact]
    public void WorkDuration_Should_SubtractBreakTime()
    {
        // Arrange
        var workDay = WorkDay.Create(
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            new TimeOnly(13, 0),
            new TimeOnly(14, 0)).Value;

        // Act
        var duration = workDay.WorkDuration;

        // Assert
        duration.Should().Be(TimeSpan.FromHours(8));
    }

    [Theory]
    [InlineData(9, 0, true)]
    [InlineData(12, 0, true)]
    [InlineData(13, 0, false)] // During break
    [InlineData(13, 30, false)] // During break
    [InlineData(14, 0, true)]
    [InlineData(18, 0, false)] // End time is exclusive
    [InlineData(20, 0, false)]
    public void IsWorkingTime_Should_ReturnCorrectValue(int hour, int minute, bool expected)
    {
        // Arrange
        var workDay = WorkDay.Create(
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            new TimeOnly(13, 0),
            new TimeOnly(14, 0)).Value;
        var time = new TimeOnly(hour, minute);

        // Act
        var result = workDay.IsWorkingTime(time);

        // Assert
        result.Should().Be(expected);
    }
}
