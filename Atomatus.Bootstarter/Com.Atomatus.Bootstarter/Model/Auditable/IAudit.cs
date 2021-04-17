using System;

namespace Com.Atomatus.Bootstarter.Model
{
    /// <summary>
    /// Auditable
    /// </summary>
    public interface IAudit
    {
        /// <summary>
        /// Created at.
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Updated at.
        /// </summary>
        DateTime? Updated { get; set; }
    }
}
