using FluentValidation;
namespace GlowNow.Team.Application.Commands.DeleteBlockedTime;

/// <summary>
/// Validator for the DeleteBlockedTimeCommand.
/// </summary>
public sealed class DeleteBlockedTimeCommandValidator : AbstractValidator<DeleteBlockedTimeCommand>
{
    public DeleteBlockedTimeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Blocked time ID is required.");
    }
}
