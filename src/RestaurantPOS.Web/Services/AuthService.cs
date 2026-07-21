using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RestaurantPOS.Web.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;

    public string? Token { get; private set; }
    public string? UserEmail { get; private set; }
    public string? FullName { get; private set; }
    public string? Role { get; private set; }
    public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient http, ILogger<AuthService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null) return false;

            Token = result.Token;
            UserEmail = result.Email;
            FullName = result.FullName;
            Role = result.Role;

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return false;
        }
    }

    public void Logout()
    {
        Token = null;
        UserEmail = null;
        FullName = null;
        Role = null;
        _http.DefaultRequestHeaders.Authorization = null;
        OnAuthStateChanged?.Invoke();
    }

    public string GetToken() => Token ?? "";
    public bool HasRole(string role) => Role == role;

    private record AuthResponse(string Token, string Email, string FullName, string Role, DateTime Expiration);
}
