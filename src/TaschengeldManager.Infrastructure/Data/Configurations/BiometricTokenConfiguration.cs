using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class BiometricTokenConfiguration : IEntityTypeConfiguration<BiometricToken>
{
    public void Configure(EntityTypeBuilder<BiometricToken> builder)
    {
        builder.ToTable("biometric_tokens");

        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.DeviceId)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(bt => bt.DeviceId);

        builder.Property(bt => bt.DeviceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(bt => bt.Platform)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(bt => bt.BiometryType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(bt => bt.TokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasOne(bt => bt.User)
            .WithMany(u => u.BiometricTokens)
            .HasForeignKey(bt => bt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
