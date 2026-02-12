using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.Enums;
using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Team.Application.Queries.GetStaffAvailability;

/// <summary>
/// Query to get a staff member's availability for a date range.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">The start date of the range.</param>
/// <param name="EndDate">The end date of the range.</param>
public sealed record GetStaffAvailabilityQuery(
    Guid StaffProfileId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<StaffAvailabilityResponse>;

/// <summary>
/// Response DTO for staff availability.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="StartDate">The start date of the range.</param>
/// <param name="EndDate">The end date of the range.</param>
/// <param name="Availability">The availability for each date.</param>
public sealed record StaffAvailabilityResponse(
    Guid StaffProfileId,
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyDictionary<string, IReadOnlyList<TimeSlotResponse>> Availability);

/// <summary>
/// Response DTO for a time slot.
/// </summary>
/// <param name="Start">The start time (HH:mm format).</param>
/// <param name="End">The end time (HH:mm format).</param>
public sealed record TimeSlotResponse(string Start, string End);
