using Amazon.CognitoIdentityProvider;
using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Infrastructure.Persistence;
using GlowNow.Identity.Infrastructure.Persistence.Repositories;
using GlowNow.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IIdentityUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        // Configure Cognito
        services.Configure<CognitoSettings>(configuration.GetSection(CognitoSettings.SectionName));

        services.AddSingleton<IAmazonCognitoIdentityProvider>(sp =>
        {
            var settings = configuration.GetSection(CognitoSettings.SectionName).Get<CognitoSettings>();
            var region = Amazon.RegionEndpoint.GetBySystemName(settings?.Region ?? "us-east-1");

            if (!string.IsNullOrEmpty(settings?.AccessKey) && !string.IsNullOrEmpty(settings?.SecretKey))
            {
                var credentials = new Amazon.Runtime.BasicAWSCredentials(settings.AccessKey, settings.SecretKey);
                return new AmazonCognitoIdentityProviderClient(credentials, region);
            }

            return new AmazonCognitoIdentityProviderClient(region);
        });

        services.AddScoped<ICognitoIdentityProvider, CognitoIdentityProvider>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBusinessMembershipRepository, BusinessMembershipRepository>();

        return services;
    }
}
