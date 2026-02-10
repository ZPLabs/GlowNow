namespace GlowNow.Team.Domain.Entities;

/// <summary>
/// Represents a blocked time period for a staff member.
/// Can be recurring (weekly) or one-time.
/// </summary>
public sealed class BlockedTime : AggregateRoot<Guid>, ITenantScoped
{
    private BlockedTime(
        Guid id,
        Guid staffProfileId,
        Guid businessId,
        string? title,
        TimeOnly startTime,
        TimeOnly endTime,
        bool isRecurring,
        DayOfWeek? recurringDayOfWeek,
        DateOnly? specificDate,
        DateTime createdAtUtc)
        : base(id)
    {
        StaffProfileId = staffProfileId;
        BusinessId = businessId;
        Title = title;
        StartTime = startTime;
        EndTime = endTime;
        IsRecurring = isRecurring;
        RecurringDayOfWeek = recurringDayOfWeek;
        SpecificDate = specificDate;
        CreatedAtUtc = createdAtUtc;
    }

    private BlockedTime()
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
    /// Gets the optional title/reason for the blocked time.
    /// </summary>
    public string? Title { get; private set; }

    /// <summary>
    /// Gets the start time of the blocked period.
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// Gets the end time of the blocked period.
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// Gets whether this is a recurring blocked time.
    /// </summary>
    public bool IsRecurring { get; private set; }

    /// <summary>
    /// Gets the day of week for recurring blocked time.
    /// </summary>
    public DayOfWeek? RecurringDayOfWeek { get; private set; }

    /// <summary>
    /// Gets the specific date for one-time blocked time.
    /// </summary>
    public DateOnly? SpecificDate { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the blocked time was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Creates a recurring blocked time that repeats weekly on a specific day.
    /// </summary>
    public static Result<BlockedTime> CreateRecurring(
        Guid staffProfileId,
        Guid businessId,
        string? title,
        TimeOnly startTime,
        TimeOnly endTime,
        DayOfWeek dayOfWeek,
        DateTime createdAtUtc)
    {
        if (endTime <= startTime)
        {
            return Result.Failure<BlockedTime>(TeamErrors.InvalidBlockedTimeTimes);
        }

        return new BlockedTime(
            Guid.NewGuid(),
            staffProfileId,
            businessId,
            title?.Trim(),
            startTime,
            endTime,
            isRecurring: true,
            recurringDayOfWeek: dayOfWeek,
            specificDate: null,
            createdAtUtc);
    }

    /// <summary>
    /// Creates a one-time blocked time for a specific date.
    /// </summary>
    public static Result<BlockedTime> CreateOneTime(
        Guid staffProfileId,
        Guid businessId,
        string? title,
        TimeOnly startTime,
        TimeOnly endTime,
        DateOnly specificDate,
        DateTime createdAtUtc)
    {
        if (endTime <= startTime)
        {
            return Result.Failure<BlockedTime>(TeamErrors.InvalidBlockedTimeTimes);
        }

        return new BlockedTime(
            Guid.NewGuid(),
            staffProfileId,
            businessId,
            title?.Trim(),
            startTime,
            endTime,
            isRecurring: false,
            recurringDayOfWeek: null,
            specificDate: specificDate,
            createdAtUtc);
    }

    /// <summary>
    /// Gets the duration of the blocked time.
    /// </summary>
    public TimeSpan Duration => EndTime - StartTime;

    /// <summary>
    /// Checks if this blocked time applies to a specific date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if the blocked time applies to the date.</returns>
    public bool AppliesToDate(DateOnly date)
    {
        if (IsRecurring)
        {
            return date.DayOfWeek == RecurringDayOfWeek;
        }
        else
        {
            return date == SpecificDate;
        }
    }

    /// <summary>
    /// Checks if a specific time falls within this blocked period on a given date.
    /// </summary>
    /// <param name="dateTime">The date and time to check.</param>
    /// <returns>True if the time is blocked.</returns>
    public bool IsTimeBlocked(DateTime dateTime)
    {
        var date = DateOnly.FromDateTime(dateTime);
        if (!AppliesToDate(date))
        {
            return false;
        }

        var time = TimeOnly.FromDateTime(dateTime);
        return time >= StartTime && time < EndTime;
    }

    /// <summary>
    /// Checks if this blocked time overlaps with another time range on a given date.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="start">The start time.</param>
    /// <param name="end">The end time.</param>
    /// <returns>True if there's an overlap.</returns>
    public bool OverlapsWith(DateOnly date, TimeOnly start, TimeOnly end)
    {
        if (!AppliesToDate(date))
        {
            return false;
        }

        return StartTime < end && EndTime > start;
    }
}
