using FluentValidation;

namespace GlowNow.Business.Application.Commands.UpdateBusinessSettings;

/// <summary>
/// Validator for the UpdateBusinessSettingsCommand.
/// </summary>
public sealed class UpdateBusinessSettingsCommandValidator : AbstractValidator<UpdateBusinessSettingsCommand>
{
    public UpdateBusinessSettingsCommandValidator()
    {
        RuleFor(x => x.BusinessId)
            .NotEmpty()
            .WithMessage("Business ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Business name is required.")
            .MaximumLength(200)
            .WithMessage("Business name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.LogoUrl)
            .Must(url => url is null || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Logo URL must be a valid absolute URL.");
    }
}
