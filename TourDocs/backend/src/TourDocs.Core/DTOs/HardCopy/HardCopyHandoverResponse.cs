namespace TourDocs.Core.DTOs.HardCopy;

/// <summary>
/// Response DTO for a hard copy handover event.
/// </summary>
public class HardCopyHandoverResponse
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public string FromUserName { get; set; } = string.Empty;
    public Guid ToUserId { get; set; }
    public string ToUserName { get; set; } = string.Empty;
    public string FromRole { get; set; } = string.Empty;
    public string ToRole { get; set; } = string.Empty;
    public string HandoverType { get; set; } = string.Empty;
    public string? ConfirmationMethod { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordedAt { get; set; }
}
