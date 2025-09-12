using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly HotelBookingContext _context;

    public HotelsController(HotelBookingContext context)
    {
        _context = context;
    }

    // GET: api/Hotels?name={name}
    [HttpGet]
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

    // GET: api/Hotels/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHotels()
    {
        return await _context.Hotels.Include(h => h.Rooms).ToListAsync();
    }
}
