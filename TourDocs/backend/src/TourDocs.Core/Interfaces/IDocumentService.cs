using TourDocs.Core.DTOs.Common;
using TourDocs.Core.DTOs.Documents;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for document management operations.
/// </summary>
public interface IDocumentService
{
    Task<PagedResponse<DocumentResponse>> GetByOrganizationAsync(Guid organizationId, PagedRequest request);
    Task<DocumentResponse> UploadAsync(UploadDocumentRequest request);
    Task<DocumentResponse> GetByIdAsync(Guid id);
    Task<DocumentResponse> VerifyAsync(Guid id, string? notes = null);
    Task<DocumentResponse> RejectAsync(Guid id, string? reason = null);
    Task<IReadOnlyList<DocumentResponse>> GetExpiringDocumentsAsync(int daysThreshold = 30);
    Task<Stream> DownloadAsync(Guid id);
    Task<DocumentResponse> ReuploadAsync(Guid documentId, UploadDocumentRequest request);
}
