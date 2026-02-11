using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Clients.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddClientsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Client repository and DbContext will be added when module is expanded
        return services;
    }
}
