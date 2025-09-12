using HotelRoomBooking.Models;
using HotelRoomBooking.Repositories;

namespace HotelRoomBooking.Services;

/// <summary>
/// Room service implementation
/// </summary>
public class RoomService : IRoomService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoomService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut, int guests)
    {
        // Validate input parameters
        if (checkIn >= checkOut)
            throw new ArgumentException("Check-out date must be after check-in date.");

        if (guests <= 0)
            throw new ArgumentException("Number of guests must be greater than 0.");

        // Get all rooms that meet capacity requirements
        var suitableRooms = await _unitOfWork.Rooms.FindAsync(r => r.Capacity >= guests);

        // Filter out rooms that have overlapping bookings
        var availableRooms = new List<Room>();

        foreach (var room in suitableRooms)
        {
            var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(room.Id, checkIn, checkOut);
            if (isAvailable)
            {
                availableRooms.Add(room);
            }
        }

        return availableRooms;
    }

    public async Task<Room?> GetRoomWithHotelAsync(int roomId)
    {
        return await _unitOfWork.Rooms.GetWithIncludesAsync(r => r.Hotel)
            .ContinueWith(t => t.Result.FirstOrDefault(r => r.Id == roomId));
    }

    public async Task<bool> IsRoomSuitableForGuestsAsync(int roomId, int guestCount)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(roomId);
        return room != null && room.Capacity >= guestCount;
    }
}
