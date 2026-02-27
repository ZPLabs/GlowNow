using FluentValidation;

namespace GlowNow.Identity.Application.Commands.RegisterBusiness;

public sealed class RegisterBusinessCommandValidator : AbstractValidator<RegisterBusinessCommand>
{
    public RegisterBusinessCommandValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.BusinessRuc).NotEmpty();
        RuleFor(x => x.BusinessAddress).NotEmpty().MaximumLength(500);
    }
}
