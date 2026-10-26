using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // POST: /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email та пароль є обов'язковими." });
        }

        // Пошук користувача/працівника в БД SQLite
        var user = await _context.Attendants.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Перевірка збігу хешу пароля (або простого рядка / BCrypt)
        if (user == null || (user.PasswordHash != request.Password && !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)))
        {
            return Unauthorized(new { message = "Невірний email або пароль." });
        }

        // Генерація JWT-токена
        var token = GenerateJwtToken(user);

        return Ok(new
        {
            token = token,
            userID = user.ID,
            email = user.Email,
            name = user.Name
        });
    }

    private string GenerateJwtToken(Attendant user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}