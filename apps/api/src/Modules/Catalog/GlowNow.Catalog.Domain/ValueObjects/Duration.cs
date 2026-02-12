using GlowNow.Catalog.Domain.Errors;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Catalog.Domain.ValueObjects;

/// <summary>
/// Represents a service duration in minutes.
/// </summary>
public sealed class Duration : ValueObject
{
    public const int MinMinutes = 5;
    public const int MaxMinutes = 480; // 8 hours

    private Duration(int minutes)
    {
        Minutes = minutes;
    }

    private Duration()
    {
        Minutes = 0;
    }

    /// <summary>
    /// Gets the duration in minutes.
    /// </summary>
    public int Minutes { get; private set; }

    /// <summary>
    /// Creates a new Duration instance.
    /// </summary>
    /// <param name="minutes">The duration in minutes.</param>
    /// <returns>A Result containing the Duration or an error.</returns>
    public static Result<Duration> Create(int minutes)
    {
        if (minutes < MinMinutes)
        {
            return Result.Failure<Duration>(CatalogErrors.InvalidDuration(MinMinutes, MaxMinutes));
        }

        if (minutes > MaxMinutes)
        {
            return Result.Failure<Duration>(CatalogErrors.InvalidDuration(MinMinutes, MaxMinutes));
        }

        return new Duration(minutes);
    }

    /// <summary>
    /// Gets the duration as a TimeSpan.
    /// </summary>
    public TimeSpan ToTimeSpan() => TimeSpan.FromMinutes(Minutes);

    public override IEnumerable<object?> GetAtomicValues()
    {
        yield return Minutes;
    }

    public override string ToString() => $"{Minutes} min";

    public static implicit operator int(Duration duration) => duration.Minutes;
}
