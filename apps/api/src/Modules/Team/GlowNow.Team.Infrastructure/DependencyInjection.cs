using GlowNow.Team.Application.Interfaces;
using GlowNow.Team.Infrastructure.Persistence;
using GlowNow.Team.Infrastructure.Persistence.Repositories;
using GlowNow.Team.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Team.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTeamInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<TeamDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITeamUnitOfWork>(sp => sp.GetRequiredService<TeamDbContext>());
        services.AddScoped<IStaffProfileRepository, StaffProfileRepository>();
        services.AddScoped<ITimeOffRepository, TimeOffRepository>();
        services.AddScoped<IBlockedTimeRepository, BlockedTimeRepository>();
        services.AddScoped<IServiceValidator, ServiceValidator>();

        return services;
    }
}
