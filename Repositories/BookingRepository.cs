using HotelRoomBooking.Data;
using HotelRoomBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Repositories;

/// <summary>
/// Booking repository implementation
/// </summary>
public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(HotelBookingContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Booking>> GetBookingsByRoomAsync(int roomId)
    {
        return await FindAsync(b => b.RoomId == roomId);
    }

    public async Task<IEnumerable<Booking>> GetOverlappingBookingsAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return await _dbSet
            .Where(b => b.RoomId == roomId &&
                       ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                        (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut) ||
                        (b.CheckInDate >= checkIn && b.CheckOutDate <= checkOut)))
            .ToListAsync();
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var overlappingBookings = await GetOverlappingBookingsAsync(roomId, checkIn, checkOut);
        return !overlappingBookings.Any();
    }

    public async Task<Booking?> GetByReferenceAsync(string reference)
    {
        return await _dbSet
            .Include(b => b.Room)
            .ThenInclude(r => r.Hotel)
            .FirstOrDefaultAsync(b => b.BookingReference == reference);
    }
}
