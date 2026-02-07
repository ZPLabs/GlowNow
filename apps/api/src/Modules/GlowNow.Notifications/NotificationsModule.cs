using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Notifications;

/// <summary>
/// Registers Notifications module services into the DI container.
/// </summary>
public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        return services;
    }
}
