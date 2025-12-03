using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EFEnergiaAPI.Data;
using Api.Models;

namespace EFEnergiaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Endpoint para registrar novo usuário
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verifica se usuário já existe
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            return BadRequest(new { message = "Username já existe" });

        if (!string.IsNullOrEmpty(request.Email) && 
            await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email já cadastrado" });

        // Hash da senha
        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuário registrado com sucesso", username = user.Username });
    }

    /// <summary>
    /// Endpoint para login e geração de token JWT
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Busca usuário
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return Unauthorized(new { message = "Credenciais inválidas" });

        // Verifica se usuário está ativo
        if (!user.IsActive)
            return Unauthorized(new { message = "Usuário inativo" });

        // Verifica senha
        if (!VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Credenciais inválidas" });

        // Gera token JWT
        var token = GenerateJwtToken(user);
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var response = new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            Username = user.Username
        };

        return Ok(response);
    }

    /// <summary>
    /// Endpoint para criar usuário admin inicial (apenas para desenvolvimento)
    /// </summary>
    [HttpPost("seed-admin")]
    public async Task<IActionResult> SeedAdmin()
    {
        // Verifica se já existe algum usuário
        if (await _context.Users.AnyAsync())
            return BadRequest(new { message = "Usuários já existem no sistema" });

        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            Email = "admin@efenergia.com",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        return Ok(new 
        { 
            message = "Usuário admin criado com sucesso",
            username = "admin",
            password = "admin123",
            warning = "AVISO: Altere essa senha imediatamente em produção!"
        });
    }

    #region Métodos Auxiliares

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? "EFEnergiaAPI";
        var jwtAudience = _configuration["Jwt:Audience"] ?? "EFEnergiaAPI";
        var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        // Usando SHA256 para simplicidade (em produção, use BCrypt ou similar)
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == passwordHash;
    }

    #endregion
}
