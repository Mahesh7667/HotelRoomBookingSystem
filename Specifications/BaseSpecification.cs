using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HotelRoomBooking.Specifications;

/// <summary>
/// Base implementation of the specification pattern
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public abstract class BaseSpecification<T> : ISpecification<T> where T : class
{
    public Expression<Func<T, bool>> Criteria { get; protected set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public List<(Expression<Func<T, object>> KeySelector, bool Ascending)> OrderBy { get; } = new();
    public int? Take { get; private set; }
    public int? Skip { get; private set; }

    protected BaseSpecification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    protected BaseSpecification()
    {
        Criteria = _ => true; // Default to match all
    }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression, bool ascending = true)
    {
        OrderBy.Add((orderByExpression, ascending));
    }

    protected virtual void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
    }

    public virtual IQueryable<T> Apply(IQueryable<T> query)
    {
        // Apply includes
        foreach (var include in Includes)
        {
            query = query.Include(include);
        }

        // Apply string includes
        foreach (var include in IncludeStrings)
        {
            query = query.Include(include);
        }

        // Apply criteria
        query = query.Where(Criteria);

        // Apply ordering
        if (OrderBy.Any())
        {
            var firstOrder = OrderBy.First();
            var orderedQuery = firstOrder.Ascending
                ? query.OrderBy(firstOrder.KeySelector)
                : query.OrderByDescending(firstOrder.KeySelector);

            foreach (var orderBy in OrderBy.Skip(1))
            {
                orderedQuery = orderBy.Ascending
                    ? orderedQuery.ThenBy(orderBy.KeySelector)
                    : orderedQuery.ThenByDescending(orderBy.KeySelector);
            }

            query = orderedQuery;
        }

        // Apply paging
        if (Skip.HasValue)
        {
            query = query.Skip(Skip.Value);
        }

        if (Take.HasValue)
        {
            query = query.Take(Take.Value);
        }

        return query;
    }
}
