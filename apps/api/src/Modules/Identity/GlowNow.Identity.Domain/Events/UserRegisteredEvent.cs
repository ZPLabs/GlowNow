using GlowNow.SharedKernel.Domain.Events;

namespace GlowNow.Identity.Domain.Events;

public sealed record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName) : IDomainEvent;
