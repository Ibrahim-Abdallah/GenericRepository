namespace GenericRepository.EFCore.Repositories
{
    /// <summary>
    /// Generic repository implementation for accessing entities in a database context.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public class Repository<T>(DbContext context, IBulkConfigProvider? bulkConfigProvider = null, ILogger<Repository<T>>? logger = null) : IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The EF DbContext instance.</param>
        /// <param name="bulkConfigProvider">Optional provider for bulk operation configurations.</param>
        /// <param name="logger">Optional logger for logging operations and fallbacks.</param>
        protected DbContext _context { get; set; } = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IBulkConfigProvider? _bulkConfigProvider = bulkConfigProvider;
        private readonly ILogger<Repository<T>>? _logger = logger;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        /// <summary>
        /// Builds a base <see cref="IQueryable{T}"/> from the entity set, 
        /// applying optional filters for soft deletion and tracking behavior.
        /// </summary>
        /// <param name="asNoTracking">
        /// If <c>true</c>, the query will be executed without tracking entities in the context, 
        /// improving read-only performance. Defaults to <c>true</c>.
        /// </param>
        /// <param name="includeDeleted">
        /// If <c>true</c>, includes entities marked as deleted (soft-deleted). 
        /// If <c>false</c> and the entity implements <see cref="ISoftDeletable"/>, filters out soft-deleted entities. 
        /// Defaults to <c>false</c>.
        /// </param>
        /// <returns>
        /// An <see cref="IQueryable{T}"/> representing the filtered entity set, 
        /// optionally excluding soft-deleted items and using no-tracking if specified.
        /// </returns>
        private IQueryable<T> GetQueryable(bool asNoTracking = true, bool includeDeleted = false)
        {
            IQueryable<T> query = _dbSet;

            query = query.ApplySoftDeleteFilter(includeDeleted);

            return asNoTracking ? query.AsNoTracking() : query;
        }

        /// <inheritdoc />
        public IQueryable<T> GetAll(bool asNoTracking = true, bool includeDeleted = false)
            => GetQueryable(asNoTracking, includeDeleted);

        /// <inheritdoc />
        public async Task<List<T>> GetAllAsync(bool asNoTracking = true, bool includeDeleted = false, CancellationToken cancellationToken = default)
            => await GetQueryable(asNoTracking, includeDeleted).ToListAsync(cancellationToken);

        /// <inheritdoc />
        public async Task<T?> FindByIdAsync(object id, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new[] { id }, cancellationToken);

            if (!includeDeleted && entity is ISoftDeletable deletable && deletable.IsDeleted)
                return null;

            return entity;
        }

        /// <inheritdoc />
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = true, bool includeDeleted = false)
            => GetQueryable(asNoTracking, includeDeleted).Where(predicate);

        /// <inheritdoc />
        public async Task<List<T>> FindByAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = true, bool includeDeleted = false, CancellationToken cancellationToken = default)
            => await GetQueryable(asNoTracking, includeDeleted).Where(predicate).ToListAsync(cancellationToken);

        /// <inheritdoc />
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool includeDeleted = false, CancellationToken cancellationToken = default)
            => await GetQueryable(true, includeDeleted).AnyAsync(predicate, cancellationToken);

        /// <inheritdoc />
        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            query = query.ApplySoftDeleteFilter(includeDeleted);

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
            ISpecification<T> specification,
            Expression<Func<T, TResult>>? selector = null,
            int page = 1,
            int pageSize = PaginationConstants.DefaultPageSize,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
            if (pageSize <= 0 || pageSize > PaginationConstants.MaxPageSize) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var query = ApplySpecification(specification).ApplySoftDeleteFilter(includeDeleted);

            return await query.ToPagedResultAsync<T, TResult>(
                page,
                pageSize,
                selector,
                cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<T> GetOnlyDeleted(bool asNoTracking = true)
        {
            if (!typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
                throw new InvalidOperationException($"{typeof(T).Name} doesn't support soft delete.");

            return GetQueryable(asNoTracking, includeDeleted: true)
                .Where(e => EF.Property<bool>(e, nameof(ISoftDeletable.IsDeleted)) == true);
        }

        /// <inheritdoc />
        public IQueryable<T> ApplySpecification(ISpecification<T> specification, bool asNoTracking = true, bool includeDeleted = false)
        {
            var query = SpecificationEvaluator<T>.ApplySpecification(_dbSet.AsQueryable(), specification);

            query = query.ApplySoftDeleteFilter(includeDeleted);

            return asNoTracking ? query.AsNoTracking() : query;
        }

        /// <inheritdoc />
        public async Task<T?> GetSingleOrDefaultAsync(ISpecification<T> specification, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification, includeDeleted: includeDeleted);
            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<T>> GetListAsync(ISpecification<T> specification, bool includeDeleted = false, CancellationToken cancellationToken = default)
        {
            var query = ApplySpecification(specification, includeDeleted: includeDeleted);
            return await query.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public IQueryable<T> GetQueryableWithIncludes(bool asNoTracking = true, bool includeDeleted = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return GetQueryable(asNoTracking, includeDeleted: includeDeleted);
        }

        /// <inheritdoc />
        public async Task<List<TResult>> ProjectListAsync<TResult>(
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, TResult>>? selector = null,
            bool asNoTracking = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().ApplySoftDeleteFilter(includeDeleted);

            if (predicate is not null)
                query = query.Where(predicate);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (selector is not null)
                return await query.Select(selector).ToListAsync(cancellationToken);

            return await query.Cast<TResult>().ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResult?> ProjectFirstOrDefaultAsync<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>>? selector = null,
            bool asNoTracking = true,
            bool includeDeleted = false,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<T>().ApplySoftDeleteFilter(includeDeleted);

            query = query.Where(predicate);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (selector is not null)
                return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);

            return await query.Cast<TResult>().FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _dbSet.AddAsync(entity, cancellationToken);

            return entity;
        }

        /// <inheritdoc />
        public void Update(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Update(entity);
        }

        /// <inheritdoc />
        public void Delete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _dbSet.Remove(entity);
        }

        /// <inheritdoc />
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.ChangeTracker.HasChanges() ? await _context.SaveChangesAsync(cancellationToken) : 0;
        }

        /// <inheritdoc />
        public async Task AddRangeAsync(List<T> entities, int batchSize = 1000, CancellationToken cancellationToken = default)
        {
            for (int i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize).ToList();
                await _dbSet.AddRangeAsync(batch, cancellationToken);
            }
        }

        /// <inheritdoc />
        public void UpdateRange(List<T> entities, int batchSize = 1000)
        {
            for (int i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize).ToList();
                _dbSet.UpdateRange(batch);
            }
        }

        /// <inheritdoc />
        public void DeleteRange(List<T> entities, int batchSize = 1000)
        {
            for (int i = 0; i < entities.Count; i += batchSize)
            {
                var batch = entities.Skip(i).Take(batchSize).ToList();
                _dbSet.RemoveRange(batch);
            }
        }

        /// <inheritdoc />
        public void SoftDelete(T entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            if (entity is ISoftDeletable deletableEntity)
            {
                deletableEntity.IsDeleted = true;
                deletableEntity.DeletedAt = DateTime.UtcNow;
                Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        /// <inheritdoc />
        public void SoftDeleteRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                SoftDelete(entity);
            }
        }

        /// <inheritdoc />
        public async Task<List<T>> BulkInsertAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                await _context.BulkInsertAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to AddRangeAsync + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF insert
                await AddRangeAsync(entities, cancellationToken: cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return entities;
        }

        /// <inheritdoc />
        public async Task<List<T>> BulkInsertAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                entities.ApplyCreationAudit(userId);
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                await _context.BulkInsertAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to AddRangeAsync + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF insert
                await AddRangeAsync(entities, cancellationToken: cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return entities;
        }

        /// <inheritdoc />
        public async Task BulkUpdateAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                await _context.BulkUpdateAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to UpdateRange + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF update
                UpdateRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task BulkUpdateAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                entities.ApplyModificationAudit(userId);
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                await _context.BulkUpdateAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to UpdateRange + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF update
                UpdateRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task BulkDeleteAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                config.SetOutputIdentity = false; // Ensure no output identity is set for deletion
                await _context.BulkDeleteAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to RemoveRange + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF remove
                DeleteRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task BulkDeleteAsync<TUser>(List<T> entities, TUser? userId, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(entities);

            if (_bulkConfigProvider is not null)
            {
                entities.ApplyDeletionAudit(userId);
                var config = _bulkConfigProvider.GetOptions().ToEfBulkConfig();
                config.SetOutputIdentity = false; // Ensure no output identity is set for deletion
                await _context.BulkDeleteAsync(entities, config, cancellationToken: cancellationToken);
            }
            else
            {
                _logger?.LogWarning("BulkConfigProvider is missing. Falling back to RemoveRange + SaveChangesAsync.");
                // TO DO: Log the warning or handle it as needed by Serilog
                // Fallback to normal EF remove
                DeleteRange(entities);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public void UpdatePartial(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            _dbSet.Attach(entity);
            foreach (var property in updatedProperties)
            {
                _context.Entry(entity).Property(property).IsModified = true;
            }
        }

        /// <inheritdoc />
        public async Task ExecuteInTransactionAsync(Func<Task> actions, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await actions();
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                // TO DO: Log the exception or handle it as needed
            }
        }
    }
}
