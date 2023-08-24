using System;
using System.Linq.Expressions;
using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter
{

    /// <summary>
    /// CRUD Contract class for expression callback function usage in requests by id.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="ID">entity id type</typeparam>
	public interface ICrudExpression<TEntity, ID> : ICrudExpression<TEntity>
        where TEntity : IModel<ID>
    {
        #region [R]ead
        /// <summary>
        /// Check if where condition makes matches with some data.
        /// </summary>
        /// <param name="whereCondition">where condition</param>
        /// <returns>true if any elements in the source sequence pass the test in the specified predicate; otherwise, false.</returns>
        bool Exists(Expression<Func<TEntity, bool>> whereCondition);
        #endregion
    }
}
