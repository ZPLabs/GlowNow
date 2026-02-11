using GlowNow.Team.Domain.Entities;
using GlowNow.Team.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Team.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the StaffProfile entity.
/// </summary>
internal sealed class StaffProfileConfiguration : IEntityTypeConfiguration<StaffProfile>
{
    public void Configure(EntityTypeBuilder<StaffProfile> builder)
    {
        builder.ToTable("staff_profiles");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.BusinessId)
            .HasColumnName("business_id")
            .IsRequired();

        builder.Property(sp => sp.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(sp => sp.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(sp => sp.Bio)
            .HasColumnName("bio")
            .HasMaxLength(1000);

        builder.Property(sp => sp.ProfileImageUrl)
            .HasColumnName("profile_image_url")
            .HasMaxLength(500);

        builder.Property(sp => sp.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.Property(sp => sp.AcceptsOnlineBookings)
            .HasColumnName("accepts_online_bookings")
            .IsRequired()
            .HasDefaultValue(true);

        // WeeklySchedule stored as JSON
        builder.Property(sp => sp.DefaultSchedule)
            .HasColumnName("default_schedule")
            .HasColumnType("jsonb")
            .HasConversion(
                v => v.ToJson(),
                v => WeeklySchedule.FromJson(v).Value)
            .IsRequired();

        builder.Property(sp => sp.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(sp => sp.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(sp => sp.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        // Navigation property
        builder.HasMany(sp => sp.ServiceAssignments)
            .WithOne()
            .HasForeignKey(sa => sa.StaffProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(sp => sp.BusinessId)
            .HasDatabaseName("ix_staff_profiles_business_id");

        builder.HasIndex(sp => sp.UserId)
            .HasDatabaseName("ix_staff_profiles_user_id");

        builder.HasIndex(sp => new { sp.BusinessId, sp.UserId })
            .HasFilter("is_deleted = false")
            .IsUnique()
            .HasDatabaseName("ix_staff_profiles_business_user_unique");

        builder.HasIndex(sp => new { sp.BusinessId, sp.Status })
            .HasDatabaseName("ix_staff_profiles_business_status");

        // Query filter for soft-deleted records
        builder.HasQueryFilter(sp => !sp.IsDeleted);
    }
}
