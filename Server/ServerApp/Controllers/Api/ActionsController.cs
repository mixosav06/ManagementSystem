using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.DTOs;
using ServerApp.Models;

namespace ServerApp.Controllers.Api;

[ApiController]
[Route("api/actions")]
[Authorize]
public class ActionsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ActionsController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/actions — створення нового бронювання з валідацією
    [HttpPost]
    public async Task<IActionResult> CreateAction([FromBody] CreateActionDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userExists = await _context.Attendants.AnyAsync(u => u.ID == dto.UserID);
        var spot = await _context.ParkingSpots.FindAsync(dto.SpotID);

        if (!userExists)
        {
            return BadRequest(new { Message = $"Працівника з ID {dto.UserID} не знайдено." });
        }

        if (spot == null)
        {
            return BadRequest(new { Message = $"Паркувальне місце з ID {dto.SpotID} не знайдено." });
        }

        var reservation = new Reservation
        {
            UserID = dto.UserID,
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