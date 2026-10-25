using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;

namespace ServerApp.Controllers.Api;

[ApiController]
[Route("api/items")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/items — отримання списку всіх паркувальних місць
    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        var items = await _context.ParkingSpots
            .Select(spot => new
            {
                spot.ID,
                spot.Number,
                spot.Status,
                spot.UserID
            })
            .ToListAsync();

        return Ok(items);
    }
}