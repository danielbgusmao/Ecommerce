using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.IntegrationTests.Infrastructure;

namespace Ecommerce.Api.IntegrationTests.Auth;

public class LoginTests
    : IClassFixture<EcommerceApiFactory>
{
    private readonly HttpClient _client;

    public LoginTests(
        EcommerceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new
        {
            Email = "dev@martech.com",
            Password = "Senha@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/login",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var content =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(content);
        Assert.False(
            string.IsNullOrWhiteSpace(
                content.AccessToken));
    }

    private sealed record LoginResponse(
        string AccessToken);
}