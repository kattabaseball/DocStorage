using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.DocumentRequests;

/// <summary>
/// Request DTO for creating a document request.
/// </summary>
public class CreateDocumentRequestDto
{
    public Guid? CaseId { get; set; }

    [Required]
    public Guid MemberId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DocumentType { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? FormatRequirements { get; set; }

    public Urgency Urgency { get; set; } = Urgency.Normal;

    [MaxLength(2000)]
    public string? Notes { get; set; }
}
