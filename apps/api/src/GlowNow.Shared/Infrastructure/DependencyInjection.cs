using System.Reflection;
using GlowNow.Shared.Application.Interfaces;
using GlowNow.Shared.Infrastructure.Persistence;
using GlowNow.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GlowNow.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] moduleAssemblies)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddSingleton(moduleAssemblies);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // This overrides the NoOpTransactionManager if called after AddSharedServices
        services.TryAddScoped<ITransactionManager, EfCoreTransactionManager>();
        
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.TryAddScoped<ITenantProvider, HttpTenantProvider>();

        return services;
    }
}
