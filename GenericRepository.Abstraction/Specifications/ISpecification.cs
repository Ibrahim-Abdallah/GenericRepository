namespace GenericRepository.Abstractions.Specifications
{
    /// <summary>
    /// Defines the criteria for querying entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// The criteria expression to filter entities.
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary>
        /// The list of expressions to include related entities.
        /// </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary>
        /// The expression to order the results by.
        /// </summary>
        Expression<Func<T, object>> OrderBy { get; }

        /// <summary>
        /// Indicates whether the order is ascending.
        /// </summary>
        bool Ascending { get; }

        /// <summary>
        /// The number of entities to skip in the query.
        /// </summary>
        int Skip { get; }

        /// <summary>
        /// The number of entities to take from the query.
        /// </summary>
        int Take { get; }
    }
}
