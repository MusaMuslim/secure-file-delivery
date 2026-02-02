using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SecureFileDelivery.Tests;

public class DownloadLinkTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public DownloadLinkTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GenerateLink_ForExistingStatement_ReturnsLink()
    {
        // Arrange - Upload a statement first
        var statementId = await UploadTestStatement();

        var linkRequest = new
        {
            statementId = statementId,
            expirationMinutes = 60,
            maxAccessCount = (int?)null,
            createdBy = "TestUser"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/statements/{statementId}/links", linkRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<DownloadLinkResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.DownloadUrl.Should().Contain("/api/download/");
    }

    [Fact]
    public async Task DownloadFile_WithValidToken_ReturnsFile()
    {
        // Arrange - Upload statement and generate link
        var statementId = await UploadTestStatement();
        var token = await GenerateTestDownloadLink(statementId);

        // Act
        var response = await _client.GetAsync($"/api/download/{token}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType.Should().NotBeNull();
        var fileBytes = await response.Content.ReadAsByteArrayAsync();
        fileBytes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DownloadFile_WithInvalidToken_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/download/invalid-token-12345");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> UploadTestStatement()
    {
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4, 5 });
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        content.Add(fileContent, "file", "test.pdf");
        content.Add(new StringContent("12345678"), "accountNumber");
        content.Add(new StringContent("2024-01-01"), "periodStart");
        content.Add(new StringContent("2024-01-31"), "periodEnd");
        content.Add(new StringContent("TestUser"), "uploadedBy");

        var response = await _client.PostAsync("/api/statements", content);
        var result = await response.Content.ReadFromJsonAsync<StatementResponse>();
        return result!.Id;
    }

    private async Task<string> GenerateTestDownloadLink(Guid statementId)
    {
        var request = new
        {
            statementId = statementId,
            expirationMinutes = 60,
            maxAccessCount = (int?)null,
            createdBy = "TestUser"
        };

        var response = await _client.PostAsJsonAsync($"/api/statements/{statementId}/links", request);
        var result = await response.Content.ReadFromJsonAsync<DownloadLinkResponse>();
        return result!.Token;
    }

    private class StatementResponse
    {
        public Guid Id { get; set; }
    }

    private class DownloadLinkResponse
    {
        public string Token { get; set; } = string.Empty;
        public string DownloadUrl { get; set; } = string.Empty;
    }
}