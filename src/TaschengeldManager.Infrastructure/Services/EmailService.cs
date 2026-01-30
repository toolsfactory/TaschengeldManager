using Microsoft.Extensions.Logging;
using TaschengeldManager.Core.Interfaces.Services;

namespace TaschengeldManager.Infrastructure.Services;

/// <summary>
/// Stub email service that logs emails. Replace with actual email provider in production.
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendInvitationEmailAsync(string toEmail, string inviterName, string familyName, string invitationLink, string role, CancellationToken cancellationToken = default)
    {
        // Note: Sensitive link intentionally not logged for security reasons
        _logger.LogInformation(
            "[EMAIL] Invitation sent to {Email} from {Inviter} for family {Family} as {Role}",
            MaskEmail(toEmail), inviterName, familyName, role);

        // TODO: Implement actual email sending (e.g., SendGrid, SMTP, etc.)
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        // Note: Sensitive link intentionally not logged for security reasons
        _logger.LogInformation(
            "[EMAIL] Password reset sent to {Email}",
            MaskEmail(toEmail));

        // TODO: Implement actual email sending
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string toEmail, string nickname, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[EMAIL] Welcome email sent to {Email} for {Nickname}",
            MaskEmail(toEmail), nickname);

        // TODO: Implement actual email sending
        return Task.CompletedTask;
    }

    /// <summary>
    /// Masks an email address for safe logging (e.g., "user@example.com" -> "u***@e***.com").
    /// </summary>
    private static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            return "[empty]";

        var atIndex = email.IndexOf('@');
        if (atIndex < 1)
            return "***";

        var localPart = email[..atIndex];
        var domainPart = email[(atIndex + 1)..];

        var maskedLocal = localPart.Length > 1
            ? $"{localPart[0]}***"
            : "***";

        var dotIndex = domainPart.LastIndexOf('.');
        var maskedDomain = dotIndex > 1
            ? $"{domainPart[0]}***{domainPart[dotIndex..]}"
            : "***";

        return $"{maskedLocal}@{maskedDomain}";
    }
}
