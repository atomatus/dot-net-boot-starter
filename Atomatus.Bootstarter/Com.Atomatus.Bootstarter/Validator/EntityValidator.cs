using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// Entity Validator implements <see cref="IValidation{TEntity}"/>
    /// and contains Validation method to check if current <typeparamref name="TEntity"/>
    /// is valid by <see cref="IValidatableObject.Validate(ValidationContext)"/> method results,
    /// when contains one or more <see cref="ValidationResult"/> is not valid, otherwise is valid.
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
	public abstract class EntityValidator<TEntity> : IValidation<TEntity>
	{
        #region Validation
        private static void ValidateLocal([NotNull] object entity,
            [NotNull] out IEnumerable<ValidationResult> validationResults,
            out bool isValidatableObject,
            out bool isValid)
        {
            validationResults = (isValidatableObject = entity is IValidatableObject) ?
                    (entity as IValidatableObject).Validate(new ValidationContext(entity)) :
                    Enumerable.Empty<ValidationResult>();
            isValid = !validationResults.Any();
        }

        /// <summary>
        /// Require validate target <typeparamref name="TEntity"/>,
        /// otherwise throws exception.
        /// <para>
        /// <i>
        /// Fired only when <typeparamref name="TEntity"/> implements <see cref="IValidatableObject"/>,
        /// otherwise will be ignored.
        /// </i>
        /// </para>
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <exception cref="AggregateValidationException">throws when entity is not valid</exception>
        internal static void RequireValidateModel([NotNull] TEntity entity)
        {
            RequireValidateLocal(entity);
        }

        /// <summary>
        /// Require validate target <typeparamref name="TEntity"/>,
        /// otherwise throws exception.
        /// </summary>
        /// <typeparam name="TEntityValidatable">target entity type when it implements IValidatableObject</typeparam>
        /// <param name="entity">current target entity when it implements IValidatableObject</param>
        internal protected static void RequireValidate<TEntityValidatable>(TEntityValidatable entity)
            where TEntityValidatable : TEntity, IValidatableObject
        {
            RequireValidateLocal(entity);
        }

        private static void RequireValidateLocal([NotNull] object entity)
        {
            ValidateLocal(entity, out var validationResults, out bool _, out bool isValid);
            if (!isValid)
            {
                throw new AggregateValidationException(entity, validationResults);
            }
            else
            {
                entity.GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !p.GetGetMethod()?.IsVirtual ?? false)//ignore virtual props. To avoid circular reference.
                    .SelectMany(p =>
                    {
                        var value = p.GetValue(entity);
                        return
                            value is IEnumerable e ? e.OfType<IValidatableObject>() :
                            value is IValidatableObject vo ? new[] { vo } : Enumerable.Empty<object>();
                    })
                    .ToList()
                    .ForEach(p => RequireValidateLocal(p));
            }
        }

        /// <summary>
        /// Use this methoid when <typeparamref name="TEntity"/> implements <see cref="IValidatableObject"/>
        /// to validate it and recover validation result when is not valid (return false).
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="validationResults">entity is not valid, contains validation results.</param>
        /// <returns>true, entity is valid, otherwise false and contains validation results.</returns>
        public bool Validate([NotNull] TEntity entity, [NotNull] out IEnumerable<ValidationResult> validationResults)
        {
            ValidateLocal(entity, out validationResults, out bool isValidatableObject, out bool isValid);
            if (!isValidatableObject)
            {
                ConsoleColored.WriteLine($"[{GetType().Name}#Validate] Entity \"{typeof(TEntity).FullName}\" does not implements IValidatableObject interface, " +
                    $"therefore Validate will always return true.", ConsoleColor.Red);
            }
            return isValid;
        }

        /// <summary>
        /// Use this method when <typeparamref name="TEntity"/> implements <see cref="IValidatableObject"/>
        /// and you desire to validate the object in service context to avoid persit a data out of valid format.
        /// <para>
        /// This method will return false when entity is not valid, contains validation results after 
        /// <see cref="IValidatableObject.Validate(ValidationContext)"/>, then for each validation result
        /// will be fired the <paramref name="addModelStateErrorAction"/> action (use this action in ModelState.AddModelError 
        /// and return the entity as View(entity) in Controller method).
        /// </para>
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="addModelStateErrorAction">action callback for ModelState.AddModelError usage.</param>
        /// <returns>
        /// True, is valid and does not contains no one validation result, 
        /// otherwise False, 
        /// contains validation result and callback will be fired for it one.</returns>
        public bool ValidateModelState([NotNull] TEntity entity, Action<string, string> addModelStateErrorAction)
        {
            bool isValid = this.Validate(entity, out var validationResults);
            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        addModelStateErrorAction.Invoke(memberName, result.ErrorMessage);
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Use this method when <typeparamref name="TEntity"/> implements <see cref="IValidatableObject"/>
        /// and you desire to validate the object in service context to avoid persit a data out of valid format.
        /// This method will check <see cref="IValidatableObject.Validate(ValidationContext)"/> if returns false,
        /// collect all ValidationResult to an Aggregate Exception and throw it.
        /// </summary>
        /// <param name="entity">target entity</param>
        protected void ValidateOrThrowsException([NotNull] TEntity entity)
        {
            if (!this.Validate(entity, out var validationResults))
            {
                throw new AggregateValidationException(entity, validationResults);
            }
        }
        #endregion
    }
}
