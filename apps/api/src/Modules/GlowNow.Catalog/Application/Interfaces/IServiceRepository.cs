using GlowNow.Catalog.Domain.Entities;

namespace GlowNow.Catalog.Application.Interfaces;

/// <summary>
/// Repository interface for Service aggregate operations.
/// </summary>
public interface IServiceRepository
{
    /// <summary>
    /// Gets a service by its unique identifier.
    /// </summary>
    Task<Service?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all services for a business.
    /// </summary>
    Task<IReadOnlyList<Service>> GetAllByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all services for a specific category.
    /// </summary>
    Task<IReadOnlyList<Service>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a service with the given name exists for a business.
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid businessId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a service with the given name exists for a business, excluding a specific service.
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid businessId, string name, Guid excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new service to the repository.
    /// </summary>
    void Add(Service service);

    /// <summary>
    /// Updates an existing service in the repository.
    /// </summary>
    void Update(Service service);

    /// <summary>
    /// Removes a service from the repository.
    /// </summary>
    void Remove(Service service);
}
