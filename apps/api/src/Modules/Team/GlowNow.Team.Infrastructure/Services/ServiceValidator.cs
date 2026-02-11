using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Team.Application.Interfaces;

namespace GlowNow.Team.Infrastructure.Services;

/// <summary>
/// Implementation of IServiceValidator using the Catalog module.
/// </summary>
internal sealed class ServiceValidator : IServiceValidator
{
    private readonly IServiceRepository _serviceRepository;

    public ServiceValidator(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<bool> ValidateServiceExistsAsync(
        Guid serviceId,
        Guid businessId,
        CancellationToken cancellationToken = default)
    {
        var service = await _serviceRepository.GetByIdAsync(serviceId, cancellationToken);
        return service is not null && service.BusinessId == businessId;
    }
}
