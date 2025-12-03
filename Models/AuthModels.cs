using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class LoginRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password é obrigatório")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Username { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Username é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username deve ter entre 3 e 100 caracteres")]
    public string Username { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password é obrigatório")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
}
