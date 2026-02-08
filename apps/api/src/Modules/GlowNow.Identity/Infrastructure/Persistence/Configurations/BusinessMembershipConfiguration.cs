using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Identity.Infrastructure.Persistence.Configurations;

internal sealed class BusinessMembershipConfiguration : IEntityTypeConfiguration<BusinessMembership>
{
    public void Configure(EntityTypeBuilder<BusinessMembership> builder)
    {
        builder.ToTable("business_memberships");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.UserId).IsRequired();
        builder.Property(m => m.BusinessId).IsRequired();
        builder.Property(m => m.Role).IsRequired();

        builder.HasIndex(m => new { m.UserId, m.BusinessId }).IsUnique();
    }
}
