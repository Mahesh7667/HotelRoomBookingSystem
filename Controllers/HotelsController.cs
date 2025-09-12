using HotelRoomBooking.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    // GET: api/Hotels?name={name}
    [HttpGet]
    public ActionResult<Hotel> GetHotelByName([FromQuery] string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BadRequest("Hotel name is required.");
        }

        var hotel = DataStore.Hotels
            .FirstOrDefault(h => h.Name.ToLower() == name.ToLower());

        if (hotel == null)
        {
            return NotFound("Hotel not found.");
        }

        return hotel;
    }

    // GET: api/GetAllHotels
    [HttpGet("/api/GetAllHotels")]
    public ActionResult<IEnumerable<Hotel>> GetAllHotels()
    {
        return DataStore.Hotels.ToList();
    }
}
