using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;

namespace FlapKapVendingMachine.WebApi.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidSeller_ShouldReturnToken()
    {
        // Arrange
        var username = "seller11";
        var password = "123";

        // Act
        var response = await _client.PostAsync($"/api/auth/login?username={username}&password={password}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().ContainKey("token");
        json["token"].Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var username = "wrong";
        var password = "wrong";

        // Act
        var response = await _client.PostAsync($"/api/auth/login?username={username}&password={password}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
