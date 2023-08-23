using System;
namespace Com.Atomatus.Bootstarter.Model.SoftDelete
{
    /// <summary>
    /// Soft delete Model base within generic id and object validator.
    /// </summary>
    /// <typeparam name="ID">id type</typeparam>
    public abstract class SoftDeleteModel<ID> : ModelBase<ID>, ISoftDeleteModel<ID>
	{
        /// <summary>
        /// Deleted at date.
        /// </summary>
        public DateTime? Deleted { get; set; }
    }
}

