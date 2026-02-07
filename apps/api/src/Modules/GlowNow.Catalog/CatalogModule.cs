using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Catalog;

/// <summary>
/// Registers Catalog module services into the DI container.
/// </summary>
public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services)
    {
        return services;
    }
}
