namespace GenericRepository.Abstractions.Specifications
{
    /// <summary>
    /// Represents a base class for creating specifications used to filter and query entities.
    /// Implements the Specification Pattern, allowing for flexible query construction.
    /// </summary>
    /// <typeparam name="T">The type of the entity being queried.</typeparam>
    public abstract class Specification<T>(Expression<Func<T, bool>> criteria = null!) : ISpecification<T>
    {
        /// <inheritdoc />
        public Expression<Func<T, bool>> Criteria { get; private set; } = criteria;

        /// <inheritdoc />
        public List<Expression<Func<T, object>>> Includes { get; private set; } = [];

        /// <inheritdoc />
        public Expression<Func<T, object>> OrderBy { get; private set; } = null!;

        /// <inheritdoc />
        public bool Ascending { get; private set; } = true;  // Assume ascending order

        /// <inheritdoc />
        public int Skip { get; private set; }

        /// <inheritdoc />
        public int Take { get; private set; }

        /// <summary>
        /// Replaces the current filtering criteria with the provided expression.
        /// </summary>
        /// <param name="criteria">The filtering expression to apply.</param>
        public void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary>
        /// Sets the order-by expression and sorting direction.
        /// </summary>
        /// <param name="orderBy">The expression used to order the result set.</param>
        /// <param name="ascending">Whether the ordering is ascending. Default is true.</param>
        public void AddOrderBy(Expression<Func<T, object>> orderBy, bool ascending = true)
        {
            OrderBy = orderBy;
            Ascending = ascending;
        }

        /// <summary>
        /// Applies paging to the query by specifying how many items to skip and take.
        /// </summary>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The number of items to take.</param>
        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }

        /// <summary>
        /// Adds a navigation property to be included in the query.
        /// Useful for eager loading related data.
        /// </summary>
        /// <param name="includeExpression">The expression representing the related entity to include.</param>
        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
