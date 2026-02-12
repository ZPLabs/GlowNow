using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Catalog.Application.Queries.GetAllServices;

/// <summary>
/// Query to get all services for a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
public sealed record GetAllServicesQuery(Guid BusinessId) : IQuery<IReadOnlyList<ServiceResponse>>;

/// <summary>
/// Response DTO for a service.
/// </summary>
/// <param name="Id">The service ID.</param>
/// <param name="CategoryId">The optional category ID.</param>
/// <param name="Name">The service name.</param>
/// <param name="Description">The service description.</param>
/// <param name="DurationMinutes">The duration in minutes.</param>
/// <param name="Price">The price in USD.</param>
/// <param name="BufferTimeMinutes">The buffer time after service in minutes.</param>
/// <param name="IsActive">Whether the service is currently active.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the service was created.</param>
public sealed record ServiceResponse(
    Guid Id,
    Guid? CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    int BufferTimeMinutes,
    bool IsActive,
    int DisplayOrder,
    DateTime CreatedAtUtc);
