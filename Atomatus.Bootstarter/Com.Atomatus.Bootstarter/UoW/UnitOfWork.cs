using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Com.Atomatus.Bootstarter.UoW
{
    /// <summary>
    /// <para>
    /// The Unit of Work (UoW) pattern is a design pattern used to manage the
    /// lifecycle and coordination of multiple operations that involve
    /// different repositories (data access components) while ensuring data
    /// consistency and integrity.
    /// </para>
    /// <para>
    /// When working with Entity Framework, a popular Object-Relational
    /// Mapping (ORM) framework in .NET, the DbContext (<typeparamref name="TContext"/>) is a fundamental component
    /// that represents the database session and provides access to the database
    /// through Entity Framework. It acts as a bridge between your domain
    /// entities and the underlying database tables.
    /// </para>
    /// </summary>
    /// <typeparam name="TContext">Database context type</typeparam>
    /// <see cref="IUnitOfWork"/>
    /// <author>Carlos Matos</author>
    /// <date>2023-08-28</date> 
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        [NotNull]
        private readonly TContext dbContext;
        [NotNull]
        private readonly IServiceProvider serviceProvider;
        [NotNull]
        private readonly IDictionary<Type, IRepository> repositoryCache;

        [AllowNull]
        private IDbContextTransaction transaction;

        /// <summary>
        /// Construct with service provider inject and proving other services
        /// to be inject when requested.
        /// </summary>
        /// <param name="serviceProvider">service provider</param>
        /// <exception cref="ArgumentNullException">throws when service provider is null</exception>
        public UnitOfWork([NotNull] IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.dbContext = serviceProvider.GetRequiredService<TContext>();
            this.repositoryCache = new Dictionary<Type, IRepository>();
        }

        /// <inheritdoc />
        public TRepository GetRepository<TRepository>() where TRepository : IRepository
        {
            Type key = typeof(TRepository);
            if (!repositoryCache.TryGetValue(key, out IRepository repository))
            {
                repository = ActivatorUtilities.CreateInstance<TRepository>(serviceProvider, dbContext);
                repositoryCache[key] = repository;
            }
            return (TRepository)repository;
        }

        /// <inheritdoc />
        public void BeginTransaction()
        {
            this.RequireTransactionNotOpen();
            transaction = dbContext.Database.BeginTransaction();
        }

        /// <inheritdoc />
        public async Task BeginTransactionAsync()
        {
            this.RequireTransactionNotOpen();
            transaction = await dbContext.Database.BeginTransactionAsync();
        }

        private void RequireTransactionNotOpen()
        {
            if (transaction != null)
            {
                throw new InvalidOperationException("Transaction is already open!");
            }
        }

        private IDbContextTransaction RequireTransactionOpens()
        {
            return transaction ?? throw new InvalidOperationException("No one transaction openned (Use BeginTransaction before it)!");
        }

        private void DisposeTransaction(IDbContextTransaction transaction)
        {
            if (this.transaction == transaction)
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
        }

        private async Task DisposeTransactionAsync(IDbContextTransaction transaction)
        {
            if (this.transaction == transaction)
            {
                await this.transaction.DisposeAsync();
                this.transaction = null;
            }
        }

        /// <inheritdoc />
        public void Commit()
        {
            var transaction = this.RequireTransactionOpens();

            try
            {
                SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                this.DisposeTransaction(transaction);
            }
        }

        /// <inheritdoc />
        public async Task CommitAsync()
        {
            var transaction = this.RequireTransactionOpens();

            try
            {
                await SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await this.DisposeTransactionAsync(transaction);
            }
        }

        /// <inheritdoc />
        public void Rollback()
        {
            var transaction = this.RequireTransactionOpens();
            transaction.Rollback();
            this.DisposeTransaction(transaction);
        }

        /// <inheritdoc />
        public async Task RollbackAsync()
        {
            var transaction = this.RequireTransactionOpens();
            await transaction.RollbackAsync();
            await this.DisposeTransactionAsync(transaction);
        }

        /// <inheritdoc />
        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction.Dispose();
            }
            transaction = null;

            foreach (var repository in repositoryCache.Values)
            {
                if (repository is IDisposable disposableRepository)
                {
                    disposableRepository.Dispose();
                }
            }

            repositoryCache.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
