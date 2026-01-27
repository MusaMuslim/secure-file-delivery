namespace SecureFileDelivery.Application.Interfaces;

// Service for storing and retrieving files
public interface IFileStorageService
{
    // Saves a file to storage and returns the storage path
    Task<string> SaveFileAsync(Stream fileStream, string fileName);

    // Retrieves a file from storage
    Task<Stream> GetFileAsync(string storagePath);

    // Deletes a file from storage
    Task DeleteFileAsync(string storagePath);

    // Checks if a file exists in storage
    Task<bool> FileExistsAsync(string storagePath);

    // Gets the file size in bytes
    Task<long> GetFileSizeAsync(string storagePath);
}