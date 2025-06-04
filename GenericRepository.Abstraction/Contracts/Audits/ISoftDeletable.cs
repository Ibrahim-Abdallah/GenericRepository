namespace GenericRepository.Abstractions.Contracts.Audits
{
    /// <summary>
    /// Defines properties for soft deletion of an entity.
    /// An entity marked as deleted is not physically removed from the database, 
    /// but is considered deleted for business logic purposes.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the entity is marked as deleted.
        /// </summary>
        bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the entity was marked as deleted.
        /// </summary>
        DateTime? DeletedAt { get; set; }
    }
}
