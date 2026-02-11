using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Queries.GetStaffTimeOff;

/// <summary>
/// Query to get time off requests for a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">Optional start date filter.</param>
/// <param name="EndDate">Optional end date filter.</param>
public sealed record GetStaffTimeOffQuery(
    Guid StaffProfileId,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null) : IQuery<IReadOnlyList<TimeOffResponse>>;

/// <summary>
/// Response DTO for a time off request.
/// </summary>
/// <param name="Id">The time off request ID.</param>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">The start date.</param>
/// <param name="EndDate">The end date.</param>
/// <param name="Type">The type of time off.</param>
/// <param name="Notes">Any notes for the request.</param>
/// <param name="Status">The status of the request.</param>
/// <param name="RequestedAtUtc">When the request was made.</param>
/// <param name="ApprovedAtUtc">When the request was approved (if applicable).</param>
/// <param name="ApprovedByUserId">Who approved the request (if applicable).</param>
/// <param name="RejectionReason">The rejection reason (if applicable).</param>
/// <param name="DaysCount">The number of days in the time off period.</param>
public sealed record TimeOffResponse(
    Guid Id,
    Guid StaffProfileId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Type,
    string? Notes,
    string Status,
    DateTime RequestedAtUtc,
    DateTime? ApprovedAtUtc,
    Guid? ApprovedByUserId,
    string? RejectionReason,
    int DaysCount);
