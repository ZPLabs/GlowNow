using FluentValidation;
namespace GlowNow.Team.Application.Commands.CreateBlockedTime;

/// <summary>
/// Validator for the CreateBlockedTimeCommand.
/// </summary>
public sealed class CreateBlockedTimeCommandValidator : AbstractValidator<CreateBlockedTimeCommand>
{
    public CreateBlockedTimeCommandValidator()
    {
        RuleFor(x => x.StaffProfileId)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .When(x => x.Title is not null)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty()
            .WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time.");

        RuleFor(x => x.DayOfWeek)
            .NotNull()
            .When(x => x.IsRecurring)
            .WithMessage("Day of week is required for recurring blocked time.");

        RuleFor(x => x.SpecificDate)
            .NotNull()
            .When(x => !x.IsRecurring)
            .WithMessage("Specific date is required for one-time blocked time.");
    }
}
