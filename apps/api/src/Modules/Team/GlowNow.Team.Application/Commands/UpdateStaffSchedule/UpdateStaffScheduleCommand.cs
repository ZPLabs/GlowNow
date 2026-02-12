using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Commands.UpdateStaffSchedule;

/// <summary>
/// Command to update a staff member's weekly schedule.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="Schedule">The schedule data for each day of the week.</param>
public sealed record UpdateStaffScheduleCommand(
    Guid StaffProfileId,
    Dictionary<DayOfWeek, WorkDayInput?> Schedule) : ICommand;

/// <summary>
/// Input DTO for a single work day.
/// </summary>
/// <param name="StartTime">The start time (HH:mm format).</param>
/// <param name="EndTime">The end time (HH:mm format).</param>
/// <param name="BreakStart">The optional break start time (HH:mm format).</param>
/// <param name="BreakEnd">The optional break end time (HH:mm format).</param>
public sealed record WorkDayInput(
    string StartTime,
    string EndTime,
    string? BreakStart,
    string? BreakEnd);
