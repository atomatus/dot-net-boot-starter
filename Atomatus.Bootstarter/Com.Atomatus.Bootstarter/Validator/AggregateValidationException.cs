using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter
{
    /// <summary>
    /// Represents one or more ValidationException
    /// errors that occur during application validation.
    /// </summary>
    /// <see cref="ValidationException"/>
    /// <see cref="AggregateException"/>
    public sealed class AggregateValidationException : AggregateException
    {
        private ReadOnlyCollection<ValidationException> _rocView;

        /// <inheritdoc/>
        new public ReadOnlyCollection<ValidationException> InnerExceptions =>
            _rocView ??= new ReadOnlyCollection<ValidationException>(base.InnerExceptions.OfType<ValidationException>().ToList());

        /// <summary>
        /// Construct an AggregateValidation exception by target entity and his ValidationResults.
        /// </summary>
        /// <param name="entity">target entity</param>
        /// <param name="validationResults">entity validation results</param>
        public AggregateValidationException([NotNull] object entity, [NotNull] IEnumerable<ValidationResult> validationResults)
            : base("One or more validation error was found!",
                  validationResults.Select(r => new ValidationException(r, null, entity)))
        { }
    }
}

