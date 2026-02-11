using FluentValidation;
using GlowNow.Identity.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Identity.Api;

/// <summary>
/// Entry point for the Identity module registration.
/// </summary>
public static class IdentityModule
{
    /// <summary>
    /// Registers Identity module services into the DI container.
    /// </summary>
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register validators from Application assembly
        services.AddValidatorsFromAssemblyContaining<Application.Commands.Login.LoginCommandValidator>();

        // Register MediatR handlers from Application assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Application.Commands.Login.LoginCommandValidator>());

        // Register infrastructure services
        services.AddIdentityInfrastructure(configuration);

        return services;
    }
}
