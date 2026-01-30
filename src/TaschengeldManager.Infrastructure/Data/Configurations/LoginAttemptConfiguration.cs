using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaschengeldManager.Core.Entities;

namespace TaschengeldManager.Infrastructure.Data.Configurations;

public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.ToTable("login_attempts");

        builder.HasKey(la => la.Id);

        builder.Property(la => la.Identifier)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(la => la.Identifier);
        builder.HasIndex(la => la.CreatedAt);
        builder.HasIndex(la => la.UserId);

        builder.Property(la => la.FailureReason)
            .HasMaxLength(200);

        builder.Property(la => la.IpAddress)
            .HasMaxLength(45);

        builder.Property(la => la.UserAgent)
            .HasMaxLength(500);

        builder.Property(la => la.MfaMethod)
            .HasMaxLength(20);

        builder.Property(la => la.Location)
            .HasMaxLength(100);

        builder.HasOne(la => la.User)
            .WithMany(u => u.LoginAttempts)
            .HasForeignKey(la => la.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
