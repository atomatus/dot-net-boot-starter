using System;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Model interface.
    /// </summary>
    public interface IModel  {  }

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
    public interface IModel<ID> : IModel
    {
        /// <summary>
        /// Id (Primary key).
        /// </summary>
        ID Id { get; set; }
    }
}
