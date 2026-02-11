using GlowNow.Business.Domain.ValueObjects;
using GlowNow.SharedKernel.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BusinessEntity = GlowNow.Business.Domain.Entities.Business;

namespace GlowNow.Business.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for the Business entity.
/// </summary>
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

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        // Store operating hours as JSON using value conversion
        var operatingHoursConverter = new ValueConverter<OperatingHours, string>(
            v => v.ToJson(),
            v => OperatingHours.FromJson(v).Value);

        var operatingHoursComparer = new ValueComparer<OperatingHours>(
            (a, b) => a != null && b != null && a.ToJson() == b.ToJson(),
            v => v.ToJson().GetHashCode(),
            v => OperatingHours.FromJson(v.ToJson()).Value);

        builder.Property(b => b.OperatingHours)
            .HasColumnName("operating_hours")
            .HasColumnType("jsonb")
            .HasConversion(operatingHoursConverter)
            .Metadata.SetValueComparer(operatingHoursComparer);

        builder.Property(b => b.BusinessId)
            .HasColumnName("business_id")
            .IsRequired();

        builder.Property(b => b.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasQueryFilter(b => true); // Tenant filter will be added later globally or per entity
    }
}
