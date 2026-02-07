using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Clients;

/// <summary>
/// Registers Clients module services into the DI container.
/// </summary>
public static class ClientsModule
{
    public static IServiceCollection AddClientsModule(this IServiceCollection services)
    {
        return services;
    }
}
