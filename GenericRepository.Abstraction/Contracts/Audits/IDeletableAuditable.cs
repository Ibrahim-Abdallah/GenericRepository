namespace GenericRepository.Abstractions.Contracts.Audits
{
    /// <summary>
    /// Defines audit properties for tracking deletation metadata of an entity.
    /// </summary>
    /// <typeparam name="TUserKey">
    /// The type used for identifying the user who delete the entity (e.g., <c>Guid</c>, <c>int</c>, <c>string</c>).
    /// </typeparam>
    public interface IDeletableAuditable<TUserKey> : ISoftDeletable
    {
        /// <summary>
        /// Gets or sets the identifier of the user who deleted the entity.
        /// </summary>
        TUserKey? DeletedBy { get; set; }
    }
}
