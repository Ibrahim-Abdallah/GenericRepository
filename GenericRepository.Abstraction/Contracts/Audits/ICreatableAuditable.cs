namespace GenericRepository.Abstractions.Contracts.Audits
{
    /// <summary>
    /// Defines audit properties for tracking creation metadata of an entity.
    /// </summary>
    /// <typeparam name="TUserKey">
    /// The type used for identifying the user who created the entity (e.g., <c>Guid</c>, <c>int</c>, <c>string</c>).
    /// </typeparam>
    public interface ICreatableAuditable<TUserKey>
    {
        /// <summary>
        /// Gets or sets the date and time when the entity was created.
        /// </summary>
        DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the entity.
        /// </summary>
        TUserKey CreatedBy { get; set; }
    }
}
