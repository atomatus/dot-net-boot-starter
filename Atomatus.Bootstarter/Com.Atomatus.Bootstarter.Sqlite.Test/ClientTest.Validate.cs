using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public partial class ClientTest
    {
        public const int MIN_AGE            = 18;
        public const int MAX_AGE            = 130;

        public const int MAX_AGE_LENGTH     = 3;
        public const int MAX_NAME_LENGTH    = 100;

        protected override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Age < MIN_AGE)
            {
                yield return new ValidationResult($"Customer must be {MIN_AGE} years or older!", new[] { nameof(Age) });
            }
            else if(Age > MAX_AGE)
            {
                yield return new ValidationResult("Uooh! Call Guinness Book!!!", new[] { nameof(Age) });
            }

            else if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Must input name!", new[] { nameof(Name) });
            }
            else if (Name.Length > MAX_NAME_LENGTH)
            {
                yield return new ValidationResult("Name value is very large, " +
                    $"input a value equals or less than {MAX_NAME_LENGTH} chars!", new[] { nameof(Name) });
            }
        }
    }
}
