
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using ServerApp.Data;

namespace ServerApp.Controllers;

[Authorize]
[Route("[controller]")]
public class AttendantsController : Controller
{
    private readonly AppDbContext _context;

    public AttendantsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ATTENDANTS
    [HttpGet]
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Attendants.ToListAsync());
    }

    // GET: ATTENDANTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var attendant = await _context.Attendants
            .FirstOrDefaultAsync(m => m.ID == id);
        if (attendant == null)
        {
            return NotFound();
        }

        return View(attendant);
    }

    // GET: ATTENDANTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ATTENDANTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name,Email,PasswordHash,ParkingSpots,Reservations")] Attendant attendant)
    {
        if (ModelState.IsValid)
        {
            _context.Add(attendant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(attendant);
    }

    // GET: ATTENDANTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var attendant = await _context.Attendants.FindAsync(id);
        if (attendant == null)
        {
            return NotFound();
        }
        return View(attendant);
    }

    // POST: ATTENDANTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("ID,Name,Email,PasswordHash,ParkingSpots,Reservations")] Attendant attendant)
    {
        if (id != attendant.ID)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(attendant);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendantExists(attendant.ID))
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
        return View(attendant);
    }

    // GET: ATTENDANTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var attendant = await _context.Attendants
            .FirstOrDefaultAsync(m => m.ID == id);
        if (attendant == null)
        {
            return NotFound();
        }

        return View(attendant);
    }

    // POST: ATTENDANTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var attendant = await _context.Attendants.FindAsync(id);
        if (attendant != null)
        {
            _context.Attendants.Remove(attendant);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AttendantExists(int? id)
    {
        return _context.Attendants.Any(e => e.ID == id);
    }
}
