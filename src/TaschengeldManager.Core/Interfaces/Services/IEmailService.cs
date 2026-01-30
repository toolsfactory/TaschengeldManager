namespace TaschengeldManager.Core.Interfaces.Services;

/// <summary>
/// Service interface for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send family invitation email.
    /// </summary>
    Task SendInvitationEmailAsync(string toEmail, string inviterName, string familyName, string invitationLink, string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send password reset email.
    /// </summary>
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send welcome email after registration.
    /// </summary>
    Task SendWelcomeEmailAsync(string toEmail, string nickname, CancellationToken cancellationToken = default);
}
