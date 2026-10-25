using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers;

[Authorize]
[Route("[controller]")]
public class ParkingSpotsController : Controller
{
    private readonly AppDbContext _context;

    public ParkingSpotsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /ParkingSpots
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.ParkingSpots.ToListAsync());
    }

    // GET: /ParkingSpots/Details/5
    [HttpGet("Details/{id}")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var parkingSpot = await _context.ParkingSpots
            .FirstOrDefaultAsync(m => m.ID == id);

        if (parkingSpot == null) return NotFound();

        return View(parkingSpot);
    }

    // GET: /ParkingSpots/Create
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /ParkingSpots/Create
    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Number,Status")] ParkingSpot parkingSpot)
    {
        ModelState.Remove("User");
        ModelState.Remove("Reservations");

        if (ModelState.IsValid)
        {
            _context.Add(parkingSpot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(parkingSpot);
    }

    // GET: /ParkingSpots/Edit/5
    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var parkingSpot = await _context.ParkingSpots.FindAsync(id);
        if (parkingSpot == null) return NotFound();

        return View(parkingSpot);
    }

    // POST: /ParkingSpots/Edit/5
    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Number,Status")] ParkingSpot parkingSpot)
    {
        if (id != parkingSpot.ID) return NotFound();

        ModelState.Remove("User");
        ModelState.Remove("Reservations");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(parkingSpot);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingSpotExists(parkingSpot.ID)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(parkingSpot);
    }

    // GET: /ParkingSpots/Delete/5
    [HttpGet("Delete/{id}")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var parkingSpot = await _context.ParkingSpots
            .FirstOrDefaultAsync(m => m.ID == id);

        if (parkingSpot == null) return NotFound();

        return View(parkingSpot);
    }

    // POST: /ParkingSpots/Delete/5
    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var parkingSpot = await _context.ParkingSpots.FindAsync(id);
        if (parkingSpot != null)
        {
            _context.ParkingSpots.Remove(parkingSpot);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ParkingSpotExists(int id)
    {
        return _context.ParkingSpots.Any(e => e.ID == id);
    }
}