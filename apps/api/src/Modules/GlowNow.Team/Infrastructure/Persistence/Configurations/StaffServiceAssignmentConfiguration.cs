using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Team.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the StaffServiceAssignment entity.
/// </summary>
internal sealed class StaffServiceAssignmentConfiguration : IEntityTypeConfiguration<StaffServiceAssignment>
{
    public void Configure(EntityTypeBuilder<StaffServiceAssignment> builder)
    {
        builder.ToTable("staff_service_assignments");

        builder.HasKey(ssa => ssa.Id);

        builder.Property(ssa => ssa.StaffProfileId)
            .HasColumnName("staff_profile_id")
            .IsRequired();

        builder.Property(ssa => ssa.ServiceId)
            .HasColumnName("service_id")
            .IsRequired();

        builder.Property(ssa => ssa.AssignedAtUtc)
            .HasColumnName("assigned_at_utc")
            .IsRequired();

        // Unique constraint on staff-service pair
        builder.HasIndex(ssa => new { ssa.StaffProfileId, ssa.ServiceId })
            .IsUnique()
            .HasDatabaseName("ix_staff_service_assignments_unique");

        // Index for service lookups
        builder.HasIndex(ssa => ssa.ServiceId)
            .HasDatabaseName("ix_staff_service_assignments_service_id");
    }
}
