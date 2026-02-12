using GlowNow.Clients.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Clients.Api;

/// <summary>
/// Entry point for the Clients module registration.
/// </summary>
public static class ClientsModule
{
    /// <summary>
    /// Registers Clients module services into the DI container.
    /// </summary>
    public static IServiceCollection AddClientsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register infrastructure services
        services.AddClientsInfrastructure(configuration);

        return services;
    }
}
