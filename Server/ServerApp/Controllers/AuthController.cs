using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ServerApp.Controllers;

public class AuthController : Controller
{
    private readonly IConfiguration _config;

    // Хардкод адміністратора для швидкої перевірки
    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin123";

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    // GET: /Auth/Login (Сторінка з формою)
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Auth/Login (Отримання токена або авторизація)
    [HttpPost]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        if (username != AdminUsername || password != AdminPassword)
        {
            ViewBag.Error = "Невірний логін або пароль!";
            return View();
        }

        var token = GenerateJwtToken(username);

        // Зберігаємо токен у cookie для доступу через браузер
        Response.Cookies.Append("X-Access-Token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true
        });

        ViewBag.Token = token;
        ViewBag.Message = "Успішна авторизація!";
        return View();
    }

    // Метод генерації JWT-токена
    private string GenerateJwtToken(string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}