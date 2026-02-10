using System.Text.Json;

namespace GlowNow.Team.Domain.ValueObjects;

/// <summary>
/// Represents a staff member's weekly working schedule.
/// </summary>
public sealed class WeeklySchedule : ValueObject
{
    private readonly Dictionary<DayOfWeek, WorkDay?> _schedule;

    private WeeklySchedule(Dictionary<DayOfWeek, WorkDay?> schedule)
    {
        _schedule = schedule;
    }

    private WeeklySchedule()
    {
        _schedule = new Dictionary<DayOfWeek, WorkDay?>();
    }

    /// <summary>
    /// Creates a new WeeklySchedule instance with all days off.
    /// </summary>
    /// <returns>A WeeklySchedule with all days marked as off.</returns>
    public static WeeklySchedule CreateEmpty()
    {
        var schedule = new Dictionary<DayOfWeek, WorkDay?>
        {
            { DayOfWeek.Sunday, null },
            { DayOfWeek.Monday, null },
            { DayOfWeek.Tuesday, null },
            { DayOfWeek.Wednesday, null },
            { DayOfWeek.Thursday, null },
            { DayOfWeek.Friday, null },
            { DayOfWeek.Saturday, null }
        };

        return new WeeklySchedule(schedule);
    }

    /// <summary>
    /// Creates a new WeeklySchedule instance from a dictionary of day to work day.
    /// </summary>
    /// <param name="schedule">The schedule dictionary.</param>
    /// <returns>A Result containing the WeeklySchedule or an error.</returns>
    public static Result<WeeklySchedule> Create(Dictionary<DayOfWeek, WorkDay?> schedule)
    {
        if (schedule is null)
        {
            return Result.Failure<WeeklySchedule>(TeamErrors.InvalidWeeklySchedule);
        }

        // Ensure all days are present
        var normalizedSchedule = new Dictionary<DayOfWeek, WorkDay?>();
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            normalizedSchedule[day] = schedule.GetValueOrDefault(day);
        }

        return new WeeklySchedule(normalizedSchedule);
    }

    /// <summary>
    /// Gets the work day for a specific day.
    /// </summary>
    /// <param name="day">The day of the week.</param>
    /// <returns>The WorkDay for that day, or null if not working.</returns>
    public WorkDay? GetWorkDayFor(DayOfWeek day)
    {
        return _schedule.GetValueOrDefault(day);
    }

    /// <summary>
    /// Checks if the staff member is working on a specific day.
    /// </summary>
    /// <param name="day">The day of the week.</param>
    /// <returns>True if working, false if off.</returns>
    public bool IsWorkingOn(DayOfWeek day)
    {
        return _schedule.TryGetValue(day, out var workDay) && workDay is not null;
    }

    /// <summary>
    /// Checks if the staff member is working at a specific date and time.
    /// </summary>
    /// <param name="dateTime">The date and time to check.</param>
    /// <returns>True if working, false if off or on break.</returns>
    public bool IsWorkingAt(DateTime dateTime)
    {
        var day = dateTime.DayOfWeek;
        var time = TimeOnly.FromDateTime(dateTime);

        if (!_schedule.TryGetValue(day, out var workDay) || workDay is null)
        {
            return false;
        }

        return workDay.IsWorkingTime(time);
    }

    /// <summary>
    /// Gets the number of work days per week.
    /// </summary>
    public int WorkDaysPerWeek => _schedule.Count(kvp => kvp.Value is not null);

    /// <summary>
    /// Returns the schedule as a read-only dictionary.
    /// </summary>
    public IReadOnlyDictionary<DayOfWeek, WorkDay?> Schedule => _schedule.AsReadOnly();

    /// <summary>
    /// Serializes the weekly schedule to a JSON string for database storage.
    /// </summary>
    public string ToJson()
    {
        var serializableSchedule = _schedule.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => kvp.Value is null
                ? null
                : new
                {
                    Start = kvp.Value.StartTime.ToString("HH:mm"),
                    End = kvp.Value.EndTime.ToString("HH:mm"),
                    BreakStart = kvp.Value.BreakStart?.ToString("HH:mm"),
                    BreakEnd = kvp.Value.BreakEnd?.ToString("HH:mm")
                });

        return JsonSerializer.Serialize(serializableSchedule);
    }

    /// <summary>
    /// Deserializes a weekly schedule from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>A Result containing the WeeklySchedule or an error.</returns>
    public static Result<WeeklySchedule> FromJson(string json)
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

            var schedule = new Dictionary<DayOfWeek, WorkDay?>();
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
                if (element.TryGetProperty("Start", out var startProp) &&
                    element.TryGetProperty("End", out var endProp))
                {
                    var startStr = startProp.GetString();
                    var endStr = endProp.GetString();

                    if (startStr is not null && endStr is not null)
                    {
                        // Check for break times
                        string? breakStartStr = null;
                        string? breakEndStr = null;

                        if (element.TryGetProperty("BreakStart", out var breakStartProp) &&
                            breakStartProp.ValueKind != JsonValueKind.Null)
                        {
                            breakStartStr = breakStartProp.GetString();
                        }

                        if (element.TryGetProperty("BreakEnd", out var breakEndProp) &&
                            breakEndProp.ValueKind != JsonValueKind.Null)
                        {
                            breakEndStr = breakEndProp.GetString();
                        }

                        Result<WorkDay> workDayResult;
                        if (breakStartStr is not null && breakEndStr is not null)
                        {
                            workDayResult = WorkDay.Create(startStr, endStr, breakStartStr, breakEndStr);
                        }
                        else
                        {
                            workDayResult = WorkDay.Create(startStr, endStr);
                        }

                        if (workDayResult.IsSuccess)
                        {
                            schedule[day] = workDayResult.Value;
                        }
                    }
                }
            }

            return Create(schedule);
        }
        catch (JsonException)
        {
            return Result.Failure<WeeklySchedule>(TeamErrors.InvalidWeeklySchedule);
        }
    }

    public override IEnumerable<object?> GetAtomicValues()
    {
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            yield return day;
            var workDay = _schedule.GetValueOrDefault(day);
            if (workDay is not null)
            {
                yield return workDay.StartTime;
                yield return workDay.EndTime;
                yield return workDay.BreakStart;
                yield return workDay.BreakEnd;
            }
            else
            {
                yield return null;
                yield return null;
                yield return null;
                yield return null;
            }
        }
    }
}
