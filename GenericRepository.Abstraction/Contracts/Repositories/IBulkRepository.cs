namespace GenericRepository.Abstractions.Contracts.Repositories
{
    /// <summary>
    /// Provides bulk operations for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IBulkRepository<T> where T : class
    {
        /// <summary>
        /// Performs a bulk insert of multiple entities into the repository.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task<List<T>> BulkInsertAsync(List<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs bulk insert for a list of entities, with optional audit info applied if the entity supports it.
        /// </summary>
        /// <typeparam name="TUser">The type of the user identifier (e.g., string, Guid, long)</typeparam>
        /// <param name="entities">List of entities to insert</param>
        /// <param name="userId">The user ID for auditing (optional)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of inserted entities</returns>
        Task<List<T>> BulkInsertAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a bulk update of multiple entities in the repository.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task BulkUpdateAsync(List<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs bulk update on a list of entities with optional audit metadata fallback.
        /// </summary>
        /// <typeparam name="TUser">The type of the user identifier</typeparam>
        /// <param name="entities">The entities to update</param>
        /// <param name="userId">The user performing the update</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task BulkUpdateAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs a bulk delete of multiple entities from the repository.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
        Task BulkDeleteAsync(List<T> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Performs bulk delete on a list of entities with optional audit metadata fallback.
        /// </summary>
        /// <typeparam name="TUser">The type of the user identifier</typeparam>
        /// <param name="entities">The entities to delete</param>
        /// <param name="userId">The user performing the delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task BulkDeleteAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default);
    }
}
