using HotelRoomBooking.Models;

namespace HotelRoomBooking.Specifications;

/// <summary>
/// Specification for finding available rooms within date range and guest count
/// </summary>
public class AvailableRoomsSpecification : BaseSpecification<Room>
{
    public AvailableRoomsSpecification(DateTime checkIn, DateTime checkOut, int guests)
    {
        // Rooms that can accommodate the number of guests
        Criteria = room => room.Capacity >= guests;

        // Include hotel information
        AddInclude(r => r.Hotel);

        // Order by price ascending
        ApplyOrderBy(r => r.Price);
    }
}

/// <summary>
/// Specification for finding rooms by hotel
/// </summary>
public class RoomsByHotelSpecification : BaseSpecification<Room>
{
    public RoomsByHotelSpecification(int hotelId)
    {
        Criteria = room => room.HotelId == hotelId;
        AddInclude(r => r.Hotel);
        ApplyOrderBy(r => r.Type);
        ApplyOrderBy(r => r.Capacity);
    }
}

/// <summary>
/// Specification for finding bookings by date range
/// </summary>
public class BookingsByDateRangeSpecification : BaseSpecification<Booking>
{
    public BookingsByDateRangeSpecification(DateTime startDate, DateTime endDate)
    {
        Criteria = booking =>
            booking.CheckInDate < endDate && booking.CheckOutDate > startDate;

        AddInclude(b => b.Room);
        ApplyOrderBy(b => b.CheckInDate);
    }
}

/// <summary>
/// Specification for finding overlapping bookings for a room
/// </summary>
public class OverlappingBookingsSpecification : BaseSpecification<Booking>
{
    public OverlappingBookingsSpecification(int roomId, DateTime checkIn, DateTime checkOut)
    {
        Criteria = booking =>
            booking.RoomId == roomId &&
            ((booking.CheckInDate <= checkIn && booking.CheckOutDate > checkIn) ||
             (booking.CheckInDate < checkOut && booking.CheckOutDate >= checkOut) ||
             (booking.CheckInDate >= checkIn && booking.CheckOutDate <= checkOut));

        AddInclude(b => b.Room);
    }
}

/// <summary>
/// Specification for finding hotels with available rooms
/// </summary>
public class HotelsWithAvailableRoomsSpecification : BaseSpecification<Hotel>
{
    public HotelsWithAvailableRoomsSpecification(DateTime checkIn, DateTime checkOut, int guests)
    {
        // Include rooms and filter hotels that have available rooms
        AddInclude(h => h.Rooms);

        // This would need to be handled in the service layer since we need to check bookings
        // For now, just include all hotels with rooms
        Criteria = hotel => hotel.Rooms.Any(r => r.Capacity >= guests);
    }
}
