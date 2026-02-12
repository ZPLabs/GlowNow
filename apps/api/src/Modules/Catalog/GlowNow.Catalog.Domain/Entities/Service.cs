using GlowNow.Catalog.Domain.Errors;
using GlowNow.Catalog.Domain.Events;
using GlowNow.Catalog.Domain.ValueObjects;
using GlowNow.SharedKernel.Domain.Errors;
using GlowNow.SharedKernel.Domain.Primitives;

namespace GlowNow.Catalog.Domain.Entities;

/// <summary>
/// Represents a service offered by a business.
/// </summary>
public sealed class Service : AggregateRoot<Guid>, ITenantScoped
{
    private Service(
        Guid id,
        Guid businessId,
        Guid? categoryId,
        string name,
        string? description,
        Duration duration,
        Money price,
        int bufferTimeMinutes,
        bool isActive,
        int displayOrder,
        DateTime createdAtUtc)
        : base(id)
    {
        BusinessId = businessId;
        CategoryId = categoryId;
        Name = name;
        Description = description;
        Duration = duration;
        Price = price;
        BufferTimeMinutes = bufferTimeMinutes;
        IsActive = isActive;
        DisplayOrder = displayOrder;
        CreatedAtUtc = createdAtUtc;
        IsDeleted = false;
    }

    private Service()
    {
        Name = null!;
        Duration = null!;
        Price = null!;
    }

    /// <summary>
    /// Gets the business ID this service belongs to.
    /// </summary>
    public Guid BusinessId { get; private set; }

    /// <summary>
    /// Gets the category ID (optional).
    /// </summary>
    public Guid? CategoryId { get; private set; }

    /// <summary>
    /// Gets the service name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the service description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the service duration.
    /// </summary>
    public Duration Duration { get; private set; }

    /// <summary>
    /// Gets the service price.
    /// </summary>
    public Money Price { get; private set; }

    /// <summary>
    /// Gets the buffer time in minutes after the service.
    /// </summary>
    public int BufferTimeMinutes { get; private set; }

    /// <summary>
    /// Gets whether the service is currently active and bookable.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets the display order for UI sorting.
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// Gets the UTC timestamp when the service was created.
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets whether the service is soft-deleted.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Creates a new Service instance.
    /// </summary>
    public static Result<Service> Create(
        Guid businessId,
        Guid? categoryId,
        string name,
        string? description,
        Duration duration,
        Money price,
        int bufferTimeMinutes,
        int displayOrder,
        DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Service>(CatalogErrors.InvalidServiceName);
        }

        if (bufferTimeMinutes < 0)
        {
            bufferTimeMinutes = 0;
        }

        var service = new Service(
            Guid.NewGuid(),
            businessId,
            categoryId,
            name.Trim(),
            description?.Trim(),
            duration,
            price,
            bufferTimeMinutes,
            isActive: true,
            displayOrder,
            createdAtUtc);

        service.RaiseDomainEvent(new ServiceCreatedEvent(
            service.Id,
            service.BusinessId,
            service.Name,
            service.Price.Amount,
            service.Duration.Minutes));

        return service;
    }

    /// <summary>
    /// Updates the service details.
    /// </summary>
    public Result Update(
        Guid? categoryId,
        string name,
        string? description,
        Duration duration,
        Money price,
        int bufferTimeMinutes,
        int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(CatalogErrors.InvalidServiceName);
        }

        CategoryId = categoryId;
        Name = name.Trim();
        Description = description?.Trim();
        Duration = duration;
        Price = price;
        BufferTimeMinutes = bufferTimeMinutes < 0 ? 0 : bufferTimeMinutes;
        DisplayOrder = displayOrder;

        RaiseDomainEvent(new ServiceUpdatedEvent(
            Id,
            BusinessId,
            Name,
            Price.Amount,
            Duration.Minutes));

        return Result.Success();
    }

    /// <summary>
    /// Activates the service for booking.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the service.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Soft-deletes the service.
    /// </summary>
    public Result Delete()
    {
        if (IsActive)
        {
            return Result.Failure(CatalogErrors.ServiceIsActive);
        }

        IsDeleted = true;
        return Result.Success();
    }

    /// <summary>
    /// Gets the total time including buffer.
    /// </summary>
    public TimeSpan TotalDuration => Duration.ToTimeSpan() + TimeSpan.FromMinutes(BufferTimeMinutes);
}
