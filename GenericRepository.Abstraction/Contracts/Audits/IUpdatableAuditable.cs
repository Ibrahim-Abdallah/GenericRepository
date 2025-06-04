namespace GenericRepository.Abstractions.Contracts.Audits
{
    /// <summary>
    /// Defines audit properties for tracking update metadata of an entity.
    /// </summary>
    /// <typeparam name="TUserKey">
    /// The type used for identifying the user who updated the entity (e.g., <c>Guid</c>, <c>int</c>, <c>string</c>).
    /// </typeparam>
    public interface IUpdatableAuditable<TUserKey>
    {
        /// <summary>
        /// Gets or sets the date and time when the entity was last updated.
        /// </summary>
        DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the entity.
        /// </summary>
        TUserKey? UpdatedBy { get; set; }
    }
}
