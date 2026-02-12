using FluentValidation;
namespace GlowNow.Team.Application.Commands.DeleteStaffProfile;

/// <summary>
/// Validator for the DeleteStaffProfileCommand.
/// </summary>
public sealed class DeleteStaffProfileCommandValidator : AbstractValidator<DeleteStaffProfileCommand>
{
    public DeleteStaffProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");
    }
}
