using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin123";

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // ==========================================
    // 1. ВХІД АДМІНІСТРАТОРА (MVC У БРАУЗЕРІ)
    // ==========================================

    [HttpGet("Auth/Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("Auth/Login")]
    public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
    {
        if (username != AdminUsername || password != AdminPassword)
        {
            ViewBag.Error = "Невірний логін або пароль адміністратора!";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties { IsPersistent = true };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Index", "ParkingSpots");
    }

    [HttpGet("Auth/Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    // ==========================================
    // 2. REST API ВХІД (ДЛЯ MAUI / POSTMAN / SWAGGER)
    // ==========================================

    [HttpPost("api/auth/login")]
    public async Task<IActionResult> ApiLogin([FromBody] LoginDto request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email та пароль є обов'язковими." });
        }

        var user = await _context.Attendants.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || (user.PasswordHash != request.Password && !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)))
        {
            return Unauthorized(new { message = "Невірний email або пароль." });
        }

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