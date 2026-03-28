using System.ComponentModel.DataAnnotations;
using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.DocumentRequests;

/// <summary>
/// Request DTO for updating a document request status.
/// </summary>
public class UpdateDocumentRequestStatusDto
{
    [Required]
    public DocumentRequestStatus Status { get; set; }

    [MaxLength(1000)]
    public string? DeclineReason { get; set; }

    public Guid? FulfilledDocumentId { get; set; }
}
