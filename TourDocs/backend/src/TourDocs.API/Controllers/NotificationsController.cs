using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Notifications;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Controllers;

/// <summary>
/// Manages user notifications.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Gets notifications for the current user with pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.GetByUserAsync(userId, page, pageSize);
        return Ok(ApiResponse<IReadOnlyList<NotificationResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets unread notifications for the current user.
    /// </summary>
    [HttpGet("unread")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnread()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _notificationService.GetUnreadAsync(userId);
        return Ok(ApiResponse<IReadOnlyList<NotificationResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Gets the unread notification count for the current user.
    /// </summary>
    [HttpGet("unread/count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(ApiResponse<int>.SuccessResult(count));
    }

    /// <summary>
    /// Marks a single notification as read.
    /// </summary>
    [HttpPut("{id:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Marks all notifications as read for the current user.
    /// </summary>
    [HttpPut("read-all")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}
