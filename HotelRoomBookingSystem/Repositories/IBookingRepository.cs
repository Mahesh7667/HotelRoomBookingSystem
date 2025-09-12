using HotelRoomBooking.Models;

namespace HotelRoomBooking.Repositories;

/// <summary>
/// Booking-specific repository operations
/// </summary>
public interface IBookingRepository : IRepository<Booking>
{
    /// <summary>
    /// Get bookings for a specific room
    /// </summary>
    Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId);

    /// <summary>
    /// Get bookings that overlap with the given date range for a room
    /// </summary>
    Task<IEnumerable<Booking>> GetOverlappingBookingsAsync(int roomId, DateTime checkIn, DateTime checkOut);

    /// <summary>
    /// Check if a room is available for the given dates
    /// </summary>
    Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut);

    /// <summary>
    /// Get booking by reference
    /// </summary>
    Task<Booking?> GetByReferenceAsync(string reference);
}
