using GlowNow.SharedKernel.Domain.Events;

namespace GlowNow.Catalog.Domain.Events;

/// <summary>
/// Domain event raised when a new service category is created.
/// </summary>
/// <param name="CategoryId">The category ID.</param>
/// <param name="BusinessId">The business ID.</param>
/// <param name="Name">The category name.</param>
public sealed record ServiceCategoryCreatedEvent(
    Guid CategoryId,
    Guid BusinessId,
    string Name) : IDomainEvent;
