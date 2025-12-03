using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

namespace ResourceBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: List all bookings
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Resource)  // Include related resource
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Show create booking form
        public async Task<IActionResult> Create()
        {
            // Get available resources for dropdown
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            return View();
        }

        // POST: Create a new booking
        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Validate that end time is after start time
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time");
            }

            if (ModelState.IsValid)
            {
                // Check for booking conflicts
                bool hasConflict = await _context.Bookings
                    .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                        ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                         (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                         (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

                if (hasConflict)
                {
                    ModelState.AddModelError("", "This resource is already booked during that time!");
                }
                else
                {
                    _context.Add(booking);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload available resources if validation fails
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            return View(booking);
        }
    }
}
