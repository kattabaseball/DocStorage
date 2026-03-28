using TourDocs.Core.DTOs.Notifications;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for notification management and delivery.
/// </summary>
public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> GetByUserAsync(Guid userId, int page = 1, int pageSize = 20);
    Task<IReadOnlyList<NotificationResponse>> GetUnreadAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task SendAsync(CreateNotificationRequest request);
}
