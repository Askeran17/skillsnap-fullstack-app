using SkillSnap.Client.Models;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginModel model);
    Task<bool> RegisterAsync(RegisterModel model);
}
