using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;
using GlowNow.Team.Domain.Errors;

namespace GlowNow.Team.Domain.ValueObjects;

/// <summary>
/// Represents a single work day schedule with optional break time.
/// </summary>
public sealed class WorkDay : ValueObject
{
    private WorkDay(TimeOnly startTime, TimeOnly endTime, TimeOnly? breakStart, TimeOnly? breakEnd)
    {
        StartTime = startTime;
        EndTime = endTime;
        BreakStart = breakStart;
        BreakEnd = breakEnd;
    }

    private WorkDay()
    {
        StartTime = default;
        EndTime = default;
    }

    /// <summary>
    /// Gets the work day start time.
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// Gets the work day end time.
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// Gets the optional break start time.
    /// </summary>
    public TimeOnly? BreakStart { get; private set; }

    /// <summary>
    /// Gets the optional break end time.
    /// </summary>
    public TimeOnly? BreakEnd { get; private set; }

    /// <summary>
    /// Creates a new WorkDay without a break.
    /// </summary>
    /// <param name="startTime">The work day start time.</param>
    /// <param name="endTime">The work day end time.</param>
    /// <returns>A Result containing the WorkDay or an error.</returns>
    public static Result<WorkDay> Create(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidWorkDayTimes);
        }

        return new WorkDay(startTime, endTime, null, null);
    }

    /// <summary>
    /// Creates a new WorkDay with a break.
    /// </summary>
    /// <param name="startTime">The work day start time.</param>
    /// <param name="endTime">The work day end time.</param>
    /// <param name="breakStart">The break start time.</param>
    /// <param name="breakEnd">The break end time.</param>
    /// <returns>A Result containing the WorkDay or an error.</returns>
    public static Result<WorkDay> Create(
        TimeOnly startTime,
        TimeOnly endTime,
        TimeOnly breakStart,
        TimeOnly breakEnd)
    {
        if (endTime <= startTime)
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidWorkDayTimes);
        }

        if (breakEnd <= breakStart)
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidBreakTimes);
        }

        if (breakStart < startTime || breakEnd > endTime)
        {
            return Result.Failure<WorkDay>(TeamErrors.BreakOutsideWorkHours);
        }

        return new WorkDay(startTime, endTime, breakStart, breakEnd);
    }

    /// <summary>
    /// Creates a new WorkDay from string representations.
    /// </summary>
    /// <param name="startTime">The start time string (HH:mm format).</param>
    /// <param name="endTime">The end time string (HH:mm format).</param>
    /// <returns>A Result containing the WorkDay or an error.</returns>
    public static Result<WorkDay> Create(string startTime, string endTime)
    {
        if (!TimeOnly.TryParse(startTime, out var parsedStartTime))
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidWorkDayTimes);
        }

        if (!TimeOnly.TryParse(endTime, out var parsedEndTime))
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidWorkDayTimes);
        }

        return Create(parsedStartTime, parsedEndTime);
    }

    /// <summary>
    /// Creates a new WorkDay from string representations with a break.
    /// </summary>
    /// <param name="startTime">The start time string (HH:mm format).</param>
    /// <param name="endTime">The end time string (HH:mm format).</param>
    /// <param name="breakStart">The break start time string (HH:mm format).</param>
    /// <param name="breakEnd">The break end time string (HH:mm format).</param>
    /// <returns>A Result containing the WorkDay or an error.</returns>
    public static Result<WorkDay> Create(
        string startTime,
        string endTime,
        string breakStart,
        string breakEnd)
    {
        if (!TimeOnly.TryParse(startTime, out var parsedStartTime) ||
            !TimeOnly.TryParse(endTime, out var parsedEndTime) ||
            !TimeOnly.TryParse(breakStart, out var parsedBreakStart) ||
            !TimeOnly.TryParse(breakEnd, out var parsedBreakEnd))
        {
            return Result.Failure<WorkDay>(TeamErrors.InvalidWorkDayTimes);
        }

        return Create(parsedStartTime, parsedEndTime, parsedBreakStart, parsedBreakEnd);
    }

    /// <summary>
    /// Gets whether this work day has a break.
    /// </summary>
    public bool HasBreak => BreakStart.HasValue && BreakEnd.HasValue;

    /// <summary>
    /// Gets the total work duration excluding break time.
    /// </summary>
    public TimeSpan WorkDuration
    {
        get
        {
            var total = EndTime - StartTime;
            if (HasBreak)
            {
                total -= (BreakEnd!.Value - BreakStart!.Value);
            }
            return total;
        }
    }

    /// <summary>
    /// Checks if a specific time falls within working hours (excluding break).
    /// </summary>
    /// <param name="time">The time to check.</param>
    /// <returns>True if the time is within working hours, false otherwise.</returns>
    public bool IsWorkingTime(TimeOnly time)
    {
        if (time < StartTime || time >= EndTime)
        {
            return false;
        }

        if (HasBreak && time >= BreakStart!.Value && time < BreakEnd!.Value)
        {
            return false;
        }

        return true;
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return StartTime;
        yield return EndTime;
        yield return BreakStart;
        yield return BreakEnd;
    }

    public override string ToString()
    {
        var basic = $"{StartTime:HH:mm}-{EndTime:HH:mm}";
        if (HasBreak)
        {
            basic += $" (break {BreakStart:HH:mm}-{BreakEnd:HH:mm})";
        }
        return basic;
    }
}
