using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public BookingsController(HotelBookingContext context)
    {
        _context = context;
    }

    // POST: api/Bookings
    [HttpPost]
    public async Task<ActionResult<Booking>> CreateBooking([FromBody] BookingRequest request)
    {
        if (request.CheckIn >= request.CheckOut)
        {
            return BadRequest("Check-out date must be after check-in date.");
        }

        if (request.NumberOfGuests <= 0)
        {
            return BadRequest("Number of guests must be greater than 0.");
        }

        // Check if room exists and has capacity
        var room = await _context.Rooms.FindAsync(request.RoomId);
        if (room == null)
        {
            return NotFound("Room not found.");
        }

        if (room.Capacity < request.NumberOfGuests)
        {
            return BadRequest("Room does not have sufficient capacity.");
        }

        // Check for overlapping bookings
        var overlappingBooking = await _context.Bookings
            .AnyAsync(b => b.RoomId == request.RoomId &&
                          ((b.CheckInDate <= request.CheckIn && b.CheckOutDate > request.CheckIn) ||
                           (b.CheckInDate < request.CheckOut && b.CheckOutDate >= request.CheckOut) ||
                           (b.CheckInDate >= request.CheckIn && b.CheckOutDate <= request.CheckOut)));

        if (overlappingBooking)
        {
            return Conflict("Room is not available for the selected dates.");
        }

        // Generate unique booking reference
        var bookingReference = GenerateBookingReference();

        var booking = new Booking
        {
            BookingReference = bookingReference,
            RoomId = request.RoomId,
            CustomerName = request.CustomerName,
            CheckInDate = request.CheckIn,
            CheckOutDate = request.CheckOut,
            NumberOfGuests = request.NumberOfGuests
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBooking), new { reference = bookingReference }, booking);
    }

    // GET: api/Bookings/{reference}
    [HttpGet("{reference}")]
    public async Task<ActionResult<Booking>> GetBooking(string reference)
    {
        var booking = await _context.Bookings
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.BookingReference == reference);

        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        return booking;
    }

    private string GenerateBookingReference()
    {
        // Simple reference generation - in production, use a more robust method
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}

public class BookingRequest
{
    public int RoomId { get; set; }
    public string CustomerName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
}
