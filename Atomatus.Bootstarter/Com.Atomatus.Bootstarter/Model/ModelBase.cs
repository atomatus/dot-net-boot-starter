using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Model
{
    public abstract class ModelBase<ID> :
        IModel<ID>,
        IModelAltenateKey,
        IEquatable<ModelBase<ID>>,
        IComparable<ModelBase<ID>>,
        IComparable,
        IValidatableObject
    {
        public ID Id { get; set; }

        public Guid Uuid { get; set; }

        object IModel.Id { get => Id; }

        #region IEquatable
        public sealed override int GetHashCode()
        {
            return this.GetHashCodeFromPublicFields();
        }

        public virtual bool Equals(ModelBase<ID> other)
        {
            return other != null && other == this || this.CompareTo(other) == 0;
        }

        public virtual bool EqualsId(ModelBase<ID> other)
        {
            return other != null && this.GetType() == other.GetType() && 
                Objects.Compare(this.Id, other.Id);
        }

        public bool EqualsAnyId(ModelBase<ID> other)
        {
            return other != null && this.GetType() == other.GetType() && 
                (Objects.Compare(this.Id, other.Id) || Objects.Compare(this.Uuid, other.Uuid));
        }
        #endregion

        #region IComparable
        public int CompareTo(ModelBase<ID> other)
        {
            return other is null ? 1 : this.GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj)
        {
            return obj is ModelBase<ID> mb ? this.CompareTo(mb) : 1;
        }
        #endregion

        #region IValidatableObject
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return Enumerable.Empty<ValidationResult>();
        }
        #endregion
    }
}
