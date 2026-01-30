using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable("sessions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.RefreshTokenHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasIndex(s => s.RefreshTokenHash)
            .IsUnique();

        builder.Property(s => s.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(45);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.ExpiresAt);

        // Composite index for active session queries
        builder.HasIndex(s => new { s.UserId, s.IsRevoked, s.ExpiresAt })
            .HasDatabaseName("IX_sessions_UserId_IsRevoked_ExpiresAt");

        builder.HasOne(s => s.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
