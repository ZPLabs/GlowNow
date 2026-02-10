namespace GlowNow.Team.Application.Commands.UnassignServiceFromStaff;

/// <summary>
/// Validator for the UnassignServiceFromStaffCommand.
/// </summary>
public sealed class UnassignServiceFromStaffCommandValidator : AbstractValidator<UnassignServiceFromStaffCommand>
{
    public UnassignServiceFromStaffCommandValidator()
    {
        RuleFor(x => x.StaffProfileId)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.ServiceId)
            .NotEmpty()
            .WithMessage("Service ID is required.");
    }
}
