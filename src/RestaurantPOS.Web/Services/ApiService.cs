using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace RestaurantPOS.Web.Services;

public class ApiService
{
    private readonly HttpClient _http;
    private readonly AuthService _auth;
    private readonly ILogger<ApiService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(HttpClient http, AuthService auth, ILogger<ApiService> logger)
    {
        _http = http;
        _auth = auth;
        _logger = logger;
    }

    private void EnsureAuth()
    {
        if (!string.IsNullOrEmpty(_auth.Token))
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _auth.Token);
    }

    public async Task<List<T>> GetAsync<T>(string endpoint)
    {
        EnsureAuth();
        try
        {
            var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<T>>(JsonOptions) ?? new List<T>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET {Endpoint} failed", endpoint);
            return new List<T>();
        }
    }

    public async Task<T?> GetSingleAsync<T>(string endpoint)
    {
        EnsureAuth();
        try
        {
            var response = await _http.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET {Endpoint} failed", endpoint);
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        EnsureAuth();
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, data);
            if (!response.IsSuccessStatusCode) return default;
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST {Endpoint} failed", endpoint);
            return default;
        }
    }

    public async Task<bool> PutAsync(string endpoint, object data)
    {
        EnsureAuth();
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, data);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT {Endpoint} failed", endpoint);
            return false;
        }
    }

    public async Task<bool> PatchAsync(string endpoint, object? data = null)
    {
        EnsureAuth();
        try
        {
            var content = data != null
                ? new StringContent(JsonSerializer.Serialize(data, JsonOptions), Encoding.UTF8, "application/json")
                : new StringContent("{}", Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint) { Content = content };
            var response = await _http.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PATCH {Endpoint} failed", endpoint);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        EnsureAuth();
        try
        {
            var response = await _http.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE {Endpoint} failed", endpoint);
            return false;
        }
    }

    public async Task<byte[]?> GetBytesAsync(string endpoint)
    {
        EnsureAuth();
        try
        {
            var response = await _http.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET bytes {Endpoint} failed", endpoint);
            return null;
        }
    }
}
