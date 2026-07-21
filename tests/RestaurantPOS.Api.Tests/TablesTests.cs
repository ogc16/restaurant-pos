using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class TablesTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public TablesTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_Authenticated_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/tables");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAvailable_Authenticated_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/tables/available");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_AdminRole_ReturnsCreated()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.PostAsJsonAsync("/api/tables", new
        {
            TableNumber = "T99",
            Capacity = 4,
            Seats = 4,
            Section = "Test"
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_NoAuth_ReturnsUnauthorized()
    {
        var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tables", new
        {
            TableNumber = "T99",
            Capacity = 4,
            Seats = 4,
            Section = "Test"
        });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
