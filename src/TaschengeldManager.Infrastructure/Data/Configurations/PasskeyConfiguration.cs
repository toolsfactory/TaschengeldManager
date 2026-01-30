using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class PasskeyConfiguration : IEntityTypeConfiguration<Passkey>
{
    public void Configure(EntityTypeBuilder<Passkey> builder)
    {
        builder.ToTable("passkeys");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CredentialId)
            .IsRequired()
            .HasMaxLength(1024);

        builder.HasIndex(p => p.CredentialId)
            .IsUnique();

        builder.Property(p => p.PublicKey)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(p => p.DeviceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.AaGuid)
            .HasMaxLength(64);

        builder.Property(p => p.CredentialType)
            .HasMaxLength(20);

        builder.Property(p => p.Transports)
            .HasMaxLength(100);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Passkeys)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
