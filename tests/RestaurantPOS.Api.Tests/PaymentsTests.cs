using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace RestaurantPOS.Api.Tests;

public class PaymentsTests : IClassFixture<TestFixture>
{
    private readonly TestFixture _fixture;

    public PaymentsTests(TestFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task GetAll_AdminRole_ReturnsOk()
    {
        var client = TestHelper.GetAuthenticatedClient(_fixture, "Admin");
        var response = await client.GetAsync("/api/payments");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ProcessPayment_NoAuth_ReturnsUnauthorized()
    {
        var client = _fixture.CreateClient();
        var response = await client.PostAsJsonAsync("/api/payments", new
        {
            OrderId = 1,
            Method = 0,
            Amount = 25.00,
            TipAmount = 5.00
        });
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
