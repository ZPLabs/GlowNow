using GlowNow.SharedKernel.Domain.Events;

namespace GlowNow.Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a new service is created.
/// </summary>
/// <param name="ServiceId">The service ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The service name.</param>
/// <param name="Price">The service price.</param>
/// <param name="DurationMinutes">The service duration in minutes.</param>
public sealed record ServiceCreatedEvent(
    Guid ServiceId,
    Guid BusinessId,
    string Name,
    decimal Price,
    int DurationMinutes) : IDomainEvent;
