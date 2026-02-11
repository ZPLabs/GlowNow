using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Entities;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.CreateServiceCategory;

/// <summary>
/// Handler for the CreateServiceCategoryCommand.
/// </summary>
internal sealed class CreateServiceCategoryCommandHandler
    : ICommandHandler<CreateServiceCategoryCommand, CreateServiceCategoryResponse>
{
    private readonly IServiceCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateServiceCategoryCommandHandler(
        IServiceCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<CreateServiceCategoryResponse>> Handle(
        CreateServiceCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (await _categoryRepository.ExistsByNameAsync(command.BusinessId, command.Name, cancellationToken))
        {
            return Result.Failure<CreateServiceCategoryResponse>(CatalogErrors.DuplicateCategoryName);
        }

        var category = ServiceCategory.Create(
            command.BusinessId,
            command.Name,
            command.Description,
            command.DisplayOrder,
            _dateTimeProvider.UtcNow);

        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateServiceCategoryResponse(category.Id);
    }
}
