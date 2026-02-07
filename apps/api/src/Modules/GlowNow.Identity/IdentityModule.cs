using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Identity;

/// <summary>
/// Registers Identity module services into the DI container.
/// </summary>
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        return services;
    }
}
