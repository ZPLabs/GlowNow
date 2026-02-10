namespace GlowNow.Team.Application.Commands.UpdateStaffProfile;

/// <summary>
/// Validator for the UpdateStaffProfileCommand.
/// </summary>
public sealed class UpdateStaffProfileCommandValidator : AbstractValidator<UpdateStaffProfileCommand>
{
    public UpdateStaffProfileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Staff profile ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Staff title is required.")
            .MaximumLength(100)
            .WithMessage("Staff title cannot exceed 100 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .When(x => x.Bio is not null)
            .WithMessage("Bio cannot exceed 1000 characters.");

        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500)
            .When(x => x.ProfileImageUrl is not null)
            .WithMessage("Profile image URL cannot exceed 500 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be non-negative.");
    }
}
