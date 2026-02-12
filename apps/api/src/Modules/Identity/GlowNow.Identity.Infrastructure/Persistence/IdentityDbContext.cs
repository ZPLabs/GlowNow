using GlowNow.Identity.Application.Interfaces;
using GlowNow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext, IIdentityUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<BusinessMembership> BusinessMemberships => Set<BusinessMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
