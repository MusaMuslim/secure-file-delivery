using Microsoft.Extensions.Configuration;
using SecureFileDelivery.Application.Interfaces;

namespace SecureFileDelivery.Infrastructure.Services;

// Local file system storage service
// In production, this could be replaced with AWS S3, Azure Blob Storage, etc.
public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public FileStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["FileStorage:RootPath"] ?? "FileStorage";

        // Create storage directory if it doesn't exist
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        // Generate unique filename to avoid collisions
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(_storagePath, uniqueFileName);

        // Save file to disk
        await using var fileStreamOutput = File.Create(fullPath);
        await fileStream.CopyToAsync(fileStreamOutput);

        return uniqueFileName; // Return relative path
    }

    public async Task<Stream> GetFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(_storagePath, storagePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"File not found: {storagePath}");
        }

        var memoryStream = new MemoryStream();
        await using var fileStream = File.OpenRead(fullPath);
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public Task DeleteFileAsync(string storagePath)
    {
        var fullPath = Path.Combine(_storagePath, storagePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<bool> FileExistsAsync(string storagePath)
    {
        var fullPath = Path.Combine(_storagePath, storagePath);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<long> GetFileSizeAsync(string storagePath)
    {
        var fullPath = Path.Combine(_storagePath, storagePath);
        var fileInfo = new FileInfo(fullPath);
        return Task.FromResult(fileInfo.Length);
    }
}