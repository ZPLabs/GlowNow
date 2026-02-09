using System.Reflection;
using GlowNow.Shared.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Shared.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    private readonly Assembly[] _moduleAssemblies;

    public AppDbContext(DbContextOptions<AppDbContext> options, Assembly[] moduleAssemblies)
        : base(options)
    {
        _moduleAssemblies = moduleAssemblies;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var assembly in _moduleAssemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        base.OnModelCreating(modelBuilder);
    }
}
