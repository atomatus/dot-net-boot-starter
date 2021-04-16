using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Model base within generic id and object validator.
    /// </summary>
    /// <typeparam name="ID"></typeparam>
    public abstract class ModelBase<ID> :
        IModel<ID>,
        IModelAltenateKey,
        IEquatable<ModelBase<ID>>,
        IComparable<ModelBase<ID>>,
        IComparable,
        IValidatableObject
    {
        /// <summary>
        /// Id.
        /// Primary key. 
        /// </summary>
        public ID Id { get; set; }

        /// <summary>
        /// Alternate key.
        /// </summary>
        public Guid Uuid { get; set; }

        #region IEquatable
        /// <summary>
        /// Hashcode generate it from public field values.
        /// </summary>
        /// <returns></returns>
        public sealed override int GetHashCode()
        {
            return this.GetHashCodeFromPublicFields();
        }

        /// <summary>
        /// Compare objects.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(ModelBase<ID> other)
        {
            return other != null && other == this || this.CompareTo(other) == 0;
        }

        /// <summary>
        /// Compare objects id
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool EqualsId(ModelBase<ID> other)
        {
            return other != null && this.GetType() == other.GetType() && 
                Objects.Compare(this.Id, other.Id);
        }

        /// <summary>
        /// Compare objects id or uuid
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool EqualsAnyId(ModelBase<ID> other)
        {
            return other != null && this.GetType() == other.GetType() && 
                (Objects.Compare(this.Id, other.Id) || Objects.Compare(this.Uuid, other.Uuid));
        }
        #endregion

        #region IComparable
        /// <summary>
        /// Compare to other object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ModelBase<ID> other)
        {
            return other is null ? 1 : this.GetHashCode().CompareTo(other.GetHashCode());
        }

        /// <summary>
        /// Compare to other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return obj is ModelBase<ID> mb ? this.CompareTo(mb) : 1;
        }
        #endregion

        #region IValidatableObject
        /// <summary>
        /// Determines whether the specified object is valid.
        /// </summary>
        /// <param name="validationContext"> The validation context.</param>
        /// <returns>A collection that holds failed-validation information.</returns>
        protected virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext) => Enumerable.Empty<ValidationResult>();

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext) => Validate(validationContext);
        #endregion
    }
}
