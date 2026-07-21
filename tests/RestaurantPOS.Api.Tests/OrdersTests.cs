using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class OrdersTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public OrdersTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_Authenticated_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/orders");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_NonExistent_ReturnsNotFound()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture);
        var response = await client.GetAsync("/api/orders/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_NoAuth_ReturnsUnauthorized()
    {
        var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            TableId = 1,
            SeatNumber = 1,
            Items = new[] { new { MenuItemId = 1, Quantity = 1 } }
        });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
