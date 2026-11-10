using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Data;
using ServerApp.DTOs;
using ServerApp.Models;

namespace ServerApp.Controllers.Api;

[ApiController]
[Route("api/actions")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ActionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActionsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/actions — бронювання місця від імені користувача з токена
    [HttpPost]
    public async Task<IActionResult> CreateAction([FromBody] CreateActionDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new { message = "Токен не містить ідентифікатора користувача." });
        }

        var spot = await _context.ParkingSpots.FindAsync(dto.SpotID);
        if (spot == null)
        {
            return BadRequest(new { message = $"Паркувальне місце з ID {dto.SpotID} не знайдено." });
        }

        if (spot.Status != "Free")
        {
            return Conflict(new { message = $"Місце {spot.Number} уже зайняте." });
        }

        var reservation = new Reservation
        {
            UserID = userId,
            SpotID = dto.SpotID,
            ReservationDate = DateTime.UtcNow,
            Status = "Active"
        };

        spot.Status = "Occupied";
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(CreateAction), new { id = reservation.ID }, new
        {
            reservation.ID,
            reservation.UserID,
            reservation.SpotID,
            reservation.ReservationDate,
            reservation.Status
        });
    }
}
