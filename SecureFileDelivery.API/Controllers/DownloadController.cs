using Microsoft.AspNetCore.Mvc;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Exceptions;

namespace SecureFileDelivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DownloadController : ControllerBase
{
    private readonly IDownloadLinkService _downloadLinkService;
    private readonly ILogger<DownloadController> _logger;

    public DownloadController(
        IDownloadLinkService downloadLinkService,
        ILogger<DownloadController> logger)
    {
        _downloadLinkService = downloadLinkService;
        _logger = logger;
    }

    // Download a file using a secure token
    // This is the main endpoint customers use to download their statements
    [HttpGet("{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<IActionResult> DownloadFile(string token)
    {
        try
        {
            // Get client IP address for audit logging
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            _logger.LogInformation("Download request received for token {Token} from {IpAddress}", token, ipAddress);

            // Download file (validates token, records access, decrypts file)
            var (fileStream, fileName, contentType) = await _downloadLinkService.DownloadFileAsync(token, ipAddress);

            _logger.LogInformation("File {FileName} downloaded successfully via token {Token}", fileName, token);

            // Return file to client
            return File(fileStream, contentType, fileName);
        }
        catch (InvalidLinkException ex)
        {
            _logger.LogWarning("Invalid download attempt with token {Token}: {Message}", token, ex.Message);

            // Return 410 Gone for expired links (better than 404)
            if (ex.Message.Contains("expired", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status410Gone, new
                {
                    error = "This download link has expired",
                    message = ex.Message
                });
            }

            return NotFound(new
            {
                error = "Download link not found or invalid",
                message = ex.Message
            });
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogError("Statement not found for token {Token}: {Message}", token, ex.Message);
            return NotFound(new { error = "File not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file with token {Token}", token);
            return StatusCode(500, new { error = "An error occurred while downloading the file" });
        }
    }
}