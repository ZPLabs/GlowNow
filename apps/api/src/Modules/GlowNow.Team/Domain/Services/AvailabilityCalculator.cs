namespace GlowNow.Team.Domain.Services;

/// <summary>
/// Service for calculating staff availability.
/// </summary>
public sealed class AvailabilityCalculator
{
    /// <summary>
    /// Calculates the available time slots for a staff member on a specific date.
    /// </summary>
    /// <param name="date">The date to calculate availability for.</param>
    /// <param name="schedule">The staff member's weekly schedule.</param>
    /// <param name="timeOffs">Approved time off that applies to the date.</param>
    /// <param name="blockedTimes">Blocked times that apply to the date.</param>
    /// <returns>A list of available time slots.</returns>
    public IReadOnlyList<TimeSlot> CalculateAvailability(
        DateOnly date,
        WeeklySchedule schedule,
        IReadOnlyList<TimeOff> timeOffs,
        IReadOnlyList<BlockedTime> blockedTimes)
    {
        // Check if staff is working on this day
        var workDay = schedule.GetWorkDayFor(date.DayOfWeek);
        if (workDay is null)
        {
            return Array.Empty<TimeSlot>();
        }

        // Check if staff has approved time off on this date
        if (timeOffs.Any(t => t.ContainsDate(date) && t.Status == TimeOffStatus.Approved))
        {
            return Array.Empty<TimeSlot>();
        }

        // Start with the full work day
        var availableSlots = new List<TimeSlot>
        {
            new(workDay.StartTime, workDay.EndTime)
        };

        // Remove break time if any
        if (workDay.HasBreak)
        {
            availableSlots = SubtractTimeRange(availableSlots, workDay.BreakStart!.Value, workDay.BreakEnd!.Value);
        }

        // Remove blocked times that apply to this date
        foreach (var blockedTime in blockedTimes.Where(bt => bt.AppliesToDate(date)))
        {
            availableSlots = SubtractTimeRange(availableSlots, blockedTime.StartTime, blockedTime.EndTime);
        }

        return availableSlots;
    }

    /// <summary>
    /// Calculates the available time slots for a staff member over a date range.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="schedule">The staff member's weekly schedule.</param>
    /// <param name="timeOffs">Approved time off that overlaps with the range.</param>
    /// <param name="blockedTimes">Blocked times that might apply to the range.</param>
    /// <returns>A dictionary of date to available time slots.</returns>
    public IReadOnlyDictionary<DateOnly, IReadOnlyList<TimeSlot>> CalculateAvailabilityRange(
        DateOnly startDate,
        DateOnly endDate,
        WeeklySchedule schedule,
        IReadOnlyList<TimeOff> timeOffs,
        IReadOnlyList<BlockedTime> blockedTimes)
    {
        var result = new Dictionary<DateOnly, IReadOnlyList<TimeSlot>>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var slots = CalculateAvailability(date, schedule, timeOffs, blockedTimes);
            result[date] = slots;
        }

        return result;
    }

    /// <summary>
    /// Checks if a specific time slot is available.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="startTime">The start time of the slot.</param>
    /// <param name="endTime">The end time of the slot.</param>
    /// <param name="schedule">The staff member's weekly schedule.</param>
    /// <param name="timeOffs">Approved time off.</param>
    /// <param name="blockedTimes">Blocked times.</param>
    /// <returns>True if the time slot is fully available.</returns>
    public bool IsSlotAvailable(
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        WeeklySchedule schedule,
        IReadOnlyList<TimeOff> timeOffs,
        IReadOnlyList<BlockedTime> blockedTimes)
    {
        var availableSlots = CalculateAvailability(date, schedule, timeOffs, blockedTimes);

        // Check if the requested slot fits entirely within any available slot
        return availableSlots.Any(slot =>
            slot.Start <= startTime && slot.End >= endTime);
    }

    private static List<TimeSlot> SubtractTimeRange(List<TimeSlot> slots, TimeOnly start, TimeOnly end)
    {
        var result = new List<TimeSlot>();

        foreach (var slot in slots)
        {
            // No overlap
            if (end <= slot.Start || start >= slot.End)
            {
                result.Add(slot);
                continue;
            }

            // Blocked time completely covers this slot
            if (start <= slot.Start && end >= slot.End)
            {
                continue;
            }

            // Blocked time starts before slot and ends in middle
            if (start <= slot.Start && end < slot.End)
            {
                result.Add(new TimeSlot(end, slot.End));
                continue;
            }

            // Blocked time starts in middle and ends after slot
            if (start > slot.Start && end >= slot.End)
            {
                result.Add(new TimeSlot(slot.Start, start));
                continue;
            }

            // Blocked time is in the middle of the slot
            if (start > slot.Start && end < slot.End)
            {
                result.Add(new TimeSlot(slot.Start, start));
                result.Add(new TimeSlot(end, slot.End));
            }
        }

        return result;
    }
}

/// <summary>
/// Represents a time slot.
/// </summary>
/// <param name="Start">The start time.</param>
/// <param name="End">The end time.</param>
public sealed record TimeSlot(TimeOnly Start, TimeOnly End)
{
    /// <summary>
    /// Gets the duration of this time slot.
    /// </summary>
    public TimeSpan Duration => End - Start;
}
