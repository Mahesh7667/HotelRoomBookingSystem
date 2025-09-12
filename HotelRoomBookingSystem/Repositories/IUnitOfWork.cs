namespace HotelRoomBooking.Repositories;

/// <summary>
/// Unit of Work interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Booking repository
    /// </summary>
    IBookingRepository Bookings { get; }

    /// <summary>
    /// Room repository
    /// </summary>
    IRepository<Models.Room> Rooms { get; }

    /// <summary>
    /// Hotel repository
    /// </summary>
    IRepository<Models.Hotel> Hotels { get; }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackAsync();
}
