using HotelRoomBooking.Models;

namespace HotelRoomBooking.Services;

/// <summary>
/// Room service interface
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// Get available rooms for specific dates and guest count
    /// </summary>
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests);

    /// <summary>
    /// Get room by ID with hotel information
    /// </summary>
    Task<Room?> GetRoomWithHotelAsync(int roomId);

    /// <summary>
    /// Validate room capacity for guest count
    /// </summary>
    Task<bool> IsRoomSuitableForGuestsAsync(int roomId, int guestCount);
}
