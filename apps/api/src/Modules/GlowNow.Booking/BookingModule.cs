using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Booking;

/// <summary>
/// Registers Booking module services into the DI container.
/// </summary>
public static class BookingModule
{
    public static IServiceCollection AddBookingModule(this IServiceCollection services)
    {
        return services;
    }
}
