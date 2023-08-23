using System;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Model base auditable and soft delete.
    /// </summary>
    /// <typeparam name="ID">id type</typeparam>
    public abstract class SoftDeleteAuditableModel<ID> : AuditableModel<ID>, ISoftDeleteModel<ID>
    {
        /// <summary>
        /// Deleted at date.
        /// </summary>
        public DateTime? Deleted { get; set; }
    }
}

