namespace GlowNow.Team.Domain.Entities;

/// <summary>
/// Represents a time off request for a staff member.
/// </summary>
public sealed class TimeOff : AggregateRoot<Guid>, ITenantScoped
{
    private TimeOff(
        Guid id,
        Guid staffProfileId,
        Guid businessId,
        DateOnly startDate,
        DateOnly endDate,
        TimeOffType type,
        string? notes,
        TimeOffStatus status,
        DateTime requestedAtUtc)
        : base(id)
    {
        StaffProfileId = staffProfileId;
        BusinessId = businessId;
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
        Notes = notes;
        Status = status;
        RequestedAtUtc = requestedAtUtc;
    }

    private TimeOff()
    {
    }

    /// <summary>
    /// Gets the staff profile ID.
    /// </summary>
    public Guid StaffProfileId { get; private set; }

    /// <summary>
    /// Gets the business ID.
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// Gets the start date of the time off.
    /// </summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// Gets the end date of the time off (inclusive).
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>
    /// Gets the type of time off.
    /// </summary>
    public TimeOffType Type { get; private set; }

    /// <summary>
    /// Gets any notes for the time off request.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Gets the status of the time off request.
    /// </summary>
    public TimeOffStatus Status { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the request was made.
    /// </summary>
    public DateTime RequestedAtUtc { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the request was approved (if approved).
    /// </summary>
    public DateTime? ApprovedAtUtc { get; private set; }

    /// <summary>
    /// Gets the user ID of who approved the request (if approved).
    /// </summary>
    public Guid? ApprovedByUserId { get; private set; }

    /// <summary>
    /// Gets the rejection reason (if rejected).
    /// </summary>
    public string? RejectionReason { get; private set; }

    /// <summary>
    /// Creates a new TimeOff request.
    /// </summary>
    public static Result<TimeOff> Create(
        Guid staffProfileId,
        Guid businessId,
        DateOnly startDate,
        DateOnly endDate,
        TimeOffType type,
        string? notes,
        DateTime requestedAtUtc)
    {
        if (endDate < startDate)
        {
            return Result.Failure<TimeOff>(TeamErrors.InvalidTimeOffDates);
        }

        var timeOff = new TimeOff(
            Guid.NewGuid(),
            staffProfileId,
            businessId,
            startDate,
            endDate,
            type,
            notes?.Trim(),
            TimeOffStatus.Pending,
            requestedAtUtc);

        timeOff.RaiseDomainEvent(new TimeOffRequestedEvent(
            timeOff.Id,
            timeOff.StaffProfileId,
            timeOff.BusinessId,
            timeOff.StartDate,
            timeOff.EndDate,
            timeOff.Type));

        return timeOff;
    }

    /// <summary>
    /// Approves the time off request.
    /// </summary>
    /// <param name="approvedByUserId">The ID of the user who approved the request.</param>
    /// <param name="approvedAtUtc">The UTC timestamp of approval.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result Approve(Guid approvedByUserId, DateTime approvedAtUtc)
    {
        if (Status == TimeOffStatus.Approved)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyApproved);
        }

        if (Status == TimeOffStatus.Rejected)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyRejected);
        }

        if (Status == TimeOffStatus.Cancelled)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyCancelled);
        }

        if (Status != TimeOffStatus.Pending)
        {
            return Result.Failure(TeamErrors.TimeOffNotPending);
        }

        Status = TimeOffStatus.Approved;
        ApprovedAtUtc = approvedAtUtc;
        ApprovedByUserId = approvedByUserId;

        RaiseDomainEvent(new TimeOffApprovedEvent(Id, StaffProfileId, BusinessId, approvedByUserId));

        return Result.Success();
    }

    /// <summary>
    /// Rejects the time off request.
    /// </summary>
    /// <param name="reason">The reason for rejection.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result Reject(string? reason)
    {
        if (Status == TimeOffStatus.Approved)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyApproved);
        }

        if (Status == TimeOffStatus.Rejected)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyRejected);
        }

        if (Status == TimeOffStatus.Cancelled)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyCancelled);
        }

        if (Status != TimeOffStatus.Pending)
        {
            return Result.Failure(TeamErrors.TimeOffNotPending);
        }

        Status = TimeOffStatus.Rejected;
        RejectionReason = reason?.Trim();

        return Result.Success();
    }

    /// <summary>
    /// Cancels the time off request.
    /// </summary>
    /// <param name="currentDate">The current date for validation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result Cancel(DateOnly currentDate)
    {
        if (Status == TimeOffStatus.Cancelled)
        {
            return Result.Failure(TeamErrors.TimeOffAlreadyCancelled);
        }

        // Cannot cancel time off that has already started
        if (currentDate >= StartDate)
        {
            return Result.Failure(TeamErrors.CannotCancelPastTimeOff);
        }

        Status = TimeOffStatus.Cancelled;

        return Result.Success();
    }

    /// <summary>
    /// Gets the number of days in this time off period.
    /// </summary>
    public int DaysCount => EndDate.DayNumber - StartDate.DayNumber + 1;

    /// <summary>
    /// Checks if a specific date falls within this time off period.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the date is within the time off period.</returns>
    public bool ContainsDate(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    /// <summary>
    /// Checks if this time off overlaps with another period.
    /// </summary>
    /// <param name="start">The start date of the other period.</param>
    /// <param name="end">The end date of the other period.</param>
    /// <returns>True if the periods overlap.</returns>
    public bool OverlapsWith(DateOnly start, DateOnly end)
    {
        return StartDate <= end && EndDate >= start;
    }
}
