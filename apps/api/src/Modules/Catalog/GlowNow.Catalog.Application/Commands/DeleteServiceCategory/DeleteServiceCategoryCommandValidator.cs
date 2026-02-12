using FluentValidation;

namespace GlowNow.Catalog.Application.Commands.DeleteServiceCategory;

/// <summary>
/// Validator for the DeleteServiceCategoryCommand.
/// </summary>
public sealed class DeleteServiceCategoryCommandValidator : AbstractValidator<DeleteServiceCategoryCommand>
{
    public DeleteServiceCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
