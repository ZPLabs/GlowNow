using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Team.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the BlockedTime entity.
/// </summary>
internal sealed class BlockedTimeConfiguration : IEntityTypeConfiguration<BlockedTime>
{
    public void Configure(EntityTypeBuilder<BlockedTime> builder)
    {
        builder.ToTable("blocked_times");

        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.StaffProfileId)
            .HasColumnName("staff_profile_id")
            .IsRequired();

        builder.Property(bt => bt.BusinessId)
            .HasColumnName("business_id")
            .IsRequired();

        builder.Property(bt => bt.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(bt => bt.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(bt => bt.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(bt => bt.IsRecurring)
            .HasColumnName("is_recurring")
            .IsRequired();

        builder.Property(bt => bt.RecurringDayOfWeek)
            .HasColumnName("recurring_day_of_week")
            .HasConversion<int?>();

        builder.Property(bt => bt.SpecificDate)
            .HasColumnName("specific_date");

        builder.Property(bt => bt.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        // Indexes
        builder.HasIndex(bt => bt.StaffProfileId)
            .HasDatabaseName("ix_blocked_times_staff_profile_id");

        builder.HasIndex(bt => bt.BusinessId)
            .HasDatabaseName("ix_blocked_times_business_id");

        builder.HasIndex(bt => new { bt.StaffProfileId, bt.IsRecurring, bt.RecurringDayOfWeek })
            .HasDatabaseName("ix_blocked_times_staff_recurring");

        builder.HasIndex(bt => new { bt.StaffProfileId, bt.SpecificDate })
            .HasDatabaseName("ix_blocked_times_staff_specific_date");

        // Foreign key to staff profile
        builder.HasOne<StaffProfile>()
            .WithMany()
            .HasForeignKey(bt => bt.StaffProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
