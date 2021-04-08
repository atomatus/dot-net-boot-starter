using System;

namespace Com.Atomatus.Bootstarter.Model.Auditable
{
    public abstract class AuditableModel<ID> : ModelBase<ID>, IAudit
    {
        public DateTime Created { get; set; }
        
        public DateTime? Updated { get; set; }
    }
}
