using System.ComponentModel.DataAnnotations;

namespace HotelRoomBooking.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [RegularExpression("^(single|double|deluxe|suite)$", ErrorMessage = "Room type must be single, double, deluxe, or suite")]
        public string Type { get; set; }

        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int HotelId { get; set; }

        public Hotel Hotel { get; set; }
    }
}
