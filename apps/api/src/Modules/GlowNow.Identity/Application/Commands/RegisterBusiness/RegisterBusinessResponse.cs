namespace GlowNow.Identity.Application.Commands.RegisterBusiness;

public sealed record RegisterBusinessResponse(
    Guid UserId,
    Guid BusinessId,
    string Email);
