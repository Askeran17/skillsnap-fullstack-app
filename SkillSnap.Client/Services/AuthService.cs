using SkillSnap.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SkillSnap.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    private readonly UserSessionService _session;

    private const string TokenKey = "authToken";

    public bool IsAuthenticated { get; private set; } = false;
    public string? UserEmail { get; private set; }
    public string? UserName { get; private set; }

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, UserSessionService session)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
        _session = session;
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (data != null && !string.IsNullOrEmpty(data.Token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, data.Token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", data.Role);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", data.Email);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userName", data.UserName);

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);

                UserEmail = data.Email;
                UserName = data.UserName;
                _session.Role = data.Role;

                IsAuthenticated = true;
                OnAuthStateChanged?.Invoke();
                return true;
            }
        }

        IsAuthenticated = false;
        UserEmail = null;
        UserName = null;
        _session.Role = string.Empty;
        OnAuthStateChanged?.Invoke();
        return false;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            return await LoginAsync(new LoginModel
            {
                Email = model.Email,
                Password = model.Password
            });
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/auth/logout", null);

        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userName");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");

        IsAuthenticated = false;
        UserEmail = null;
        UserName = null;
        _session.Role = string.Empty;

        _httpClient.DefaultRequestHeaders.Authorization = null;

        OnAuthStateChanged?.Invoke();
    }

    public async Task TryRestoreSessionAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        var role = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole");
        var email = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");
        var name = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userName");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            IsAuthenticated = true;
            UserEmail = email;
            UserName = name;
            _session.Role = role ?? string.Empty;

            Console.WriteLine($"🔍 Restored Role: {_session.Role}");
            Console.WriteLine($"🧠 IsAdmin: {_session.IsAdmin}");
        }
        else
        {
            IsAuthenticated = false;
            UserEmail = null;
            UserName = null;
            _session.Role = string.Empty;
        }

        OnAuthStateChanged?.Invoke();
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
    }
}


public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}




