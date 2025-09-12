using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    // GET: api/Rooms/available?checkIn={date}&checkOut={date}&guests={number}
    [HttpGet("available")]
    public ActionResult<IEnumerable<Room>> GetAvailableRooms(
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
        var availableRooms = DataStore.Rooms
            .Where(r => r.Capacity >= guests)
            .Where(r => !DataStore.Bookings.Any(b =>
                b.RoomId == r.Id &&
                ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                 (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) ||
                 (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut))))
            .ToList();

        return availableRooms;
    }
}
