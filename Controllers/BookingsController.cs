using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelRoomBooking.Controllers;

/// <summary>
/// Controller for managing hotel room bookings
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public BookingsController(HotelBookingContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Create a new room booking
    /// </summary>
    /// <param name="request">Booking details including room, dates, and guest information</param>
    /// <returns>Created booking with unique reference</returns>
    /// <response code="201">Booking created successfully</response>
    /// <response code="400">Invalid booking data or room capacity exceeded</response>
    /// <response code="404">Room not found</response>
    /// <response code="409">Room not available for selected dates</response>
    [HttpPost]
    [ProducesResponseType(typeof(Booking), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
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

    /// <summary>
    /// Get booking details by booking reference
    /// </summary>
    /// <param name="reference">Unique booking reference (8 characters)</param>
    /// <returns>Complete booking information including room and hotel details</returns>
    /// <response code="200">Booking found and returned</response>
    /// <response code="404">Booking not found</response>
    [HttpGet("{reference}")]
    [ProducesResponseType(typeof(Booking), 200)]
    [ProducesResponseType(404)]
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

/// <summary>
/// Request model for creating a new booking
/// </summary>
public class BookingRequest
{
    /// <summary>
    /// ID of the room to book
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Room ID must be a positive number")]
    public int RoomId { get; set; }

    /// <summary>
    /// Full name of the customer making the booking
    /// </summary>
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Customer name can only contain letters and spaces")]
    public string CustomerName { get; set; }

    /// <summary>
    /// Check-in date (must be in the future)
    /// </summary>
    [Required(ErrorMessage = "Check-in date is required")]
    public DateTime CheckIn { get; set; }

    /// <summary>
    /// Check-out date (must be after check-in date)
    /// </summary>
    [Required(ErrorMessage = "Check-out date is required")]
    [DateGreaterThan("CheckIn", ErrorMessage = "Check-out date must be after check-in date")]
    public DateTime CheckOut { get; set; }

    /// <summary>
    /// Number of guests (must be between 1 and room capacity)
    /// </summary>
    [Required(ErrorMessage = "Number of guests is required")]
    [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
    public int NumberOfGuests { get; set; }
}
