using GlowNow.Infrastructure.Core.Application.Messaging;

namespace GlowNow.Identity.Application.Commands.RegisterBusiness;

public sealed record RegisterBusinessCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string BusinessName,
    string BusinessRuc,
    string BusinessAddress,
    string? BusinessPhoneNumber,
    string? BusinessEmail) : ICommand<RegisterBusinessResponse>;
