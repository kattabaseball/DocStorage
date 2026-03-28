using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Notifications;

/// <summary>
/// Response DTO for a notification.
/// </summary>
public class NotificationResponse
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Link { get; set; }
}
