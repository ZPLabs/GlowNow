using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Application.Queries.GetAllServices;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Shared.Domain.Errors;

namespace GlowNow.Catalog.Application.Queries.GetService;

/// <summary>
/// Handler for the GetServiceQuery.
/// </summary>
internal sealed class GetServiceQueryHandler : IQueryHandler<GetServiceQuery, ServiceResponse>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<Result<ServiceResponse>> Handle(
        GetServiceQuery query,
        CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(query.Id, cancellationToken);

        if (service is null)
        {
            return Result.Failure<ServiceResponse>(CatalogErrors.ServiceNotFound);
        }

        return new ServiceResponse(
            service.Id,
            service.CategoryId,
            service.Name,
            service.Description,
            service.Duration.Minutes,
            service.Price.Amount,
            service.BufferTimeMinutes,
            service.IsActive,
            service.DisplayOrder,
            service.CreatedAtUtc);
    }
}
