namespace TourDocs.Domain.Enums;

/// <summary>
/// Lifecycle status of a case from creation through completion.
/// </summary>
public enum CaseStatus
{
    Draft = 0,
    Active = 1,
    DocsComplete = 2,
    Submitted = 3,
    Completed = 4,
    Cancelled = 5
}
