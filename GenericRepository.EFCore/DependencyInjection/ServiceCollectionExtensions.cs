using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenericRepository.EFCore.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for registering services related to the generic repository pattern 
    /// in the dependency injection container of an ASP.NET Core application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register repository with custom bulk config provider.
        /// </summary>
        public static IServiceCollection AddGenericRepository<TBulkProvider>(this IServiceCollection services)
            where TBulkProvider : class, IBulkConfigProvider
        {
            services.AddSingleton<IBulkConfigProvider, TBulkProvider>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddLogging();

            return services;
        }

        /// <summary>
        /// Register repository with optional default bulk config fallback.
        /// </summary>
        public static IServiceCollection AddGenericRepository(this IServiceCollection services)
        {
            // Register the bulk config provider for use in bulk operations.
            services.TryAddSingleton<IBulkConfigProvider, DefaultBulkConfigProvider>();

            // Register the generic repository for all entities.
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Register the logging service for dependency injection.
            services.AddLogging();

            return services;
        }
    }
}
