using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;
using GlowNow.Team.Domain.Enums;
using GlowNow.Team.Domain.Errors;
using GlowNow.Team.Domain.Events;
using GlowNow.Team.Domain.ValueObjects;

namespace GlowNow.Team.Domain.Entities;

/// <summary>
/// Represents a staff member profile within a business.
/// </summary>
public sealed class StaffProfile : AggregateRoot<Guid>, ITenantScoped
{
    private readonly List<StaffServiceAssignment> _serviceAssignments = new();

    private StaffProfile(
        Guid id,
        Guid businessId,
        Guid userId,
        string title,
        string? bio,
        string? profileImageUrl,
        int displayOrder,
        bool acceptsOnlineBookings,
        WeeklySchedule defaultSchedule,
        StaffStatus status,
        DateTime createdAtUtc)
        : base(id)
    {
        BusinessId = businessId;
        UserId = userId;
        Title = title;
        Bio = bio;
        ProfileImageUrl = profileImageUrl;
        DisplayOrder = displayOrder;
        AcceptsOnlineBookings = acceptsOnlineBookings;
        DefaultSchedule = defaultSchedule;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        IsDeleted = false;
    }

    private StaffProfile()
    {
        Title = null!;
        DefaultSchedule = null!;
    }

    /// <summary>
    /// Gets the business ID this staff profile belongs to.
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// Gets the user ID associated with this staff profile.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the staff member's title/role.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the staff member's bio/description.
    /// </summary>
    public string? Bio { get; private set; }

    /// <summary>
    /// Gets the URL of the staff member's profile image.
    /// </summary>
    public string? ProfileImageUrl { get; private set; }

    /// <summary>
    /// Gets the display order for UI sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets whether this staff member accepts online bookings.
    /// </summary>
    public bool AcceptsOnlineBookings { get; private set; }

    /// <summary>
    /// Gets the default weekly working schedule.
    /// </summary>
    public WeeklySchedule DefaultSchedule { get; private set; }

    /// <summary>
    /// Gets the staff member's current status.
    /// </summary>
    public StaffStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the profile was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets whether the profile is soft-deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Gets the services assigned to this staff member.
    /// </summary>
    public IReadOnlyCollection<StaffServiceAssignment> ServiceAssignments => _serviceAssignments.AsReadOnly();

    /// <summary>
    /// Creates a new StaffProfile instance.
    /// </summary>
    public static Result<StaffProfile> Create(
        Guid businessId,
        Guid userId,
        string title,
        string? bio,
        string? profileImageUrl,
        int displayOrder,
        bool acceptsOnlineBookings,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<StaffProfile>(TeamErrors.InvalidStaffTitle);
        }

        var profile = new StaffProfile(
            Guid.NewGuid(),
            businessId,
            userId,
            title.Trim(),
            bio?.Trim(),
            profileImageUrl?.Trim(),
            displayOrder,
            acceptsOnlineBookings,
            WeeklySchedule.CreateEmpty(),
            StaffStatus.Pending,
            createdAtUtc);

        profile.RaiseDomainEvent(new StaffProfileCreatedEvent(
            profile.Id,
            profile.BusinessId,
            profile.UserId,
            profile.Title));

        return profile;
    }

    /// <summary>
    /// Updates the staff profile details.
    /// </summary>
    public Result Update(
        string title,
        string? bio,
        string? profileImageUrl,
        int displayOrder,
        bool acceptsOnlineBookings)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure(TeamErrors.InvalidStaffTitle);
        }

        Title = title.Trim();
        Bio = bio?.Trim();
        ProfileImageUrl = profileImageUrl?.Trim();
        DisplayOrder = displayOrder;
        AcceptsOnlineBookings = acceptsOnlineBookings;

        return Result.Success();
    }

    /// <summary>
    /// Updates the default weekly schedule.
    /// </summary>
    /// <param name="schedule">The new weekly schedule.</param>
    public void UpdateSchedule(WeeklySchedule schedule)
    {
        DefaultSchedule = schedule;
    }

    /// <summary>
    /// Activates the staff member.
    /// </summary>
    public Result Activate()
    {
        if (Status == StaffStatus.Active)
        {
            return Result.Failure(TeamErrors.StaffAlreadyActive);
        }

        Status = StaffStatus.Active;
        return Result.Success();
    }

    /// <summary>
    /// Deactivates the staff member.
    /// </summary>
    public Result Deactivate()
    {
        if (Status == StaffStatus.Inactive)
        {
            return Result.Failure(TeamErrors.StaffAlreadyInactive);
        }

        Status = StaffStatus.Inactive;
        return Result.Success();
    }

    /// <summary>
    /// Sets the staff member on leave.
    /// </summary>
    public void SetOnLeave()
    {
        Status = StaffStatus.OnLeave;
    }

    /// <summary>
    /// Soft-deletes the staff profile.
    /// </summary>
    public Result Delete()
    {
        if (Status == StaffStatus.Active)
        {
            return Result.Failure(TeamErrors.CannotDeleteActiveStaff);
        }

        IsDeleted = true;
        return Result.Success();
    }

    /// <summary>
    /// Assigns a service to this staff member.
    /// </summary>
    /// <param name="serviceId">The service ID to assign.</param>
    /// <param name="assignedAtUtc">The assignment timestamp.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result AssignService(Guid serviceId, DateTime assignedAtUtc)
    {
        if (_serviceAssignments.Any(sa => sa.ServiceId == serviceId))
        {
            return Result.Failure(TeamErrors.ServiceAlreadyAssigned);
        }

        var assignment = StaffServiceAssignment.Create(Id, serviceId, assignedAtUtc);
        _serviceAssignments.Add(assignment);

        RaiseDomainEvent(new StaffServiceAssignedEvent(Id, serviceId, BusinessId));

        return Result.Success();
    }

    /// <summary>
    /// Unassigns a service from this staff member.
    /// </summary>
    /// <param name="serviceId">The service ID to unassign.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result UnassignService(Guid serviceId)
    {
        var assignment = _serviceAssignments.FirstOrDefault(sa => sa.ServiceId == serviceId);
        if (assignment is null)
        {
            return Result.Failure(TeamErrors.ServiceNotAssigned);
        }

        _serviceAssignments.Remove(assignment);

        RaiseDomainEvent(new StaffServiceUnassignedEvent(Id, serviceId, BusinessId));

        return Result.Success();
    }

    /// <summary>
    /// Checks if this staff member can perform a specific service.
    /// </summary>
    /// <param name="serviceId">The service ID to check.</param>
    /// <returns>True if the staff can perform the service.</returns>
    public bool CanPerformService(Guid serviceId)
    {
        return _serviceAssignments.Any(sa => sa.ServiceId == serviceId);
    }

    /// <summary>
    /// Checks if the staff member is available for bookings.
    /// </summary>
    public bool IsAvailableForBookings => Status == StaffStatus.Active && !IsDeleted;
}
