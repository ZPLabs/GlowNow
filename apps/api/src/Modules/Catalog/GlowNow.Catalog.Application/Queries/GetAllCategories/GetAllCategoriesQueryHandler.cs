using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Queries.GetAllCategories;

/// <summary>
/// Handler for the GetAllCategoriesQuery.
/// </summary>
internal sealed class GetAllCategoriesQueryHandler
    : IQueryHandler<GetAllCategoriesQuery, IReadOnlyList<ServiceCategoryResponse>>
{
    private readonly IServiceCategoryRepository _categoryRepository;

    public GetAllCategoriesQueryHandler(IServiceCategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IReadOnlyList<ServiceCategoryResponse>>> Handle(
        GetAllCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllByBusinessIdAsync(query.BusinessId, cancellationToken);

        var response = categories
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .Select(c => new ServiceCategoryResponse(
                c.Id,
                c.Name,
                c.Description,
                c.DisplayOrder,
                c.CreatedAtUtc))
            .ToList();

        return response;
    }
}
