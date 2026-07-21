using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class MenuItemsTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public MenuItemsTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_NoAuth_ReturnsOk()
    {
        var client = _fixture.CreateClient();
        var response = await client.GetAsync("/api/menuitems");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNotFound()
    {
        var client = _fixture.CreateClient();
        var response = await client.GetAsync("/api/menuitems/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_AdminRole_ReturnsCreated()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.PostAsJsonAsync("/api/menuitems", new
        {
            Name = "Test Item",
            Description = "Test description",
            Price = 9.99,
            Category = 0,
            PrepTimeMinutes = 10,
            IsDailySpecial = false
        });
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_NoAuth_ReturnsUnauthorized()
    {
        var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/menuitems", new
        {
            Name = "Test Item",
            Description = "Test",
            Price = 9.99,
            Category = 0,
            PrepTimeMinutes = 10,
            IsDailySpecial = false
        });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
