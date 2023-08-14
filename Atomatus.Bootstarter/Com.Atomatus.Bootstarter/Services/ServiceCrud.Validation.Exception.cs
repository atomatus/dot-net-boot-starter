using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Services
{
    /// <summary>
    /// Represents one or more ValidationException
    /// errors that occur during application validation.
    /// </summary>
    /// <see cref="ValidationException"/>
    /// <see cref="AggregateException"/>
	public sealed class AggregateValidationException : AggregateException
    {
		public AggregateValidationException([NotNull] IEnumerable<ValidationResult> validationResults)
            : base("One or more validation error was found!",
                  validationResults.Select(r => new ValidationException(r, null, null))) { }
    }
}
