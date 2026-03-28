using TourDocs.Domain.Enums;

namespace TourDocs.Core.DTOs.Documents;

/// <summary>
/// Request DTO for uploading a new document.
/// </summary>
public class UploadDocumentRequest
{
    public Guid MemberId { get; set; }
    public DocumentCategory Category { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public bool IsHardCopyNeeded { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public Stream FileStream { get; set; } = Stream.Null;
}
