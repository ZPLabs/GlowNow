using FluentValidation;
namespace GlowNow.Team.Application.Commands.ActivateStaff;

/// <summary>
/// Validator for the ActivateStaffCommand.
/// </summary>
public sealed class ActivateStaffCommandValidator : AbstractValidator<ActivateStaffCommand>
{
    public ActivateStaffCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");
    }
}
