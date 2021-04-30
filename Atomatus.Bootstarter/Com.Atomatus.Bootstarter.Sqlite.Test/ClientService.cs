using Com.Atomatus.Bootstarter.Services;
using System.Diagnostics.CodeAnalysis;

namespace Com.Atomatus.Bootstarter.Sqlite.Test
{
    public sealed class ClientService : ServiceCrud<ClientContext, ClientTest, long>
    {
        public ClientService([NotNull] ClientContext context) : base(context, context.Clients)
        {

        }
    }
}
