using System.ComponentModel.DataAnnotations;

namespace HotelRoomBooking.Models
{
    public class Hotel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        public ICollection<Room> Rooms { get; set; }
    }
}
