using TourDocs.Core.DTOs.DocumentRequests;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for document request operations.
/// </summary>
public interface IDocumentRequestService
{
    Task<DocumentRequestResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<DocumentRequestResponse>> GetByMemberAsync(Guid memberId);
    Task<IReadOnlyList<DocumentRequestResponse>> GetByCaseAsync(Guid caseId);
    Task<DocumentRequestResponse> CreateAsync(CreateDocumentRequestDto request);
    Task<DocumentRequestResponse> UpdateStatusAsync(Guid id, UpdateDocumentRequestStatusDto request);
    Task<DocumentRequestResponse> FulfillAsync(Guid id, Guid documentId);
    Task<DocumentRequestResponse> DeclineAsync(Guid id, string reason);
}
