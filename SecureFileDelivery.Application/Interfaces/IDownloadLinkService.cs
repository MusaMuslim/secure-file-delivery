using SecureFileDelivery.Application.DTOs;

namespace SecureFileDelivery.Application.Interfaces;

// Service for managing download links
public interface IDownloadLinkService
{
    // Generates a new download link for a statement
    Task<DownloadLinkResponse> GenerateLinkAsync(GenerateLinkRequest request, string baseUrl);

    // Gets a download link by token and validates it
    Task<DownloadLinkResponse> GetLinkByTokenAsync(string token);

    // Downloads a file using a token (records access)
    Task<(Stream fileStream, string fileName, string contentType)> DownloadFileAsync(string token, string ipAddress);

    // Gets all links for a statement
    Task<IEnumerable<DownloadLinkResponse>> GetLinksByStatementIdAsync(Guid statementId);

    // Revokes a download link
    Task RevokeLinkAsync(Guid linkId);
}