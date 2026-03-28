namespace TourDocs.Domain.Enums;

/// <summary>
/// Types of notifications that can be sent to users.
/// </summary>
public enum NotificationType
{
    DocumentExpiring = 0,
    DocumentUploaded = 1,
    DocumentVerified = 2,
    DocumentRejected = 3,
    CaseCreated = 4,
    AccessGranted = 5,
    AccessRevoked = 6,
    HardCopyStatusChanged = 7,
    DocumentRequested = 8,
    CaseComplete = 9
}
