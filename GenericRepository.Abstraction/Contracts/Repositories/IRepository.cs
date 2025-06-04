namespace GenericRepository.Abstractions.Contracts.Repositories
{
    /// <summary>
    /// Provides all CRUD and bulk operations for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IRepository<T> :
    IReadRepository<T>,
    IWriteRepository<T>,
    IBulkRepository<T>
    where T : class, IEntity
    {
    }
}
