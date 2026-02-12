using FluentValidation;
using GlowNow.Team.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Team.Api;

/// <summary>
/// Entry point for the Team module registration.
/// </summary>
public static class TeamModule
{
    /// <summary>
    /// Registers Team module services into the DI container.
    /// </summary>
    public static IServiceCollection AddTeamModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register validators from Application assembly
        services.AddValidatorsFromAssemblyContaining<Application.Commands.CreateStaffProfile.CreateStaffProfileCommandValidator>();

        // Register MediatR handlers from Application assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Application.Commands.CreateStaffProfile.CreateStaffProfileCommandValidator>());

        // Register infrastructure services
        services.AddTeamInfrastructure(configuration);

        return services;
    }
}
