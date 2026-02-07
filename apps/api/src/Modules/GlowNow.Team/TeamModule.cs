using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Team;

/// <summary>
/// Registers Team module services into the DI container.
/// </summary>
public static class TeamModule
{
    public static IServiceCollection AddTeamModule(this IServiceCollection services)
    {
        return services;
    }
}
