using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.UpdateServiceCategory;

/// <summary>
/// Handler for the UpdateServiceCategoryCommand.
/// </summary>
internal sealed class UpdateServiceCategoryCommandHandler : ICommandHandler<UpdateServiceCategoryCommand>
{
    private readonly IServiceCategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateServiceCategoryCommandHandler(
        IServiceCategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateServiceCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CatalogErrors.CategoryNotFound);
        }

        if (await _categoryRepository.ExistsByNameAsync(category.BusinessId, command.Name, command.Id, cancellationToken))
        {
            return Result.Failure(CatalogErrors.DuplicateCategoryName);
        }

        var updateResult = category.Update(command.Name, command.Description, command.DisplayOrder);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
