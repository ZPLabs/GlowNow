using GlowNow.Notifications.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Notifications.Api;

/// <summary>
/// Entry point for the Notifications module registration.
/// </summary>
public static class NotificationsModule
{
    /// <summary>
    /// Registers Notifications module services into the DI container.
    /// </summary>
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        // Register infrastructure services
        services.AddNotificationsInfrastructure();

        return services;
    }
}
