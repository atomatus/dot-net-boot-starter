using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// Entity validation interface.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    public interface IValidation<TEntity>
    {
        /// <summary>
        /// Use this methoid when <typeparamref name="TEntity"/> implements <see cref="IValidatableObject"/>
        /// to validate it and recover validation result when is not valid (return false).
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="validationResults">entity is not valid, contains validation results.</param>
        /// <returns>true, entity is valid, otherwise false and contains validation results.</returns>
        bool Validate(TEntity entity, out IEnumerable<ValidationResult> validationResults);
    }
}
