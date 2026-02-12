using GlowNow.Catalog.Application.Interfaces;
using GlowNow.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContext : DbContext, ICatalogUnitOfWork
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
