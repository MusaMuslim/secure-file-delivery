using Microsoft.AspNetCore.Mvc;
using SecureFileDelivery.Application.DTOs;
using SecureFileDelivery.Application.Interfaces;
using SecureFileDelivery.Domain.Exceptions;

namespace SecureFileDelivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatementsController : ControllerBase
{
    private readonly IStatementService _statementService;
    private readonly IDownloadLinkService _downloadLinkService;
    private readonly ILogger<StatementsController> _logger;

    public StatementsController(
        IStatementService statementService,
        IDownloadLinkService downloadLinkService,
        ILogger<StatementsController> logger)
    {
        _statementService = statementService;
        _downloadLinkService = downloadLinkService;
        _logger = logger;
    }

    // Upload a new statement file
    [HttpPost]
    [ProducesResponseType(typeof(StatementResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadStatement([FromForm] UploadStatementRequest request)
    {
        try
        {
            _logger.LogInformation("Uploading statement for account {AccountNumber}", request.AccountNumber);

            var result = await _statementService.UploadStatementAsync(request);

            return CreatedAtAction(
                nameof(GetStatement),
                new { id = result.Id },
                result
            );
        }
        catch (InvalidFileException ex)
        {
            _logger.LogWarning(ex, "Invalid file upload attempt");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading statement");
            return StatusCode(500, new { error = "An error occurred while uploading the statement" });
        }
    }

    // Get a statement by ID
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StatementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatement(Guid id)
    {
        try
        {
            var statement = await _statementService.GetStatementByIdAsync(id);

            if (statement == null)
                return NotFound(new { error = $"Statement {id} not found" });

            return Ok(statement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statement {StatementId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the statement" });
        }
    }

    // Get all statements for an account
    [HttpGet("account/{accountNumber}")]
    [ProducesResponseType(typeof(IEnumerable<StatementResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatementsByAccount(string accountNumber)
    {
        try
        {
            var statements = await _statementService.GetStatementsByAccountAsync(accountNumber);
            return Ok(statements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving statements for account {AccountNumber}", accountNumber);
            return StatusCode(500, new { error = "An error occurred while retrieving statements" });
        }
    }

    // Generate a download link for a statement
    [HttpPost("{id}/links")]
    [ProducesResponseType(typeof(DownloadLinkResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateDownloadLink(Guid id, [FromBody] GenerateLinkRequest request)
    {
        try
        {
            // Set the statement ID from the route
            request.StatementId = id;

            // Get base URL for download link
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var link = await _downloadLinkService.GenerateLinkAsync(request, baseUrl);

            _logger.LogInformation(
                "Generated download link for statement {StatementId}, expires at {ExpiresAt}",
                id,
                link.ExpiresAt
            );

            return CreatedAtAction(
                nameof(GetDownloadLink),
                new { token = link.Token },
                link
            );
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating download link for statement {StatementId}", id);
            return StatusCode(500, new { error = "An error occurred while generating the download link" });
        }
    }

    // Get download link details by token
    [HttpGet("links/{token}")]
    [ProducesResponseType(typeof(DownloadLinkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDownloadLink(string token)
    {
        try
        {
            var link = await _downloadLinkService.GetLinkByTokenAsync(token);
            return Ok(link);
        }
        catch (InvalidLinkException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving download link {Token}", token);
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    // Get all download links for a statement
    [HttpGet("{id}/links")]
    [ProducesResponseType(typeof(IEnumerable<DownloadLinkResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatementLinks(Guid id)
    {
        try
        {
            var links = await _downloadLinkService.GetLinksByStatementIdAsync(id);
            return Ok(links);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving links for statement {StatementId}", id);
            return StatusCode(500, new { error = "An error occurred" });
        }
    }

    // Delete a statement
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStatement(Guid id)
    {
        try
        {
            await _statementService.DeleteStatementAsync(id);
            _logger.LogInformation("Deleted statement {StatementId}", id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting statement {StatementId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the statement" });
        }
    }
}