using HotelRoomBooking.Controllers;
using HotelRoomBooking.Models;

namespace HotelRoomBooking.Factories;

/// <summary>
/// Factory interface for creating bookings
/// </summary>
public interface IBookingFactory
{
    /// <summary>
    /// Create a booking from a request
    /// </summary>
    Booking CreateBooking(BookingRequest request);

    /// <summary>
    /// Create a booking with custom parameters
    /// </summary>
    Booking CreateBooking(int roomId, string customerName, DateTime checkIn, DateTime checkOut, int guests);
}

/// <summary>
/// Factory interface for creating rooms
/// </summary>
public interface IRoomFactory
{
    /// <summary>
    /// Create a standard room
    /// </summary>
    Room CreateRoom(string type, int capacity, decimal price, int hotelId);

    /// <summary>
    /// Create a single room
    /// </summary>
    Room CreateSingleRoom(decimal price, int hotelId);

    /// <summary>
    /// Create a double room
    /// </summary>
    Room CreateDoubleRoom(decimal price, int hotelId);

    /// <summary>
    /// Create a deluxe room
    /// </summary>
    Room CreateDeluxeRoom(decimal price, int hotelId);

    /// <summary>
    /// Create a suite
    /// </summary>
    Room CreateSuite(decimal price, int hotelId);
}

/// <summary>
/// Factory interface for creating hotels
/// </summary>
public interface IHotelFactory
{
    /// <summary>
    /// Create a hotel with default settings
    /// </summary>
    Hotel CreateHotel(string name);

    /// <summary>
    /// Create a hotel with custom settings
    /// </summary>
    Hotel CreateHotel(string name, string location, int starRating);
}
