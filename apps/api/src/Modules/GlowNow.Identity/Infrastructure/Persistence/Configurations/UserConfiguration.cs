using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GlowNow.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(Email.MaxLength)
                .IsRequired();

            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(u => u.PhoneNumber, phoneBuilder =>
        {
            phoneBuilder.Property(p => p.Value)
                .HasColumnName("phone_number")
                .HasMaxLength(20);
        });

        builder.Property(u => u.CognitoUserId)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(u => u.CognitoUserId).IsUnique();

        builder.HasMany(u => u.Memberships)
            .WithOne()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(u => u.Memberships).AutoInclude();
    }
}
