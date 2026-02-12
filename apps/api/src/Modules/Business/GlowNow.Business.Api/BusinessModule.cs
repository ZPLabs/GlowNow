using FluentValidation;
using GlowNow.Business.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Business.Api;

/// <summary>
/// Entry point for the Business module registration.
/// </summary>
public static class BusinessModule
{
    /// <summary>
    /// Registers Business module services into the DI container.
    /// </summary>
    public static IServiceCollection AddBusinessModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register validators from Application assembly
        services.AddValidatorsFromAssemblyContaining<Application.Commands.SetOperatingHours.SetOperatingHoursCommandValidator>();

        // Register MediatR handlers from Application assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Application.Commands.SetOperatingHours.SetOperatingHoursCommandValidator>());

        // Register infrastructure services
        services.AddBusinessInfrastructure(configuration);

        return services;
    }
}
