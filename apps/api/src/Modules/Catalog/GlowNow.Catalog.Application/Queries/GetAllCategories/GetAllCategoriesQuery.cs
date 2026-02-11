using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Catalog.Application.Queries.GetAllCategories;

/// <summary>
/// Query to get all service categories for a business.
/// </summary>
/// <param name="BusinessId">The business ID.</param>
public sealed record GetAllCategoriesQuery(Guid BusinessId) : IQuery<IReadOnlyList<ServiceCategoryResponse>>;

/// <summary>
/// Response DTO for a service category.
/// </summary>
/// <param name="Id">The category ID.</param>
/// <param name="Name">The category name.</param>
/// <param name="Description">The category description.</param>
/// <param name="DisplayOrder">The display order for sorting.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the category was created.</param>
public sealed record ServiceCategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder,
    DateTime CreatedAtUtc);
