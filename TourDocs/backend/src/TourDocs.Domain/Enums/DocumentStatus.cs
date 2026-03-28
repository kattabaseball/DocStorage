namespace TourDocs.Domain.Enums;

/// <summary>
/// Lifecycle status of a document.
/// </summary>
public enum DocumentStatus
{
    Uploaded = 0,
    UnderReview = 1,
    Verified = 2,
    Rejected = 3,
    Expired = 4,
    Archived = 5
}
