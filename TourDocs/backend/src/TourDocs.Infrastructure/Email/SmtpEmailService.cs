using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using TourDocs.Core.Interfaces;

namespace TourDocs.Infrastructure.Email;

/// <summary>
/// Email service using SMTP via MailKit (free, no paid dependencies).
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<SmtpSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        await SendAsync(to, subject, htmlBody, null);
    }

    public async Task SendAsync(string to, string subject, string htmlBody, string? fromName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName ?? _settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {To} with subject '{Subject}'", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send email to {To}. SMTP may not be configured.", to);
        }
    }

    public async Task SendWelcomeEmailAsync(string to, string fullName, string organizationName)
    {
        var html = $"""
            <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #1565C0;">Welcome to TourDocs!</h2>
                <p>Hi {fullName},</p>
                <p>Your account has been set up for <strong>{organizationName}</strong>.</p>
                <p>You can now start managing your organization's documents, members, and cases.</p>
                <p style="color: #757575; font-size: 12px;">— The TourDocs Team</p>
            </div>
            """;
        await SendAsync(to, $"Welcome to TourDocs - {organizationName}", html);
    }

    public async Task SendDocumentExpiryWarningAsync(string to, string fullName, string documentTitle, int daysUntilExpiry)
    {
        var urgencyColor = daysUntilExpiry <= 7 ? "#EF5350" : "#FFA726";
        var html = $"""
            <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: {urgencyColor};">Document Expiring Soon</h2>
                <p>Hi {fullName},</p>
                <p>The document <strong>{documentTitle}</strong> will expire in <strong>{daysUntilExpiry} days</strong>.</p>
                <p>Please arrange to renew or update this document before it expires.</p>
                <p style="color: #757575; font-size: 12px;">— TourDocs</p>
            </div>
            """;
        await SendAsync(to, $"Document Expiring: {documentTitle} ({daysUntilExpiry} days)", html);
    }

    public async Task SendInvitationEmailAsync(string to, string organizationName, string inviterName)
    {
        var html = $"""
            <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #1565C0;">You're Invited!</h2>
                <p><strong>{inviterName}</strong> has invited you to join <strong>{organizationName}</strong> on TourDocs.</p>
                <p>Sign up to start collaborating on document management.</p>
                <p style="color: #757575; font-size: 12px;">— TourDocs</p>
            </div>
            """;
        await SendAsync(to, $"Invitation to join {organizationName} on TourDocs", html);
    }

    public async Task SendPasswordResetAsync(string to, string fullName, string resetLink)
    {
        var html = $"""
            <div style="font-family: sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #1565C0;">Password Reset</h2>
                <p>Hi {fullName},</p>
                <p>We received a request to reset your password. Click the link below:</p>
                <p><a href="{resetLink}" style="color: #1565C0;">Reset Password</a></p>
                <p>If you didn't request this, ignore this email.</p>
                <p style="color: #757575; font-size: 12px;">— TourDocs</p>
            </div>
            """;
        await SendAsync(to, "TourDocs - Password Reset", html);
    }
}
