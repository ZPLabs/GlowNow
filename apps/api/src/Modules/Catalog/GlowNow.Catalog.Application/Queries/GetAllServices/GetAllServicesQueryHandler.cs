using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Queries.GetAllServices;

/// <summary>
/// Handler for the GetAllServicesQuery.
/// </summary>
internal sealed class GetAllServicesQueryHandler
    : IQueryHandler<GetAllServicesQuery, IReadOnlyList<ServiceResponse>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Result<IReadOnlyList<ServiceResponse>>> Handle(
        GetAllServicesQuery query,
        CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetAllByBusinessIdAsync(query.BusinessId, cancellationToken);

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
