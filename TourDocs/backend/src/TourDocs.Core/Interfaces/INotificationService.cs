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

    /// <summary>
    /// Sends a notification informing a user that a document has been verified.
    /// </summary>
    Task SendDocumentVerifiedAsync(Guid organizationId, Guid userId, string documentTitle, string memberName);

    /// <summary>
    /// Sends a notification informing a user that a document has been rejected.
    /// </summary>
    Task SendDocumentRejectedAsync(Guid organizationId, Guid userId, string documentTitle, string reason);

    /// <summary>
    /// Sends a notification warning a user that a document is about to expire.
    /// </summary>
    Task SendDocumentExpiringAsync(Guid organizationId, Guid userId, string documentTitle, int daysLeft);

    /// <summary>
    /// Deletes read notifications that are older than the specified cutoff date.
    /// </summary>
    Task DeleteOldNotificationsAsync(DateTime cutoff);
}
