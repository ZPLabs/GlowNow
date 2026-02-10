using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Errors;

namespace GlowNow.UnitTests.Team.Domain.Entities;

public class BlockedTimeTests
{
    private readonly Guid _staffProfileId = Guid.NewGuid();
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void CreateRecurring_Should_ReturnSuccess_When_ValidData()
    {
        // Arrange
        var start = new TimeOnly(13, 0);
        var end = new TimeOnly(14, 0);
        var day = DayOfWeek.Monday;

        // Act
        var result = BlockedTime.CreateRecurring(_staffProfileId, _businessId, "Lunch", start, end, day, _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsRecurring.Should().BeTrue();
        result.Value.RecurringDayOfWeek.Should().Be(day);
        result.Value.StartTime.Should().Be(start);
        result.Value.EndTime.Should().Be(end);
    }

    [Fact]
    public void CreateOneTime_Should_ReturnSuccess_When_ValidData()
    {
        // Arrange
        var start = new TimeOnly(10, 0);
        var end = new TimeOnly(11, 0);
        var date = new DateOnly(2025, 6, 15);

        // Act
        var result = BlockedTime.CreateOneTime(_staffProfileId, _businessId, "Meeting", start, end, date, _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsRecurring.Should().BeFalse();
        result.Value.SpecificDate.Should().Be(date);
    }

    [Fact]
    public void CreateRecurring_Should_ReturnFailure_When_EndBeforeStart()
    {
        // Act
        var result = BlockedTime.CreateRecurring(
            _staffProfileId, _businessId, null, new TimeOnly(14, 0), new TimeOnly(13, 0), DayOfWeek.Monday, _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.InvalidBlockedTimeTimes);
    }

    [Fact]
    public void AppliesToDate_Should_ReturnTrue_When_RecurringDayMatches()
    {
        // Arrange
        var blocked = BlockedTime.CreateRecurring(
            _staffProfileId, _businessId, null, new TimeOnly(9, 0), new TimeOnly(10, 0), DayOfWeek.Monday, _now).Value;
        var monday = new DateOnly(2025, 5, 12);

        // Act & Assert
        blocked.AppliesToDate(monday).Should().BeTrue();
    }

    [Fact]
    public void AppliesToDate_Should_ReturnFalse_When_RecurringDayDoesNotMatch()
    {
        // Arrange
        var blocked = BlockedTime.CreateRecurring(
            _staffProfileId, _businessId, null, new TimeOnly(9, 0), new TimeOnly(10, 0), DayOfWeek.Monday, _now).Value;
        var tuesday = new DateOnly(2025, 5, 13);

        // Act & Assert
        blocked.AppliesToDate(tuesday).Should().BeFalse();
    }

    [Fact]
    public void IsTimeBlocked_Should_ReturnTrue_When_TimeInRangeOnCorrectDate()
    {
        // Arrange
        var date = new DateOnly(2025, 5, 12);
        var blocked = BlockedTime.CreateOneTime(
            _staffProfileId, _businessId, null, new TimeOnly(9, 0), new TimeOnly(10, 0), date, _now).Value;
        
        var checkTime = new DateTime(2025, 5, 12, 9, 30, 0);

        // Act & Assert
        blocked.IsTimeBlocked(checkTime).Should().BeTrue();
    }
}
