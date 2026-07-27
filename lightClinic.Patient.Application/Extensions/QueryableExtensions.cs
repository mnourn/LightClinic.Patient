using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace lightClinic.Patient.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>([NotNull] this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}
