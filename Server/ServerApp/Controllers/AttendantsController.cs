using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Route("Attendants")]
    public class AttendantsController : Controller
    {
        private readonly AppDbContext _context;

        public AttendantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Attendants
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Attendants.ToListAsync());
        }

        // GET: /Attendants/Details/5
        [HttpGet("Details/{id?}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var attendant = await _context.Attendants.FirstOrDefaultAsync(m => m.ID == id);
            if (attendant == null) return NotFound();

            return View(attendant);
        }

        // GET: /Attendants/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Attendants/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Email,PasswordHash")] Attendant attendant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(attendant);
        }

        // GET: /Attendants/Edit/5
        [HttpGet("Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var attendant = await _context.Attendants.FindAsync(id);
            if (attendant == null) return NotFound();

            return View(attendant);
        }

        // POST: /Attendants/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Email,PasswordHash")] Attendant attendant)
        {
            if (id != attendant.ID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendantExists(attendant.ID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(attendant);
        }

        // GET: /Attendants/Delete/5
        [HttpGet("Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var attendant = await _context.Attendants.FirstOrDefaultAsync(m => m.ID == id);
            if (attendant == null) return NotFound();

            return View(attendant);
        }

        // POST: /Attendants/Delete/5
        [HttpPost("Delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendant = await _context.Attendants.FindAsync(id);
            if (attendant != null)
            {
                _context.Attendants.Remove(attendant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AttendantExists(int id)
        {
            return _context.Attendants.Any(e => e.ID == id);
        }
    }
}