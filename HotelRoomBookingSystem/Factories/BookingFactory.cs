using HotelRoomBooking.Controllers;
using HotelRoomBooking.Models;

namespace HotelRoomBooking.Factories;

/// <summary>
/// Standard booking factory implementation
/// </summary>
public class BookingFactory : IBookingFactory
{
    public Booking CreateBooking(BookingRequest request)
    {
        return CreateBooking(
            request.RoomId,
            request.CustomerName,
            request.CheckIn,
            request.CheckOut,
            request.NumberOfGuests);
    }

    public Booking CreateBooking(int roomId, string customerName, DateTime checkIn, DateTime checkOut, int guests)
    {
        return new Booking
        {
            BookingReference = GenerateBookingReference(),
            RoomId = roomId,
            CustomerName = customerName,
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            NumberOfGuests = guests
        };
    }

    private string GenerateBookingReference()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}

/// <summary>
/// Room factory implementation
/// </summary>
public class RoomFactory : IRoomFactory
{
    public Room CreateRoom(string type, int capacity, decimal price, int hotelId)
    {
        return new Room
        {
            Type = type,
            Capacity = capacity,
            Price = price,
            HotelId = hotelId
        };
    }

    public Room CreateSingleRoom(decimal price, int hotelId)
    {
        return CreateRoom("single", 1, price, hotelId);
    }

    public Room CreateDoubleRoom(decimal price, int hotelId)
    {
        return CreateRoom("double", 2, price, hotelId);
    }

    public Room CreateDeluxeRoom(decimal price, int hotelId)
    {
        return CreateRoom("deluxe", 4, price, hotelId);
    }
}

/// <summary>
/// Hotel factory implementation
/// </summary>
public class HotelFactory : IHotelFactory
{
    public Hotel CreateHotel(string name)
    {
        return new Hotel
        {
            Name = name,
            // Additional default properties can be set here
        };
    }

    public Hotel CreateHotel(string name, string location, int starRating)
    {
        var hotel = CreateHotel(name);
        // In a real implementation, you might have additional properties
        // like Location and StarRating in the Hotel model
        return hotel;
    }
}
