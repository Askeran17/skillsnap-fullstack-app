using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Minimum length is 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = string.Empty;
}

