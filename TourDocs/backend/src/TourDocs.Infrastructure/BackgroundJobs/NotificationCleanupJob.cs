using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Infrastructure.BackgroundJobs;

/// <summary>
/// Hangfire recurring job that cleans up old read notifications weekly.
/// </summary>
public class NotificationCleanupJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationCleanupJob> _logger;

    public NotificationCleanupJob(IServiceScopeFactory scopeFactory, ILogger<NotificationCleanupJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Notification cleanup started");

        using var scope = _scopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var cutoff = DateTime.UtcNow.AddDays(-90);
        var oldNotifications = await unitOfWork.Notifications.FindAsync(
            n => n.IsRead && n.CreatedAt < cutoff);

        var count = 0;
        foreach (var notification in oldNotifications)
        {
            unitOfWork.Notifications.Remove(notification);
            count++;
        }

        if (count > 0)
        {
            await unitOfWork.SaveChangesAsync();
        }

        _logger.LogInformation("Notification cleanup completed. Removed {Count} old notifications", count);
    }
}
