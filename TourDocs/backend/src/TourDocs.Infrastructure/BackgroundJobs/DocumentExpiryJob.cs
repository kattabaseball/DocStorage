using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Enums;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire recurring job that checks for expiring and expired documents daily.
/// Sends notification emails and updates document status.
/// </summary>
public class DocumentExpiryJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DocumentExpiryJob> _logger;

    public DocumentExpiryJob(IServiceScopeFactory scopeFactory, ILogger<DocumentExpiryJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Document expiry check started");

        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // Mark expired documents
        var expiredDocs = await unitOfWork.Documents.FindAsync(
            d => d.ExpiryDate != null &&
                 d.ExpiryDate <= DateTime.UtcNow &&
                 d.Status == DocumentStatus.Verified);

        var expiredCount = 0;
        foreach (var doc in expiredDocs)
        {
            doc.Status = DocumentStatus.Expired;
            doc.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Documents.Update(doc);
            expiredCount++;
        }

        if (expiredCount > 0)
        {
            await unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Marked {Count} documents as expired", expiredCount);
        }

        // Send warnings for documents expiring in 30, 14, 7 days
        var warningThresholds = new[] { 30, 14, 7 };
        foreach (var days in warningThresholds)
        {
            var threshold = DateTime.UtcNow.AddDays(days);
            var warningDocs = await unitOfWork.Documents.FindAsync(
                d => d.ExpiryDate != null &&
                     d.ExpiryDate.Value.Date == threshold.Date &&
                     d.Status == DocumentStatus.Verified);

            foreach (var doc in warningDocs)
            {
                if (doc.Member?.Email != null)
                {
                    var memberName = $"{doc.Member.LegalFirstName} {doc.Member.LegalLastName}".Trim();
                    await emailService.SendDocumentExpiryWarningAsync(
                        doc.Member.Email, memberName, doc.Title, days);
                }
            }
        }

        _logger.LogInformation("Document expiry check completed. Expired: {Count}", expiredCount);
    }
}
