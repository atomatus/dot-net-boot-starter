using System;
using System.Threading;
using System.Threading.Tasks;
using Com.Atomatus.Bootstarter.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
    /// Mapping (ORM) framework in .NET, the DbContext is a fundamental component
    /// that represents the database session and provides access to the database
    /// through Entity Framework. It acts as a bridge between your domain
    /// entities and the underlying database tables.
    /// </para>
    /// </summary>
    /// <author>Carlos Matos</author>
    /// <date>2023-08-28</date> 
    public interface IUnitOfWork : IDisposable
	{
        /// <summary>
        /// Recover and/or initialize a Repository by service provider in
        /// registered in service collection.
        /// </summary>
        /// <typeparam name="TRepository">repository type</typeparam>
        /// <returns>repository initialized</returns>
        TRepository GetRepository<TRepository>() where TRepository : IRepository;

        /// <inheritdoc cref="DatabaseFacade.BeginTransaction"/>
        void BeginTransaction();

        /// <inheritdoc cref="IDbContextTransaction.Commit"/>
        void Commit();

        /// <inheritdoc cref="IDbContextTransaction.Rollback"/>
        void Rollback();

        /// <inheritdoc cref="DbContext.SaveChanges()"/>
        int SaveChanges();

        /// <inheritdoc cref="DatabaseFacade.BeginTransactionAsync"/>
        Task BeginTransactionAsync();

        /// <inheritdoc cref="IDbContextTransaction.CommitAsync(CancellationToken)"/>
        Task CommitAsync();

        /// <inheritdoc cref="IDbContextTransaction.RollbackAsync"/>
        Task RollbackAsync();

        /// <inheritdoc cref="DbContext.SaveChangesAsync(CancellationToken)"/>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
