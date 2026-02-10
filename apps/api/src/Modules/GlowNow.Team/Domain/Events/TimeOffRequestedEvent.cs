using GlowNow.Shared.Domain.Events;

namespace GlowNow.Team.Domain.Events;

/// <summary>
/// Domain event raised when a staff member requests time off.
/// </summary>
/// <param name="TimeOffId">The time off request ID.</param>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="StartDate">The start date of the time off.</param>
/// <param name="EndDate">The end date of the time off.</param>
/// <param name="Type">The type of time off.</param>
public sealed record TimeOffRequestedEvent(
    Guid TimeOffId,
    Guid StaffProfileId,
    Guid BusinessId,
    DateOnly StartDate,
    DateOnly EndDate,
    TimeOffType Type) : IDomainEvent;
