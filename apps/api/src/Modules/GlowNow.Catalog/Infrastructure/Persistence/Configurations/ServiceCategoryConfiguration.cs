using GlowNow.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Catalog.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the ServiceCategory entity.
/// </summary>
internal sealed class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
{
    public void Configure(EntityTypeBuilder<ServiceCategory> builder)
    {
        builder.ToTable("service_categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.BusinessId)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.DisplayOrder)
            .IsRequired();

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Index for business lookups
        builder.HasIndex(c => c.BusinessId);

        // Unique index for name within a business (excluding deleted)
        builder.HasIndex(c => new { c.BusinessId, c.Name })
            .HasFilter("is_deleted = false")
            .IsUnique();

        // Query filter for soft-deleted records
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
