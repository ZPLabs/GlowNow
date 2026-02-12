using GlowNow.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Service entity.
/// </summary>
internal sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("services");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.BusinessId)
            .IsRequired();

        builder.Property(s => s.CategoryId);

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        // Duration value object
        builder.OwnsOne(s => s.Duration, durationBuilder =>
        {
            durationBuilder.Property(d => d.Minutes)
                .HasColumnName("duration_minutes")
                .IsRequired();
        });

        // Money value object
        builder.OwnsOne(s => s.Price, priceBuilder =>
        {
            priceBuilder.Property(p => p.Amount)
                .HasColumnName("price")
                .HasColumnType("decimal(10,2)")
                .IsRequired();
        });

        builder.Property(s => s.BufferTimeMinutes)
            .HasColumnName("buffer_time_minutes")
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.DisplayOrder)
            .HasColumnName("display_order")
            .IsRequired();

        builder.Property(s => s.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign key to category (optional)
        builder.HasOne<ServiceCategory>()
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for business lookups
        builder.HasIndex(s => s.BusinessId);

        // Index for category lookups
        builder.HasIndex(s => s.CategoryId);

        // Unique index for name within a business (excluding deleted)
        builder.HasIndex(s => new { s.BusinessId, s.Name })
            .HasFilter("is_deleted = false")
            .IsUnique();

        // Query filter for soft-deleted records
        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
