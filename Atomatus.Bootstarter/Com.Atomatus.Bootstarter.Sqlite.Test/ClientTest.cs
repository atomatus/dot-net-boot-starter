using Com.Atomatus.Bootstarter.Model;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public partial class ClientTest : AuditableModel<long>
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }
}
