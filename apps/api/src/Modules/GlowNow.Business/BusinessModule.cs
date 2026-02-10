using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Business;

/// <summary>
/// Registers Business module services into the DI container.
/// </summary>
public static class BusinessModule
{
    public static IServiceCollection AddBusinessModule(this IServiceCollection services)
    {
        services.AddScoped<IBusinessRepository, BusinessRepository>();

        return services;
    }
}
