using System.Net;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class InventoryTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public InventoryTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_AdminRole_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture, "Admin");
        var response = await client.GetAsync("/api/inventory");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLowStock_AdminRole_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture, "Admin");
        var response = await client.GetAsync("/api/inventory/low-stock");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAll_NoAuth_ReturnsUnauthorized()
    {
        var client = _fixture.CreateClient();
        var response = await client.GetAsync("/api/inventory");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
