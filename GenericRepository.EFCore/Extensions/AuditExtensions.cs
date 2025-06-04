namespace GenericRepository.EFCore.Extensions
{
    /// <summary>
    /// Extension methods for applying audit metadata to entities.
    /// </summary>
    public static class AuditExtensions
    {
        /// <summary>
        /// Applies creation audit information to entities that implement <see cref="ICreatableAuditable{TUser}"/>.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="TUser">The type of the user identifier</typeparam>
        /// <param name="entities">The list of entities to audit</param>
        /// <param name="userId">The user ID performing the creation</param>
        public static void ApplyCreationAudit<T, TUser>(this IEnumerable<T> entities, TUser userId)
        {
            foreach (var entity in entities)
            {
                if (entity is ICreatableAuditable<TUser> creatable &&
                    (creatable.CreatedBy is null || IsEmpty(creatable.CreatedBy)))
                {
                    creatable.CreatedAt = DateTime.UtcNow;
                    creatable.CreatedBy = userId;
                }
            }
        }

        /// <summary>
        /// Applies modification audit information to entities that implement <see cref="IUpdatableAuditable{TUser}"/>.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="TUser">The type of the user identifier</typeparam>
        /// <param name="entities">The list of entities to audit</param>
        /// <param name="userId">The user ID performing the modification</param>
        public static void ApplyModificationAudit<T, TUser>(this IEnumerable<T> entities, TUser? userId)
        {
            foreach (var entity in entities)
            {
                if (entity is IUpdatableAuditable<TUser> updatable)
                {
                    updatable.UpdatedAt = DateTime.UtcNow;
                    updatable.UpdatedBy = userId;
                }
            }
        }

        /// <summary>
        /// Applies deletion audit information to entities that implement <see cref="IDeletableAuditable{TUser}"/> and <see cref="ISoftDeletable"/>.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <typeparam name="TUser">The type of the user identifier</typeparam>
        /// <param name="entities">The list of entities to audit</param>
        /// <param name="userId">The user ID performing the deletion</param>
        public static void ApplyDeletionAudit<T, TUser>(this IEnumerable<T> entities, TUser? userId)
        {
            foreach (var entity in entities)
            {
                if (entity is IDeletableAuditable<TUser> deletable &&
                    entity is ISoftDeletable soft && soft.IsDeleted)
                {
                    deletable.DeletedAt = DateTime.UtcNow;
                    deletable.DeletedBy = userId;
                }
            }
        }

        /// <summary>
        /// Determines whether the given value is considered empty.
        /// </summary>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="value">The value to evaluate</param>
        /// <returns><c>true</c> if the value is null or empty; otherwise, <c>false</c>.</returns>
        private static bool IsEmpty<TValue>(TValue? value)
        {
            return value == null || value is string s && string.IsNullOrWhiteSpace(s);
        }
    }

}
