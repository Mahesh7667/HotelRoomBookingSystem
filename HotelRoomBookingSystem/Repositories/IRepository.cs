using System.Linq.Expressions;

namespace HotelRoomBooking.Repositories;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Get entity by ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Find entities by predicate
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Add new entity
    /// </summary>
    Task AddAsync(T entity);

    /// <summary>
    /// Update existing entity
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete entity
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Check if entity exists
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Get entities with includes
    /// </summary>
    Task<IEnumerable<T>> GetWithIncludesAsync(params Expression<Func<T, object>>[] includes);
}
