using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string UserName { get; set; } = string.Empty;
}

