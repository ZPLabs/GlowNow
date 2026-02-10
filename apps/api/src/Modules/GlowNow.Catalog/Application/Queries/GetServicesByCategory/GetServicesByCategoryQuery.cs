using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Shared.Application.Messaging;

namespace GlowNow.Catalog.Application.Queries.GetServicesByCategory;

/// <summary>
/// Query to get all services in a specific category.
/// </summary>
/// <param name="CategoryId">The category ID.</param>
public sealed record GetServicesByCategoryQuery(Guid CategoryId) : IQuery<IReadOnlyList<ServiceResponse>>;
