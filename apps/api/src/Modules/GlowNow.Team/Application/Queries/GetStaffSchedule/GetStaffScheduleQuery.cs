using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Team.Application.Queries.GetStaffSchedule;

/// <summary>
/// Query to get a staff member's weekly schedule.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
public sealed record GetStaffScheduleQuery(Guid StaffProfileId) : IQuery<StaffScheduleResponse>;

/// <summary>
/// Response DTO for a staff schedule.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="Schedule">The weekly schedule.</param>
public sealed record StaffScheduleResponse(
    Guid StaffProfileId,
    IReadOnlyDictionary<string, WorkDayResponse?> Schedule);

/// <summary>
/// Response DTO for a work day.
/// </summary>
/// <param name="StartTime">The start time (HH:mm format).</param>
/// <param name="EndTime">The end time (HH:mm format).</param>
/// <param name="BreakStart">The optional break start time (HH:mm format).</param>
/// <param name="BreakEnd">The optional break end time (HH:mm format).</param>
public sealed record WorkDayResponse(
    string StartTime,
    string EndTime,
    string? BreakStart,
    string? BreakEnd);
