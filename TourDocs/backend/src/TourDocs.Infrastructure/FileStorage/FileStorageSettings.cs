namespace TourDocs.Infrastructure.FileStorage;

/// <summary>
/// Configuration settings for local file storage.
/// </summary>
public class FileStorageSettings
{
    /// <summary>
    /// Root directory path for storing files.
    /// </summary>
    public string BasePath { get; set; } = "Storage";

    /// <summary>
    /// Base URL for serving files (used for generating download links).
    /// </summary>
    public string BaseUrl { get; set; } = "/files";

    /// <summary>
    /// Maximum allowed file size in bytes (default: 50MB).
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 52428800;
}
