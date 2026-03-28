using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using TourDocs.Core.Interfaces;

namespace TourDocs.API.Filters;

/// <summary>
/// Action filter that automatically logs all API actions to the audit log.
/// </summary>
public class AuditActionFilter : IAsyncActionFilter
{
    private readonly IAuditService _auditService;
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(IAuditService auditService, ILogger<AuditActionFilter> logger)
    {
        _auditService = auditService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = await next();

        if (result.Exception != null)
        {
            return;
        }

        try
        {
            var userId = GetUserId(context.HttpContext);
            var orgId = GetOrganizationId(context.HttpContext);
            var action = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.HttpContext.Request.Headers.UserAgent.ToString();

            await _auditService.LogAsync(
                orgId, userId, action,
                ipAddress: ipAddress, userAgent: userAgent);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to record audit log for action");
        }
    }

    private static Guid? GetUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static Guid? GetOrganizationId(HttpContext context)
    {
        var orgIdClaim = context.User.FindFirstValue("org_id");
        return Guid.TryParse(orgIdClaim, out var orgId) ? orgId : null;
    }
}
