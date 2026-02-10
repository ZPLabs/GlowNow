using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Application.Messaging;
using GlowNow.Shared.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.DeleteService;

/// <summary>
/// Handler for the DeleteServiceCommand.
/// </summary>
internal sealed class DeleteServiceCommandHandler : ICommandHandler<DeleteServiceCommand>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteServiceCommandHandler(
        IServiceRepository serviceRepository,
        IUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteServiceCommand command, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(command.Id, cancellationToken);

        if (service is null)
        {
            return Result.Failure(CatalogErrors.ServiceNotFound);
        }

        // Deactivate if active, then delete
        if (service.IsActive)
        {
            service.Deactivate();
        }

        var deleteResult = service.Delete();
        if (deleteResult.IsFailure)
        {
            return deleteResult;
        }

        _serviceRepository.Update(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
