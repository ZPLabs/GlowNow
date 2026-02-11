using GlowNow.Catalog.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Catalog.Api;

/// <summary>
/// Registers Catalog module services into the DI container.
/// </summary>
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogInfrastructure(configuration);
        return services;
    }
}
