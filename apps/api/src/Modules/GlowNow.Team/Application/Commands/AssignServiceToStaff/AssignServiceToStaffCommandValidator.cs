namespace GlowNow.Team.Application.Commands.AssignServiceToStaff;

/// <summary>
/// Validator for the AssignServiceToStaffCommand.
/// </summary>
public sealed class AssignServiceToStaffCommandValidator : AbstractValidator<AssignServiceToStaffCommand>
{
    public AssignServiceToStaffCommandValidator()
    {
        RuleFor(x => x.StaffProfileId)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.ServiceId)
            .NotEmpty()
            .WithMessage("Service ID is required.");
    }
}
