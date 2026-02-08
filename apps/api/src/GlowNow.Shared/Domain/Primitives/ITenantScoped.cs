namespace GlowNow.Shared.Domain.Primitives;

/// <summary>
/// Defines a contract for entities that are scoped to a specific tenant (Business).
/// </summary>
public interface ITenantScoped
{
    Guid BusinessId { get; }
}
