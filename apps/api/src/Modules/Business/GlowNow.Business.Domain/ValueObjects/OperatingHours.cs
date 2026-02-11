using System.Text.Json;
using GlowNow.Business.Domain.Errors;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Business.Domain.ValueObjects;

/// <summary>
/// Represents the weekly operating hours for a business.
/// </summary>
public sealed class OperatingHours : ValueObject
{
    private readonly Dictionary<DayOfWeek, TimeRange?> _schedule;

    private OperatingHours(Dictionary<DayOfWeek, TimeRange?> schedule)
    {
        _schedule = schedule;
    }

    private OperatingHours()
    {
        _schedule = new Dictionary<DayOfWeek, TimeRange?>();
    }

    /// <summary>
    /// Creates a new OperatingHours instance with default closed days.
    /// </summary>
    /// <returns>An OperatingHours with all days closed.</returns>
    public static OperatingHours CreateEmpty()
    {
        var schedule = new Dictionary<DayOfWeek, TimeRange?>
        {
            { DayOfWeek.Sunday, null },
            { DayOfWeek.Monday, null },
            { DayOfWeek.Tuesday, null },
            { DayOfWeek.Wednesday, null },
            { DayOfWeek.Thursday, null },
            { DayOfWeek.Friday, null },
            { DayOfWeek.Saturday, null }
        };

        return new OperatingHours(schedule);
    }

    /// <summary>
    /// Creates a new OperatingHours instance from a dictionary of day to time range.
    /// </summary>
    /// <param name="schedule">The schedule dictionary.</param>
    /// <returns>A Result containing the OperatingHours or an error.</returns>
    public static Result<OperatingHours> Create(Dictionary<DayOfWeek, TimeRange?> schedule)
    {
        if (schedule is null)
        {
            return Result.Failure<OperatingHours>(BusinessErrors.InvalidOperatingHours);
        }

        // Ensure all days are present
        var normalizedSchedule = new Dictionary<DayOfWeek, TimeRange?>();
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            normalizedSchedule[day] = schedule.GetValueOrDefault(day);
        }

        return new OperatingHours(normalizedSchedule);
    }

    /// <summary>
    /// Gets the time range for a specific day.
    /// </summary>
    /// <param name="day">The day of the week.</param>
    /// <returns>The time range for that day, or null if closed.</returns>
    public TimeRange? GetHoursForDay(DayOfWeek day)
    {
        return _schedule.GetValueOrDefault(day);
    }

    /// <summary>
    /// Checks if the business is open on a specific day.
    /// </summary>
    /// <param name="day">The day of the week.</param>
    /// <returns>True if open, false if closed.</returns>
    public bool IsOpenOn(DayOfWeek day)
    {
        return _schedule.TryGetValue(day, out var timeRange) && timeRange is not null;
    }

    /// <summary>
    /// Checks if the business is open at a specific date and time.
    /// </summary>
    /// <param name="dateTime">The date and time to check.</param>
    /// <returns>True if open, false if closed.</returns>
    public bool IsOpenAt(DateTime dateTime)
    {
        var day = dateTime.DayOfWeek;
        var time = TimeOnly.FromDateTime(dateTime);

        if (!_schedule.TryGetValue(day, out var timeRange) || timeRange is null)
        {
            return false;
        }

        return timeRange.Contains(time);
    }

    /// <summary>
    /// Gets the number of days the business is open per week.
    /// </summary>
    public int OpenDaysPerWeek => _schedule.Count(kvp => kvp.Value is not null);

    /// <summary>
    /// Returns the schedule as a read-only dictionary.
    /// </summary>
    public IReadOnlyDictionary<DayOfWeek, TimeRange?> Schedule => _schedule.AsReadOnly();

    /// <summary>
    /// Serializes the operating hours to a JSON string for database storage.
    /// </summary>
    public string ToJson()
    {
        var serializableSchedule = _schedule.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value is null
                ? null
                : new { Open = kvp.Value.OpenTime.ToString("HH:mm"), Close = kvp.Value.CloseTime.ToString("HH:mm") });

        return JsonSerializer.Serialize(serializableSchedule);
    }

    /// <summary>
    /// Deserializes operating hours from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>A Result containing the OperatingHours or an error.</returns>
    public static Result<OperatingHours> FromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return CreateEmpty();
        }

        try
        {
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement?>>(json);
            if (parsed is null)
            {
                return CreateEmpty();
            }

            var schedule = new Dictionary<DayOfWeek, TimeRange?>();
            foreach (var kvp in parsed)
            {
                if (!Enum.TryParse<DayOfWeek>(kvp.Key, out var day))
                {
                    continue;
                }

                if (!kvp.Value.HasValue || kvp.Value.Value.ValueKind == JsonValueKind.Null)
                {
                    schedule[day] = null;
                    continue;
                }

                var element = kvp.Value.Value;
                if (element.TryGetProperty("Open", out var openProp) &&
                    element.TryGetProperty("Close", out var closeProp))
                {
                    var openStr = openProp.GetString();
                    var closeStr = closeProp.GetString();

                    if (openStr is not null && closeStr is not null)
                    {
                        var timeRangeResult = TimeRange.Create(openStr, closeStr);
                        if (timeRangeResult.IsSuccess)
                        {
                            schedule[day] = timeRangeResult.Value;
                        }
                    }
                }
            }

            return Create(schedule);
        }
        catch (JsonException)
        {
            return Result.Failure<OperatingHours>(BusinessErrors.InvalidOperatingHours);
        }
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            yield return day;
            var timeRange = _schedule.GetValueOrDefault(day);
            yield return timeRange?.OpenTime;
            yield return timeRange?.CloseTime;
        }
    }
}
