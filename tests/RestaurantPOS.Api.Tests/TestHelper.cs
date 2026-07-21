using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using RestaurantPOS.Core.Entities;
using RestaurantPOS.Infrastructure.Data;
using RestaurantPOS.Infrastructure.Identity;

namespace RestaurantPOS.Api.Tests;

public class TestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var ctx = services.GetRequiredService<AppDbContext>();
        ctx.Database.EnsureCreated();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        SeedIdentityAsync(roleManager, userManager).GetAwaiter().GetResult();

        return host;
    }

    private static async Task SeedIdentityAsync(RoleManager<IdentityRole<int>> roleManager, UserManager<AppUser> userManager)
    {
        string[] roles = { "Admin", "Cashier", "Waiter" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int>(role));
        }

        var adminEmail = "admin@restaurant.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Admin",
                Role = "Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123456");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

public static class TestHelper
{
    private const string JwtKey = "RestaurantPOS_SuperSecretKey_2024_MinLength32Chars!";

    public static string GenerateTestToken(string role = "Admin")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Email, "test@test.com"),
            new Claim("fullName", "Test User"),
            new Claim(ClaimTypes.Role, role)
        };
        var token = new JwtSecurityToken(
            issuer: "RestaurantPOS",
            audience: "RestaurantPOS",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static HttpClient GetAuthenticatedClient(WebApplicationFactory<Program> factory, string role = "Admin")
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GenerateTestToken(role));
        return client;
    }
}
