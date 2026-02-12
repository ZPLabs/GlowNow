using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Errors;
using GlowNow.Infrastructure.Core.Application.Messaging;
using GlowNow.SharedKernel.Domain.Errors;

namespace GlowNow.Catalog.Application.Commands.DeleteServiceCategory;

/// <summary>
/// Handler for the DeleteServiceCategoryCommand.
/// </summary>
internal sealed class DeleteServiceCategoryCommandHandler : ICommandHandler<DeleteServiceCategoryCommand>
{
    private readonly IServiceCategoryRepository _categoryRepository;
    private readonly ICatalogUnitOfWork _unitOfWork;

    public DeleteServiceCategoryCommandHandler(
        IServiceCategoryRepository categoryRepository,
        ICatalogUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteServiceCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CatalogErrors.CategoryNotFound);
        }

        if (await _categoryRepository.HasServicesAsync(command.Id, cancellationToken))
        {
            return Result.Failure(CatalogErrors.CategoryHasServices);
        }

        category.Delete();
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
