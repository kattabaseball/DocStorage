namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for sending emails.
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
    Task SendAsync(string to, string subject, string htmlBody, string? fromName);
    Task SendWelcomeEmailAsync(string to, string fullName, string organizationName);
    Task SendDocumentExpiryWarningAsync(string to, string fullName, string documentTitle, int daysUntilExpiry);
    Task SendInvitationEmailAsync(string to, string organizationName, string inviterName);
    Task SendPasswordResetAsync(string to, string fullName, string resetLink);
}
