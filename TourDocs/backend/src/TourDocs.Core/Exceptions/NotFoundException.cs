namespace TourDocs.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested entity cannot be found.
/// </summary>
public class NotFoundException : Exception
{
    public string EntityName { get; }
    public object EntityKey { get; }

    public NotFoundException(string entityName, object entityKey)
        : base($"{entityName} with key '{entityKey}' was not found.")
    {
        EntityName = entityName;
        EntityKey = entityKey;
    }

    public NotFoundException(string message) : base(message)
    {
        EntityName = string.Empty;
        EntityKey = string.Empty;
    }
}
