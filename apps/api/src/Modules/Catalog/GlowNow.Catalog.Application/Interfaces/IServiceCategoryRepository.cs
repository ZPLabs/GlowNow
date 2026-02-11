using GlowNow.Catalog.Domain.Entities;

namespace GlowNow.Catalog.Application.Interfaces;

/// <summary>
/// Repository interface for ServiceCategory aggregate operations.
/// </summary>
public interface IServiceCategoryRepository
{
    /// <summary>
    /// Gets a category by its unique identifier.
    /// </summary>
    Task<ServiceCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories for a business.
    /// </summary>
    Task<IReadOnlyList<ServiceCategory>> GetAllByBusinessIdAsync(Guid businessId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the given name exists for a business.
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid businessId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category with the given name exists for a business, excluding a specific category.
    /// </summary>
    Task<bool> ExistsByNameAsync(Guid businessId, string name, Guid excludeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a category has any services.
    /// </summary>
    Task<bool> HasServicesAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new category to the repository.
    /// </summary>
    void Add(ServiceCategory category);

    /// <summary>
    /// Updates an existing category in the repository.
    /// </summary>
    void Update(ServiceCategory category);

    /// <summary>
    /// Removes a category from the repository.
    /// </summary>
    void Remove(ServiceCategory category);
}
