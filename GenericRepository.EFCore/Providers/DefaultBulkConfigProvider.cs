namespace GenericRepository.EFCore.Providers
{
    /// <summary>
    /// Provides default configuration settings for bulk operations.
    /// Implements both generic <see cref="BulkOptions"/> for abstraction,
    /// and EF Core-specific <see cref="BulkConfig"/> for use with EFCore.BulkExtensions.
    /// </summary>
    public class DefaultBulkConfigProvider : IBulkConfigProvider
    {
        /// <summary>
        /// Returns a default <see cref="BulkOptions"/> instance with preconfigured values
        /// that are suitable for most bulk operations in a generic context.
        /// </summary>
        /// <returns>A configured <see cref="BulkOptions"/> object.</returns>
        public BulkOptions GetOptions()
        {
            return new BulkOptions
            {
                BatchSize = 1000,
                SetOutputIdentity = true,
                PreserveInsertOrder = true,
                TrackingEntities = false
            };
        }
    }
}
