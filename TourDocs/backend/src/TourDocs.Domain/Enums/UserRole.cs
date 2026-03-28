namespace TourDocs.Domain.Enums;

/// <summary>
/// Roles that users can hold within an organization.
/// </summary>
public enum UserRole
{
    OrgOwner = 0,
    OrgMember = 1,
    CaseManager = 2,
    DocumentHandler = 3,
    Member = 4
}
