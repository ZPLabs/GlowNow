using FluentValidation;
namespace GlowNow.Team.Application.Commands.ApproveTimeOff;

/// <summary>
/// Validator for the ApproveTimeOffCommand.
/// </summary>
public sealed class ApproveTimeOffCommandValidator : AbstractValidator<ApproveTimeOffCommand>
{
    public ApproveTimeOffCommandValidator()
    {
        RuleFor(x => x.TimeOffId)
            .NotEmpty()
            .WithMessage("Time off request ID is required.");

        RuleFor(x => x.ApprovedByUserId)
            .NotEmpty()
            .WithMessage("Approver user ID is required.");
    }
}
