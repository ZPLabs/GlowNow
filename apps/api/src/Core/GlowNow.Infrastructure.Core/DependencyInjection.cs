using GlowNow.Infrastructure.Core.Infrastructure.Services;

namespace GlowNow.Infrastructure.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureCore(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);

            // Behaviors are executed in the order they are registered
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(TransactionBehavior<,>));
            config.AddOpenBehavior(typeof(PerformanceBehavior<,>));
        });

        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.TryAddScoped<ITransactionManager, NoOpTransactionManager>();

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.TryAddScoped<ITenantProvider, HttpTenantProvider>();

        return services;
    }
}
