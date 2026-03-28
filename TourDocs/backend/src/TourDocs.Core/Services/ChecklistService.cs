using AutoMapper;
using Microsoft.Extensions.Logging;
using TourDocs.Core.DTOs.Checklists;
using TourDocs.Core.Exceptions;
using TourDocs.Core.Interfaces;
using TourDocs.Domain.Entities;
using TourDocs.Domain.Interfaces;

namespace TourDocs.Core.Services;

/// <summary>
/// Business logic service for managing document checklists.
/// </summary>
public class ChecklistService : IChecklistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ChecklistService> _logger;

    public ChecklistService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ITenantContext tenantContext,
        ILogger<ChecklistService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task<ChecklistResponse> GetByIdAsync(Guid id)
    {
        var checklist = await _unitOfWork.Checklists.GetByIdAsync(id)
            ?? throw new NotFoundException("Checklist", id);

        var items = await _unitOfWork.ChecklistItems.FindAsync(i => i.ChecklistId == id);
        var response = _mapper.Map<ChecklistResponse>(checklist);
        response.Items = _mapper.Map<IReadOnlyList<ChecklistItemResponse>>(items.OrderBy(i => i.SortOrder).ToList());
        return response;
    }

    public async Task<IReadOnlyList<ChecklistResponse>> GetByCountryAsync(string countryCode)
    {
        var checklists = await _unitOfWork.Checklists.FindAsync(
            c => c.CountryCode == countryCode && c.IsActive);
        return _mapper.Map<IReadOnlyList<ChecklistResponse>>(checklists);
    }

    public async Task<IReadOnlyList<ChecklistResponse>> GetByOrganizationAsync(Guid organizationId)
    {
        var checklists = await _unitOfWork.Checklists.FindAsync(
            c => c.OrganizationId == organizationId && c.IsActive);
        return _mapper.Map<IReadOnlyList<ChecklistResponse>>(checklists);
    }

    public async Task<IReadOnlyList<ChecklistResponse>> GetSystemChecklistsAsync()
    {
        var checklists = await _unitOfWork.Checklists.FindAsync(c => c.IsSystem && c.IsActive);
        return _mapper.Map<IReadOnlyList<ChecklistResponse>>(checklists);
    }

    public async Task<ChecklistResponse> CreateAsync(CreateChecklistRequest request)
    {
        var checklist = _mapper.Map<Checklist>(request);
        checklist.Id = Guid.NewGuid();
        checklist.OrganizationId = _tenantContext.OrganizationId;
        checklist.IsSystem = false;
        checklist.IsActive = true;
        checklist.Version = 1;

        await _unitOfWork.Checklists.AddAsync(checklist);

        var sortOrder = 0;
        foreach (var itemRequest in request.Items)
        {
            var item = _mapper.Map<ChecklistItem>(itemRequest);
            item.Id = Guid.NewGuid();
            item.ChecklistId = checklist.Id;
            item.SortOrder = sortOrder++;
            await _unitOfWork.ChecklistItems.AddAsync(item);
        }

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Checklist {ChecklistId} created for org {OrgId}",
            checklist.Id, _tenantContext.OrganizationId);

        return await GetByIdAsync(checklist.Id);
    }

    public async Task<ChecklistResponse> UpdateAsync(Guid id, UpdateChecklistRequest request)
    {
        var checklist = await _unitOfWork.Checklists.GetByIdAsync(id)
            ?? throw new NotFoundException("Checklist", id);

        checklist.Name = request.Name;
        checklist.ChecklistType = request.ChecklistType;
        checklist.Notes = request.Notes;
        checklist.IsActive = request.IsActive;
        checklist.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Checklists.Update(checklist);
        await _unitOfWork.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var checklist = await _unitOfWork.Checklists.GetByIdAsync(id)
            ?? throw new NotFoundException("Checklist", id);

        if (checklist.IsSystem)
        {
            throw new BusinessRuleException("System checklists cannot be deleted.");
        }

        checklist.IsActive = false;
        _unitOfWork.Checklists.Update(checklist);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ChecklistItemResponse> AddItemAsync(Guid checklistId, CreateChecklistItemRequest request)
    {
        var checklist = await _unitOfWork.Checklists.GetByIdAsync(checklistId)
            ?? throw new NotFoundException("Checklist", checklistId);

        var item = _mapper.Map<ChecklistItem>(request);
        item.Id = Guid.NewGuid();
        item.ChecklistId = checklistId;

        await _unitOfWork.ChecklistItems.AddAsync(item);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ChecklistItemResponse>(item);
    }

    public async Task RemoveItemAsync(Guid checklistId, Guid itemId)
    {
        var item = await _unitOfWork.ChecklistItems.GetByIdAsync(itemId)
            ?? throw new NotFoundException("ChecklistItem", itemId);

        if (item.ChecklistId != checklistId)
        {
            throw new BusinessRuleException("Item does not belong to the specified checklist.");
        }

        _unitOfWork.ChecklistItems.Remove(item);
        await _unitOfWork.SaveChangesAsync();
    }
}
