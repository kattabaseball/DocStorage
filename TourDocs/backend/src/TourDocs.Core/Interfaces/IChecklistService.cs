using TourDocs.Core.DTOs.Checklists;

namespace TourDocs.Core.Interfaces;

/// <summary>
/// Service contract for checklist management operations.
/// </summary>
public interface IChecklistService
{
    Task<ChecklistResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ChecklistResponse>> GetByCountryAsync(string countryCode);
    Task<IReadOnlyList<ChecklistResponse>> GetByOrganizationAsync(Guid organizationId);
    Task<IReadOnlyList<ChecklistResponse>> GetSystemChecklistsAsync();
    Task<ChecklistResponse> CreateAsync(CreateChecklistRequest request);
    Task<ChecklistResponse> UpdateAsync(Guid id, UpdateChecklistRequest request);
    Task DeleteAsync(Guid id);
    Task<ChecklistItemResponse> AddItemAsync(Guid checklistId, CreateChecklistItemRequest request);
    Task RemoveItemAsync(Guid checklistId, Guid itemId);
}
