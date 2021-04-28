using Com.Atomatus.Bootstarter.Context;
using Com.Atomatus.Bootstarter.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Service provider extensions to 
    /// retrieve IServiceCrud and IServiceCrudAsync
    /// subscription.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        #region IServiceCrud<TEntity, ID>
        /// <summary>
        /// Retrieve service CRUD of Entity model defined in 
        /// AddDbContextAs[DatabaseType] by <see cref="IContextServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEntity">target entity type</typeparam>
        /// <typeparam name="TID">target entity id type</typeparam>
        /// <param name="provider">service provider</param>
        /// <returns>service CRUD to target entity</returns>
        public static IServiceCrud<TEntity, TID> GetServiceTo<TEntity, TID>([NotNull] this IServiceProvider provider)
            where TEntity : IModel<TID>
        {
            return provider.GetService<IServiceCrud<TEntity, TID>>();
        }

        /// <summary>
        /// Retrieve service CRUD async of Entity model defined in 
        /// AddDbContextAs[DatabaseType] by <see cref="IContextServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEntity">target entity type</typeparam>
        /// <typeparam name="TID">target entity id type</typeparam>
        /// <param name="provider">service provider</param>
        /// <returns>service CRUD to target entity</returns>
        public static IServiceCrudAsync<TEntity, TID> GetServiceAsyncTo<TEntity, TID>([NotNull] this IServiceProvider provider)
            where TEntity : IModel<TID>
        {
            return provider.GetService<IServiceCrudAsync<TEntity, TID>>();
        }
        #endregion

        #region IServiceCrud<TEntity>
        /// <summary>
        /// Retrieve service CRUD of Entity model defined in 
        /// AddDbContextAs[DatabaseType] by <see cref="IContextServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEntity">target entity type</typeparam>
        /// <param name="provider">service provider</param>
        /// <returns>service CRUD to target entity</returns>
        public static IServiceCrud<TEntity> GetServiceTo<TEntity>([NotNull] this IServiceProvider provider)
            where TEntity : IModel
        {
            return provider.GetService<IServiceCrud<TEntity>>();
        }

        /// <summary>
        /// Retrieve service CRUD async of Entity model defined in 
        /// AddDbContextAs[DatabaseType] by <see cref="IContextServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEntity">target entity type</typeparam>
        /// <param name="provider">service provider</param>
        /// <returns>service CRUD to target entity</returns>
        public static IServiceCrudAsync<TEntity> GetServiceAsyncTo<TEntity>([NotNull] this IServiceProvider provider)
            where TEntity : IModel
        {
            return provider.GetService<IServiceCrudAsync<TEntity>>();
        }
        #endregion
    }
}
