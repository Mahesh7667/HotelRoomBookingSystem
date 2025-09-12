using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

/// <summary>
/// Controller for managing hotel information and operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HotelsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public HotelsController(HotelBookingContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Find a hotel by its name (case-insensitive search)
    /// </summary>
    /// <param name="name">The name of the hotel to search for</param>
    /// <returns>Hotel details including all rooms if found</returns>
    /// <response code="200">Hotel found and returned successfully</response>
    /// <response code="400">Hotel name is required</response>
    /// <response code="404">Hotel not found</response>
    [HttpGet]
    [ProducesResponseType(typeof(Hotel), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<Hotel>> GetHotelByName([FromQuery] string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest("Hotel name is required.");
        }

        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Name.ToLower() == name.ToLower());

        if (hotel == null)
        {
            return NotFound("Hotel not found.");
        }

        return hotel;
    }

    /// <summary>
    /// Get all available hotels with their room information
    /// </summary>
    /// <returns>List of all hotels with their rooms</returns>
    /// <response code="200">Hotels retrieved successfully</response>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<Hotel>), 200)]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHotels()
    {
        return await _context.Hotels.Include(h => h.Rooms).ToListAsync();
    }
}
