using GlowNow.Business.Application.Interfaces;
using GlowNow.Business.Infrastructure.Persistence;
using GlowNow.Business.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GlowNow.Business.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<BusinessDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IBusinessUnitOfWork>(sp => sp.GetRequiredService<BusinessDbContext>());
        services.AddScoped<IBusinessRepository, BusinessRepository>();

        return services;
    }
}
