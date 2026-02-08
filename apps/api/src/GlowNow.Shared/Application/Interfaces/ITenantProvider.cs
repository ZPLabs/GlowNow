namespace GlowNow.Shared.Application.Interfaces;

/// <summary>
/// Defines a contract for retrieving the current tenant (Business) information.
/// </summary>
public interface ITenantProvider
{
    Guid GetCurrentBusinessId();
    Guid? TryGetCurrentBusinessId();
}
