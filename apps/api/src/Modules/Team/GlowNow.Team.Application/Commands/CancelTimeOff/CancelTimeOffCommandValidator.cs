using FluentValidation;
namespace GlowNow.Team.Application.Commands.CancelTimeOff;

/// <summary>
/// Validator for the CancelTimeOffCommand.
/// </summary>
public sealed class CancelTimeOffCommandValidator : AbstractValidator<CancelTimeOffCommand>
{
    public CancelTimeOffCommandValidator()
    {
        RuleFor(x => x.TimeOffId)
            .NotEmpty()
            .WithMessage("Time off request ID is required.");
    }
}
