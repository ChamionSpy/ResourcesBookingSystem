using Azure.Core;
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
                .Where(b => b.Status != "Canceled") // Show all ACTIVE bookings (not canceled)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Show canceled bookings
        public async Task<IActionResult> Canceled()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Resource)
                .Where(b => b.Status == "Canceled")
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }

        // GET: Show booking details
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Show create booking form
        public async Task<IActionResult> Create()
        {
            // Get available resources for dropdown
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            // Set default times (1 hour from now, for 1 hour duration)
            var model = new Booking
            {
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2),
                Status = "Active"
            };

            return View();
        }

        // POST: Create a new booking
        [HttpPost]
        public async Task<IActionResult> Create(Booking booking)
        {
            // Set default status
            booking.Status = "Active";

            // Validate that end time is after start time
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time");
            }

            if (ModelState.IsValid)
            {
                // Check for booking conflicts
                bool hasConflict = await _context.Bookings
                    .Where(b => b.Status == "Active")
                    .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                        ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                         (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                         (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

                if (hasConflict)
                {
                    ModelState.AddModelError("", "This resource is already booked during the requested time.Please choose another slot or resource, or adjust your times.");
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

        // GET: Show edit booking form
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == "Canceled")
            {
                TempData["ErrorMessage"] = "Cannot edit a canceled booking.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            // Get available resources for dropdown
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            return View(booking);
        }

        // POST: Handle booking edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            // Keep original status unless changed
            var existingBooking = await _context.Bookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            if (existingBooking.Status == "Canceled")
            {
                TempData["ErrorMessage"] = "Cannot edit a canceled booking.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            // Server-side validation: End time must be after start time
            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time");
            }

            if (ModelState.IsValid)
            {
                // Check for booking conflicts (excluding current booking)
                bool hasConflict = await _context.Bookings
                    .Where(b => b.Status == "Active" && b.Id != id)
                    .AnyAsync(b => b.ResourceId == booking.ResourceId &&
                        ((booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                         (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                         (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)));

                if (hasConflict)
                {
                    ModelState.AddModelError("", "This resource is already booked during that time! Please choose another time or resource.");
                }
                else
                {
                    // Update booking
                    existingBooking.ResourceId = booking.ResourceId;
                    existingBooking.StartTime = booking.StartTime;
                    existingBooking.EndTime = booking.EndTime;
                    existingBooking.BookedBy = booking.BookedBy;
                    existingBooking.Purpose = booking.Purpose;
                    existingBooking.Status = "Active"; // Ensure status stays Active

                    _context.Update(existingBooking);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }

            // Reload resources if validation failed
            ViewBag.Resources = await _context.Resources
                .Where(r => r.IsAvailable)
                .ToListAsync();

            return View(booking);
        }

        // GET: Show cancel booking confirmation
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Resource)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status == "Canceled")
            {
                TempData["ErrorMessage"] = "This booking is already canceled.";
                return RedirectToAction(nameof(Details), new { id = booking.Id });
            }

            return View(booking);
        }

        // POST: Handle booking cancellation
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Status = "Canceled";
            _context.Update(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking canceled successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete booking (admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking permanently deleted!";
            }

            return RedirectToAction(nameof(Canceled));
        }
    }
}
