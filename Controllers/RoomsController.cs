using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

/// <summary>
/// Controller for managing room availability and search operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public RoomsController(HotelBookingContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Find available rooms for specific dates and guest count
    /// </summary>
    /// <param name="checkIn">Check-in date (must be in the future)</param>
    /// <param name="checkOut">Check-out date (must be after check-in)</param>
    /// <param name="guests">Number of guests (must be greater than 0)</param>
    /// <returns>List of available rooms that can accommodate the guests</returns>
    /// <response code="200">Available rooms returned successfully</response>
    /// <response code="400">Invalid dates or guest count</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<Room>), 200)]
    [ProducesResponseType(400)]
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
