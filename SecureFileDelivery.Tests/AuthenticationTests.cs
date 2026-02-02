using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace SecureFileDelivery.Tests;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GenerateToken_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new { username = "testuser", password = "testpass" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.ExpiresIn.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GenerateToken_WithEmptyCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new { username = "", password = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/token", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private class TokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = string.Empty;
    }
}