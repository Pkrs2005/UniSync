using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace UniSync.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    // Храним токен и роль прямо тут, чтобы иметь к ним доступ из любого места приложения
    public static string? Token { get; private set; }
    public static string? UserRole { get; private set; }

    public static void ClearSession()
    {
        Token = null;
        UserRole = null;
    }

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5134/") // 👈 Адрес твоего бэкенда
        };
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { username, password });
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
            if (result != null)
            {
                Token = result.Token;
                UserRole = result.Role;
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}