namespace GlowNow.Team.Application.Commands.DeactivateStaff;

/// <summary>
/// Validator for the DeactivateStaffCommand.
/// </summary>
public sealed class DeactivateStaffCommandValidator : AbstractValidator<DeactivateStaffCommand>
{
    public DeactivateStaffCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");
    }
}
