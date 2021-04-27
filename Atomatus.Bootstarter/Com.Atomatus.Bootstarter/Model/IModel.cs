using System;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Model interface.
    /// </summary>
    public interface IModel  {  }

    /// <summary>
    /// Model equatable.
    /// </summary>
    public interface IModelEquatable
    {
        /// <summary>
        /// Compare IModel objects are equals.
        /// </summary>
        /// <param name="other">other IModel</param>
        /// <returns>true, objects are equals.</returns>
        bool Equals(object other);

        /// <summary>
        /// Compare IModel objects ID.
        /// </summary>
        /// <param name="other">other IModel</param>
        /// <returns>true, both id are equals</returns>
        bool EqualsId(object other);

        /// <summary>
        /// Compare any IDs (Primary or Alternaty) are equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>are equals</returns>
        bool EqualsAnyId(object other);
    }

    /// <summary>
    /// Model alternate key.
    /// </summary>
    public interface IModelAltenateKey : IModel
    {
        /// <summary>
        /// Uuid (alternate key).
        /// </summary>
        Guid Uuid { get; set; }
    }

    /// <summary>
    /// Model generic id.
    /// </summary>
    /// <typeparam name="ID"></typeparam>
    public interface IModel<ID> : IModel, IModelEquatable
    {
        /// <summary>
        /// Id (Primary key).
        /// </summary>
        ID Id { get; set; }
    }
}
