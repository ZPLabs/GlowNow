using GlowNow.Shared.Domain.Events;

namespace GlowNow.Team.Domain.Events;

/// <summary>
/// Domain event raised when a service is unassigned from a staff member.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="ServiceId">The service ID.</param>
/// <param name="BusinessId">The business ID.</param>
public sealed record StaffServiceUnassignedEvent(
    Guid StaffProfileId,
    Guid ServiceId,
    Guid BusinessId) : IDomainEvent;
