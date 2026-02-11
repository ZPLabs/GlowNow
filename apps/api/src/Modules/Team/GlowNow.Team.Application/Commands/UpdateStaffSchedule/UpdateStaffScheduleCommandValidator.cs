using FluentValidation;
namespace GlowNow.Team.Application.Commands.UpdateStaffSchedule;

/// <summary>
/// Validator for the UpdateStaffScheduleCommand.
/// </summary>
public sealed class UpdateStaffScheduleCommandValidator : AbstractValidator<UpdateStaffScheduleCommand>
{
    public UpdateStaffScheduleCommandValidator()
    {
        RuleFor(x => x.StaffProfileId)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.Schedule)
            .NotNull()
            .WithMessage("Schedule is required.");

        RuleForEach(x => x.Schedule)
            .ChildRules(entry =>
            {
                entry.When(x => x.Value is not null, () =>
                {
                    entry.RuleFor(x => x.Value!.StartTime)
                        .NotEmpty()
                        .WithMessage("Start time is required for working days.")
                        .Matches(@"^\d{2}:\d{2}$")
                        .WithMessage("Start time must be in HH:mm format.");

                    entry.RuleFor(x => x.Value!.EndTime)
                        .NotEmpty()
                        .WithMessage("End time is required for working days.")
                        .Matches(@"^\d{2}:\d{2}$")
                        .WithMessage("End time must be in HH:mm format.");

                    entry.When(x => x.Value!.BreakStart is not null, () =>
                    {
                        entry.RuleFor(x => x.Value!.BreakStart)
                            .Matches(@"^\d{2}:\d{2}$")
                            .WithMessage("Break start time must be in HH:mm format.");

                        entry.RuleFor(x => x.Value!.BreakEnd)
                            .NotEmpty()
                            .WithMessage("Break end time is required when break start is provided.")
                            .Matches(@"^\d{2}:\d{2}$")
                            .WithMessage("Break end time must be in HH:mm format.");
                    });
                });
            });
    }
}
