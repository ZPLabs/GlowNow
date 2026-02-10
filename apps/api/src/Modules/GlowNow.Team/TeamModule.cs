using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Infrastructure.Persistence.Repositories;
using GlowNow.Team.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Team;

/// <summary>
/// Registers Team module services into the DI container.
/// </summary>
public static class TeamModule
{
    public static IServiceCollection AddTeamModule(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();
        services.AddScoped<ITimeOffRepository, TimeOffRepository>();
        services.AddScoped<IBlockedTimeRepository, BlockedTimeRepository>();

        // Services
        services.AddScoped<IServiceValidator, ServiceValidator>();

        return services;
    }
}
