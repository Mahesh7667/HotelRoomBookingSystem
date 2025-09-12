using HotelRoomBooking.Models;
using HotelRoomBooking.Services;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
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
        var result = await _bookingService.CreateBookingAsync(request);

        if (!result.IsSuccess)
        {
            var errorMessage = string.Join(" ", result.Errors);
            if (errorMessage.Contains("not found"))
            {
                return NotFound(new { errors = result.Errors });
            }
            else if (errorMessage.Contains("not available"))
            {
                return Conflict(new { errors = result.Errors });
            }
            else
            {
                return BadRequest(new { errors = result.Errors });
            }
        }

        return CreatedAtAction(nameof(GetBooking), new { reference = result.Booking!.BookingReference }, result.Booking);
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
        var booking = await _bookingService.GetBookingByReferenceAsync(reference);

        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        return Ok(booking);
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
