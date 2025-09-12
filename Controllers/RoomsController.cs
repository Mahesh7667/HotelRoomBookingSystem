using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public RoomsController(HotelBookingContext context)
    {
        _context = context;
    }

    // GET: api/Rooms/available?checkIn={date}&checkOut={date}&guests={number}
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms(
        [FromQuery] DateTime checkIn,
        [FromQuery] DateTime checkOut,
        [FromQuery] int guests)
    {
        if (checkIn >= checkOut)
        {
            return BadRequest("Check-out date must be after check-in date.");
        }

        if (guests <= 0)
        {
            return BadRequest("Number of guests must be greater than 0.");
        }

        // Find rooms that are available for the entire period and have sufficient capacity
        var availableRooms = await _context.Rooms
            .Where(r => r.Capacity >= guests)
            .Where(r => !_context.Bookings.Any(b =>
                b.RoomId == r.Id &&
                ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                 (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) ||
                 (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut))))
            .Include(r => r.Hotel)
            .ToListAsync();

        return availableRooms;
    }
}
