namespace TourDocs.Core.DTOs.Common;

/// <summary>
/// Standard pagination request parameters.
/// </summary>
public class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public string? SearchTerm { get; set; }
}
