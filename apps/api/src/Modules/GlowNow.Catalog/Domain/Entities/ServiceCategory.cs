using GlowNow.Catalog.Domain.Errors;
using GlowNow.Catalog.Domain.Events;
using GlowNow.Shared.Domain.Errors;
using GlowNow.Shared.Domain.Primitives;

namespace GlowNow.Catalog.Domain.Entities;

/// <summary>
/// Represents a category for grouping services.
/// </summary>
public sealed class ServiceCategory : AggregateRoot<Guid>, ITenantScoped
{
    private ServiceCategory(
        Guid id,
        Guid businessId,
        string name,
        string? description,
        int displayOrder,
        DateTime createdAtUtc)
        : base(id)
    {
        BusinessId = businessId;
        Name = name;
        Description = description;
        DisplayOrder = displayOrder;
        CreatedAtUtc = createdAtUtc;
        IsDeleted = false;
    }

    private ServiceCategory()
    {
        Name = null!;
    }

    /// <summary>
    /// Gets the business ID this category belongs to.
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// Gets the category name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the category description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the display order for UI sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the category was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets whether the category is soft-deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Creates a new ServiceCategory instance.
    /// </summary>
    public static ServiceCategory Create(
        Guid businessId,
        string name,
        string? description,
        int displayOrder,
        DateTime createdAtUtc)
    {
        var category = new ServiceCategory(
            Guid.NewGuid(),
            businessId,
            name.Trim(),
            description?.Trim(),
            displayOrder,
            createdAtUtc);

        category.RaiseDomainEvent(new ServiceCategoryCreatedEvent(
            category.Id,
            category.BusinessId,
            category.Name));

        return category;
    }

    /// <summary>
    /// Updates the category details.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <param name="displayOrder">The new display order.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Result Update(string name, string? description, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CatalogErrors.InvalidCategoryName);
        }

        Name = name.Trim();
        Description = description?.Trim();
        DisplayOrder = displayOrder;

        RaiseDomainEvent(new ServiceCategoryUpdatedEvent(Id, BusinessId, Name));

        return Result.Success();
    }

    /// <summary>
    /// Soft-deletes the category.
    /// </summary>
    public void Delete()
    {
        IsDeleted = true;
    }

    /// <summary>
    /// Restores a soft-deleted category.
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
    }
}
