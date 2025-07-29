using System.Net.Http;
using System.Net.Http.Json;
using SkillSnap.Client.Models;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", model);
        return response.IsSuccessStatusCode;
    }
}
