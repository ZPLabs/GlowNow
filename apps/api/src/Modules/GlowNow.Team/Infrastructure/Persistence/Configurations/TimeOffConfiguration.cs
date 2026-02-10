using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Team.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the TimeOff entity.
/// </summary>
internal sealed class TimeOffConfiguration : IEntityTypeConfiguration<TimeOff>
{
    public void Configure(EntityTypeBuilder<TimeOff> builder)
    {
        builder.ToTable("time_offs");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.StaffProfileId)
            .HasColumnName("staff_profile_id")
            .IsRequired();

        builder.Property(t => t.BusinessId)
            .HasColumnName("business_id")
            .IsRequired();

        builder.Property(t => t.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(t => t.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.Property(t => t.Type)
            .HasColumnName("type")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Notes)
            .HasColumnName("notes")
            .HasMaxLength(500);

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.RequestedAtUtc)
            .HasColumnName("requested_at_utc")
            .IsRequired();

        builder.Property(t => t.ApprovedAtUtc)
            .HasColumnName("approved_at_utc");

        builder.Property(t => t.ApprovedByUserId)
            .HasColumnName("approved_by_user_id");

        builder.Property(t => t.RejectionReason)
            .HasColumnName("rejection_reason")
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(t => t.StaffProfileId)
            .HasDatabaseName("ix_time_offs_staff_profile_id");

        builder.HasIndex(t => t.BusinessId)
            .HasDatabaseName("ix_time_offs_business_id");

        builder.HasIndex(t => new { t.StaffProfileId, t.Status })
            .HasDatabaseName("ix_time_offs_staff_profile_status");

        builder.HasIndex(t => new { t.StaffProfileId, t.StartDate, t.EndDate })
            .HasDatabaseName("ix_time_offs_staff_profile_dates");

        // Foreign key to staff profile
        builder.HasOne<StaffProfile>()
            .WithMany()
            .HasForeignKey(t => t.StaffProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
