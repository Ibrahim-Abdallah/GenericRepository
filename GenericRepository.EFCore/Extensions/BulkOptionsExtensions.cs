namespace GenericRepository.EFCore.Extensions
{
    /// <summary>
    /// Provides extension methods for converting generic bulk operation options
    /// to EFCore.BulkExtensions-specific configurations.
    /// </summary>
    public static class BulkOptionsExtensions
    {
        /// <summary>
        /// Converts a <see cref="BulkOptions"/> instance into a <see cref="BulkConfig"/> object
        /// compatible with the EFCore.BulkExtensions library.
        /// </summary>
        /// <param name="options">The bulk options to convert.</param>
        /// <returns>A configured <see cref="BulkConfig"/> object.</returns>
        public static BulkConfig ToEfBulkConfig(this BulkOptions options)
        {
            return new BulkConfig
            {
                BatchSize = options.BatchSize,
                SetOutputIdentity = options.SetOutputIdentity,
                PreserveInsertOrder = options.PreserveInsertOrder,
                TrackingEntities = options.TrackingEntities
            };
        }
    }
}
