namespace GenericRepository.Abstractions.Contracts.Repositories
{
    /// <summary>
    /// Provides write operations for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IWriteRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Saves changes to the repository.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        /// <returns>The number of affected rows.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds multiple entities to the repository in a batch.
        /// </summary>
        /// <param name="entities">The entities to add.</param>
        /// <param name="batchSize">The size of each batch.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task AddRangeAsync(List<T> entities, int batchSize = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates multiple entities in the repository in a batch.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="batchSize">The size of each batch.</param>
        void UpdateRange(List<T> entities, int batchSize = 1000);

        /// <summary>
        /// Deletes multiple entities from the repository in a batch.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <param name="batchSize">The size of each batch.</param>
        void DeleteRange(List<T> entities, int batchSize = 1000);

        /// <summary>
        /// Soft deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to soft delete.</param>
        void SoftDelete(T entity);

        /// <summary>
        /// Soft deletes multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to soft delete.</param>
        void SoftDeleteRange(List<T> entities);

        /// <summary>
        /// Updates only the specified properties of an entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="updatedProperties">The properties to update.</param>
        void UpdatePartial(T entity, params Expression<Func<T, object>>[] updatedProperties);

        /// <summary>
        /// Executes a series of operations in a single transaction.
        /// </summary>
        /// <param name="operations">The operations to execute.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task ExecuteInTransactionAsync(Func<Task> operations, CancellationToken cancellationToken);
    }
}
