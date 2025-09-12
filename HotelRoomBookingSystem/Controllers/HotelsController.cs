using HotelRoomBooking.Repositories;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomBooking.Controllers;

/// <summary>
/// Controller for managing hotel information and operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class HotelsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public HotelsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        var hotels = await _unitOfWork.Hotels.FindAsync(h => h.Name.ToLower() == name.ToLower());
        var hotel = hotels.FirstOrDefault();

        if (hotel == null)
        {
            return NotFound("Hotel not found.");
        }

        // Load rooms for the hotel
        var rooms = await _unitOfWork.Rooms.FindAsync(r => r.HotelId == hotel.Id);
        hotel.Rooms = rooms.ToList();

        return Ok(hotel);
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
        var hotels = await _unitOfWork.Hotels.GetWithIncludesAsync(h => h.Rooms);
        return Ok(hotels);
    }
}
