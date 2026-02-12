using GlowNow.Infrastructure.Core.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence;

public sealed class BusinessDbContext : DbContext, IUnitOfWork
{
    public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
        : base(options)
    {
    }

    public DbSet<BusinessEntity> Businesses => Set<BusinessEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("business");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
