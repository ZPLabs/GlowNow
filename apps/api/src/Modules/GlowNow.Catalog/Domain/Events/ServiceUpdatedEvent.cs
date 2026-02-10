using GlowNow.Shared.Domain.Events;

namespace GlowNow.Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a service is updated.
/// </summary>
/// <param name="ServiceId">The service ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The updated service name.</param>
/// <param name="Price">The updated service price.</param>
/// <param name="DurationMinutes">The updated service duration in minutes.</param>
public sealed record ServiceUpdatedEvent(
    Guid ServiceId,
    Guid BusinessId,
    string Name,
    decimal Price,
    int DurationMinutes) : IDomainEvent;
