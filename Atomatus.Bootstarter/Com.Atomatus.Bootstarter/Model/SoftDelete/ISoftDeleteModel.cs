using System;

namespace Com.Atomatus.Bootstarter.Model
{
	/// <summary>
	/// Soft delete model.
	/// </summary>
	public interface ISoftDeleteModel : IModel, IModelAltenateKey
	{
		/// <summary>
		/// Deleted at date.
		/// </summary>
		DateTime? Deleted { get; set; }
	}

    /// <summary>
    /// Soft delete model id.
    /// </summary>
    /// <typeparam name="ID">id type</typeparam>
    public interface ISoftDeleteModel<ID> : ISoftDeleteModel, IModel<ID> { }
}
