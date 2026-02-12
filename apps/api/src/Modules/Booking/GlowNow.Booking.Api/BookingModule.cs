using GlowNow.Booking.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Booking.Api;

/// <summary>
/// Entry point for the Booking module registration.
/// </summary>
public static class BookingModule
{
    /// <summary>
    /// Registers Booking module services into the DI container.
    /// </summary>
    public static IServiceCollection AddBookingModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register infrastructure services
        services.AddBookingInfrastructure(configuration);

        return services;
    }
}
