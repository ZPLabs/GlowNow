using GlowNow.Notifications.Application.Interfaces;
using GlowNow.Notifications.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Notifications.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationsInfrastructure(this IServiceCollection services)
    {
        // Register placeholder services (replace with actual implementations for production)
        services.AddScoped<IEmailService, LoggingEmailService>();
        services.AddScoped<ISmsService, LoggingSmsService>();

        return services;
    }
}
