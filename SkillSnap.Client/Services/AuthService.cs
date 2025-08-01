using SkillSnap.Client.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    private const string TokenKey = "authToken";
    private const string EmailKey = "userEmail";
    private const string UserNameKey = "userName";

    public bool IsAuthenticated { get; private set; } = false;
    public string? UserEmail { get; private set; }
    public string? UserName { get; private set; }

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (!string.IsNullOrEmpty(data?.Token))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, data.Token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EmailKey, model.Email);

                IsAuthenticated = true;
                UserEmail = model.Email;
                OnAuthStateChanged?.Invoke();
                return true;
            }
        }

        IsAuthenticated = false;
        UserEmail = null;
        OnAuthStateChanged?.Invoke();
        return false;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            // ✅ Сохраняем имя пользователя
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserNameKey, model.UserName);

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
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", EmailKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserNameKey);

        IsAuthenticated = false;
        UserEmail = null;
        UserName = null;

        OnAuthStateChanged?.Invoke();
    }

    public async Task TryRestoreSessionAsync()
    {
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
        var email = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", EmailKey);
        var userName = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserNameKey);

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
        {
            IsAuthenticated = true;
            UserEmail = email;
            UserName = userName;
        }
        else
        {
            IsAuthenticated = false;
            UserEmail = null;
            UserName = null;
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
    public string? Token { get; set; }
}



