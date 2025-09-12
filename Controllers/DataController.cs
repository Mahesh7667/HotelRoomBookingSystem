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
        // Drop and recreate database to ensure clean state with seeded data
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.MigrateAsync();
        return Ok("Database recreated and seeded with test data successfully.");
    }

    // POST: api/Data/reset
    [HttpPost("reset")]
    public async Task<IActionResult> ResetData()
    {
        // Delete all bookings, rooms, and hotels to reset to clean state
        _context.Bookings.RemoveRange(_context.Bookings);
        _context.Rooms.RemoveRange(_context.Rooms);
        _context.Hotels.RemoveRange(_context.Hotels);
        await _context.SaveChangesAsync();

        return Ok("All data reset successfully. Use /seed to repopulate test data.");
    }
}
