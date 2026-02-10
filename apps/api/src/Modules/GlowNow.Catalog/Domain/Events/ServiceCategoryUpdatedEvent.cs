using GlowNow.Shared.Domain.Events;

namespace GlowNow.Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a service category is updated.
/// </summary>
/// <param name="CategoryId">The category ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The updated category name.</param>
public sealed record ServiceCategoryUpdatedEvent(
    Guid CategoryId,
    Guid BusinessId,
    string Name) : IDomainEvent;
