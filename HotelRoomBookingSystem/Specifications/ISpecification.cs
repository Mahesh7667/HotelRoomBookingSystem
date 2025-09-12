using System.Linq.Expressions;

namespace HotelRoomBooking.Specifications;

/// <summary>
/// Specification pattern interface for complex query logic
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// The criteria expression for filtering
    /// </summary>
    Expression<Func<T, bool>> Criteria { get; }

    /// <summary>
    /// Navigation properties to include
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// String-based includes for dynamic scenarios
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Ordering expressions
    /// </summary>
    List<(Expression<Func<T, object>> KeySelector, bool Ascending)> OrderBy { get; }

    /// <summary>
    /// Take N records
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Skip N records
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Apply the specification to a query
    /// </summary>
    IQueryable<T> Apply(IQueryable<T> query);
}
