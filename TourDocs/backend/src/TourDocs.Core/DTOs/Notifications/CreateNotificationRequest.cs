using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Notifications;

/// <summary>
/// Request DTO for creating a notification.
/// </summary>
public class CreateNotificationRequest
{
    [Required]
    public Guid UserId { get; set; }

    public Guid? OrganizationId { get; set; }

    [Required]
    public NotificationType Type { get; set; }

    [Required]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? EntityType { get; set; }

    public Guid? EntityId { get; set; }

    [MaxLength(50)]
    public string? Channel { get; set; }
}
