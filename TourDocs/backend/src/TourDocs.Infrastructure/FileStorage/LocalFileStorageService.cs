using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TourDocs.Core.Interfaces;

namespace TourDocs.Infrastructure.FileStorage;

/// <summary>
/// File storage service implementation using the local file system.
/// Stores files in a configurable directory with organization/member/category folder structure.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(
        IOptions<FileStorageSettings> settings,
        ILogger<LocalFileStorageService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (!Directory.Exists(_settings.BasePath))
        {
            Directory.CreateDirectory(_settings.BasePath);
        }
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var sanitizedFileName = SanitizeFileName(fileName);
        var directoryPath = Path.Combine(_settings.BasePath, folder);

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(folder, sanitizedFileName);
        var fullPath = Path.Combine(_settings.BasePath, filePath);

        // Handle duplicate file names
        if (File.Exists(fullPath))
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(sanitizedFileName);
            var extension = Path.GetExtension(sanitizedFileName);
            sanitizedFileName = $"{nameWithoutExt}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
            filePath = Path.Combine(folder, sanitizedFileName);
            fullPath = Path.Combine(_settings.BasePath, filePath);
        }

        await using var fileStreamWriter = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fileStreamWriter, cancellationToken);

        _logger.LogInformation("File uploaded: {FilePath} ({Size} bytes)", filePath, fileStream.Length);

        return filePath.Replace("\\", "/");
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.BasePath, filePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        var memoryStream = new MemoryStream();
        await using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return memoryStream;
    }

    /// <inheritdoc />
    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.BasePath, filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            _logger.LogInformation("File deleted: {FilePath}", filePath);
        }
        else
        {
            _logger.LogWarning("File not found for deletion: {FilePath}", filePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_settings.BasePath, filePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    /// <inheritdoc />
    public string GetFileUrl(string filePath)
    {
        return $"{_settings.BaseUrl}/{filePath.Replace("\\", "/")}";
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalidChars.Contains(c))
            .ToArray());

        return string.IsNullOrWhiteSpace(sanitized)
            ? $"file_{Guid.NewGuid():N}"
            : sanitized;
    }
}
