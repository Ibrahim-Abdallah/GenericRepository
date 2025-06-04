namespace GenericRepository.EFCore.Extensions
{
    internal static class QueryableExtensions
    {
        /// <summary>
        /// Applies soft delete filtering to a query if the entity implements ISoftDeletable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The queryable source.</param>
        /// <param name="includeDeleted">Whether to include soft-deleted entities.</param>
        /// <returns>A filtered queryable with soft-deleted items excluded if needed.</returns>
        public static IQueryable<T> ApplySoftDeleteFilter<T>(this IQueryable<T> query, bool includeDeleted)
            where T : class
        {
            if (!includeDeleted && typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e =>
                    EF.Property<bool>(e, nameof(ISoftDeletable.IsDeleted)) == false
                );
            }

            return query;
        }

        public static async Task<PagedResult<TResult>> ToPagedResultAsync<TSource, TResult>(
            this IQueryable<TSource> query,
            int page,
            int pageSize,
            Expression<Func<TSource, TResult>>? selector = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

            var totalCount = await query.CountAsync(cancellationToken);
            var pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);
            var skip = (page - 1) * pageSize;

            var projected = selector is not null
                ? await query.Skip(skip).Take(pageSize).Select(selector).ToListAsync(cancellationToken)
                : await query.Skip(skip).Take(pageSize).Cast<TResult>().ToListAsync(cancellationToken); // fallback: assumes TSource == TResult

            return new PagedResult<TResult>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalCount,
                PageCount = pageCount,
                Results = projected
            };
        }
    }
}
