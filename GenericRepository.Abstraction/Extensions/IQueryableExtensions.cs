namespace GenericRepository.Abstractions.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IQueryable{T}"/> to support dynamic LINQ operations,
    /// such as ordering by property name at runtime.
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Orders the elements of a sequence according to a specified property name at runtime.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence of values to order.</param>
        /// <param name="propertyName">The name of the property to order by.</param>
        /// <param name="ascending">Determines whether the sorting should be ascending or descending.</param>
        /// <returns>
        /// An <see cref="IOrderedQueryable{T}"/> whose elements are sorted according to the specified property.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the property name is invalid or not found on type <typeparamref name="T"/>.</exception>
        /// <remarks>
        /// This method uses expression trees and reflection to build a dynamic OrderBy or OrderByDescending LINQ expression.
        /// </remarks>
        public static IOrderedQueryable<T> OrderByPropertyName<T>(this IQueryable<T> source, string propertyName, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var result = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result!;
        }
    }
}
