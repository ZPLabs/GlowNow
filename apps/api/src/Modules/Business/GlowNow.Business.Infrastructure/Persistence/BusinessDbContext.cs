using GlowNow.Business.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence;

public sealed class BusinessDbContext : DbContext, IBusinessUnitOfWork
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
