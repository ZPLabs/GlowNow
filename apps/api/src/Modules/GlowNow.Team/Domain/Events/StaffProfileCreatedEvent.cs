using GlowNow.Shared.Domain.Events;

namespace GlowNow.Team.Domain.Events;

/// <summary>
/// Domain event raised when a new staff profile is created.
/// </summary>
/// <param name="StaffProfileId">The staff profile ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="UserId">The user ID.</param>
/// <param name="Title">The staff title.</param>
public sealed record StaffProfileCreatedEvent(
    Guid StaffProfileId,
    Guid BusinessId,
    Guid UserId,
    string Title) : IDomainEvent;
