using FluentValidation;
namespace GlowNow.Team.Application.Commands.RequestTimeOff;

/// <summary>
/// Validator for the RequestTimeOffCommand.
/// </summary>
public sealed class RequestTimeOffCommandValidator : AbstractValidator<RequestTimeOffCommand>
{
    public RequestTimeOffCommandValidator()
    {
        RuleFor(x => x.StaffProfileId)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid time off type.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => x.Notes is not null)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
}
