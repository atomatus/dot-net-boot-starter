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
        /// Created in date.
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// Updated in date.
        /// </summary>
        public DateTime? Updated { get; set; }
    }
}
