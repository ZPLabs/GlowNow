using Amazon.CognitoIdentityProvider;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Infrastructure.Persistence.Repositories;
using GlowNow.Identity.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Identity;

/// <summary>
/// Registers Identity module services into the DI container.
/// </summary>
public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CognitoSettings>(configuration.GetSection(CognitoSettings.SectionName));

        services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
        {
            var settings = configuration.GetSection(CognitoSettings.SectionName).Get<CognitoSettings>();
            var region = Amazon.RegionEndpoint.GetBySystemName(settings?.Region ?? "us-east-1");
            return new AmazonCognitoIdentityProviderClient(region);
        });

        services.AddScoped<ICognitoIdentityProvider, CognitoIdentityProvider>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBusinessMembershipRepository, BusinessMembershipRepository>();
        services.AddScoped<IBusinessRepository, BusinessRepository>();

        return services;
    }
}
