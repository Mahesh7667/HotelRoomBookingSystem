using HotelRoomBooking.Controllers;
using HotelRoomBooking.Models;

namespace HotelRoomBooking.Services;

/// <summary>
/// Result of booking operations
/// </summary>
public class BookingResult
{
    public bool IsSuccess { get; private set; }
    public Booking? Booking { get; private set; }
    public IEnumerable<string> Errors { get; private set; }

    private BookingResult(bool isSuccess, Booking? booking = null, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Booking = booking;
        Errors = errors ?? Array.Empty<string>();
    }

    public static BookingResult Success(Booking booking) =>
        new BookingResult(true, booking);

    public static BookingResult Failure(IEnumerable<string> errors) =>
        new BookingResult(false, errors: errors);

    public static BookingResult Failure(string error) =>
        new BookingResult(false, errors: new[] { error });
}

/// <summary>
/// Booking service interface
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Create a new booking
    /// </summary>
    Task<BookingResult> CreateBookingAsync(BookingRequest request);

    /// <summary>
    /// Get booking by reference
    /// </summary>
    Task<Booking?> GetBookingByReferenceAsync(string reference);

    /// <summary>
    /// Check if room is available for dates
    /// </summary>
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);

    /// <summary>
    /// Get all bookings
    /// </summary>
    Task<IEnumerable<Booking>> GetAllBookingsAsync();
}
