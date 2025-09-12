using HotelRoomBooking.Data;
using HotelRoomBooking.Models;

namespace HotelRoomBooking.Repositories;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly HotelBookingContext _context;
    private IBookingRepository _bookingRepository;
    private IRepository<Room> _roomRepository;
    private IRepository<Hotel> _hotelRepository;

    public UnitOfWork(HotelBookingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IBookingRepository Bookings =>
        _bookingRepository ??= new BookingRepository(_context);

    public IRepository<Room> Rooms =>
        _roomRepository ??= new Repository<Room>(_context);

    public IRepository<Hotel> Hotels =>
        _hotelRepository ??= new Repository<Hotel>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
