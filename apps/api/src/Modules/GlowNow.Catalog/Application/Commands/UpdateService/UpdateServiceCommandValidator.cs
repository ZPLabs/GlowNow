using FluentValidation;
using GlowNow.Catalog.Domain.ValueObjects;

namespace GlowNow.Catalog.Application.Commands.UpdateService;

/// <summary>
/// Validator for the UpdateServiceCommand.
/// </summary>
public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Service ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Service name is required.")
            .MaximumLength(200)
            .WithMessage("Service name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(Duration.MinMinutes, Duration.MaxMinutes)
            .WithMessage($"Duration must be between {Duration.MinMinutes} and {Duration.MaxMinutes} minutes.");

        RuleFor(x => x.Price)
            .InclusiveBetween(Money.MinAmount, Money.MaxAmount)
            .WithMessage($"Price must be between ${Money.MinAmount} and ${Money.MaxAmount}.");

        RuleFor(x => x.BufferTimeMinutes)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Buffer time cannot be negative.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order must be non-negative.");
    }
}
