using GlowNow.SharedKernel.Domain.Events;

namespace GlowNow.Business.Domain.Events;

public sealed record BusinessRegisteredEvent(
    Guid BusinessId,
    string Name,
    string Ruc) : IDomainEvent;
