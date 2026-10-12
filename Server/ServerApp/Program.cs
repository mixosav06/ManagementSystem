using Microsoft.EntityFrameworkCore;
using ServerApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Додаємо DbContext з SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Parking Management System API is running!");

app.Run();