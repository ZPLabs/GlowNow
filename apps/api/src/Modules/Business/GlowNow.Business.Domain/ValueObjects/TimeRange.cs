using GlowNow.Business.Domain.Errors;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Business.Domain.ValueObjects;

/// <summary>
/// Represents a time range with opening and closing times for a business day.
/// </summary>
public sealed class TimeRange : ValueObject
{
    private TimeRange(TimeOnly openTime, TimeOnly closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }

    private TimeRange()
    {
        OpenTime = default;
        CloseTime = default;
    }

    /// <summary>
    /// Gets the opening time.
    /// </summary>
    public TimeOnly OpenTime { get; private set; }

    /// <summary>
    /// Gets the closing time.
    /// </summary>
    public TimeOnly CloseTime { get; private set; }

    /// <summary>
    /// Creates a new TimeRange instance after validating the times.
    /// </summary>
    /// <param name="openTime">The opening time.</param>
    /// <param name="closeTime">The closing time.</param>
    /// <returns>A Result containing the TimeRange or an error.</returns>
    public static Result<TimeRange> Create(TimeOnly openTime, TimeOnly closeTime)
    {
        if (closeTime <= openTime)
        {
            return Result.Failure<TimeRange>(BusinessErrors.InvalidTimeRange);
        }

        return new TimeRange(openTime, closeTime);
    }

    /// <summary>
    /// Creates a new TimeRange instance from string representations.
    /// </summary>
    /// <param name="openTime">The opening time as a string (HH:mm format).</param>
    /// <param name="closeTime">The closing time as a string (HH:mm format).</param>
    /// <returns>A Result containing the TimeRange or an error.</returns>
    public static Result<TimeRange> Create(string openTime, string closeTime)
    {
        if (!TimeOnly.TryParse(openTime, out var parsedOpenTime))
        {
            return Result.Failure<TimeRange>(BusinessErrors.InvalidTimeRange);
        }

        if (!TimeOnly.TryParse(closeTime, out var parsedCloseTime))
        {
            return Result.Failure<TimeRange>(BusinessErrors.InvalidTimeRange);
        }

        return Create(parsedOpenTime, parsedCloseTime);
    }

    /// <summary>
    /// Gets the duration of the time range.
    /// </summary>
    public TimeSpan Duration => CloseTime - OpenTime;

    /// <summary>
    /// Checks if a specific time falls within this time range.
    /// </summary>
    /// <param name="time">The time to check.</param>
    /// <returns>True if the time is within the range, false otherwise.</returns>
    public bool Contains(TimeOnly time)
    {
        return time >= OpenTime && time < CloseTime;
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return OpenTime;
        yield return CloseTime;
    }

    public override string ToString() => $"{OpenTime:HH:mm}-{CloseTime:HH:mm}";
}
