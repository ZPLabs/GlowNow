using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Entities;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Catalog.Domain.ValueObjects;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.CreateService;

/// <summary>
/// Handler for the CreateServiceCommand.
/// </summary>
internal sealed class CreateServiceCommandHandler
    : ICommandHandler<CreateServiceCommand, CreateServiceResponse>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IServiceCategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateServiceCommandHandler(
        IServiceRepository serviceRepository,
        IServiceCategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _serviceRepository = serviceRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CreateServiceResponse>> Handle(
        CreateServiceCommand command,
        CancellationToken cancellationToken)
    {
        // Validate category exists if provided
        if (command.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                return Result.Failure<CreateServiceResponse>(CatalogErrors.CategoryNotFound);
            }
        }

        // Check for duplicate name
        if (await _serviceRepository.ExistsByNameAsync(command.BusinessId, command.Name, cancellationToken))
        {
            return Result.Failure<CreateServiceResponse>(CatalogErrors.DuplicateServiceName);
        }

        // Create value objects
        var durationResult = Duration.Create(command.DurationMinutes);
        if (durationResult.IsFailure)
        {
            return Result.Failure<CreateServiceResponse>(durationResult.Error);
        }

        var priceResult = Money.Create(command.Price);
        if (priceResult.IsFailure)
        {
            return Result.Failure<CreateServiceResponse>(priceResult.Error);
        }

        // Create service
        var serviceResult = Service.Create(
            command.BusinessId,
            command.CategoryId,
            command.Name,
            command.Description,
            durationResult.Value,
            priceResult.Value,
            command.BufferTimeMinutes,
            command.DisplayOrder,
            _dateTimeProvider.UtcNow);

        if (serviceResult.IsFailure)
        {
            return Result.Failure<CreateServiceResponse>(serviceResult.Error);
        }

        _serviceRepository.Add(serviceResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateServiceResponse(serviceResult.Value.Id);
    }
}
