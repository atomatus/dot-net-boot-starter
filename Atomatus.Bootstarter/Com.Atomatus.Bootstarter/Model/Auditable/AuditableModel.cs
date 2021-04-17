using System;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Model base auditable.
    /// Contains Create and update date properties.
    /// </summary>
    /// <typeparam name="ID"></typeparam>
    public abstract class AuditableModel<ID> : ModelBase<ID>, IAudit
    {
        /// <summary>
        /// Created at date.
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// Updated at date.
        /// </summary>
        public DateTime? Updated { get; set; }
    }
}
