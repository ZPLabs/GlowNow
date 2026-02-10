using FluentValidation;

namespace GlowNow.Business.Application.Commands.SetOperatingHours;

/// <summary>
/// Validator for the SetOperatingHoursCommand.
/// </summary>
public sealed class SetOperatingHoursCommandValidator : AbstractValidator<SetOperatingHoursCommand>
{
    public SetOperatingHoursCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required.");

        RuleFor(x => x.Schedule)
            .NotNull()
            .WithMessage("Schedule is required.");

        RuleForEach(x => x.Schedule)
            .ChildRules(daySchedule =>
            {
                daySchedule.RuleFor(x => x.Value)
                    .ChildRules(schedule =>
                    {
                        schedule.RuleFor(s => s!.OpenTime)
                            .NotEmpty()
                            .When(s => s is not null)
                            .WithMessage("Open time is required when day is not closed.");

                        schedule.RuleFor(s => s!.CloseTime)
                            .NotEmpty()
                            .When(s => s is not null)
                            .WithMessage("Close time is required when day is not closed.");
                    });
            });
    }
}
