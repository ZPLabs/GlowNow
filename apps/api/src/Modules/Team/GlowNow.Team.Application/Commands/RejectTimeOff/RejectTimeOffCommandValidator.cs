using FluentValidation;
namespace GlowNow.Team.Application.Commands.RejectTimeOff;

/// <summary>
/// Validator for the RejectTimeOffCommand.
/// </summary>
public sealed class RejectTimeOffCommandValidator : AbstractValidator<RejectTimeOffCommand>
{
    public RejectTimeOffCommandValidator()
    {
        RuleFor(x => x.TimeOffId)
            .NotEmpty()
            .WithMessage("Time off request ID is required.");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => x.Reason is not null)
            .WithMessage("Rejection reason cannot exceed 500 characters.");
    }
}
