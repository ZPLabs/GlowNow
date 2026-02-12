using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Queries.GetStaffBlockedTimes;

/// <summary>
/// Query to get blocked times for a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">Optional start date filter.</param>
/// <param name="EndDate">Optional end date filter.</param>
public sealed record GetStaffBlockedTimesQuery(
    Guid StaffProfileId,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null) : IQuery<IReadOnlyList<BlockedTimeResponse>>;

/// <summary>
/// Response DTO for a blocked time.
/// </summary>
/// <param name="Id">The blocked time ID.</param>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="Title">The optional title/reason.</param>
/// <param name="StartTime">The start time.</param>
/// <param name="EndTime">The end time.</param>
/// <param name="IsRecurring">Whether this is a recurring blocked time.</param>
/// <param name="DayOfWeek">The day of week for recurring (null for one-time).</param>
/// <param name="SpecificDate">The specific date for one-time (null for recurring).</param>
/// <param name="CreatedAtUtc">When the blocked time was created.</param>
public sealed record BlockedTimeResponse(
    Guid Id,
    Guid StaffProfileId,
    string? Title,
    string StartTime,
    string EndTime,
    bool IsRecurring,
    string? DayOfWeek,
    DateOnly? SpecificDate,
    DateTime CreatedAtUtc);
