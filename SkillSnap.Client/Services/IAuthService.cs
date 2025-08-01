using SkillSnap.Client.Models;
using System.Threading.Tasks;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginModel model);
    Task<bool> RegisterAsync(RegisterModel model);
    Task LogoutAsync();
    Task TryRestoreSessionAsync();
    Task<string?> GetTokenAsync();

    bool IsAuthenticated { get; }
    string? UserEmail { get; }
    event Action? OnAuthStateChanged;
}

