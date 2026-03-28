using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TourDocs.API.Hubs;

/// <summary>
/// SignalR hub for real-time notifications and document status updates.
/// Users join groups based on their organization and user IDs.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var orgId = Context.User?.FindFirstValue("org_id");

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation("User {UserId} connected to NotificationHub", userId);
        }

        if (!string.IsNullOrEmpty(orgId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"org_{orgId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Marks a notification as read from the client side.
    /// </summary>
    public async Task MarkNotificationRead(Guid notificationId)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogDebug("User {UserId} marked notification {NotifId} as read", userId, notificationId);
        await Clients.Caller.SendAsync("NotificationMarkedRead", notificationId);
    }
}
