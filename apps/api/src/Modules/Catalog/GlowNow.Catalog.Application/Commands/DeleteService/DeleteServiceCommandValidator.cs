using FluentValidation;

namespace GlowNow.Catalog.Application.Commands.DeleteService;

/// <summary>
/// Validator for the DeleteServiceCommand.
/// </summary>
public sealed class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
{
    public DeleteServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Service ID is required.");
    }
}
