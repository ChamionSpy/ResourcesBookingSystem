using Microsoft.EntityFrameworkCore;
using ResourceBookingSystem.Data;
using ResourceBookingSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext to use SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Seed database with sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    // Clear any existing data
    db.Resources.RemoveRange(db.Resources);
    db.Bookings.RemoveRange(db.Bookings);
    await db.SaveChangesAsync();

    // Add sample resources
    var resources = new List<Resource>
    {
        new Resource
        {
            Name = "Conference Room A",
            Description = "Large conference room with 4K projector, whiteboard, and video conferencing system",
            Location = "3rd Floor, West Wing",
            Capacity = 25,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Meeting Room B",
            Description = "Medium-sized meeting room with smart TV and flip chart",
            Location = "2nd Floor, East Wing",
            Capacity = 12,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Board Room",
            Description = "Executive board room with leather chairs and teleconference system",
            Location = "4th Floor, Executive Suite",
            Capacity = 18,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Company Car - Toyota Corolla",
            Description = "2023 Toyota Corolla Hybrid - Fuel efficient sedan for client visits",
            Location = "Underground Parking, Bay 12",
            Capacity = 5,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Company Van",
            Description = "Ford Transit - For equipment transport and team outings",
            Location = "Parking Lot B, Bay 3",
            Capacity = 12,
            IsAvailable = false,
        },
        new Resource
        {
            Name = "HD Projector",
            Description = "Epson 4K projector with 120\" screen and HDMI cables",
            Location = "IT Department, Equipment Room",
            Capacity = 1,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Video Conferencing Kit",
            Description = "Logitech MeetUp with camera, microphone, and speaker system",
            Location = "IT Department, AV Closet",
            Capacity = 1,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Training Room",
            Description = "Large training facility with 30 laptops and presentation equipment",
            Location = "1st Floor, Training Center",
            Capacity = 30,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Creative Studio",
            Description = "Soundproof room with recording equipment and green screen",
            Location = "5th Floor, Media Department",
            Capacity = 6,
            IsAvailable = true
        },
        new Resource
        {
            Name = "Patio Meeting Area",
            Description = "Outdoor patio with seating and power outlets for informal meetings",
            Location = "Ground Floor, Garden Patio",
            Capacity = 15,
            IsAvailable = false  // Closed for winter
        }
    };

    db.Resources.AddRange(resources);
    await db.SaveChangesAsync();

    // Add sample bookings
    var today = DateTime.Today;
    var bookings = new List<Booking>
    {
        // Conference Room A bookings
        new Booking
        {
            ResourceId = 1,
            StartTime = today.AddDays(1).AddHours(9),
            EndTime = today.AddDays(1).AddHours(11),
            BookedBy = "Sarah Johnson",
            Purpose = "Quarterly Planning Meeting",
            Status = "Active"
        },
        new Booking
        {
            ResourceId = 1,
            StartTime = today.AddDays(2).AddHours(14),
            EndTime = today.AddDays(2).AddHours(16),
            BookedBy = "Michael Chen",
            Purpose = "Client Presentation",
            Status = "Active"
        },
        
        // Meeting Room B bookings
        new Booking
        {
            ResourceId = 2,
            StartTime = today.AddHours(13), // Today 1 PM
            EndTime = today.AddHours(15),   // Today 3 PM
            BookedBy = "David Wilson",
            Purpose = "Team Stand-up",
            Status = "Active"
        },
        new Booking
        {
            ResourceId = 2,
            StartTime = today.AddDays(3).AddHours(10),
            EndTime = today.AddDays(3).AddHours(12),
            BookedBy = "Lisa Rodriguez",
            Purpose = "Project Review",
            Status = "Active"
        },
        
        // Board Room bookings
        new Booking
        {
            ResourceId = 3,
            StartTime = today.AddDays(1).AddHours(13),
            EndTime = today.AddDays(1).AddHours(15),
            BookedBy = "CEO Office",
            Purpose = "Executive Committee",
            Status = "Active"
        },
        
        // Company Car bookings
        new Booking
        {
            ResourceId = 4,
            StartTime = today.AddDays(2).AddHours(8),
            EndTime = today.AddDays(2).AddHours(17),
            BookedBy = "James Miller",
            Purpose = "Client Site Visits",
            Status = "Active"
        },
        
        // Projector bookings
        new Booking
        {
            ResourceId = 6,
            StartTime = today.AddDays(1).AddHours(9),
            EndTime = today.AddDays(1).AddHours(17),
            BookedBy = "Marketing Team",
            Purpose = "Trade Show Preparation",
            Status = "Active"
        },
        
        // Video Conferencing Kit bookings
        new Booking
        {
            ResourceId = 7,
            StartTime = today.AddDays(2).AddHours(11),
            EndTime = today.AddDays(2).AddHours(12),
            BookedBy = "Remote Team",
            Purpose = "Global Team Meeting",
            Status = "Active"
        },
        
        // Training Room bookings
        new Booking
        {
            ResourceId = 8,
            StartTime = today.AddDays(5).AddHours(9),
            EndTime = today.AddDays(5).AddHours(17),
            BookedBy = "HR Department",
            Purpose = "New Employee Orientation",
            Status = "Active"
        },
        
        // Creative Studio bookings
        new Booking
        {
            ResourceId = 9,
            StartTime = today.AddDays(1).AddHours(15),
            EndTime = today.AddDays(1).AddHours(17),
            BookedBy = "Content Team",
            Purpose = "Podcast Recording",
            Status = "Active"
        },
        
        // Canceled booking example
        new Booking
        {
            ResourceId = 1,
            StartTime = today.AddDays(-1).AddHours(10), // Yesterday
            EndTime = today.AddDays(-1).AddHours(12),
            BookedBy = "Test User",
            Purpose = "Test Meeting (Canceled)",
            Status = "Canceled"
        },
        
        // Past booking example
        new Booking
        {
            ResourceId = 2,
            StartTime = today.AddDays(-2).AddHours(14), // 2 days ago
            EndTime = today.AddDays(-2).AddHours(16),
            BookedBy = "Completed User",
            Purpose = "Completed Project Review",
            Status = "Active"
        }
    };

    db.Bookings.AddRange(bookings);
    await db.SaveChangesAsync();

    Console.WriteLine("Database seeded with 10 resources and 12 bookings!");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
