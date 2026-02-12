using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;

namespace GlowNow.UnitTests.Team.Domain.Entities;

public class TimeOffTests
{
    private readonly Guid _staffProfileId = Guid.NewGuid();
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidData()
    {
        // Arrange
        var start = new DateOnly(2025, 7, 1);
        var end = new DateOnly(2025, 7, 14);

        // Act
        var result = TimeOff.Create(_staffProfileId, _businessId, start, end, TimeOffType.Vacation, "Summer trip", _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StartDate.Should().Be(start);
        result.Value.EndDate.Should().Be(end);
        result.Value.Status.Should().Be(TimeOffStatus.Pending);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_EndBeforeStart()
    {
        // Act
        var result = TimeOff.Create(
            _staffProfileId, _businessId, new DateOnly(2025, 7, 14), new DateOnly(2025, 7, 1), TimeOffType.Vacation, null, _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.InvalidTimeOffDates);
    }

    [Fact]
    public void Approve_Should_SetStatusToApproved()
    {
        // Arrange
        var timeOff = TimeOff.Create(
            _staffProfileId, _businessId, new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 5), TimeOffType.Vacation, null, _now).Value;
        var approverId = Guid.NewGuid();

        // Act
        var result = timeOff.Approve(approverId, _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        timeOff.Status.Should().Be(TimeOffStatus.Approved);
        timeOff.ApprovedByUserId.Should().Be(approverId);
    }

    [Fact]
    public void Reject_Should_SetStatusToRejected()
    {
        // Arrange
        var timeOff = TimeOff.Create(
            _staffProfileId, _businessId, new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 5), TimeOffType.Vacation, null, _now).Value;

        // Act
        var result = timeOff.Reject("Too many people off");

        // Assert
        result.IsSuccess.Should().BeTrue();
        timeOff.Status.Should().Be(TimeOffStatus.Rejected);
        timeOff.RejectionReason.Should().Be("Too many people off");
    }

    [Fact]
    public void Cancel_Should_ReturnSuccess_When_BeforeStartDate()
    {
        // Arrange
        var today = new DateOnly(2025, 6, 20);
        var timeOff = TimeOff.Create(
            _staffProfileId, _businessId, new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 5), TimeOffType.Vacation, null, _now).Value;

        // Act
        var result = timeOff.Cancel(today);

        // Assert
        result.IsSuccess.Should().BeTrue();
        timeOff.Status.Should().Be(TimeOffStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_When_AlreadyStarted()
    {
        // Arrange
        var today = new DateOnly(2025, 7, 2);
        var timeOff = TimeOff.Create(
            _staffProfileId, _businessId, new DateOnly(2025, 7, 1), new DateOnly(2025, 7, 5), TimeOffType.Vacation, null, _now).Value;

        // Act
        var result = timeOff.Cancel(today);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.CannotCancelPastTimeOff);
    }
}
