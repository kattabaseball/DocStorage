using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Notifications;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Enums;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing notifications.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NotificationResponse>> GetByUserAsync(Guid userId, int page = 1, int pageSize = 20)
    {
        var allNotifications = await _unitOfWork.Notifications.FindAsync(n => n.UserId == userId);
        var paged = allNotifications
            .OrderByDescending(n => n.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return _mapper.Map<IReadOnlyList<NotificationResponse>>(paged);
    }

    public async Task<IReadOnlyList<NotificationResponse>> GetUnreadAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsRead);

        return _mapper.Map<IReadOnlyList<NotificationResponse>>(
            notifications.OrderByDescending(n => n.SentAt).ToList());
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _unitOfWork.Notifications.CountAsync(
            n => n.UserId == userId && !n.IsRead);
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId)
            ?? throw new NotFoundException("Notification", notificationId);

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        _unitOfWork.Notifications.Update(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(Guid userId)
    {
        var unread = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsRead);

        var now = DateTime.UtcNow;
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
            _unitOfWork.Notifications.Update(notification);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendAsync(CreateNotificationRequest request)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            OrganizationId = request.OrganizationId,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            EntityType = request.EntityType,
            EntityId = request.EntityId,
            Channel = request.Channel ?? "InApp",
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Notification {NotifId} of type {Type} sent to user {UserId}",
            notification.Id, request.Type, request.UserId);
    }

    public async Task SendDocumentVerifiedAsync(Guid organizationId, Guid userId, string documentTitle, string memberName)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrganizationId = organizationId,
            Type = NotificationType.DocumentVerified,
            Title = "Document Verified",
            Message = $"The document \"{documentTitle}\" for {memberName} has been verified.",
            EntityType = "Document",
            Link = $"/documents?member={Uri.EscapeDataString(memberName)}",
            Channel = "InApp",
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document verified notification {NotifId} sent to user {UserId} for document \"{DocumentTitle}\"",
            notification.Id, userId, documentTitle);
    }

    public async Task SendDocumentRejectedAsync(Guid organizationId, Guid userId, string documentTitle, string reason)
    {
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrganizationId = organizationId,
            Type = NotificationType.DocumentRejected,
            Title = "Document Rejected",
            Message = $"The document \"{documentTitle}\" has been rejected. Reason: {reason}",
            EntityType = "Document",
            Link = "/documents",
            Channel = "InApp",
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document rejected notification {NotifId} sent to user {UserId} for document \"{DocumentTitle}\"",
            notification.Id, userId, documentTitle);
    }

    public async Task SendDocumentExpiringAsync(Guid organizationId, Guid userId, string documentTitle, int daysLeft)
    {
        var daysLabel = daysLeft == 1 ? "1 day" : $"{daysLeft} days";
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrganizationId = organizationId,
            Type = NotificationType.DocumentExpiring,
            Title = "Document Expiring Soon",
            Message = $"The document \"{documentTitle}\" will expire in {daysLabel}.",
            EntityType = "Document",
            Link = "/documents",
            Channel = "InApp",
            IsRead = false,
            SentAt = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Document expiring notification {NotifId} sent to user {UserId} for document \"{DocumentTitle}\" ({DaysLeft} days left)",
            notification.Id, userId, documentTitle, daysLeft);
    }

    public async Task DeleteOldNotificationsAsync(DateTime cutoff)
    {
        var oldReadNotifications = await _unitOfWork.Notifications.FindAsync(
            n => n.IsRead && n.SentAt < cutoff);

        if (!oldReadNotifications.Any())
        {
            _logger.LogInformation("No old read notifications found before cutoff {Cutoff}", cutoff);
            return;
        }

        foreach (var notification in oldReadNotifications)
        {
            _unitOfWork.Notifications.Remove(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Deleted {Count} old read notifications sent before {Cutoff}",
            oldReadNotifications.Count, cutoff);
    }
}
