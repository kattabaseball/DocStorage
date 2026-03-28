using TourDocs.Core.DTOs.HardCopy;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for hard copy document chain-of-custody operations.
/// </summary>
public interface IHardCopyService
{
    Task<IReadOnlyList<HardCopyRequestResponse>> GetByOrganizationAsync(Guid organizationId);
    Task<HardCopyRequestResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<HardCopyRequestResponse>> GetByCaseAsync(Guid caseId);
    Task<HardCopyRequestResponse> CreateRequestAsync(CreateHardCopyRequestDto request);
    Task<HardCopyRequestResponse> UpdateStatusAsync(Guid id, UpdateHardCopyStatusRequest request);
    Task<HardCopyHandoverResponse> RecordHandoverAsync(Guid hardCopyRequestId, CreateHardCopyHandoverRequest request);
    Task<IReadOnlyList<HardCopyHandoverResponse>> GetHandoverHistoryAsync(Guid hardCopyRequestId);
}
