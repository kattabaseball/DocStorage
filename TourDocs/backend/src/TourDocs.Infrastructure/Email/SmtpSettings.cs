namespace TourDocs.Infrastructure.Email;

/// <summary>
/// Configuration for SMTP email service.
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = true;
    public string FromEmail { get; set; } = "noreply@tourdocs.com";
    public string FromName { get; set; } = "TourDocs";
}
