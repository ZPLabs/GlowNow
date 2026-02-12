using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Catalog.Domain.ValueObjects;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.UpdateService;

/// <summary>
/// Handler for the UpdateServiceCommand.
/// </summary>
internal sealed class UpdateServiceCommandHandler : ICommandHandler<UpdateServiceCommand>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceCategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public UpdateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IServiceCategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateServiceCommand command, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(command.Id, cancellationToken);

        if (service is null)
        {
            return Result.Failure(CatalogErrors.ServiceNotFound);
        }

        // Validate category exists if provided
        if (command.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                return Result.Failure(CatalogErrors.CategoryNotFound);
            }
        }

        // Check for duplicate name (excluding current service)
        if (await _serviceRepository.ExistsByNameAsync(service.BusinessId, command.Name, command.Id, cancellationToken))
        {
            return Result.Failure(CatalogErrors.DuplicateServiceName);
        }

        // Create value objects
        var durationResult = Duration.Create(command.DurationMinutes);
        if (durationResult.IsFailure)
        {
            return Result.Failure(durationResult.Error);
        }

        var priceResult = Money.Create(command.Price);
        if (priceResult.IsFailure)
        {
            return Result.Failure(priceResult.Error);
        }

        // Update service
        var updateResult = service.Update(
            command.CategoryId,
            command.Name,
            command.Description,
            durationResult.Value,
            priceResult.Value,
            command.BufferTimeMinutes,
            command.DisplayOrder);

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        _serviceRepository.Update(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
