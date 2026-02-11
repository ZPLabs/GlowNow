using GlowNow.Infrastructure.Core.Application.Interfaces;
using GlowNow.Team.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GlowNow.Team.Infrastructure.Persistence;

public sealed class TeamDbContext : DbContext, IUnitOfWork
{
    public TeamDbContext(DbContextOptions<TeamDbContext> options)
        : base(options)
    {
    }

    public DbSet<StaffProfile> StaffProfiles => Set<StaffProfile>();
    public DbSet<StaffServiceAssignment> StaffServiceAssignments => Set<StaffServiceAssignment>();
    public DbSet<TimeOff> TimeOffs => Set<TimeOff>();
    public DbSet<BlockedTime> BlockedTimes => Set<BlockedTime>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("team");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TeamDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
