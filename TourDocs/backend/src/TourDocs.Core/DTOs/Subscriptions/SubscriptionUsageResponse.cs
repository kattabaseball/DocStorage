namespace TourDocs.Core.DTOs.Subscriptions;

/// <summary>
/// Response DTO for current subscription usage statistics.
/// </summary>
public class SubscriptionUsageResponse
{
    public int CurrentMembers { get; set; }
    public int MaxMembers { get; set; }
    public int CurrentCasesThisMonth { get; set; }
    public int MaxCasesMonthly { get; set; }
    public long CurrentStorageBytes { get; set; }
    public long MaxStorageBytes { get; set; }
    public double MemberUsagePercent => MaxMembers > 0 ? (double)CurrentMembers / MaxMembers * 100 : 0;
    public double StorageUsagePercent => MaxStorageBytes > 0 ? (double)CurrentStorageBytes / MaxStorageBytes * 100 : 0;
}
