namespace TourDocs.Domain.Enums;

/// <summary>
/// Status of a document request from creation through fulfillment or decline.
/// </summary>
public enum DocumentRequestStatus
{
    Requested = 0,
    Acknowledged = 1,
    InProgress = 2,
    Fulfilled = 3,
    Declined = 4
}
