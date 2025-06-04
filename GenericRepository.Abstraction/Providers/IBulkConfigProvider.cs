namespace GenericRepository.Abstractions.Providers
{
    public class BulkOptions
    {
        public int BatchSize { get; set; }
        public bool SetOutputIdentity { get; set; }
        public bool PreserveInsertOrder { get; set; }
        public bool TrackingEntities { get; set; }
    }

    /// <summary>
    /// Defines a provider for configuring bulk operation settings using EFCore.BulkExtensions.
    /// </summary>
    public interface IBulkConfigProvider
    {
        /// <summary>
        /// Returns the configuration settings to be used for bulk operations.
        /// </summary>
        /// <returns>A <see cref="BulkOptions"/> object containing bulk operation options.</returns>
        BulkOptions GetOptions();
    }
}
