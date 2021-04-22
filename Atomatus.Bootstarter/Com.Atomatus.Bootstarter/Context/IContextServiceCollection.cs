using Com.Atomatus.Bootstarter.Model;
using Com.Atomatus.Bootstarter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter.Context
{
    #region Contracts
    /// <summary>
    /// Specifies the contract for a collection of <see cref="IService"/>.
    /// </summary>    
    public interface IContextServiceOperationCollection
    {
        /// <summary>
        /// <para>
        /// Add specified contract service to target dbcontext.
        /// </para>
        /// <para>
        /// <i>Obs.: Can extract entity type from service contract to Context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="serviceLifetime">service lifetime mode</param>
        /// <returns>current context service collection instance</returns>
        IContextServiceOperationCollection AddService<TService>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TService : IService;

        /// <summary>
        /// <para>
        /// Add specified contract service to target dbcontext as <see cref="ServiceLifetime.Scoped"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: Can extract entity type from service contract to Context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextServiceOperationCollection AddScoped<TService>() where TService : IService;

        /// <summary>
        /// <para>
        /// Add specified contract service to target dbcontext as <see cref="ServiceLifetime.Singleton"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: Can extract entity type from service contract to Context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextServiceOperationCollection AddSingleton<TService>() where TService : IService;

        /// <summary>
        /// <para>
        /// Add specified contract service to target dbcontext as <see cref="ServiceLifetime.Transient"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: Can extract entity type from service contract to Context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextServiceOperationCollection AddTransient<TService>() where TService : IService;
    }

    /// <summary>
    /// Specifies the contract for a collection of <see cref="IModel{TID}"/>.
    /// </summary>    
    public interface IContextModelIdOperationCollection
    {
        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <param name="serviceLifetime">service lifetime mode</param>
        /// <returns>current context service collection instance</returns>
        IContextModelIdOperationCollection AddServiceTo<TModel, TID>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TModel : IModel<TID>;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Scoped"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelIdOperationCollection AddScoped<TModel, TID>() where TModel : IModel<TID>;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Singleton"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelIdOperationCollection AddSingleton<TModel, TID>() where TModel : IModel<TID>;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Transient"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelIdOperationCollection AddTransient<TModel, TID>() where TModel : IModel<TID>;
    }

    /// <summary>
    /// Specifies the contract for a collection of <see cref="IModel"/>.
    /// </summary>    
    public interface IContextModelOperationCollection
    {
        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <param name="serviceLifetime">service lifetime mode</param>
        /// <returns>current context service collection instance</returns>
        IContextModelOperationCollection AddServiceTo<TModel>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TModel : IModel;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Scoped"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelOperationCollection AddScoped<TModel>() where TModel : IModel;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Singleton"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelOperationCollection AddSingleton<TModel>() where TModel : IModel;

        /// <summary>
        /// <para>
        /// Generate and Add dynamic service from specified contract IModel to target dbcontext as <see cref="ServiceLifetime.Transient"/>.
        /// </para>
        /// <para>
        /// <i>Obs.: The TModel can be assigned as property in target context.</i>
        /// </para>
        /// </summary>
        /// <typeparam name="TModel">model to generate service type</typeparam>
        /// <typeparam name="TID">id model type</typeparam>
        /// <returns>current context service collection instance</returns>
        IContextModelOperationCollection AddTransient<TModel>() where TModel : IModel;
    }

    /// <summary>
    /// Specifies the contract for a collection of DBContext service.
    /// </summary>    
    public interface IContextServiceCollection : IContextServiceOperationCollection, IContextModelIdOperationCollection, IContextModelOperationCollection { }

    internal interface IContextServiceTypes : IEnumerable<Type>, IEnumerable { }
    #endregion

    #region Implementations
    internal sealed class ContextServiceTypeCollection : ContextServiceCollection<ContextBase> 
    {
        public ContextServiceTypeCollection() : base() { }
    }

    internal class ContextServiceCollection<TContext> : IContextServiceCollection, IContextServiceTypes, IDisposable
        where TContext : ContextBase
    {
        private IServiceCollection services;
        private HashSet<Type> serviceTypes;
        private bool disposed;

        ~ContextServiceCollection()
        {
            this.Dispose(false);
        }

        public ContextServiceCollection([NotNull] IServiceCollection services) : this()
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected ContextServiceCollection()
        {
            this.serviceTypes = new HashSet<Type>();
        }

        #region IContextServiceOperationCollection
        public IContextServiceOperationCollection AddService<TService>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TService : IService
        {
            this.RequireNonDisposed();
            this.services?.AddService<TContext, TService>(serviceLifetime);
            this.serviceTypes.Add(typeof(TService));
            return this;
        }

        public IContextServiceOperationCollection AddScoped<TService>() where TService : IService => AddService<TService>(ServiceLifetime.Scoped);

        public IContextServiceOperationCollection AddSingleton<TService>() where TService : IService => AddService<TService>(ServiceLifetime.Singleton);

        public IContextServiceOperationCollection AddTransient<TService>() where TService : IService => AddService<TService>(ServiceLifetime.Transient);
        #endregion

        #region IContextModelOperationCollection
        public IContextModelIdOperationCollection AddServiceTo<TModel, TID>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TModel : IModel<TID>
        {
            this.AddService<IServiceCrud<TModel, TID>>(serviceLifetime);
            this.AddService<IServiceCrudAsync<TModel, TID>>(serviceLifetime);            
            return this;
        }

        IContextModelIdOperationCollection IContextModelIdOperationCollection.AddScoped<TModel, TID>() => AddServiceTo<TModel, TID>(ServiceLifetime.Scoped);

        IContextModelIdOperationCollection IContextModelIdOperationCollection.AddSingleton<TModel, TID>() => AddServiceTo<TModel, TID>(ServiceLifetime.Singleton);

        IContextModelIdOperationCollection IContextModelIdOperationCollection.AddTransient<TModel, TID>() => AddServiceTo<TModel, TID>(ServiceLifetime.Transient);
        #endregion

        #region IContextModelOperationCollection
        public IContextModelOperationCollection AddServiceTo<TModel>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TModel : IModel
        {
            Type mType  = typeof(TModel);
            Type gmType = mType.GetGenericInterfaceType(typeof(IModel<>)) ?? throw new InvalidOperationException(
                $"The Entity type {typeof(TModel).Name} does not " +
                $"implements the interface \"{typeof(IModel<>)}\". " +
                $"So is not possible identify your ID type!");

            Type idType = gmType.GetGenericArguments().First();

            Type operModelIdType = typeof(IContextModelIdOperationCollection);
            var res = operModelIdType.GetMethod(nameof(AddServiceTo), BindingFlags.Public | BindingFlags.Instance)
                .MakeGenericMethod(mType, idType)
                .Invoke(this, new object[] { serviceLifetime });

            return res as IContextModelOperationCollection;
        }

        IContextModelOperationCollection IContextModelOperationCollection.AddScoped<TModel>() => AddServiceTo<TModel>(ServiceLifetime.Scoped);

        IContextModelOperationCollection IContextModelOperationCollection.AddSingleton<TModel>() => AddServiceTo<TModel>(ServiceLifetime.Singleton);

        IContextModelOperationCollection IContextModelOperationCollection.AddTransient<TModel>() => AddServiceTo<TModel>(ServiceLifetime.Transient);
        #endregion

        #region IContextServiceTypes
        public IEnumerator<Type> GetEnumerator()
        {
            this.RequireNonDisposed();
            return serviceTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            this.RequireNonDisposed();
            return serviceTypes.GetEnumerator();
        }
        #endregion

        #region IDisposable
        private void RequireNonDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.serviceTypes?.Clear();
            }

            this.serviceTypes = null;
            this.services = null;
            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
    #endregion
}
