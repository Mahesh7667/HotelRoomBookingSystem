using System.ComponentModel.DataAnnotations;

namespace HotelRoomBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string BookingReference { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int RoomId { get; set; }

        public Room Room { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string CustomerName { get; set; }

        [Required]
        [Range(1, 10)]
        public int NumberOfGuests { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DateGreaterThan("CheckInDate")]
        public DateTime CheckOutDate { get; set; }
    }

    // Custom validation attribute for date comparison
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}");

            return ValidationResult.Success;
        }
    }
}
