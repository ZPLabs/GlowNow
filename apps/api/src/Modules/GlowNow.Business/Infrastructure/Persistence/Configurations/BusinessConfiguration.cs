using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence.Configurations;

internal sealed class BusinessConfiguration : IEntityTypeConfiguration<BusinessEntity>
{
    public void Configure(EntityTypeBuilder<BusinessEntity> builder)
    {
        builder.ToTable("businesses");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(b => b.Ruc, rucBuilder =>
        {
            rucBuilder.Property(r => r.Value)
                .HasColumnName("ruc")
                .HasMaxLength(13)
                .IsRequired();

            rucBuilder.HasIndex(r => r.Value).IsUnique();
        });

        builder.Property(b => b.Address)
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(b => b.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("phone_number")
                .HasMaxLength(20);
        });

        builder.OwnsOne(b => b.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(Email.MaxLength)
                .IsRequired();
        });

        builder.Property(b => b.BusinessId)
            .IsRequired();

        builder.HasQueryFilter(b => true); // Tenant filter will be added later globally or per entity
    }
}
