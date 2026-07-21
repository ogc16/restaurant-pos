using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class AuthTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;
    private readonly HttpClient _client;

    public AuthTests(TestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "wrong@test.com", Password = "WrongPass1!" });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_Unauthenticated_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/auth/me");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_Authenticated_ReturnsOkOrNotFound()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/auth/me");
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUsers_AdminRole_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/auth/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
