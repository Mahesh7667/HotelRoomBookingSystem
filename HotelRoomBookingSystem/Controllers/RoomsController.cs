using HotelRoomBooking.Services;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomBooking.Controllers;

/// <summary>
/// Controller for managing room availability and search operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
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
        try
        {
            var availableRooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut, guests);
            return Ok(availableRooms);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
