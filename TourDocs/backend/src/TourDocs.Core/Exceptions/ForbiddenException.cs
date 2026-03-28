namespace TourDocs.Core.Exceptions;

/// <summary>
/// Exception thrown when a user is authenticated but not authorized for an action.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You do not have permission to perform this action.")
        : base(message)
    {
    }
}
