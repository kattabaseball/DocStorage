namespace TourDocs.Domain.Enums;

/// <summary>
/// Tracks the physical location and handover status of a hard copy document.
/// </summary>
public enum HardCopyStatus
{
    WithMember = 0,
    Requested = 1,
    Acknowledged = 2,
    CollectedByManager = 3,
    HandedToHandler = 4,
    AtAuthority = 5,
    ReturnedToManager = 6,
    ReturnedToMember = 7
}
