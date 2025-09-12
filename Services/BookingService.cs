using HotelRoomBooking.Controllers;
using HotelRoomBooking.Models;
using HotelRoomBooking.Repositories;

namespace HotelRoomBooking.Services;

/// <summary>
/// Booking service implementation
/// </summary>
public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<BookingResult> CreateBookingAsync(BookingRequest request)
    {
        // Validate request
        var validationErrors = await ValidateBookingRequestAsync(request);
        if (validationErrors.Any())
        {
            return BookingResult.Failure(validationErrors);
        }

        // Check room availability
        var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(
            request.RoomId, request.CheckIn, request.CheckOut);

        if (!isAvailable)
        {
            return BookingResult.Failure("Room is not available for the selected dates.");
        }

        // Generate booking reference
        var bookingReference = GenerateBookingReference();

        // Create booking
        var booking = new Booking
        {
            BookingReference = bookingReference,
            RoomId = request.RoomId,
            CustomerName = request.CustomerName,
            CheckInDate = request.CheckIn,
            CheckOutDate = request.CheckOut,
            NumberOfGuests = request.NumberOfGuests
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return BookingResult.Success(booking);
    }

    public async Task<Booking?> GetBookingByReferenceAsync(string reference)
    {
        return await _unitOfWork.Bookings.GetByReferenceAsync(reference);
    }

    public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime checkIn, DateTime checkOut)
    {
        return await _unitOfWork.Bookings.IsRoomAvailableAsync(roomId, checkIn, checkOut);
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
    {
        return await _unitOfWork.Bookings.GetWithIncludesAsync(b => b.Room, b => b.Room.Hotel);
    }

    private async Task<List<string>> ValidateBookingRequestAsync(BookingRequest request)
    {
        var errors = new List<string>();

        // Check if room exists
        var roomExists = await _unitOfWork.Rooms.ExistsAsync(request.RoomId);
        if (!roomExists)
        {
            errors.Add("Room not found.");
            return errors; // Early return if room doesn't exist
        }

        // Get room details for capacity validation
        var room = await _unitOfWork.Rooms.GetByIdAsync(request.RoomId);
        if (room == null)
        {
            errors.Add("Room not found.");
            return errors;
        }

        // Validate capacity
        if (request.NumberOfGuests > room.Capacity)
        {
            errors.Add($"Room capacity is {room.Capacity} guests, but {request.NumberOfGuests} guests requested.");
        }

        // Validate dates
        if (request.CheckIn >= request.CheckOut)
        {
            errors.Add("Check-out date must be after check-in date.");
        }

        // Check if dates are in the future (optional business rule)
        if (request.CheckIn <= DateTime.Now.Date)
        {
            errors.Add("Check-in date must be in the future.");
        }

        return errors;
    }

    private string GenerateBookingReference()
    {
        // Generate a unique 8-character reference
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}
