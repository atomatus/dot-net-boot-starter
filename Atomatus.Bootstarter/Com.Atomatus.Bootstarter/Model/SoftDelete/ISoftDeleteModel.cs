using System;

namespace Com.Atomatus.Bootstarter.Model
{
	/// <summary>
	/// Soft delete model.
	/// </summary>
	/// <typeparam name="ID">id type</typeparam>
	public interface ISoftDeleteModel<ID> : IModel<ID>, IModelAltenateKey
	{
		/// <summary>
		/// Deleted at date.
		/// </summary>
		DateTime? Deleted { get; set; }
	}
}

