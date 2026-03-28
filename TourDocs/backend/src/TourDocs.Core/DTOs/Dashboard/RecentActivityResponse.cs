namespace TourDocs.Core.DTOs.Dashboard;

/// <summary>
/// Response DTO for recent activity on the dashboard.
/// </summary>
public class RecentActivityResponse
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public string? UserName { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; }
}
