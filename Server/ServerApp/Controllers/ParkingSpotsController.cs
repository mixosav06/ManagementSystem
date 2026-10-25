
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

// [Authorize]
public class ParkingSpotsController : Controller
{
    private readonly AppDbContext _context;

    public ParkingSpotsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PARKINGSPOTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ParkingSpots.ToListAsync());
    }

    // GET: PARKINGSPOTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkingspot = await _context.ParkingSpots
            .FirstOrDefaultAsync(m => m.ID == id);
        if (parkingspot == null)
        {
            return NotFound();
        }

        return View(parkingspot);
    }

    // GET: PARKINGSPOTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PARKINGSPOTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Number,Status,UserID,User,Reservations")] ParkingSpot parkingspot)
    {
        if (ModelState.IsValid)
        {
            _context.Add(parkingspot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(parkingspot);
    }

    // GET: PARKINGSPOTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkingspot = await _context.ParkingSpots.FindAsync(id);
        if (parkingspot == null)
        {
            return NotFound();
        }
        return View(parkingspot);
    }

    // POST: PARKINGSPOTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("ID,Number,Status,UserID,User,Reservations")] ParkingSpot parkingspot)
    {
        if (id != parkingspot.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(parkingspot);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingSpotExists(parkingspot.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(parkingspot);
    }

    // GET: PARKINGSPOTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var parkingspot = await _context.ParkingSpots
            .FirstOrDefaultAsync(m => m.ID == id);
        if (parkingspot == null)
        {
            return NotFound();
        }

        return View(parkingspot);
    }

    // POST: PARKINGSPOTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var parkingspot = await _context.ParkingSpots.FindAsync(id);
        if (parkingspot != null)
        {
            _context.ParkingSpots.Remove(parkingspot);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ParkingSpotExists(int? id)
    {
        return _context.ParkingSpots.Any(e => e.ID == id);
    }
}
