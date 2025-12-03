using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;
using System.Diagnostics;

namespace ResourceBookingSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Calculate stats for the dashboard
            var totalResources = await _context.Resources.CountAsync();
            var availableResources = await _context.Resources.CountAsync(r => r.IsAvailable);
            var totalBookings = await _context.Bookings.CountAsync(b => b.Status == "Active");

            var today = DateTime.Today;
            var todayBookings = await _context.Bookings
                .CountAsync(b => b.Status == "Active" &&
                                b.StartTime.Date == today);

            // Pass stats to view
            ViewBag.TotalResources = totalResources;
            ViewBag.AvailableResources = availableResources;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.TodayBookings = todayBookings;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
