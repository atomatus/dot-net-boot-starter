using System;

namespace Com.Atomatus.Bootstarter.Model
{
    public interface IAudit
    {
        DateTime Created { get; set; }

        DateTime? Updated { get; set; }
    }
}
