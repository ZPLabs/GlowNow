using GlowNow.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Identity.Infrastructure.Persistence.Configurations;

internal sealed class BusinessMembershipConfiguration : IEntityTypeConfiguration<BusinessMembership>
{
    public void Configure(EntityTypeBuilder<BusinessMembership> builder)
    {
        builder.ToTable("business_memberships");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(m => m.BusinessId)
            .HasColumnName("business_id")
            .IsRequired();

        builder.Property(m => m.Role)
            .HasColumnName("role")
            .IsRequired();

        builder.Property(m => m.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(m => new { m.UserId, m.BusinessId }).IsUnique();
    }
}
