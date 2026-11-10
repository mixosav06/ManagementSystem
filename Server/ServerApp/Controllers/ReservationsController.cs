using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Reservations")]
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Reservations або /Reservations/Index
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Spot)
                .ToListAsync();

            return View(reservations);
        }

        // GET: /Reservations/Details/5
        [HttpGet("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Spot)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: /Reservations/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            ViewData["UserID"] = new SelectList(_context.Attendants, "ID", "Name");
            ViewData["SpotID"] = new SelectList(_context.ParkingSpots, "ID", "Number");
            return View();
        }

        // POST: /Reservations/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,UserID,SpotID,ReservationDate,Status")] Reservation reservation)
        {
            ModelState.Remove("User");
            ModelState.Remove("Spot");

            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserID"] = new SelectList(_context.Attendants, "ID", "Name", reservation.UserID);
            ViewData["SpotID"] = new SelectList(_context.ParkingSpots, "ID", "Number", reservation.SpotID);
            return View(reservation);
        }

        // GET: /Reservations/Edit/5
        [HttpGet("Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["UserID"] = new SelectList(_context.Attendants, "ID", "Name", reservation.UserID);
            ViewData["SpotID"] = new SelectList(_context.ParkingSpots, "ID", "Number", reservation.SpotID);
            return View(reservation);
        }

        // POST: /Reservations/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,UserID,SpotID,ReservationDate,Status")] Reservation reservation)
        {
            if (id != reservation.ID)
            {
                return NotFound();
            }

            ModelState.Remove("User");
            ModelState.Remove("Spot");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ID))
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

            ViewData["UserID"] = new SelectList(_context.Attendants, "ID", "Name", reservation.UserID);
            ViewData["SpotID"] = new SelectList(_context.ParkingSpots, "ID", "Number", reservation.SpotID);
            return View(reservation);
        }

        // GET: /Reservations/Delete/5
        [HttpGet("Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Spot)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: /Reservations/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ID == id);
        }
    }
}