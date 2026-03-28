namespace TourDocs.Core.Interfaces;

/// <summary>
/// Abstraction for file storage operations enabling local or cloud storage backends.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Uploads a file and returns the stored file path.
    /// </summary>
    Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file and returns its content stream.
    /// </summary>
    Task<Stream> DownloadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file at the given path.
    /// </summary>
    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a file exists at the given path.
    /// </summary>
    Task<bool> ExistsAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a URL or relative path for accessing the file.
    /// </summary>
    string GetFileUrl(string filePath);
}
