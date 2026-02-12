using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Team.Domain.Events;
using GlowNow.Team.Domain.ValueObjects;

namespace GlowNow.UnitTests.Team.Domain.Entities;

public class StaffProfileTests
{
    private readonly Guid _businessId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly string _title = "Stylist";
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_Should_ReturnSuccess_When_ValidData()
    {
        // Act
        var result = StaffProfile.Create(
            _businessId,
            _userId,
            _title,
            "Expert stylist",
            "https://example.com/image.jpg",
            displayOrder: 1,
            acceptsOnlineBookings: true,
            _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.BusinessId.Should().Be(_businessId);
        result.Value.UserId.Should().Be(_userId);
        result.Value.Title.Should().Be(_title);
        result.Value.Bio.Should().Be("Expert stylist");
        result.Value.ProfileImageUrl.Should().Be("https://example.com/image.jpg");
        result.Value.DisplayOrder.Should().Be(1);
        result.Value.AcceptsOnlineBookings.Should().BeTrue();
        result.Value.Status.Should().Be(StaffStatus.Pending);
        result.Value.CreatedAtUtc.Should().Be(_now);
        result.Value.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_Should_RaiseStaffProfileCreatedEvent()
    {
        // Act
        var result = StaffProfile.Create(
            _businessId,
            _userId,
            _title,
            null,
            null,
            0,
            true,
            _now);

        // Assert
        result.Value.GetDomainEvents().Should().ContainSingle(e => e is StaffProfileCreatedEvent);
        var domainEvent = result.Value.GetDomainEvents().OfType<StaffProfileCreatedEvent>().First();
        domainEvent.StaffProfileId.Should().Be(result.Value.Id);
        domainEvent.BusinessId.Should().Be(_businessId);
        domainEvent.UserId.Should().Be(_userId);
        domainEvent.Title.Should().Be(_title);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_TitleIsEmpty()
    {
        // Act
        var result = StaffProfile.Create(
            _businessId,
            _userId,
            "",
            null,
            null,
            0,
            true,
            _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.InvalidStaffTitle);
    }

    [Fact]
    public void Update_Should_ModifyProperties()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;

        // Act
        var result = profile.Update(
            "Senior Stylist",
            "New bio",
            "new-image.jpg",
            5,
            false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Title.Should().Be("Senior Stylist");
        profile.Bio.Should().Be("New bio");
        profile.ProfileImageUrl.Should().Be("new-image.jpg");
        profile.DisplayOrder.Should().Be(5);
        profile.AcceptsOnlineBookings.Should().BeFalse();
    }

    [Fact]
    public void Activate_Should_SetStatusToActive()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;

        // Act
        var result = profile.Activate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Status.Should().Be(StaffStatus.Active);
    }

    [Fact]
    public void Activate_Should_ReturnFailure_When_AlreadyActive()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        profile.Activate();

        // Act
        var result = profile.Activate();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.StaffAlreadyActive);
    }

    [Fact]
    public void Deactivate_Should_SetStatusToInactive()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        profile.Activate();

        // Act
        var result = profile.Deactivate();

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Status.Should().Be(StaffStatus.Inactive);
    }

    [Fact]
    public void Delete_Should_ReturnSuccess_When_StaffIsInactive()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        profile.Activate();
        profile.Deactivate();

        // Act
        var result = profile.Delete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_Should_ReturnFailure_When_StaffIsActive()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        profile.Activate();

        // Act
        var result = profile.Delete();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.CannotDeleteActiveStaff);
        profile.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void AssignService_Should_AddAssignmentAndRaiseEvent()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        var serviceId = Guid.NewGuid();
        profile.ClearDomainEvents();

        // Act
        var result = profile.AssignService(serviceId, _now);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.ServiceAssignments.Should().ContainSingle(a => a.ServiceId == serviceId);
        profile.GetDomainEvents().Should().ContainSingle(e => e is StaffServiceAssignedEvent);
    }

    [Fact]
    public void AssignService_Should_ReturnFailure_When_AlreadyAssigned()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        var serviceId = Guid.NewGuid();
        profile.AssignService(serviceId, _now);

        // Act
        var result = profile.AssignService(serviceId, _now);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.ServiceAlreadyAssigned);
    }

    [Fact]
    public void UnassignService_Should_RemoveAssignmentAndRaiseEvent()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        var serviceId = Guid.NewGuid();
        profile.AssignService(serviceId, _now);
        profile.ClearDomainEvents();

        // Act
        var result = profile.UnassignService(serviceId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.ServiceAssignments.Should().BeEmpty();
        profile.GetDomainEvents().Should().ContainSingle(e => e is StaffServiceUnassignedEvent);
    }

    [Fact]
    public void UnassignService_Should_ReturnFailure_When_NotAssigned()
    {
        // Arrange
        var profile = StaffProfile.Create(_businessId, _userId, _title, null, null, 0, true, _now).Value;
        var serviceId = Guid.NewGuid();

        // Act
        var result = profile.UnassignService(serviceId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TeamErrors.ServiceNotAssigned);
    }
}
