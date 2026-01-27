using SecureFileDelivery.Application.DTOs;

namespace SecureFileDelivery.Application.Interfaces;

// Service for managing statements
public interface IStatementService
{
    // Uploads and stores a new statement
    Task<StatementResponse> UploadStatementAsync(UploadStatementRequest request);

    // Gets a statement by ID
    Task<StatementResponse?> GetStatementByIdAsync(Guid id);

    // Gets all statements for an account
    Task<IEnumerable<StatementResponse>> GetStatementsByAccountAsync(string accountNumber);

    // Deletes a statement and its file
    Task DeleteStatementAsync(Guid id);
}