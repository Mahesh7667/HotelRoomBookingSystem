using HotelRoomBooking.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public DataController(HotelBookingContext context)
    {
        _context = context;
    }

    // POST: api/Data/seed
    [HttpPost("seed")]
    public async Task<IActionResult> SeedData()
    {
        // Database will be seeded automatically via OnModelCreating
        // But we can ensure migrations are applied
        await _context.Database.MigrateAsync();
        return Ok("Database seeded successfully.");
    }

    // POST: api/Data/reset
    [HttpPost("reset")]
    public async Task<IActionResult> ResetData()
    {
        // Delete all bookings
        _context.Bookings.RemoveRange(_context.Bookings);
        await _context.SaveChangesAsync();

        // Reset database (optional - this would drop and recreate)
        // await _context.Database.EnsureDeletedAsync();
        // await _context.Database.MigrateAsync();

        return Ok("Data reset successfully.");
    }
}
