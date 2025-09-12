using Microsoft.AspNetCore.Mvc;

namespace HotelRoomBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    // POST: api/Data/seed
    [HttpPost("seed")]
    public IActionResult SeedData()
    {
        DataStore.SeedData();
        return Ok("Data seeded successfully.");
    }

    // POST: api/Data/reset
    [HttpPost("reset")]
    public IActionResult ResetData()
    {
        DataStore.ResetData();
        return Ok("Data reset successfully.");
    }
}
