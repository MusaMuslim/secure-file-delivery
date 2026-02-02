using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SecureFileDelivery.Tests;

public class StatementTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public StatementTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UploadStatement_WithValidFile_ReturnsCreated()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4, 5 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        content.Add(fileContent, "file", "test.pdf");
        content.Add(new StringContent("12345678"), "accountNumber");
        content.Add(new StringContent("2024-01-01"), "periodStart");
        content.Add(new StringContent("2024-01-31"), "periodEnd");
        content.Add(new StringContent("TestUser"), "uploadedBy");

        // Act
        var response = await _client.PostAsync("/api/statements", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<StatementResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.AccountNumber.Should().Be("12345678");
    }

    [Fact]
    public async Task GetStatement_WithValidId_ReturnsStatement()
    {
        // Arrange - First upload a statement
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        uploadContent.Add(fileContent, "file", "test.pdf");
        uploadContent.Add(new StringContent("99999999"), "accountNumber");
        uploadContent.Add(new StringContent("2024-01-01"), "periodStart");
        uploadContent.Add(new StringContent("2024-01-31"), "periodEnd");
        uploadContent.Add(new StringContent("TestUser"), "uploadedBy");

        var uploadResponse = await _client.PostAsync("/api/statements", uploadContent);
        var uploadedStatement = await uploadResponse.Content.ReadFromJsonAsync<StatementResponse>();

        // Act
        var response = await _client.GetAsync($"/api/statements/{uploadedStatement!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<StatementResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(uploadedStatement.Id);
    }

    private class StatementResponse
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}