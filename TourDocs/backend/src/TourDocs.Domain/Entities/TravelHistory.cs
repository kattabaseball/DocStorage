using TourDocs.Domain.Common;

namespace TourDocs.Domain.Entities;

/// <summary>
/// Records a member's travel history including visa and trip details.
/// </summary>
public class TravelHistory : BaseEntity
{
    public Guid MemberId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? VisaType { get; set; }
    public DateTime? EntryDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public string? Purpose { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Member Member { get; set; } = null!;
}
