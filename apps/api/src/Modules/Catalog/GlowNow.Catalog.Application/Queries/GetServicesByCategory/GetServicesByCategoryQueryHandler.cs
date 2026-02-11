using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Queries.GetServicesByCategory;

/// <summary>
/// Handler for the GetServicesByCategoryQuery.
/// </summary>
internal sealed class GetServicesByCategoryQueryHandler
    : IQueryHandler<GetServicesByCategoryQuery, IReadOnlyList<ServiceResponse>>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceCategoryRepository _categoryRepository;

    public GetServicesByCategoryQueryHandler(
        IServiceRepository serviceRepository,
        IServiceCategoryRepository categoryRepository)
    {
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IReadOnlyList<ServiceResponse>>> Handle(
        GetServicesByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(query.CategoryId, cancellationToken);

        if (category is null)
        {
            return Result.Failure<IReadOnlyList<ServiceResponse>>(CatalogErrors.CategoryNotFound);
        }

        var services = await _serviceRepository.GetByCategoryIdAsync(query.CategoryId, cancellationToken);

        var response = services
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Select(s => new ServiceResponse(
                s.Id,
                s.CategoryId,
                s.Name,
                s.Description,
                s.Duration.Minutes,
                s.Price.Amount,
                s.BufferTimeMinutes,
                s.IsActive,
                s.DisplayOrder,
                s.CreatedAtUtc))
            .ToList();

        return response;
    }
}
