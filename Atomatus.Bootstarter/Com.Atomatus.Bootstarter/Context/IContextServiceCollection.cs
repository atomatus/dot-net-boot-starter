using Com.Atomatus.Bootstarter.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Context
{
    /// <summary>
    /// Specifies the contract for a collection of DBContext service.
    /// </summary>    
    public interface IContextServiceCollection
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
        IContextServiceCollection AddService<TService>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TService : IService;

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
        IContextServiceCollection AddScoped<TService>() where TService : IService;

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
        IContextServiceCollection AddSingleton<TService>() where TService : IService;

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
        IContextServiceCollection AddTransient<TService>() where TService : IService;
    }

    internal interface IContextServiceTypes : IEnumerable<Type>, IEnumerable { }

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

        #region IContextServiceCollection
        public IContextServiceCollection AddService<TService>(ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) where TService : IService
        {
            this.RequireNonDisposed();
            this.services?.AddService<TContext, TService>(serviceLifetime);
            this.serviceTypes.Add(typeof(TService));
            return this;
        }

        public IContextServiceCollection AddScoped<TService>() where TService : IService
        {
            return AddService<TService>(ServiceLifetime.Scoped);
        }

        public IContextServiceCollection AddSingleton<TService>() where TService : IService
        {
            return AddService<TService>(ServiceLifetime.Singleton);
        }

        public IContextServiceCollection AddTransient<TService>() where TService : IService
        {
            return AddService<TService>(ServiceLifetime.Transient);
        }
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
}
