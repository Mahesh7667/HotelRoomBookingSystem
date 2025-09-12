using HotelRoomBooking.Controllers;
using HotelRoomBooking.Models;
using HotelRoomBooking.Repositories;

namespace HotelRoomBooking.Commands;

/// <summary>
/// Command to create a new booking
/// </summary>
public class CreateBookingCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BookingRequest _request;
    private Booking? _createdBooking;

    public CreateBookingCommand(IUnitOfWork unitOfWork, BookingRequest request)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public bool CanUndo => true;

    public async Task ExecuteAsync()
    {
        // Validate room exists and capacity
        var room = await _unitOfWork.Rooms.GetByIdAsync(_request.RoomId);
        if (room == null)
            throw new InvalidOperationException("Room not found");

        if (room.Capacity < _request.NumberOfGuests)
            throw new InvalidOperationException($"Room capacity ({room.Capacity}) is insufficient for {_request.NumberOfGuests} guests");

        // Check availability
        var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(
            _request.RoomId, _request.CheckIn, _request.CheckOut);

        if (!isAvailable)
            throw new InvalidOperationException("Room is not available for the selected dates");

        // Generate reference and create booking
        var reference = GenerateBookingReference();
        _createdBooking = new Booking
        {
            BookingReference = reference,
            RoomId = _request.RoomId,
            CustomerName = _request.CustomerName,
            CheckInDate = _request.CheckIn,
            CheckOutDate = _request.CheckOut,
            NumberOfGuests = _request.NumberOfGuests
        };

        await _unitOfWork.Bookings.AddAsync(_createdBooking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UndoAsync()
    {
        if (_createdBooking != null && CanUndo)
        {
            await _unitOfWork.Bookings.DeleteAsync(_createdBooking);
            await _unitOfWork.SaveChangesAsync();
            _createdBooking = null;
        }
    }

    private string GenerateBookingReference()
    {
        return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}

/// <summary>
/// Command to cancel a booking
/// </summary>
public class CancelBookingCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _bookingReference;
    private Booking? _cancelledBooking;

    public CancelBookingCommand(IUnitOfWork unitOfWork, string bookingReference)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _bookingReference = bookingReference ?? throw new ArgumentNullException(nameof(bookingReference));
    }

    public bool CanUndo => true;

    public async Task ExecuteAsync()
    {
        _cancelledBooking = await _unitOfWork.Bookings.GetByReferenceAsync(_bookingReference);
        if (_cancelledBooking == null)
            throw new InvalidOperationException("Booking not found");

        // Check if booking can be cancelled (e.g., not in the past)
        if (_cancelledBooking.CheckInDate <= DateTime.Now.Date)
            throw new InvalidOperationException("Cannot cancel booking that has already started");

        await _unitOfWork.Bookings.DeleteAsync(_cancelledBooking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UndoAsync()
    {
        if (_cancelledBooking != null && CanUndo)
        {
            await _unitOfWork.Bookings.AddAsync(_cancelledBooking);
            await _unitOfWork.SaveChangesAsync();
            _cancelledBooking = null;
        }
    }
}

/// <summary>
/// Command to update booking dates
/// </summary>
public class UpdateBookingDatesCommand : ICommand
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _bookingReference;
    private readonly DateTime _newCheckIn;
    private readonly DateTime _newCheckOut;
    private DateTime _originalCheckIn;
    private DateTime _originalCheckOut;

    public UpdateBookingDatesCommand(
        IUnitOfWork unitOfWork,
        string bookingReference,
        DateTime newCheckIn,
        DateTime newCheckOut)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _bookingReference = bookingReference ?? throw new ArgumentNullException(nameof(bookingReference));
        _newCheckIn = newCheckIn;
        _newCheckOut = newCheckOut;
    }

    public bool CanUndo => true;

    public async Task ExecuteAsync()
    {
        var booking = await _unitOfWork.Bookings.GetByReferenceAsync(_bookingReference);
        if (booking == null)
            throw new InvalidOperationException("Booking not found");

        // Store original dates for undo
        _originalCheckIn = booking.CheckInDate;
        _originalCheckOut = booking.CheckOutDate;

        // Check if new dates are available
        var isAvailable = await _unitOfWork.Bookings.IsRoomAvailableAsync(
            booking.RoomId, _newCheckIn, _newCheckOut);

        if (!isAvailable)
            throw new InvalidOperationException("Room is not available for the new dates");

        // Update dates
        booking.CheckInDate = _newCheckIn;
        booking.CheckOutDate = _newCheckOut;

        await _unitOfWork.Bookings.UpdateAsync(booking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UndoAsync()
    {
        if (CanUndo)
        {
            var booking = await _unitOfWork.Bookings.GetByReferenceAsync(_bookingReference);
            if (booking != null)
            {
                booking.CheckInDate = _originalCheckIn;
                booking.CheckOutDate = _originalCheckOut;
                await _unitOfWork.Bookings.UpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
