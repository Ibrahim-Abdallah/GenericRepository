namespace GenericRepository.EFCore.Extensions
{
    /// <summary>
    /// Evaluates a given specification and applies its criteria, includes, ordering, and paging to a queryable sequence.
    /// </summary>
    /// <typeparam name="T">The entity type the specification applies to.</typeparam>
    internal static class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// Applies a specification to the provided queryable, including filtering, eager loading, sorting, and paging.
        /// </summary>
        /// <param name="query">The base query to apply the specification to.</param>
        /// <param name="specification">The specification containing the query rules.</param>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> that represents the modified query based on the specification.
        /// </returns>
        /// <remarks>
        /// This method supports applying:
        /// <list type="bullet">
        ///   <item><description>Filtering via <c>Criteria</c></description></item>
        ///   <item><description>Includes for eager loading</description></item>
        ///   <item><description>Ordering (ascending/descending)</description></item>
        ///   <item><description>Paging via Skip and Take</description></item>
        /// </list>
        /// </remarks>
        internal static IQueryable<T> ApplySpecification(IQueryable<T> query, ISpecification<T> specification)
        {
            if (specification == null)
                return query;

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            foreach (var include in specification.Includes)
            {
                query = query.Include(include);
            }

            if (specification.OrderBy != null)
            {
                query = specification.Ascending
                    ? query.OrderBy(specification.OrderBy)
                    : query.OrderByDescending(specification.OrderBy);
            }

            if (specification.Skip > 0)
                query = query.Skip(specification.Skip);

            if (specification.Take > 0)
                query = query.Take(specification.Take);

            return query;
        }
    }
}
