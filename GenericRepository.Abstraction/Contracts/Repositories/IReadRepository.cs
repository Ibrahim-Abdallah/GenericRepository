namespace GenericRepository.Abstractions.Contracts.Repositories
{
    /// <summary>
    /// Provides read operations for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IReadRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="asNoTracking">Indicates whether to disable change tracking for the query.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <returns>A queryable collection of entities.</returns>
        IQueryable<T> GetAll(bool asNoTracking = true, bool includeDeleted = false);

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <param name="asNoTracking">Whether to disable EF Core tracking for performance.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A list of all entities.</returns>
        Task<List<T>> GetAllAsync(bool asNoTracking = true, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>The entity with the specified ID, or null if not found.</returns>
        Task<T?> FindByIdAsync(object id, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to filter entities by.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <param name="asNoTracking">Indicates whether to disable change tracking for the query.</param>
        /// <returns>A queryable collection of entities that match the condition.</returns>
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, bool includeDeleted = false, bool asNoTracking = true);

        /// <summary>
        /// Retrieves entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A LINQ expression for filtering.</param>
        /// <param name="asNoTracking">Whether to disable EF Core tracking for performance.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A list of entities matching the predicate.</returns>
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the number of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">Optional predicate to count matching entities. If null, counts all.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities in the count.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>The number of matching entities.</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the number of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">Optional predicate to count matching entities. If null, counts all.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities.</param>
        /// <returns>The number of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a paged result of entities matching the specified criteria,
        /// with optional projection to a different result type.
        /// </summary>
        /// <typeparam name="TResult">The type of the result items. If <paramref name="selector"/> is not provided, it must be the same as <typeparamref name="T"/>.</typeparam>
        /// <param name="specification">A specification object that encapsulates filtering, sorting, and inclusion logic.</param>
        /// <param name="selector">An optional projection expression to transform <typeparamref name="T"/> into <typeparamref name="TResult"/>.</param>
        /// <param name="page">The page number to retrieve. Defaults to 1.</param>
        /// <param name="pageSize">The number of items per page. Defaults to <see cref="PaginationConstants.DefaultPageSize"/>.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="PagedResult{TResult}"/> containing the result items and pagination metadata.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="page"/> or <paramref name="pageSize"/> is invalid.</exception>
        Task<PagedResult<TResult>> GetPagedAsync<TResult>(
            ISpecification<T> specification,
            Expression<Func<T, TResult>>? selector = null,
            int page = 1,
            int pageSize = PaginationConstants.DefaultPageSize,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves only deleted entities.
        /// </summary>
        /// <param name="asNoTracking">Indicates whether to disable change tracking for the query.</param>
        /// <returns>A queryable collection of deleted entities.</returns>
        IQueryable<T> GetOnlyDeleted(bool asNoTracking = true);

        /// <summary>
        /// Finds entities that match the given specification.
        /// </summary>
        /// <param name="specification">The specification to filter entities by.</param>
        /// <param name="asNoTracking">Indicates whether to disable change tracking for the query.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <returns>A queryable collection of entities that match the specification.</returns>
        IQueryable<T> ApplySpecification(ISpecification<T> specification, bool asNoTracking = true, bool includeDeleted = false);

        /// <summary>
        /// Finds a single entity that matches the given specification.
        /// </summary>
        /// <param name="specification">The specification to filter the entity by.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>The entity that matches the specification, or null if not found.</returns>
        Task<T?> GetSingleOrDefaultAsync(ISpecification<T> specification, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds a list of entities that match the given specification.
        /// </summary>
        /// <param name="specification">The specification to filter entities by.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>A list of entities that match the specification.</returns>
        Task<List<T>> GetListAsync(ISpecification<T> specification, bool includeDeleted = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves entities with includes, allowing navigation properties to be loaded.
        /// </summary>
        /// <param name="asNoTracking">Indicates whether to disable change tracking for the query.</param>
        /// <param name="includeDeleted">Indicates whether to include deleted entities.</param>
        /// <param name="includeProperties">The navigation properties to include in the query.</param>
        /// <returns>A queryable collection of entities with includes.</returns>
        IQueryable<T> GetQueryableWithIncludes(bool asNoTracking = true, bool includeDeleted = false, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// Retrieves a list of projected results from the entity set that match the given predicate.
        /// </summary>
        /// <typeparam name="TResult">The result type to project to.</typeparam>
        /// <param name="predicate">An optional filtering expression to apply to the query.</param>
        /// <param name="selector">An optional projection expression to transform entities into <typeparamref name="TResult"/>.</param>
        /// <param name="asNoTracking">Whether to disable tracking for better performance in read-only scenarios. Default is <c>true</c>.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities in the result set. Default is <c>false</c>.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>A list of projected results of type <typeparamref name="TResult"/>.</returns>
        Task<List<TResult>> ProjectListAsync<TResult>(
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, TResult>>? selector = null,
            bool asNoTracking = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the first projected result that matches the specified predicate, or <c>null</c> if no match is found.
        /// </summary>
        /// <typeparam name="TResult">The result type to project to.</typeparam>
        /// <param name="predicate">A filtering expression to find the target entity.</param>
        /// <param name="selector">An optional projection expression to transform the entity into <typeparamref name="TResult"/>.</param>
        /// <param name="asNoTracking">Whether to disable tracking for better performance in read-only scenarios. Default is <c>true</c>.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities in the query. Default is <c>false</c>.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>The first projected result of type <typeparamref name="TResult"/>, or <c>null</c> if no match is found.</returns>
        Task<TResult?> ProjectFirstOrDefaultAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>>? selector = null,
            bool asNoTracking = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default);
    }
}
