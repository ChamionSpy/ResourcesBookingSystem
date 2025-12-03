using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List all resources
        public async Task<IActionResult> Index(string search = "")
        {
            // Get all resources
            var resources = _context.Resources.AsQueryable();

            // Apply search filter if search term is provided
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                resources = resources.Where(r =>
                    r.Name.ToLower().Contains(search) ||
                    r.Location.ToLower().Contains(search) ||
                    r.Description.ToLower().Contains(search));
            }

            // Pass search term to view
            ViewBag.SearchTerm = search;

            return View(await resources.ToListAsync());
        }

        // GET: Show resource details
        public async Task<IActionResult> Details(int id)
        {
            var resource = await _context.Resources
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: Show create resource form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Save new resource
        [HttpPost]
        public async Task<IActionResult> Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Show edit form
        public async Task<IActionResult> Edit(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }

        // POST: Save edited resource
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Resource resource)
        {
            if (id != resource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Show delete confirmation
        public async Task<IActionResult> Delete(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }

        // POST: Delete resource
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
